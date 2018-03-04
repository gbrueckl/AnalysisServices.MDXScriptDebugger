using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.AnalysisServices;
using System.Text.RegularExpressions;
using System.Configuration;

namespace gbrueckl.AnalysisServices.MdxScriptDebugger
{
    public partial class MdxScriptDebuggerGUI : Form
    {
        #region constants
        private const string DefaultQuery = "SELECT \r\n[Measures].members ON 0 \r\nFROM [@@[CubeName]]";
        //private const string DefaultQuery = "SELECT NON EMPTY Hierarchize({[Time Dynamic].[Time Dynamic].[Time Dynamic].Members}) DIMENSION PROPERTIES PARENT_UNIQUE_NAME,[Time Dynamic].[Time Dynamic].[Time Dynamic].[Sort Order] ON COLUMNS , NON EMPTY Hierarchize(DrilldownMember({{DrilldownMember({{DrilldownLevel({[Position].[Position Hierarchy].[Position Hierarchy]})}}, {[Position].[Position Hierarchy].&[Reporting Hierarchy]})}}, {[Position].[Position Hierarchy].&[P & L]})) DIMENSION PROPERTIES PARENT_UNIQUE_NAME,[Position].[Position Hierarchy].[Consolidation],[Position].[Position Hierarchy].[Flag1],[Position].[Position Hierarchy].[Is Financial],[Position].[Position Hierarchy].[Position Combined],[Position].[Position Hierarchy].[Position Description],[Position].[Position Hierarchy].[Position Hierarchy],[Position].[Position Hierarchy].[Position Number],[Position].[Position Hierarchy].[PositionID],[Position].[Position Hierarchy].[Sort Order],[Position].[Position Hierarchy].[Time Balance] ON ROWS  FROM (SELECT ({[Position].[Position Hierarchy].&[P & L]}) ON COLUMNS  FROM [@@[CubeName]]) WHERE ([BusinessUnit].[BusinessUnit_Hierarchy].[RHI Group],[Scale].[Scale].&[Scale_1],[View].[View ID].&[3]) CELL PROPERTIES VALUE, FORMAT_STRING, LANGUAGE, BACK_COLOR, FORE_COLOR, FONT_FLAGS";
       
        private const string EnabledColumnName = "Enabled";
        private const string TextRun = "Debug MDX Script";
        private const string TextCancel = "Cancel";
        #endregion
        private bool _queryModified;
        private bool _resultsFiltered;
        private bool _cancellationPending;
        private Server _server;
        private Database _database;
        private Cube _cube;
        private MdxScriptDebugger _msd;
        private DataColumn _enabledColumn;


        public MdxScriptDebuggerGUI(string servername, string dbname, string cubename)
        {
            InitializeComponent();

            _queryModified = false;
            _resultsFiltered = false;
            _cancellationPending = false;
            tcMain.TabPages.Remove(tpResults);
            bDebugScript1.Enabled = false;
            bDebugScript2.Enabled = false;
            bCancelDebugging.Text = TextCancel;

            if (!string.IsNullOrWhiteSpace(servername) && servername != "default")
            {
                tbServer.Text = servername;
                bConnect_Click(null, null);

                if(_server != null && !string.IsNullOrWhiteSpace(dbname))
                {
                    if (cbDatabase.Items.Contains(dbname))
                    {
                        cbDatabase.Text = dbname;

                        if (!string.IsNullOrWhiteSpace(cubename))
                        {
                            if (cbCube.Items.Contains(cubename))
                            {
                                cbCube.Text = cubename;
                            }
                        }
                    }
                }
            }

            UpdateUI(false);

            // ToolTips
            ttChangeTracking.SetToolTip(cbChangeTracking, "Tracks which Assignments actually impact the query.");
            ttUseCustomScript.SetToolTip(cbUseCustomScript1, "Use MDX Script from Tab [Custom MDX Script]");
            ttUseCustomScript.SetToolTip(cbUseCustomScript2, "Use MDX Script from Tab [Custom MDX Script]");
            ttVerbose.SetToolTip(cbVerbose, "Track impact of every MDX Script Command" + Environment.NewLine + "By default only performance relevant commands are tracked.");
            ttExport.SetToolTip(bExportXML, "Export results to current folder in format 'Results_yyyyMMdd_HHmmss.xml'");
            ttCreateFirst.SetToolTip(cbCreateFirst, "Moves all CREATE statements (members and sets) at the top of the MDX script." + Environment.NewLine + "This is useful if the reference query uses any of them.");
            ttGenerateCustomScript.SetToolTip(bGenerateCustomScript, "Generates a custom MDX script based on the selected commands.");
            ttShowOnlyEffective.SetToolTip(bShowOnlyEffective, "Filters the list to only show commands that were effecitvely used by the query.");
            ttClearCache.SetToolTip(cbClearCache, "Executes a XMLA ClearCache before Debugging");
        }
        public MdxScriptDebuggerGUI() : this(null, null, null) {}

