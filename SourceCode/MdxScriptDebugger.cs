using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AnalysisServices;
using Microsoft.AnalysisServices.AdomdClient;
using System.Data.OleDb;
using System.Timers;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml;
using System.Data;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;

namespace gbrueckl.AnalysisServices.MdxScriptDebugger
{
    public class MdxScriptDebugger : BackgroundWorker
    {
        #region constants
        public const string ColumnNameVsPrev = "vs.Prev";
        public const string ColumnNameVsRef = "vs.Ref";
        #endregion
        #region class variables
        private string _serverName;
        private string _dbName;
        private string _cubeName;
        private bool _verbose;
        private bool _useScopeTracking;
        private bool _useValueTracking;
        private bool _printPerformanceAnalysis;
        private bool _createMembersSetsFirst;
        private bool _ignoreErrors;
        private bool _clearCache;
        private bool _preExecRef_Cold;
        private bool _preExecRef_Warm;
        private bool _preExecRef_NoCalcs;
        private bool _postExecRef;
        private bool _warmCacheExecution;
        private MdxScriptCommand _preExecCmd_Cold;
        private MdxScriptCommand _preExecCmd_Warm;
        private MdxScriptCommand _preExecCmd_NoCalcs;
        private MdxScriptCommand _postExecCmd;
        private MdxScriptCommand _clearCacheCmd;
        private string _currentSsasSessionID;
        private bool _executionCancelled;
        private bool _executionRunning;
        private string _query;
        private int _queryCommandTimeout;

        private SortedDictionary<int, MdxScriptCommand> _commands;
        private List<int> usedScopes = new List<int>();
        #endregion

        #region Properties
        private string ConnectionString
        {
            get
            {
                return "Data Source=" + _serverName + ";Initial Catalog=" + _dbName + ";Cube=" + _cubeName;
            }
        }
        public bool Verbose { get { return _verbose; } set { _verbose = value; } }
        public bool ScopeTracking { get { return _useScopeTracking; } set { _useScopeTracking = value; } }
        public bool ValueTracking { get { return _useValueTracking; } set { _useValueTracking = value; } }
        public bool PrintPerformanceAnalysis { get { return _printPerformanceAnalysis; } set { _printPerformanceAnalysis = value; } }
        public bool CreateMembersAndSetsFirst { get { return _createMembersSetsFirst; } set { _createMembersSetsFirst = value; } }
        public bool ClearCache
        {
            get
            {
                return _clearCache;
            }
            set
            {
                _clearCache = value;

                if (_clearCache)
                {
                    _clearCacheCmd = MdxScriptCommand.ReferenceQuery;
                    _clearCacheCmd.CommandText = "/* CLEAR CACHE */";
                    _clearCacheCmd.CommandNumber = -20000;
                }
            }
        }
        public bool PreExecuteQueryColdCache
        {
            get
            {

                return _preExecRef_Cold;
            }
            set
            {
                _preExecRef_Cold = value;

                if (_preExecRef_Cold)
                {
                    _preExecCmd_Cold = MdxScriptCommand.ReferenceQuery;
                    _preExecCmd_Cold.CommandNumber = -10050;
                    _preExecCmd_Cold.CommandText = "/* Pre-Execute Reference-Query on Cold Cache */";
                }
                else
                    _preExecCmd_Cold = null;
            }
        }
        public bool PreExecuteQueryWarmCache
        {
            get
            {
                return _preExecRef_Warm;
            }
            set
            {
                _preExecRef_Warm = value;

                if (_preExecRef_Warm)
                {
                    _preExecCmd_Warm = MdxScriptCommand.ReferenceQuery;
                    _preExecCmd_Warm.CommandNumber = -10040;
                    _preExecCmd_Warm.CommandText = "/* Pre-Execute Reference-Query on Warm Cache */";
                }
                else
                    _preExecCmd_Warm = null;
            }
        }
        public bool PreExecuteQueryNoCalcs
        {
            get
            {
                return _preExecRef_NoCalcs;
            }
            set
            {
                _preExecRef_NoCalcs = value;

                if (_preExecRef_NoCalcs)
                {
                    _preExecCmd_NoCalcs = MdxScriptCommand.ReferenceQuery;
                    _preExecCmd_NoCalcs.CommandNumber = -10030;
                    _preExecCmd_NoCalcs.CommandText = "/* Pre-Execute Reference-Query without Script */";
                }
                else
                    _preExecCmd_NoCalcs = null;
            }
        }
        public bool PostExecuteQuery
        {
            get
            {
                return _postExecRef;
            }
            set
            {
                _postExecRef = value;

                if (_postExecRef)
                {
                    _postExecCmd = MdxScriptCommand.ReferenceQuery;
                    _postExecCmd.CommandNumber = -10000;
                    _postExecCmd.CommandText = "/* Pre-Execute Reference-Query with Script */";
                }
                else
                    _postExecCmd = null;
            }
        }
        public bool WarmCacheExecution
        {
            get
            {
                return _warmCacheExecution;
            }
            set
            {
                _warmCacheExecution = value;
                this.PreExecuteQueryWarmCache = value;
            }
        }
        public bool IgnoreErrors { get { return _ignoreErrors; } set { _ignoreErrors = value; } }
        public string Query { get { return _query; } set { _query = value; } }
        public int QueryCommandTimeout { get { return _queryCommandTimeout; } set { _queryCommandTimeout = value; } }
        public int CommandCount
        {
            get
            {
                if (_commands != null)
                    return _commands.Count;

                return 0;
            }
        }       
        #endregion

