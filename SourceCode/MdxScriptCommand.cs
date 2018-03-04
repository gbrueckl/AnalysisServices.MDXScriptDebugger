using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AnalysisServices;
using System.Text.RegularExpressions;
using System.Data;
using Microsoft.AnalysisServices.AdomdClient;
using System.Diagnostics;

namespace gbrueckl.AnalysisServices.MdxScriptDebugger
{
    public class MdxScriptCommand : IComparable<MdxScriptCommand>
    {
        #region Class Variables
        private const string ValueTrackingName = "DEBUGGERHIGHLIGHCALC";
        public const string CellPropertyValueTracking = "FONT_NAME";
        public const string ValueChangeValueTracking = "Value Changed";
        public const string ColumnNameCommandNumber = "Command Number";
        public const string ColumnNameDuration = "Duration";
        public const string ColumnNameDurationWarm = "Duration Warm";
        public const string ColumnNameEffective = "Effective";
        public const string ColumnNameLevel = "Level";
        public const string ColumnNameFullCommand = "Full Command";
        public const string ColumnNameCommandType = "Command Type";


        private int _order;
        private string _text;
        private MdxScriptCommandType _commandType;
        private int _nestingLevel;
        private MdxScriptCommand _parentCommand;
        private MdxScriptCommand _correspondingEndScopeCommand;
        private bool _isEffective;
        private bool _warmCacheExecution;
        private long _duration;
        private long _durationWarm;
        private long _durationAssignment;
        private int _isPerformanceRelevant;
        private CellSet _intermediateResults;
        private bool _ignoreErrors;
        #endregion

