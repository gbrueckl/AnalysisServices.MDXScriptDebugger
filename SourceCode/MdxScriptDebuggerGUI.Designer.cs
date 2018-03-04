namespace gbrueckl.AnalysisServices.MdxScriptDebugger
{
    partial class MdxScriptDebuggerGUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tbServer = new System.Windows.Forms.TextBox();
            this.lServer = new System.Windows.Forms.Label();
            this.cbDatabase = new System.Windows.Forms.ComboBox();
            this.lDatabase = new System.Windows.Forms.Label();
            this.lCube = new System.Windows.Forms.Label();
            this.cbCube = new System.Windows.Forms.ComboBox();
            this.bConnect = new System.Windows.Forms.Button();
            this.ttChangeTracking = new System.Windows.Forms.ToolTip(this.components);
            this.ttUseCustomScript = new System.Windows.Forms.ToolTip(this.components);
            this.ttVerbose = new System.Windows.Forms.ToolTip(this.components);
            this.tpResults = new System.Windows.Forms.TabPage();
            this.bCancelDebugging = new System.Windows.Forms.Button();
            this.bExportXML = new System.Windows.Forms.Button();
            this.bGenerateCustomScript = new System.Windows.Forms.Button();
            this.bShowOnlyEffective = new System.Windows.Forms.Button();
            this.dgResults = new System.Windows.Forms.DataGridView();
            this.tpScript = new System.Windows.Forms.TabPage();
            this.bParseScript = new System.Windows.Forms.Button();
            this.tbScript = new System.Windows.Forms.TextBox();
            this.bDebugScript2 = new System.Windows.Forms.Button();
            this.cbUseCustomScript2 = new System.Windows.Forms.CheckBox();
            this.bResetScript = new System.Windows.Forms.Button();
            this.tpQuery = new System.Windows.Forms.TabPage();
            this.cbCreateFirst = new System.Windows.Forms.CheckBox();
            this.cbUseCustomScript1 = new System.Windows.Forms.CheckBox();
            this.cbChangeTracking = new System.Windows.Forms.CheckBox();
            this.cbVerbose = new System.Windows.Forms.CheckBox();
            this.bDebugScript1 = new System.Windows.Forms.Button();
            this.tbQuery = new System.Windows.Forms.TextBox();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.ttExport = new System.Windows.Forms.ToolTip(this.components);
            this.ttShowOnlyEffective = new System.Windows.Forms.ToolTip(this.components);
            this.ttCreateStmtFirst = new System.Windows.Forms.ToolTip(this.components);
            this.ttGenerateCustomScript = new System.Windows.Forms.ToolTip(this.components);
            this.ttCreateFirst = new System.Windows.Forms.ToolTip(this.components);
            this.ssStatusBar = new System.Windows.Forms.StatusStrip();
            this.lDummy = new System.Windows.Forms.ToolStripStatusLabel();
            this.pbCommands = new System.Windows.Forms.ToolStripProgressBar();
            this.lCommandCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.cbClearCache = new System.Windows.Forms.CheckBox();
            this.ttClearCache = new System.Windows.Forms.ToolTip(this.components);
            this.cbWarmCacheExecution = new System.Windows.Forms.CheckBox();
            this.tpResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgResults)).BeginInit();
            this.tpScript.SuspendLayout();
            this.tpQuery.SuspendLayout();
            this.tcMain.SuspendLayout();
            this.ssStatusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // lStatus
            // 
            this.lStatus.Name = "lStatus";
            this.lStatus.Size = new System.Drawing.Size(0, 17);
            // 
            // tbServer
            // 
            this.tbServer.Location = new System.Drawing.Point(45, 10);
            this.tbServer.Name = "tbServer";
            this.tbServer.Size = new System.Drawing.Size(100, 20);
            this.tbServer.TabIndex = 0;
            this.tbServer.Text = "localhost";
            // 
            // lServer
            // 
            this.lServer.AutoSize = true;
            this.lServer.Location = new System.Drawing.Point(1, 12);
            this.lServer.Name = "lServer";
            this.lServer.Size = new System.Drawing.Size(38, 13);
            this.lServer.TabIndex = 1;
            this.lServer.Text = "Server";
            // 
            // cbDatabase
            // 
            this.cbDatabase.Enabled = false;
            this.cbDatabase.FormattingEnabled = true;
            this.cbDatabase.Location = new System.Drawing.Point(302, 10);
            this.cbDatabase.Name = "cbDatabase";
            this.cbDatabase.Size = new System.Drawing.Size(170, 21);
            this.cbDatabase.TabIndex = 2;
            this.cbDatabase.SelectedIndexChanged += new System.EventHandler(this.cbDatabase_SelectedIndexChanged);
            // 
            // lDatabase
            // 
            this.lDatabase.AutoSize = true;
            this.lDatabase.Location = new System.Drawing.Point(243, 12);
            this.lDatabase.Name = "lDatabase";
            this.lDatabase.Size = new System.Drawing.Size(53, 13);
            this.lDatabase.TabIndex = 3;
            this.lDatabase.Text = "Database";
            // 
            // lCube
            // 
            this.lCube.AutoSize = true;
            this.lCube.Location = new System.Drawing.Point(481, 13);
            this.lCube.Name = "lCube";
            this.lCube.Size = new System.Drawing.Size(32, 13);
            this.lCube.TabIndex = 4;
            this.lCube.Text = "Cube";
            // 
            // cbCube
            // 
            this.cbCube.Enabled = false;
            this.cbCube.FormattingEnabled = true;
            this.cbCube.Location = new System.Drawing.Point(519, 10);
            this.cbCube.Name = "cbCube";
            this.cbCube.Size = new System.Drawing.Size(170, 21);
            this.cbCube.TabIndex = 5;
            this.cbCube.SelectedIndexChanged += new System.EventHandler(this.cbCube_SelectedIndexChanged);
            // 
            // bConnect
            // 
            this.bConnect.ForeColor = System.Drawing.SystemColors.ControlText;
            this.bConnect.Location = new System.Drawing.Point(151, 8);
            this.bConnect.Name = "bConnect";
            this.bConnect.Size = new System.Drawing.Size(75, 23);
            this.bConnect.TabIndex = 8;
            this.bConnect.Text = "Connect";
            this.bConnect.UseVisualStyleBackColor = true;
            this.bConnect.Click += new System.EventHandler(this.bConnect_Click);
            // 
            // ttChangeTracking
            // 
            this.ttChangeTracking.ToolTipTitle = "Change Tracking";
            // 
            // ttUseCustomScript
            // 
            this.ttUseCustomScript.ToolTipTitle = "Use Custom Script";
            // 
            // ttVerbose
            // 
            this.ttVerbose.ToolTipTitle = "Verbose";
            // 
            // tpResults
            // 
            this.tpResults.Controls.Add(this.bCancelDebugging);
            this.tpResults.Controls.Add(this.bExportXML);
            this.tpResults.Controls.Add(this.bGenerateCustomScript);
            this.tpResults.Controls.Add(this.bShowOnlyEffective);
            this.tpResults.Controls.Add(this.dgResults);
            this.tpResults.Location = new System.Drawing.Point(4, 28);
            this.tpResults.Name = "tpResults";
            this.tpResults.Size = new System.Drawing.Size(713, 419);
            this.tpResults.TabIndex = 4;
            this.tpResults.Text = "Results";
            this.tpResults.UseVisualStyleBackColor = true;
            // 
            // bCancelDebugging
            // 
            this.bCancelDebugging.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bCancelDebugging.Location = new System.Drawing.Point(580, 393);
            this.bCancelDebugging.Name = "bCancelDebugging";
            this.bCancelDebugging.Size = new System.Drawing.Size(130, 23);
            this.bCancelDebugging.TabIndex = 15;
            this.bCancelDebugging.Text = "Cancel";
            this.bCancelDebugging.UseVisualStyleBackColor = true;
            this.bCancelDebugging.Click += new System.EventHandler(this.bCancelDebugging_Click);
            // 
            // bExportXML
            // 
            this.bExportXML.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bExportXML.Location = new System.Drawing.Point(305, 393);
            this.bExportXML.Name = "bExportXML";
            this.bExportXML.Size = new System.Drawing.Size(130, 23);
            this.bExportXML.TabIndex = 14;
            this.bExportXML.Text = "Export XML";
            this.bExportXML.UseVisualStyleBackColor = true;
            this.bExportXML.Click += new System.EventHandler(this.bExportXML_Click);
            // 
            // bGenerateCustomScript
            // 
            this.bGenerateCustomScript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bGenerateCustomScript.Location = new System.Drawing.Point(142, 393);
            this.bGenerateCustomScript.Name = "bGenerateCustomScript";
            this.bGenerateCustomScript.Size = new System.Drawing.Size(157, 23);
            this.bGenerateCustomScript.TabIndex = 13;
            this.bGenerateCustomScript.Text = "Generate Custom Script";
            this.bGenerateCustomScript.UseVisualStyleBackColor = true;
            this.bGenerateCustomScript.Click += new System.EventHandler(this.bGenerateCustomScript_Click);
            // 
            // bShowOnlyEffective
            // 
            this.bShowOnlyEffective.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bShowOnlyEffective.Location = new System.Drawing.Point(6, 393);
            this.bShowOnlyEffective.Name = "bShowOnlyEffective";
            this.bShowOnlyEffective.Size = new System.Drawing.Size(130, 23);
            this.bShowOnlyEffective.TabIndex = 12;
            this.bShowOnlyEffective.Text = "Show only Effective";
            this.bShowOnlyEffective.UseVisualStyleBackColor = true;
            this.bShowOnlyEffective.Click += new System.EventHandler(this.bShowOnlyEffective_Click);
            // 
            // dgResults
            // 
            this.dgResults.AllowUserToAddRows = false;
            this.dgResults.AllowUserToDeleteRows = false;
            this.dgResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgResults.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgResults.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgResults.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgResults.Location = new System.Drawing.Point(6, 6);
            this.dgResults.Name = "dgResults";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgResults.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgResults.RowHeadersWidth = 30;
            this.dgResults.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgResults.RowTemplate.Height = 18;
            this.dgResults.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgResults.Size = new System.Drawing.Size(704, 381);
            this.dgResults.TabIndex = 0;
            this.dgResults.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgResults_CellContentClick);
            this.dgResults.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgResults_CellValueChanged);
            // 
            // tpScript
            // 
            this.tpScript.Controls.Add(this.bParseScript);
            this.tpScript.Controls.Add(this.tbScript);
            this.tpScript.Controls.Add(this.bDebugScript2);
            this.tpScript.Controls.Add(this.cbUseCustomScript2);
            this.tpScript.Controls.Add(this.bResetScript);
            this.tpScript.Location = new System.Drawing.Point(4, 28);
            this.tpScript.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
            this.tpScript.Name = "tpScript";
            this.tpScript.Size = new System.Drawing.Size(713, 419);
            this.tpScript.TabIndex = 2;
            this.tpScript.Text = "MDX Script";
            this.tpScript.UseVisualStyleBackColor = true;
            // 
            // bParseScript
            // 
            this.bParseScript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bParseScript.Location = new System.Drawing.Point(444, 393);
            this.bParseScript.Name = "bParseScript";
            this.bParseScript.Size = new System.Drawing.Size(130, 23);
            this.bParseScript.TabIndex = 13;
            this.bParseScript.Text = "Parse MDX Script";
            this.bParseScript.UseVisualStyleBackColor = true;
            this.bParseScript.Click += new System.EventHandler(this.bParseScript_Click);
            // 
            // tbScript
            // 
            this.tbScript.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbScript.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbScript.Location = new System.Drawing.Point(6, 6);
            this.tbScript.MaxLength = 200000;
            this.tbScript.Multiline = true;
            this.tbScript.Name = "tbScript";
            this.tbScript.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbScript.Size = new System.Drawing.Size(704, 381);
            this.tbScript.TabIndex = 12;
            this.tbScript.WordWrap = false;
            // 
            // bDebugScript2
            // 
            this.bDebugScript2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bDebugScript2.Location = new System.Drawing.Point(580, 393);
            this.bDebugScript2.Name = "bDebugScript2";
            this.bDebugScript2.Size = new System.Drawing.Size(130, 23);
            this.bDebugScript2.TabIndex = 11;
            this.bDebugScript2.Text = "Debug MDX Script";
            this.bDebugScript2.UseVisualStyleBackColor = true;
            this.bDebugScript2.Click += new System.EventHandler(this.bDebugScript_Click);
            // 
            // cbUseCustomScript2
            // 
            this.cbUseCustomScript2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbUseCustomScript2.AutoSize = true;
            this.cbUseCustomScript2.Location = new System.Drawing.Point(10, 397);
            this.cbUseCustomScript2.Name = "cbUseCustomScript2";
            this.cbUseCustomScript2.Size = new System.Drawing.Size(113, 17);
            this.cbUseCustomScript2.TabIndex = 10;
            this.cbUseCustomScript2.Text = "Use Custom Script";
            this.cbUseCustomScript2.UseVisualStyleBackColor = true;
            this.cbUseCustomScript2.CheckedChanged += new System.EventHandler(this.cbUseCustomScript2_CheckedChanged);
            this.cbUseCustomScript2.Click += new System.EventHandler(this.cbUseCustomScript2_CheckedChanged);
            // 
            // bResetScript
            // 
            this.bResetScript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.bResetScript.Location = new System.Drawing.Point(127, 393);
            this.bResetScript.Name = "bResetScript";
            this.bResetScript.Size = new System.Drawing.Size(119, 23);
            this.bResetScript.TabIndex = 9;
            this.bResetScript.Text = "Reset to Original";
            this.bResetScript.UseVisualStyleBackColor = true;
            this.bResetScript.Click += new System.EventHandler(this.bResetScript_Click);
            // 
            // tpQuery
            // 
            this.tpQuery.Controls.Add(this.cbWarmCacheExecution);
            this.tpQuery.Controls.Add(this.cbClearCache);
            this.tpQuery.Controls.Add(this.cbCreateFirst);
            this.tpQuery.Controls.Add(this.cbUseCustomScript1);
            this.tpQuery.Controls.Add(this.cbChangeTracking);
            this.tpQuery.Controls.Add(this.cbVerbose);
            this.tpQuery.Controls.Add(this.bDebugScript1);
            this.tpQuery.Controls.Add(this.tbQuery);
            this.tpQuery.Location = new System.Drawing.Point(4, 28);
            this.tpQuery.Name = "tpQuery";
            this.tpQuery.Padding = new System.Windows.Forms.Padding(3);
            this.tpQuery.Size = new System.Drawing.Size(713, 419);
            this.tpQuery.TabIndex = 0;
            this.tpQuery.Text = "Query";
            this.tpQuery.UseVisualStyleBackColor = true;
            // 
            // cbCreateFirst
            // 
            this.cbCreateFirst.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbCreateFirst.AutoSize = true;
            this.cbCreateFirst.Location = new System.Drawing.Point(298, 372);
            this.cbCreateFirst.Name = "cbCreateFirst";
            this.cbCreateFirst.Size = new System.Drawing.Size(88, 17);
            this.cbCreateFirst.TabIndex = 12;
            this.cbCreateFirst.Text = "CREATE first";
            this.cbCreateFirst.UseVisualStyleBackColor = true;
            // 
            // cbUseCustomScript1
            // 
            this.cbUseCustomScript1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbUseCustomScript1.AutoSize = true;
            this.cbUseCustomScript1.Location = new System.Drawing.Point(10, 397);
            this.cbUseCustomScript1.Name = "cbUseCustomScript1";
            this.cbUseCustomScript1.Size = new System.Drawing.Size(113, 17);
            this.cbUseCustomScript1.TabIndex = 10;
            this.cbUseCustomScript1.Text = "Use Custom Script";
            this.cbUseCustomScript1.UseVisualStyleBackColor = true;
            this.cbUseCustomScript1.CheckedChanged += new System.EventHandler(this.cbUseCustomScript1_CheckedChanged);
            this.cbUseCustomScript1.Click += new System.EventHandler(this.cbUseCustomScript1_CheckedChanged);
            // 
            // cbChangeTracking
            // 
            this.cbChangeTracking.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbChangeTracking.AutoSize = true;
            this.cbChangeTracking.Checked = true;
            this.cbChangeTracking.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbChangeTracking.Location = new System.Drawing.Point(147, 372);
            this.cbChangeTracking.Name = "cbChangeTracking";
            this.cbChangeTracking.Size = new System.Drawing.Size(108, 17);
            this.cbChangeTracking.TabIndex = 11;
            this.cbChangeTracking.Text = "Change Tracking";
            this.cbChangeTracking.UseVisualStyleBackColor = true;
            this.cbChangeTracking.CheckedChanged += new System.EventHandler(this.cbValueTracking_CheckedChanged);
            // 
            // cbVerbose
            // 
            this.cbVerbose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbVerbose.AutoSize = true;
            this.cbVerbose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.cbVerbose.Location = new System.Drawing.Point(462, 397);
            this.cbVerbose.Name = "cbVerbose";
            this.cbVerbose.Size = new System.Drawing.Size(65, 17);
            this.cbVerbose.TabIndex = 14;
            this.cbVerbose.Text = "Verbose";
            this.cbVerbose.UseVisualStyleBackColor = false;
            this.cbVerbose.Visible = false;
            // 
            // bDebugScript1
            // 
            this.bDebugScript1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bDebugScript1.Location = new System.Drawing.Point(580, 393);
            this.bDebugScript1.Name = "bDebugScript1";
            this.bDebugScript1.Size = new System.Drawing.Size(130, 23);
            this.bDebugScript1.TabIndex = 15;
            this.bDebugScript1.Text = "Debug MDX Script";
            this.bDebugScript1.UseVisualStyleBackColor = true;
            this.bDebugScript1.Click += new System.EventHandler(this.bDebugScript_Click);
            // 
            // tbQuery
            // 
            this.tbQuery.AcceptsReturn = true;
            this.tbQuery.AcceptsTab = true;
            this.tbQuery.AllowDrop = true;
            this.tbQuery.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbQuery.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbQuery.ImeMode = System.Windows.Forms.ImeMode.On;
            this.tbQuery.Location = new System.Drawing.Point(6, 6);
            this.tbQuery.Multiline = true;
            this.tbQuery.Name = "tbQuery";
            this.tbQuery.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbQuery.Size = new System.Drawing.Size(704, 360);
            this.tbQuery.TabIndex = 6;
            this.tbQuery.WordWrap = false;
            this.tbQuery.TextChanged += new System.EventHandler(this.tbQuery_TextChanged);
            // 
            // tcMain
            // 
            this.tcMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcMain.Controls.Add(this.tpQuery);
            this.tcMain.Controls.Add(this.tpScript);
            this.tcMain.Controls.Add(this.tpResults);
            this.tcMain.Location = new System.Drawing.Point(0, 37);
            this.tcMain.Name = "tcMain";
            this.tcMain.Padding = new System.Drawing.Point(6, 6);
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(721, 451);
            this.tcMain.TabIndex = 9;
            // 
            // ttExport
            // 
            this.ttExport.ToolTipTitle = "Export to XML";
            // 
            // ttShowOnlyEffective
            // 
            this.ttShowOnlyEffective.ToolTipTitle = "Show only Effective";
            // 
            // ttCreateStmtFirst
            // 
            this.ttCreateStmtFirst.ToolTipTitle = "CREATE Statements first";
            // 
            // ttGenerateCustomScript
            // 
            this.ttGenerateCustomScript.ToolTipTitle = "Generate Custom Script";
            // 
            // ttCreateFirst
            // 
            this.ttCreateFirst.ToolTipTitle = "CREATE first";
            // 
            // ssStatusBar
            // 
            this.ssStatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lDummy,
            this.lStatus,
            this.pbCommands,
            this.lCommandCount});
            this.ssStatusBar.Location = new System.Drawing.Point(0, 491);
            this.ssStatusBar.Name = "ssStatusBar";
            this.ssStatusBar.Size = new System.Drawing.Size(721, 22);
            this.ssStatusBar.TabIndex = 10;
            // 
            // lDummy
            // 
            this.lDummy.Name = "lDummy";
            this.lDummy.Size = new System.Drawing.Size(470, 17);
            this.lDummy.Spring = true;
            // 
            // pbCommands
            // 
            this.pbCommands.AutoSize = false;
            this.pbCommands.Name = "pbCommands";
            this.pbCommands.Size = new System.Drawing.Size(116, 16);
            // 
            // lCommandCount
            // 
            this.lCommandCount.AutoSize = false;
            this.lCommandCount.Name = "lCommandCount";
            this.lCommandCount.Size = new System.Drawing.Size(118, 17);
            // 
            // cbClearCache
            // 
            this.cbClearCache.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbClearCache.AutoSize = true;
            this.cbClearCache.Checked = true;
            this.cbClearCache.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbClearCache.Location = new System.Drawing.Point(10, 372);
            this.cbClearCache.Name = "cbClearCache";
            this.cbClearCache.Size = new System.Drawing.Size(84, 17);
            this.cbClearCache.TabIndex = 13;
            this.cbClearCache.Text = "Clear Cache";
            this.cbClearCache.UseVisualStyleBackColor = true;
            // 
            // ttClearCache
            // 
            this.ttClearCache.ToolTipTitle = "Clear Cache";
            // 
            // cbWarmCacheExecution
            // 
            this.cbWarmCacheExecution.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbWarmCacheExecution.AutoSize = true;
            this.cbWarmCacheExecution.Checked = true;
            this.cbWarmCacheExecution.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbWarmCacheExecution.Location = new System.Drawing.Point(147, 397);
            this.cbWarmCacheExecution.Name = "cbWarmCacheExecution";
            this.cbWarmCacheExecution.Size = new System.Drawing.Size(138, 17);
            this.cbWarmCacheExecution.TabIndex = 16;
            this.cbWarmCacheExecution.Text = "Warm Cache Execution";
            this.cbWarmCacheExecution.UseVisualStyleBackColor = true;
            // 
            // MdxScriptDebuggerGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(721, 513);
            this.Controls.Add(this.ssStatusBar);
            this.Controls.Add(this.tcMain);
            this.Controls.Add(this.bConnect);
            this.Controls.Add(this.cbCube);
            this.Controls.Add(this.lCube);
            this.Controls.Add(this.lDatabase);
            this.Controls.Add(this.cbDatabase);
            this.Controls.Add(this.lServer);
            this.Controls.Add(this.tbServer);
            this.MinimumSize = new System.Drawing.Size(737, 552);
            this.Name = "MdxScriptDebuggerGUI";
            this.Text = "MDX Script Debugger by Gerhard Brueckl";
            this.tpResults.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgResults)).EndInit();
            this.tpScript.ResumeLayout(false);
            this.tpScript.PerformLayout();
            this.tpQuery.ResumeLayout(false);
            this.tpQuery.PerformLayout();
            this.tcMain.ResumeLayout(false);
            this.ssStatusBar.ResumeLayout(false);
            this.ssStatusBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbServer;
        private System.Windows.Forms.Label lServer;
        private System.Windows.Forms.ComboBox cbDatabase;
        private System.Windows.Forms.Label lDatabase;
        private System.Windows.Forms.Label lCube;
        private System.Windows.Forms.ComboBox cbCube;
        private System.Windows.Forms.Button bConnect;
        private System.Windows.Forms.ToolTip ttChangeTracking;
        private System.Windows.Forms.ToolTip ttUseCustomScript;
        private System.Windows.Forms.ToolTip ttVerbose;
        private System.Windows.Forms.TabPage tpResults;
        private System.Windows.Forms.Button bGenerateCustomScript;
        private System.Windows.Forms.Button bShowOnlyEffective;
        private System.Windows.Forms.DataGridView dgResults;
        private System.Windows.Forms.TabPage tpScript;
        private System.Windows.Forms.TextBox tbScript;
        private System.Windows.Forms.Button bDebugScript2;
        private System.Windows.Forms.CheckBox cbUseCustomScript2;
        private System.Windows.Forms.Button bResetScript;
        private System.Windows.Forms.TabPage tpQuery;
        private System.Windows.Forms.CheckBox cbUseCustomScript1;
        private System.Windows.Forms.CheckBox cbChangeTracking;
        private System.Windows.Forms.CheckBox cbVerbose;
        private System.Windows.Forms.Button bDebugScript1;
        private System.Windows.Forms.TextBox tbQuery;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.Button bExportXML;
        private System.Windows.Forms.ToolTip ttExport;
        private System.Windows.Forms.ToolTip ttShowOnlyEffective;
        private System.Windows.Forms.ToolTip ttCreateStmtFirst;
        private System.Windows.Forms.ToolTip ttGenerateCustomScript;
        private System.Windows.Forms.CheckBox cbCreateFirst;
        private System.Windows.Forms.ToolTip ttCreateFirst;
        private System.Windows.Forms.StatusStrip ssStatusBar;
        private System.Windows.Forms.ToolStripProgressBar pbCommands;
        private System.Windows.Forms.ToolStripStatusLabel lCommandCount;
        private System.Windows.Forms.ToolStripStatusLabel lDummy;
        private System.Windows.Forms.ToolStripStatusLabel lStatus;
        private System.Windows.Forms.Button bParseScript;
        private System.Windows.Forms.Button bCancelDebugging;
        private System.Windows.Forms.CheckBox cbClearCache;
        private System.Windows.Forms.ToolTip ttClearCache;
        private System.Windows.Forms.CheckBox cbWarmCacheExecution;
    }
}