        #region BackgroundWorker Methods

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            base.OnDoWork(e);

            if (!string.IsNullOrEmpty(_query))
            {
                _executionRunning = true;
                _executionCancelled = false;
                DebugMdxScript();
            }
        }

        protected override void OnProgressChanged(ProgressChangedEventArgs e)
        {
            base.OnProgressChanged(e);
        }

        protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        {
            // always return Cancelled=false in order to be able to pass a ResultSet out
            base.OnRunWorkerCompleted(new RunWorkerCompletedEventArgs(this.GetResultTable(), e.Error, false));
            _executionRunning = false;

            _currentSsasSessionID = null;
        }

        public void CancelAsyncWithKill()
        {
            _executionCancelled = true;
            base.CancelAsync();

            if (_executionRunning && !string.IsNullOrEmpty(_currentSsasSessionID))
            {
                using (AdomdConnection con = new AdomdConnection(this.ConnectionString))
                {
                    con.Open();
                    // ignore errors as the session may already has been cancelled or finished
                    MdxScriptHelper.ExecuteXMLA(con, MdxScriptHelper.CancelSessionCommand(_currentSsasSessionID), true);
                }
            }
        } 
        #endregion
        #region Constructions
        public MdxScriptDebugger(string servername, string dbname, string cubename)
        {
            _serverName = servername;
            _dbName = dbname;
            _cubeName = cubename;
            _executionRunning = false;
            _executionCancelled = false;

            Verbose = false;
            ScopeTracking = false;
            ValueTracking = false;
            PrintPerformanceAnalysis = false;
            CreateMembersAndSetsFirst = false;
            ClearCache = false;
            PreExecuteQueryColdCache = true;
            PreExecuteQueryNoCalcs = true;
            PostExecuteQuery = true;
            WarmCacheExecution = false;
            IgnoreErrors = false;
            QueryCommandTimeout = -1;

            _commands = new SortedDictionary<int, MdxScriptCommand>();

            base.WorkerSupportsCancellation = true;
            base.WorkerReportsProgress = true;

            Server server = new Server();
            server.Connect(this.ConnectionString);

            Database db = server.Databases.FindByName(_dbName);
            Cube cube = db.Cubes.FindByName(_cubeName);

            _commands = MdxScriptHelper.GetCommandsFromScript(cube.DefaultMdxScript);
        }
        #endregion

        #region Private Methods
        private void CheckQuery(string query)
        {
            string msg = "";
            //check if query contains cube-name
            if (!Regex.IsMatch(query, ".*(?:FROM)\\s*(?:" + _cubeName + ").*", RegexOptions.IgnoreCase)
                && !Regex.IsMatch(query, ".*(?:FROM)\\s*\\[(?:" + _cubeName + ")\\].*", RegexOptions.IgnoreCase))
            {
                msg = msg + "Reference Query does not match the selected Cube!" + Environment.NewLine + "Please select correct cube or fix the query!";
                
            }

            // check query syntax
            try
            {
                using (AdomdConnection con = new AdomdConnection(this.ConnectionString))
                {
                    con.Open();
                    using (AdomdCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = query;

                        cmd.Prepare();
                    }
                }
            }
            catch (Exception e)
            {
                msg = msg + Environment.NewLine + Environment.NewLine + e.Message;

                if (!this.CreateMembersAndSetsFirst)
                {
                    msg = msg + Environment.NewLine + Environment.NewLine + "If your query uses calculated members or sets you may use [CREATE first]";
                }
            }

            if (!string.IsNullOrEmpty(msg))
            {
                throw new Exception(msg.Trim());
            }
        }
        #endregion