        private void bConnect_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbServer.Text))
            {
                try
                {
                    _server = new Server();

                    _server.Connect("Data Source=" + tbServer.Text);

                    if (_server.Connected)
                        ResetDatabasesComboBox();
                    else
                        throw new Exception("Could not connect to server!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);

                    cbCube.Items.Clear();
                    cbCube.Enabled = false;
                    _cube = null;

                    cbDatabase.Items.Clear();
                    cbDatabase.Enabled = false;
                    _database = null;
                    
                    if(_server != null)
                    {
                        _server.Disconnect();
                        _server.Dispose();

                        _server = null;
                    }
                }

            }
        }

        private void ResetDatabasesComboBox()
        {
            cbDatabase.Items.Clear();

            SortedSet<string> sortedDbs = new SortedSet<string>();

            foreach(Database db in _server.Databases)
                sortedDbs.Add(db.Name);

            foreach (string db in sortedDbs)
                cbDatabase.Items.Add(db);

            if (cbDatabase.Items.Count == 0)
            {
                cbDatabase.Items.Add("No Databases found!");
                cbDatabase.Enabled = false;

                cbCube.Items.Clear();
                cbCube.Enabled = false;
            }
            else
            {
                cbDatabase.Enabled = true;
                cbDatabase.SelectedIndex = 0;
                cbDatabase_SelectedIndexChanged(null, null);
            }            
        }

        private void ResetCubesComboBox()
        {
            cbCube.Items.Clear();

            SortedSet<string> sortedCubes = new SortedSet<string>();

            foreach (Cube c in _database.Cubes)
                sortedCubes.Add(c.Name);
            
            foreach (string c in sortedCubes)
            {
                cbCube.Items.Add(c);
            }
            if (cbCube.Items.Count == 0)
            {
                cbCube.Items.Add("No Cubes found!");
                cbCube.Enabled = false;
            }
            else
            {
                cbCube.Enabled = true;
                cbCube.SelectedIndex = 0;
                cbCube_SelectedIndexChanged(null, null);
            }
        }

        private void cbDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            _database = _server.Databases.GetByName(cbDatabase.SelectedItem.ToString());
            ResetCubesComboBox();
        }

        private void cbCube_SelectedIndexChanged(object sender, EventArgs e)
        {
            _cube = _database.Cubes.GetByName(cbCube.SelectedItem.ToString());

            ResetScript();

            if (!_queryModified || tbQuery.Text.Trim().Length == 0)
            {
                tbQuery.Text = MdxScriptDebuggerGUI.DefaultQuery.Replace("@@[CubeName]", _cube.Name);
                _queryModified = false;
            }

            bDebugScript1.Enabled = true;
            bDebugScript2.Enabled = true;

            WriteCurrentSettingsToAppConfig();
        }

        private void ResetScript()
        {
            tbScript.Clear();
            foreach (Command cmd in _cube.DefaultMdxScript.Commands)
            {
                // the MdxScript uses only "\n" for new lines, we need to replace this
                tbScript.AppendText(cmd.Text.Replace("\n", Environment.NewLine));
            }
        }

        private void tbQuery_TextChanged(object sender, EventArgs e)
        {
            _queryModified = true;
        }

        private void bDebugScript_Click(object sender, EventArgs e)
        {

            if (bDebugScript1.Text == TextCancel)
            {
                bCancelDebugging_Click(null, null);
            }
            else
            {
                bParseScript_Click(null, null);

                dgResults.DataSource = MdxScriptCommand.SampleDataTable;
                FormatResultTable(false);

                UpdateUI(true);

                _msd.RunWorkerAsync();
            }
        }

        private void WriteCurrentSettingsToAppConfig()
        {
            Properties.Settings.Default.LastServer = _server.Name;
            Properties.Settings.Default.LastDatabase = _database.Name;
            Properties.Settings.Default.LastCube = _cube.Name;

            Properties.Settings.Default.Save();
        }

        void MdxScriptDebugger_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if(ssStatusBar.InvokeRequired)
            {
                ssStatusBar.Invoke(new Action<ProgressChangedEventArgs>(ProgressChanged), e);
                return;
            }

            ProgressChanged(e);
        }

        private void ProgressChanged(ProgressChangedEventArgs e)
        {
            UserState s = (UserState)e.UserState;

            lStatus.Text = s.StatusText;

            if (s.CommandNumber > 0)
            {
                pbCommands.Value = s.CommandNumber + 1;
                lCommandCount.Text = (s.CommandNumber + 1).ToString() + " / " + pbCommands.Maximum.ToString();
            }

            if (s.ChangeType == StateChangeType.ExecutionStarted || s.ChangeType == StateChangeType.ExecutionFinished)
            {
                DataGridViewCellStyle cs = new DataGridViewCellStyle();
                Color c = Color.White;
                // check if CommandNr already exists
                DataRow dr = ((DataTable)dgResults.DataSource).Rows.Find((int)s.CommandNumber);
                if (dr == null)
                {
                    ((DataTable)dgResults.DataSource).Rows.Add(s.Command.AsDataRow);
                }
                else
                {
                    dr.ItemArray = s.Command.AsDataRow;
                }

                if(s.ChangeType == StateChangeType.ExecutionStarted)
                {
                    c = Color.Yellow;
                }

                if (s.ChangeType == StateChangeType.ExecutionFinished)
                {
                    if(s.Command.IsEffective && s.Command.IsPerformanceRelevant)
                        c = Color.LightGreen;
                }

                FormatResultsRow(dgResults.Rows[dgResults.Rows.Count - 1], c);
            }
        }



        void MdxScriptDebugger_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (tcMain.InvokeRequired)
            {
                tcMain.Invoke(new Action<RunWorkerCompletedEventArgs>(ShowResults), e);
                return;
            }

            ShowResults(e);
        }

        private void ShowResults(RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled || _cancellationPending)
            {
                MessageBox.Show("Debugging was cancelled!");
                tcMain.SelectedTab = tpResults;

                lStatus.Text = "Debugging cancelled!";
            }

            if (e.Error != null)
            {
                string msg = e.Error.Message;

                MessageBox.Show(msg);
                tcMain.SelectedTab = tpQuery;

                lStatus.Text = "Aborted due to Error!";
            }

            _cancellationPending = false;

            // set ProgressBar to 100%
            pbCommands.Value = pbCommands.Maximum;

            tcMain.SelectedTab = tpResults;

            dgResults.DataSource = (DataTable)e.Result;

            FormatResultTable(true);

            _resultsFiltered = false;
            UpdateUI(false);
        }

        private void FormatResultTable(bool addColumns)
        {
            DataTable dt = (DataTable)dgResults.DataSource;
            dgResults.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgResults.ColumnHeadersHeight = 40;

            if (addColumns)
            {
                _enabledColumn = new DataColumn(EnabledColumnName, typeof(bool));
                dt.Columns.Add(_enabledColumn);
                dt.Select().ToList<DataRow>().ForEach(r => r[EnabledColumnName] = true); // set all commands to Enabled=true
                dt.Columns[EnabledColumnName].SetOrdinal(0);

                dgResults.DataSource = null; // need to remove old DataSource, otherwise column-reordering will not work for new "Enabled"-column
                dgResults.DataSource = dt;
            }

            foreach (DataGridViewColumn dc in dgResults.Columns)
            {
                dc.ReadOnly = true;
                dc.Width = 60;
                if (dc.HeaderText.Contains("Command"))
                {
                    dc.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    dc.Width = 350;

                    if (dc.HeaderText == MdxScriptCommand.ColumnNameCommandType)
                        dc.Width = 120;

                    if (dc.HeaderText == MdxScriptCommand.ColumnNameCommandNumber)
                        dc.Width = 60;
                }
                if (dc.HeaderText == EnabledColumnName)
                {
                    dc.ReadOnly = false;
                }
                if (dc.HeaderText == MdxScriptDebugger.ColumnNameVsRef)
                {
                    dc.ToolTipText = "Time vs. reference query (Avg of execution before and after Debugging)";
                }
                if(dc.ValueType.Name.StartsWith("int", StringComparison.CurrentCultureIgnoreCase))
                {
                    dc.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    dc.DefaultCellStyle.Format = "#,##0";
                }
            }

            Color c = Color.White;

            // need to do this after selecting the results tab 
            foreach (DataGridViewRow dr in dgResults.Rows)
            {
                c = Color.White;

                if (dr.ContainsColumn(MdxScriptCommand.ColumnNameEffective))
                {
                    if (Convert.ToInt32(dr.Cells[MdxScriptCommand.ColumnNameEffective].Value) == 1)
                    {
                        c = Color.LightGreen;
                    }
                }

                FormatResultsRow(dr, c);
            }
        }

        private void FormatResultsRow(DataGridViewRow dataGridViewRow, Color backColor)
        {
            int val;
            foreach (DataGridViewCell dgvc in dataGridViewRow.Cells)
            {
                if (dgvc.ValueType.Name.StartsWith("int", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (int.TryParse(dgvc.Value.ToString(), out val))
                    {
                        if (val == -1)
                            dgvc.Value = DBNull.Value;
                    }
                }

                if (backColor != null)
                    dgvc.Style.BackColor = backColor;
            }
        }

        private void bResetScript_Click(object sender, EventArgs e)
        {
            ResetScript();
        }

        private void cbUseCustomScript2_CheckedChanged(object sender, EventArgs e)
        {
            cbUseCustomScript1.Checked = cbUseCustomScript2.Checked;

            if (!cbUseCustomScript2.Checked)
            {
                tcMain.SelectedTab = tpQuery;
            }
        }

        private void cbUseCustomScript1_CheckedChanged(object sender, EventArgs e)
        {
            cbUseCustomScript2.Checked = cbUseCustomScript1.Checked;

            if (cbUseCustomScript1.Checked)
            {
                tcMain.SelectedTab = tpScript;
            }
        }

        private void bShowOnlyEffective_Click(object sender, EventArgs e)
        {
            if (dgResults.DataSource != null)
            {
                if (!_resultsFiltered)
                {
                    _resultsFiltered = true;
                    bShowOnlyEffective.Text = "Show All";
                    (dgResults.DataSource as DataTable).DefaultView.RowFilter = string.Format(MdxScriptCommand.ColumnNameEffective + " = {0}", _resultsFiltered);
                }
                else
                {
                    _resultsFiltered = false;
                    bShowOnlyEffective.Text = "Show only Effective";
                    (dgResults.DataSource as DataTable).DefaultView.RowFilter = "1 = 1";
                }
            }

            FormatResultTable(false);
        }

        private void bGenerateCustomScript_Click(object sender, EventArgs e)
        {
            tbScript.Clear();
            foreach (DataGridViewRow dr in dgResults.Rows)
            {
                if ((bool)dr.Cells[0].Value)
                {
                    tbScript.AppendText((string)dr.Cells[MdxScriptCommand.ColumnNameFullCommand].Value + Environment.NewLine);
                }
            }

            cbUseCustomScript1.Checked = true;
            cbUseCustomScript1_CheckedChanged(null, null);
            tcMain.SelectedTab = tpScript;
        }

        private void cbValueTracking_CheckedChanged(object sender, EventArgs e)
        {
            dgResults.DataSource = null;
            if (cbChangeTracking.Checked)
            {
                _resultsFiltered = false;
                bShowOnlyEffective.Visible = true;
            }
            else
            {
                _resultsFiltered = true;
                bShowOnlyEffective.Visible = false;
            }
        }

        private void bExportXML_Click(object sender, EventArgs e)
        {
            if (dgResults.DataSource != null)
            {
                ((DataTable)dgResults.DataSource).WriteXml(string.Format(Environment.CurrentDirectory + "\\Results_{0}.xml", DateTime.Now.ToString("yyyyMMdd_HHmmss")));
                MessageBox.Show("Done! Exported to XML-Results to current folder");
            }
        }

        private void UpdateUI(bool running)
        {
            this.bConnect.Enabled = !running;
            this.bExportXML.Enabled = !running;
            this.bGenerateCustomScript.Enabled = !running;
            this.bResetScript.Enabled = !running;
            this.bShowOnlyEffective.Enabled = !running;
            this.bCancelDebugging.Visible = running;

            if (running)
            {
                bDebugScript1.Text = TextCancel;
                bDebugScript2.Text = TextCancel;
                
            }
            else
            {
                bDebugScript1.Text = TextRun;
                bDebugScript2.Text = TextRun;
            }
        }

        private void bParseScript_Click(object sender, EventArgs e)
        {
            tcMain.TabPages.Remove(tpResults);
            tcMain.TabPages.Add(tpResults);
            tcMain.SelectedTab = tpResults;

            _msd = new MdxScriptDebugger(_server.Name, _database.Name, _cube.Name);
            _msd.Query = tbQuery.Text;
            _msd.Verbose = cbVerbose.Checked;
            _msd.ValueTracking = cbChangeTracking.Checked;
            _msd.CreateMembersAndSetsFirst = cbCreateFirst.Checked;
            _msd.ClearCache = cbClearCache.Checked;
            _msd.WarmCacheExecution = cbWarmCacheExecution.Checked;
            

            _msd.ProgressChanged += new ProgressChangedEventHandler(MdxScriptDebugger_ProgressChanged);
            _msd.RunWorkerCompleted += new RunWorkerCompletedEventHandler(MdxScriptDebugger_RunWorkerCompleted);
     

            if (cbUseCustomScript2.Checked || cbUseCustomScript1.Checked)
            {
                _msd.LoadCustomMdxScript(tbScript.Text);
            }

            pbCommands.Maximum = _msd.CommandCount;

            // if clicked and not manually executed
            if (sender != null)
            {
                dgResults.DataSource = _msd.GetResultTable();
                FormatResultTable(true);
            }
        }

        private void bCancelDebugging_Click(object sender, EventArgs e)
        {
            _cancellationPending = true;
            _msd.CancelAsyncWithKill();
        }

        private void dgResults_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == _enabledColumn.Table.Columns.IndexOf(_enabledColumn))
                {
                    DataGridViewRow cdr = dgResults.Rows[e.RowIndex];

                    int commandNr = (int)cdr.Cells[MdxScriptCommand.ColumnNameCommandNumber].Value;
                    string commandType = (string)cdr.Cells[MdxScriptCommand.ColumnNameCommandType].Value;
                    int level = (int)cdr.Cells[MdxScriptCommand.ColumnNameLevel].Value;
                    bool newStatus = (bool)((DataGridViewCheckBoxCell)cdr.Cells[e.ColumnIndex]).Value;

                    // if a SCOPE command is clicked also change all inner commands
                    if (commandType == MdxScriptCommandType.SCOPE.ToString())
                    {
                        foreach (DataGridViewRow dr in dgResults.Rows)
                        {
                            if ((int)dr.Cells[MdxScriptCommand.ColumnNameCommandNumber].Value > commandNr)
                            {
                                if ((int)dr.Cells[MdxScriptCommand.ColumnNameLevel].Value > level)
                                {
                                    ((DataGridViewCheckBoxCell)dr.Cells[e.ColumnIndex]).Value = newStatus;
                                }
                                else
                                {
                                    // break as soon as we leave the most outer scope
                                    break;
                                }
                            }
                        }
                    }

                    //dgResults.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
            catch (Exception e1)
            {
            }
        }

        private void dgResults_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (_enabledColumn != null)
            {
                if (e.ColumnIndex == _enabledColumn.Table.Columns.IndexOf(_enabledColumn))
                {
                    // automatically commit changes for our [Enabled]-column
                    dgResults.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            }
        }
    }

    //Extension methods must be defined in a static class 
    public static class DataGridViewRowExtension
    {
        // This is the extension method. 
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined. 
        public static bool ContainsColumn(this DataGridViewRow dgvr, string columnName)
        {
            bool ret = true;

            try
            {
                string x = dgvr.Cells[columnName].Value.ToString();
            }
            catch (Exception e)
            {
                ret = false;
            }

            return ret;
        }
    }
}