        #region Properties
        public string CommandText { get { return _text; } set { _text = value; } }
        public int CommandNumber { get { return _order; } set { _order = value; } }
        public MdxScriptCommandType CommandType { get { return _commandType; } set { _commandType = value; } }
        public MdxScriptCommand ParentCommand { get { return _parentCommand; } set { _parentCommand = value; } }
        public MdxScriptCommand CorrespondingEndScopeCommand { get { return _correspondingEndScopeCommand; } set { _correspondingEndScopeCommand = value; } }
        public bool IsEffective { get { return _isEffective; } set { _isEffective = value; } }
        public bool IgnoreErrors { get { return _ignoreErrors; } set { _ignoreErrors = value; } }
        public int ParentCommandNumber { get { return (ParentCommand == null) ? 0 : ParentCommand.CommandNumber; } }
        public bool IsAssignment
        {
            get
            {
                return (this.CommandType == MdxScriptCommandType.CALCULATE              // Calculate-command
                        || this.CommandType == MdxScriptCommandType.THIS_ASSIGNMENT          // assignment to current subcube
                        || this.CommandType == MdxScriptCommandType.DIRECT_ASSIGNMENT    // direct assignment to subcube
                        || this.CommandType == MdxScriptCommandType.CELL_CALCULATION);    // cell calculations
            }
        }
        public bool IsPerformanceRelevant
        {
            get
            {
                if(_isPerformanceRelevant == -1)
                    return (this.IsAssignment                                               // assignemtns
                            || this.CommandType == MdxScriptCommandType.CLEAR_CALCULATIONS   // Clear Calculations-command
                            || this.CommandType == MdxScriptCommandType.FREEZE              // subcube freezes
                            || this.CommandType == MdxScriptCommandType.ALTER_CUBE);         // ALTER commands (e.g. defaultmembers)

                return (_isPerformanceRelevant != 0);
            }
            set
            {
                if (value)
                    _isPerformanceRelevant = 1;
                else
                    _isPerformanceRelevant = 0;
            }
        }
        public bool IsCreateStatement
        {
            get
            {
                return (this.CommandType == MdxScriptCommandType.CREATE_MEMBER           // Create Member
                        || this.CommandType == MdxScriptCommandType.CREATE_SET);           // Create Set
            }
        }
        public bool WarmCacheExecution { get { return _warmCacheExecution; } set { _warmCacheExecution = value; } }
        public long Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                _duration = value;
            }
        }
        public long DurationWarm
        {
            get
            {
                return _durationWarm;
            }
            set
            {
                _durationWarm = value;
            }
        }
        public long DurationAssignment
        {
            get
            {
                return _durationAssignment;
            }
            set
            {
                _durationAssignment = value;
            }
        }
        public CellSet Results
        {
            get
            {
                return _intermediateResults;
            }
        }
        public static MdxScriptCommand ClearCalculationsCommand
        {
            get
            {
                return new MdxScriptCommand("CLEAR CALCULATIONS;");
            }
        }
        public static MdxScriptCommand CreateValueTrackingCommand
        {
            get
            {
                return new MdxScriptCommand("CREATE CELL CALCULATION CURRENTCUBE." + ValueTrackingName + " FOR '*' AS '[Measures].CurrentMember' , CONDITION = CalculationPassValue( [Measures].CurrentMember , -2 , RELATIVE, ALL ) <> CalculationPassValue( [Measures].CurrentMember , -1 , RELATIVE, ALL ) , " + CellPropertyValueTracking + " = \"" + ValueChangeValueTracking + "\", Calculation_Pass_Number = -1;");
            }
        }
        public static MdxScriptCommand DropValueTrackingCommand
        {
            get
            {
                return new MdxScriptCommand("DROP CELL CALCULATION CURRENTCUBE." + ValueTrackingName + ";");
            }
        }
        public static MdxScriptCommand ReferenceQuery
        {
            get
            {
                MdxScriptCommand ret = new MdxScriptCommand("/* Reference Query Execution */");
                ret.CommandType = MdxScriptCommandType.Other;

                return ret;
            }
        }

        public static DataTable SampleDataTable
        {
            get
            {
                DataTable ret = new DataTable();
                DataColumn cmdNr;
                DataColumn[] pk = new DataColumn[1];

                cmdNr = new DataColumn(ColumnNameCommandNumber, typeof(int));
                pk[0] = cmdNr;
                ret.Columns.Add(cmdNr);
                ret.Columns.Add("Command", typeof(string));
                ret.Columns.Add(ColumnNameEffective, typeof(int));
                ret.Columns.Add(ColumnNameDuration, typeof(long));
                ret.Columns.Add(ColumnNameDurationWarm, typeof(long));
                //ret.Columns.Add(ColumnNameVsPrev, typeof(long));
                //ret.Columns.Add(ColumnNameVsRef, typeof(long));
                ret.Columns.Add("Relevant", typeof(int));
                ret.Columns.Add(ColumnNameLevel, typeof(int));
                ret.Columns.Add(ColumnNameFullCommand, typeof(string));
                ret.Columns.Add(ColumnNameCommandType, typeof(string));

                ret.PrimaryKey = pk;
                return ret;
            }
        }
        public object[] AsDataRow
        {
            get
            {
                DataRow ret = SampleDataTable.NewRow();

                ret[ColumnNameCommandNumber] = this.CommandNumber;
                if(this.CommandType == MdxScriptCommandType.Other)
                    ret["Command"] = this.ToString(MdxScriptCommandDisplayOption.Original);
                else
                    ret["Command"] = this.ToString(MdxScriptCommandDisplayOption.SingleLine_Indented);
                ret[ColumnNameEffective] = this.IsEffective;
                ret[ColumnNameDuration] = Duration;
                ret[ColumnNameDurationWarm] = DurationWarm;
                //ret[ColumnNameVsPrev] = 0;
                //ret[ColumnNameVsRef] = 0;
                ret["Relevant"] = this.IsPerformanceRelevant;
                ret[ColumnNameLevel] = this._nestingLevel;
                ret[ColumnNameFullCommand] = this.ToString(MdxScriptCommandDisplayOption.Original_Trimmed);
                ret[ColumnNameCommandType] = this.CommandType.ToString();

                return ret.ItemArray;
            }
        }
        #endregion

        #region Constructors
        public MdxScriptCommand(string cmd, int order, int nestingLevel)
        {
            _text = cmd.TrimEnd().TrimEnd(';') + ";";
            _order = order;
            _nestingLevel = nestingLevel;
            _duration = -1;
            _durationWarm = -1;
            _durationAssignment = -1;
            _warmCacheExecution = true;
            _isEffective = false;
            _isPerformanceRelevant = -1;
            _ignoreErrors = true;

            _commandType = MdxScriptHelper.ParseCommandType(_text);
        }
        public MdxScriptCommand(string cmd, int order) : this(cmd, order, 0) { }
        public MdxScriptCommand(string cmd) : this(cmd, 0, 0) { }
        public MdxScriptCommand(Command cmd, int order, int nestingLevel) : this(cmd.Text, order, nestingLevel) { }
        public MdxScriptCommand(Command cmd, int order) : this(cmd.Text, order, 0) { }
        public MdxScriptCommand(Command cmd) : this(cmd.Text, 0, 0) { }
        #endregion

        #region Private Methods

        #endregion

        #region Public Methods
        public long ExecuteTestQuery(AdomdConnection con, string testQuery, int commandTimeout, bool executeTwice)
        {
            Stopwatch timer = new Stopwatch();
            int iterations = 1;

            if (executeTwice)
                iterations = 2;

            using (AdomdCommand cmd = con.CreateCommand())
            {
                if (commandTimeout > 0)
                    cmd.CommandTimeout = commandTimeout;

                timer.Start();

                for (int i = 1; i <= iterations; i++)
                {
                    timer.Restart();

                    try
                    {
                        cmd.CommandText = testQuery;
                        _intermediateResults = cmd.ExecuteCellSet();
                    }
                    catch (Exception e)
                    {
                        if (!this.IgnoreErrors)
                            throw e;
                    }
                    timer.Stop();

                    if (i == 1) // first iteration is on cold cache
                        this.Duration = timer.ElapsedMilliseconds;
                    if (i == 2) // second iteration is on warm cache
                        this.DurationWarm = timer.ElapsedMilliseconds;
                }
            }

            return this.Duration;
        }
        public long ExecuteTestQuery(AdomdConnection con, string testQuery, int commandTimeout)
        {
            return ExecuteTestQuery(con, testQuery, commandTimeout, WarmCacheExecution);
        }
        public long ExecuteTestQuery(AdomdConnection con, string testQuery)
        {
            return ExecuteTestQuery(con, testQuery, -1);
        }
        public long Execute(AdomdConnection con)
        {
            Stopwatch timer = new Stopwatch();

            using (AdomdCommand cmd = con.CreateCommand())
            {
                timer.Start();

                try
                {
                    cmd.CommandText = this.ToString(MdxScriptCommandDisplayOption.Original_Trimmed);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    if (!this.IgnoreErrors)
                        throw e;
                }

                timer.Stop();
                _durationAssignment = timer.ElapsedMilliseconds;
            }

            return DurationAssignment;
        }
        public string ToString(MdxScriptCommandDisplayOption displayOption)
        {
            string helper;
            string ret = "";
            switch (displayOption)
            {
                case MdxScriptCommandDisplayOption.Original:
                    ret = _text;
                    break;
                case MdxScriptCommandDisplayOption.Original_Trimmed:
                    ret = _text.Trim();
                    break;
                case MdxScriptCommandDisplayOption.NoComments:
                    ret = MdxScriptHelper.StripComments(_text);
                    break;
                case MdxScriptCommandDisplayOption.NoComments_Trimmed:
                    ret = MdxScriptHelper.StripComments(_text).Trim();
                    break;
                case MdxScriptCommandDisplayOption.SingleLine:
                    helper = this.ToString(MdxScriptCommandDisplayOption.NoComments_Trimmed);
                    ret = Regex.Replace(helper, "(\r\n|\r|\n)", "");
                    break;
                case MdxScriptCommandDisplayOption.SingleLine_Indented:
                    helper = this.ToString(MdxScriptCommandDisplayOption.SingleLine);
                    ret = new string(' ', 4 * this._nestingLevel) + this.ToString(MdxScriptCommandDisplayOption.SingleLine);
                    break;
                case MdxScriptCommandDisplayOption.Console:
                    helper = this.ToString(MdxScriptCommandDisplayOption.SingleLine);
                    ret = string.Format("{0,5} | {1,10:#,0} ms  |  {2}", CommandNumber, Duration, helper.SafeLeft(50));
                    break;
                default:
                    return "";
            }

            return ret;
        }
        public string ToString(MdxScriptCommandDisplayOption displayOption, int length)
        {
            string ret = this.ToString(displayOption);

            return ret.SafeLeft(length);
        }
        public override string ToString()
        {
            return this.ToString(MdxScriptCommandDisplayOption.NoComments_Trimmed);
        }
        public static string ConsoleHeader
        {
            get { return "  Nr  |      Duration  |  Command"; }
        }
        #endregion

        #region IComparable<MdxScriptCommand>
        public int CompareTo(MdxScriptCommand compareTo)
        {
            if (this.CommandNumber == compareTo.CommandNumber)
                return 0;
            if (this.CommandNumber < compareTo.CommandNumber)
                return -1;

            return 1;
        }
        #endregion
    }
    #region Enumerators
    public enum MdxScriptCommandDisplayOption
    {
        Original,
        NoComments,
        Original_Trimmed,
        NoComments_Trimmed,
        SingleLine,
        SingleLine_Indented,
        Console
    }

    #endregion
}