        #region Public Methods
        public void LoadCustomMdxScript(string customScript)
        {
            _commands.Clear();
            _commands = MdxScriptHelper.GetCommandsFromText(customScript);
        }

        public void DebugMdxScript()
        {
            DebugMdxScript(null);
        }
        public void DebugMdxScript(string query)
        {
            MdxScriptCommand clearCalcs = MdxScriptCommand.ClearCalculationsCommand;
            MdxScriptCommand cmdRec;

            if (string.IsNullOrEmpty(query) && !string.IsNullOrEmpty(_query))
                query = _query;

            query = query.Trim().TrimEnd(';');

            CheckQuery(query);

            using (AdomdConnection con = new AdomdConnection(this.ConnectionString))
            {
                con.Open();
                _currentSsasSessionID = con.SessionID;

                if (this.ClearCache)
                {
                    ReportProgress(0, new UserState("Clearing Cache ...", _clearCacheCmd, StateChangeType.ExecutionStarted));
                    MdxScriptHelper.ExecuteXMLA(con, MdxScriptHelper.ClearCacheCommand(_dbName, _cubeName));
                    ReportProgress(0, new UserState("Done!", _clearCacheCmd, StateChangeType.ExecutionFinished));
                }

                if (this.CreateMembersAndSetsFirst)
                    PreExecuteCreateStatements();

                // execute reference query on cold cache
                if (this.PreExecuteQueryColdCache && _preExecCmd_Cold != null)
                {
                    ReportProgress(0, new UserState("Running Reference Query (Cold Cache) ...", _preExecCmd_Cold, StateChangeType.ExecutionStarted));
                    _preExecCmd_Cold.ExecuteTestQuery(con, query, _queryCommandTimeout);
                    ReportProgress(0, new UserState("Done!", _preExecCmd_Cold, StateChangeType.ExecutionFinished));
                }

                // execute reference query on warm cache
                if (this.PreExecuteQueryWarmCache && _preExecCmd_Warm != null)
                {
                    ReportProgress(0, new UserState("Running Reference Query (Cold Cache) ...", _preExecCmd_Warm, StateChangeType.ExecutionStarted));
                    _preExecCmd_Warm.ExecuteTestQuery(con, query, _queryCommandTimeout);
                    ReportProgress(0, new UserState("Done!", _preExecCmd_Warm, StateChangeType.ExecutionFinished));
                }

                // clear existing calculations
                ReportProgress(0, new UserState("Running CLEAR CALCULATIONS ...", StateChangeType.PreExecute));
                MdxScriptCommand.ClearCalculationsCommand.Execute(con);

                // execute reference query without calculations
                if (this.PreExecuteQueryNoCalcs && _preExecCmd_NoCalcs != null)
                {
                    ReportProgress(0, new UserState("Running Reference Query (No Script) ...", _preExecCmd_NoCalcs, StateChangeType.ExecutionStarted));
                    _preExecCmd_NoCalcs.ExecuteTestQuery(con, query, _queryCommandTimeout);
                    ReportProgress(0, new UserState("Done!", _preExecCmd_NoCalcs, StateChangeType.ExecutionFinished));
                }
                
                if(this.ValueTracking)
                {
                    MdxScriptCommand.CreateValueTrackingCommand.Execute(con);
                    query = MdxScriptHelper.EnsureCellProperty(query, MdxScriptCommand.CellPropertyValueTracking);
                }

                foreach (MdxScriptCommand cmd in _commands.Values)
                {
                    if (base.CancellationPending)
                    {
                        _executionCancelled = true;
                        return;
                    }

                    ReportProgress((cmd.CommandNumber * 100) / this.CommandCount, new UserState("Executing MDX Script command ...", cmd, StateChangeType.ExecutionStarted));
                    cmd.Execute(con);
                    ReportProgress((cmd.CommandNumber * 100) / this.CommandCount, new UserState("Done!", cmd, StateChangeType.ExecutionFinished));

                    

                    // execute the query if the current MdxScriptCommand is PerformanceRelevant or Verbose
                    if (cmd.IsPerformanceRelevant || this.Verbose)
                    {
                        if (this.ValueTracking)
                        {
                            MdxScriptCommand.DropValueTrackingCommand.Execute(con);
                            MdxScriptCommand.CreateValueTrackingCommand.Execute(con);
                        }

                        ReportProgress((cmd.CommandNumber * 100) / this.CommandCount, new UserState("Running Query ...", cmd, StateChangeType.ExecutionStarted));
                        cmd.ExecuteTestQuery(con, query, _queryCommandTimeout);

                        if (this.ValueTracking)
                        {
                            List<string> currentCellProperties = MdxScriptHelper.GetDistinctCellProperties(cmd.Results, MdxScriptCommand.CellPropertyValueTracking);
                            if (currentCellProperties.Contains(MdxScriptCommand.ValueChangeValueTracking))
                            {
                                cmd.IsEffective = true;

                                cmdRec = cmd;
                                // also set all parent SCOPE commands and their END SCOPE as Effective
                                while (cmdRec.ParentCommand != null)
                                {
                                    cmdRec.ParentCommand.IsEffective = true;
                                    cmdRec.ParentCommand.CorrespondingEndScopeCommand.IsEffective = true;

                                    cmdRec = cmdRec.ParentCommand;
                                }
                            }
                        }

                        ReportProgress((cmd.CommandNumber * 100) / this.CommandCount, new UserState("Done!", cmd, StateChangeType.ExecutionFinished));
                    }
                    else
                    {
                        // report finished if not yet reported due to IsPerformanceRelevant
                        ReportProgress((cmd.CommandNumber * 100) / this.CommandCount, new UserState("Done!", cmd, StateChangeType.ExecutionFinished));
                    }
                }

                // execute reference query with all calculations again
                if (this.PostExecuteQuery && _postExecCmd != null)
                {
                    if (this.ClearCache)
                    {
                        ReportProgress(0, new UserState("Clearing Cache ...", _clearCacheCmd, StateChangeType.ExecutionStarted));
                        MdxScriptHelper.ExecuteXMLA(con, MdxScriptHelper.ClearCacheCommand(_dbName, _cubeName));
                        ReportProgress(0, new UserState("Done!", _clearCacheCmd, StateChangeType.ExecutionFinished));
                    }

                    ReportProgress(0, new UserState("Running Query (No Script) ...", _postExecCmd, StateChangeType.ExecutionStarted));
                    _postExecCmd.ExecuteTestQuery(con, query, _queryCommandTimeout);
                    ReportProgress(0, new UserState("Done!", _postExecCmd, StateChangeType.ExecutionFinished));
                }
            }
            _currentSsasSessionID = null;
        }
        public List<MdxScriptCommand> GetAllCommands()
        {
            List<MdxScriptCommand> ret = new List<MdxScriptCommand>();
            foreach (MdxScriptCommand cmd in _commands.Values)
            {
                ret.Add(cmd);
            }
            return ret;
        }
        public List<MdxScriptCommand> GetEffectiveCommands()
        {
            List<MdxScriptCommand> ret = new List<MdxScriptCommand>();

            foreach (MdxScriptCommand cmd in _commands.Values)
            {
                if (cmd.IsEffective)
                    ret.Add(cmd);
            }

            return ret;
        }

        public void PreExecuteCreateStatements()
        {
            SortedDictionary<int, MdxScriptCommand> creates = new SortedDictionary<int, MdxScriptCommand>();

            foreach (KeyValuePair<int, MdxScriptCommand> kvp in _commands)
            {
                if (kvp.Value.IsCreateStatement)
                    creates.Add(kvp.Key, kvp.Value);
            }

            int i = creates.Count * -1 - 100;

            foreach(KeyValuePair<int, MdxScriptCommand> kvp in creates)
            {
                using (AdomdConnection con = new AdomdConnection(this.ConnectionString))
                {

                }
                kvp.Value.CommandNumber = i;
                _commands.Add(i, kvp.Value);
                _commands.Remove(kvp.Key);

                i++;
            }
        }

        public DataTable GetResultTable()
        {
            DataTable ret = MdxScriptCommand.SampleDataTable;
            ret.TableName = "Results";

            DataRow dr;
            long durationCurrent = 0;
            long durationPrev = 0;
            long durationRefCold = 0;
            long durationRefWarm = 0;
            int currentEffective = 0;

            if (this.PreExecuteQueryColdCache && _preExecCmd_Cold != null && _executionRunning)
            {
                dr = ret.NewRow();
                dr.ItemArray = this._preExecCmd_Cold.AsDataRow;
                ret.Rows.Add(dr);
                durationRefCold = _preExecCmd_Cold.Duration;
            }

            if (this.PreExecuteQueryWarmCache && _preExecCmd_Warm != null && _executionRunning)
            {
                dr = ret.NewRow();
                dr.ItemArray = this._preExecCmd_Warm.AsDataRow;
                ret.Rows.Add(dr);
                durationRefWarm += _preExecCmd_Warm.Duration;
            }

            if (this.PreExecuteQueryNoCalcs && _preExecCmd_NoCalcs != null && _executionRunning)
            {
                dr = ret.NewRow();
                dr.ItemArray = this._preExecCmd_NoCalcs.AsDataRow;
                ret.Rows.Add(dr);
            }

            ret.Columns.Add(ColumnNameVsPrev, typeof(long));
            ret.Columns.Add(ColumnNameVsRef, typeof(long));
            

            foreach(MdxScriptCommand cmd in _commands.Values)
            {
                dr = ret.NewRow();
                dr.ItemArray = cmd.AsDataRow;

                durationCurrent = (long)dr[MdxScriptCommand.ColumnNameDuration];
                currentEffective = (int)dr[MdxScriptCommand.ColumnNameEffective];

                if (durationCurrent != -1 && currentEffective == 1)
                {
                    dr[MdxScriptDebugger.ColumnNameVsPrev] = durationCurrent - durationPrev;
                    dr[MdxScriptDebugger.ColumnNameVsRef] = durationCurrent - durationRefCold;

                    durationPrev = durationCurrent;
                }
                else
                {
                    dr[MdxScriptDebugger.ColumnNameVsPrev] = DBNull.Value;
                    dr[MdxScriptDebugger.ColumnNameVsRef] = DBNull.Value;

                    if (durationCurrent == -1)
                        dr[MdxScriptCommand.ColumnNameDuration] = DBNull.Value;
                }

                ret.Rows.Add(dr);
            }

            if (this.PostExecuteQuery && _postExecCmd != null && _executionRunning)
            {
                dr = ret.NewRow();
                dr.ItemArray = this._postExecCmd.AsDataRow;
                ret.Rows.Add(dr);
            }

            if (!_useValueTracking)
                ret.Columns.Remove(MdxScriptCommand.ColumnNameEffective);

            
            ret.Columns[ColumnNameVsPrev].SetOrdinal(4);
            ret.Columns[ColumnNameVsRef].SetOrdinal(5);

            if (!_executionRunning)
            {
                ret.Columns.Remove(MdxScriptCommand.ColumnNameDuration);
                ret.Columns.Remove(MdxScriptCommand.ColumnNameDurationWarm);
                ret.Columns.Remove(ColumnNameVsPrev);
                ret.Columns.Remove(ColumnNameVsRef);
            }

            return ret;
        }
        #endregion
    }

    #region Enums
    public enum StateChangeType
    {
        ExecutionStarted,
        ExecutionFinished,
        PreExecute,
        NotSpecified
    }
    #endregion

    #region UserState Classes
    public class UserState
    {
        public string StatusText { get; set; }
        public int CommandNumber { get; set; }
        public MdxScriptCommand Command { get; set; }
        public StateChangeType ChangeType { get; set; }

        public UserState(string statusText, MdxScriptCommand command, StateChangeType changeType)
        {
            StatusText = statusText;

            if (command != null)
                CommandNumber = command.CommandNumber;
            else
                CommandNumber = -1;

            Command = command;
            ChangeType = changeType;
        }
        public UserState(string statusText, MdxScriptCommand command) : this(statusText, command, StateChangeType.NotSpecified) { }
        public UserState(MdxScriptCommand command, StateChangeType changeType) : this(null, command, changeType) { }
        public UserState(MdxScriptCommand command) : this(null, command) { }
        public UserState(string statusText, StateChangeType changeType) : this(statusText, null, changeType) { }
        public UserState(string statusText) : this(statusText, null) { }
        
    }
    #endregion
}
