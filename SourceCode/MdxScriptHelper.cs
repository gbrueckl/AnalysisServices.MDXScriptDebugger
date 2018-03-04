using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AnalysisServices;
using System.Data;
using Microsoft.AnalysisServices.AdomdClient;
using System.Diagnostics;

namespace gbrueckl.AnalysisServices.MdxScriptDebugger
{
    static class MdxScriptHelper
    {
        #region Public Methods
        public static MdxScriptCommandType ParseCommandType(string command)
        {
            string helper = MdxScriptHelper.StripComments(command).ToUpper().Trim();

            if (helper.StartsWith("CALCULATE"))
                return MdxScriptCommandType.CALCULATE;
            else if (helper.StartsWith("CLEAR CALCULATIONS"))
                return MdxScriptCommandType.CLEAR_CALCULATIONS;
            else if (helper.StartsWith("ALTER"))
                return MdxScriptCommandType.ALTER_CUBE;
            else if (helper.StartsWith("SCOPE"))
                return MdxScriptCommandType.SCOPE;
            else if (helper.StartsWith("END SCOPE"))
                return MdxScriptCommandType.END_SCOPE;
            else if (helper.StartsWith("THIS"))
                return MdxScriptCommandType.THIS_ASSIGNMENT;
            else if (helper.StartsWith("("))    // ([Dim].[Hier].&[Key1]) = 1;
                return MdxScriptCommandType.DIRECT_ASSIGNMENT;
            else if (helper.StartsWith("["))    // [Dim].[Hier].&[Key1] = 1;
                return MdxScriptCommandType.DIRECT_ASSIGNMENT;
            else if (helper.StartsWith("CREATE MEMBER"))
                return MdxScriptCommandType.CREATE_MEMBER;
            else if (helper.StartsWith("CREATE CELL CALCULATION"))
                return MdxScriptCommandType.CELL_CALCULATION;
            else if (helper.StartsWith("CREATE"))
                return MdxScriptCommandType.CREATE_SET;
            else if (helper.StartsWith("FREEZE"))
                return MdxScriptCommandType.FREEZE;
            else if (helper.StartsWith("FORMAT_STRING"))
                return MdxScriptCommandType.FORMATTING;
            else if (helper.StartsWith("FORE_COLOR"))
                return MdxScriptCommandType.FORMATTING;
            else if (helper.StartsWith("BACK_COLOR"))
                return MdxScriptCommandType.FORMATTING;
            else
                return MdxScriptCommandType.Other;
        }

        public static SortedDictionary<int, MdxScriptCommand> GetCommandsFromScript(MdxScript script) 
        {
            SortedDictionary<int, MdxScriptCommand> ret = new SortedDictionary<int, MdxScriptCommand>();
            SortedDictionary<int, MdxScriptCommand> temp = new SortedDictionary<int, MdxScriptCommand>();

            foreach (Command cmd in script.Commands)
            {
                temp = MdxScriptHelper.GetCommandsFromText(cmd.Text.Replace("\n", Environment.NewLine), ret.Count);

                foreach (MdxScriptCommand cmd1 in temp.Values)
                {
                    ret.Add(cmd1.CommandNumber, cmd1);
                }
            }

            return ret;
        }

        public static void ExecuteXMLA(AdomdConnection con, string xmla, bool ignoreError)
        {
            try
            {
                using (AdomdCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = xmla;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                if (!ignoreError)
                    throw e;
            }
        }
        public static void ExecuteXMLA(AdomdConnection con, string xmla)
        {
            ExecuteXMLA(con, xmla, false);
        }
        public static List<string> GetDistinctCellProperties(CellSet queryResult, string propertyName)
        {
            List<string> ret = new List<string>();
            CellProperty cellProperty;
            string propertyValue;

            if (queryResult == null)
                return ret;

            foreach (Cell c in queryResult.Cells)
            {
                cellProperty = c.CellProperties.Find(propertyName);

                if (cellProperty != null)
                {
                    if (cellProperty.Value != null)
                    {
                        propertyValue = cellProperty.Value.ToString();
                        if (!ret.Contains(propertyValue))
                            ret.Add(propertyValue);
                    }
                }
            }

            return ret;
        }
        public static string EnsureCellProperty(string query, string cellProperty)
        {
            string ret;

            // cell property does not exist yet
            if (!Regex.IsMatch(query, ".*CELL\\sPROPERTIES.*" + cellProperty + ".*", RegexOptions.IgnoreCase))
            {
                // other cell properties already exist --> append 
                if (Regex.IsMatch(query, ".*CELL\\sPROPERTIES.*", RegexOptions.IgnoreCase))
                {
                    ret = query.Trim().TrimEnd(';') + ", " + cellProperty + ";";
                }
                else
                {
                    ret = query.Trim() + Environment.NewLine + "CELL PROPERTIES VALUE, " + cellProperty;
                }
            }
            else
            {
                ret = query;
            }

            return ret;
        }

        public static string ClearCacheCommand(string dbName, string cubeName)
        {
            return string.Format(@"<Batch xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine""><ClearCache><Object><DatabaseID>{0}</DatabaseID><CubeID>{1}</CubeID></Object></ClearCache></Batch>", dbName, cubeName);
        }
        public static string RefreshCubeCommand(string cubeName)
        {
            return string.Format("REFRESH CUBE [{0}]", cubeName.Trim("[]".ToCharArray()));
        }
        public static string CancelConnectionCommand(string connectionID)
        {
            return string.Format(@"<Cancel xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine""><ConnectionID>{0}</ConnectionID><CancelAssociated>1</CancelAssociated></Cancel>", connectionID);
        }
        public static string CancelSessionCommand(string sessionID)
        {
            return string.Format(@"<Cancel xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine""><SessionID>{0}</SessionID><CancelAssociated>1</CancelAssociated></Cancel>", sessionID);
        }
        public static string CancelSPIDCommand(string SPID)
        {
            return string.Format(@"<Cancel xmlns=""http://schemas.microsoft.com/analysisservices/2003/engine""><SPID>{0}</SPID><CancelAssociated>1</CancelAssociated></Cancel>", SPID);
        }
        #endregion

        #region by furmangg / MDX Script Performance Analyser
        /* this region is originally from furmangg (Greg Galloway) at https://mdxscriptperf.codeplex.com/SourceControl/latest#MDXScriptPerformance/Results.cs 
         * I extended it to fit my needs:
         *  - support for nested block comments
         *  - removed logic in catch-block by introducing the SafeSubstring string-extension
         *  - adoped it to return a list of MdxScriptCommand-objects
         */
        private enum MDXSplitStatus { InMDX, InDashComment, InSlashComment, InBlockComment, InBrackets, InDoubleQuotes, InSingleQuotes };

        public static SortedDictionary<int, MdxScriptCommand> GetCommandsFromText(string sMDX, int commandNr)
        {
            MDXSplitStatus status = MDXSplitStatus.InMDX;
            MdxScriptCommand newCmd;
            List<MdxScriptCommand> nestedScopes = new List<MdxScriptCommand>();
            int iPos = 0;
            int iLastSplit = 0;
            int blockCommentCount = 0;
            List<string> arrSplits = new List<string>();
            SortedDictionary<int, MdxScriptCommand> commands = new SortedDictionary<int, MdxScriptCommand>();

            while (iPos < sMDX.Length)
            {
                try
                {
                    if (status == MDXSplitStatus.InMDX)
                    {
                        if (sMDX.SafeSubstring(iPos, 2) == "/*")
                        {
                            status = MDXSplitStatus.InBlockComment;
                            blockCommentCount = 1;
                            iPos += 1;
                        }
                        else if (sMDX.SafeSubstring(iPos, 2) == "--")
                        {
                            status = MDXSplitStatus.InDashComment;
                            iPos += 1;
                        }
                        else if (sMDX.SafeSubstring(iPos, 2) == "//")
                        {
                            status = MDXSplitStatus.InSlashComment;
                            iPos += 1;
                        }
                        else if (sMDX.SafeSubstring(iPos, 1) == "[")
                        {
                            status = MDXSplitStatus.InBrackets;
                        }
                        else if (sMDX.SafeSubstring(iPos, 1) == "\"")
                        {
                            status = MDXSplitStatus.InDoubleQuotes;
                        }
                        else if (sMDX.SafeSubstring(iPos, 1) == "'")
                        {
                            status = MDXSplitStatus.InSingleQuotes;
                        }
                        else if (sMDX.SafeSubstring(iPos, 1) == ";") //split on semicolon only when it's in general MDX context
                        {
                            newCmd = new MdxScriptCommand(sMDX.SafeSubstring(iLastSplit, iPos - iLastSplit) + ";", commandNr, nestedScopes.Count);

                            if (nestedScopes.Count > 0)
                            {
                                newCmd.ParentCommand = nestedScopes.Last();
                            }

                            if (newCmd.CommandType == MdxScriptCommandType.SCOPE)
                            {
                                nestedScopes.Add(newCmd);
                            }
                            else if (newCmd.CommandType == MdxScriptCommandType.END_SCOPE)
                            {
                                nestedScopes.Last().CorrespondingEndScopeCommand = newCmd;
                                nestedScopes.RemoveAt(nestedScopes.Count - 1);
                            }

                            commands.Add(newCmd.CommandNumber, newCmd);
                            commandNr++;

                            iLastSplit = iPos + 1;
                        }
                    }
                    else if (status == MDXSplitStatus.InDashComment || status == MDXSplitStatus.InSlashComment)
                    {
                        if (sMDX.SafeSubstring(iPos, Environment.NewLine.Length) == Environment.NewLine)
                        {
                            status = MDXSplitStatus.InMDX;
                        }
                    }
                    else if (status == MDXSplitStatus.InBlockComment)
                    {
                        // nested InBlockComments are possible so we need to count how often we have entered an InBlockComment
                        if (sMDX.SafeSubstring(iPos, 2) == "/*")
                        {
                            blockCommentCount += 1;
                            iPos += 1;
                        }
                        if (sMDX.SafeSubstring(iPos, 2) == "*/")
                        {
                            // check if its the last InBlockComment-Block open
                            if (blockCommentCount == 1)
                            {
                                status = MDXSplitStatus.InMDX;
                                blockCommentCount = 0;
                                iPos += 1;
                            }
                            else // close the last InBlockComment-Block
                            {
                                blockCommentCount -= 1;
                                iPos += 1;
                            }
                        }
                    }
                    else if (status == MDXSplitStatus.InBrackets)
                    {
                        if (sMDX.SafeSubstring(iPos, 1) == "]" && sMDX.SafeSubstring(iPos, 2) == "]]")
                        {
                            iPos += 1;
                        }
                        else if (sMDX.SafeSubstring(iPos, 1) == "]" && sMDX.SafeSubstring(iPos, 2) != "]]")
                        {
                            status = MDXSplitStatus.InMDX;
                        }
                    }
                    else if (status == MDXSplitStatus.InDoubleQuotes)
                    {
                        if (sMDX.SafeSubstring(iPos, 1) == "\"" && sMDX.SafeSubstring(iPos, 2) == "\"\"")
                        {
                            iPos += 1;
                        }
                        else if (sMDX.SafeSubstring(iPos, 1) == "\"" && sMDX.SafeSubstring(iPos, 2) != "\"\"")
                        {
                            status = MDXSplitStatus.InMDX;
                        }
                    }
                    else if (status == MDXSplitStatus.InSingleQuotes)
                    {
                        if (sMDX.SafeSubstring(iPos, 1) == "'" && sMDX.SafeSubstring(iPos, 2) == "''")
                        {
                            iPos += 1;
                        }
                        else if (sMDX.SafeSubstring(iPos, 1) == "'" && sMDX.SafeSubstring(iPos, 2) != "''")
                        {
                            status = MDXSplitStatus.InMDX;
                        }
                    }      
                }
                catch (Exception ex)
                {
                }
                iPos++;
            }
            return commands;
        }
        public static SortedDictionary<int, MdxScriptCommand> GetCommandsFromText(string sMDX)
        {
            return MdxScriptHelper.GetCommandsFromText(sMDX, 0);
        }
        public static string StripComments(string mdx)
        {
            MDXSplitStatus status = MDXSplitStatus.InMDX;
            StringBuilder ret = new StringBuilder();
            int iPos = 0;
            int blockCommentCount = 0;

            while (iPos < mdx.Length)
            {
                try
                {
                    if (status == MDXSplitStatus.InMDX)
                    {
                        if (mdx.SafeSubstring(iPos, 2) == "/*")
                        {
                            status = MDXSplitStatus.InBlockComment;
                            blockCommentCount = 1;
                            iPos += 1;
                        }
                        else if (mdx.SafeSubstring(iPos, 2) == "--")
                        {
                            status = MDXSplitStatus.InDashComment;
                            iPos += 1;
                        }
                        else if (mdx.SafeSubstring(iPos, 2) == "//")
                        {
                            status = MDXSplitStatus.InSlashComment;
                            iPos += 1;
                        }
                        else
                        {
                            ret.Append(mdx[iPos]);
                        }
                    }
                    else if (status == MDXSplitStatus.InDashComment || status == MDXSplitStatus.InSlashComment)
                    {
                        if (mdx.SafeSubstring(iPos, Environment.NewLine.Length) == Environment.NewLine)
                        {
                            status = MDXSplitStatus.InMDX;
                        }
                    }
                    else if (status == MDXSplitStatus.InBlockComment)
                    {
                        // nested InBlockComments are possible so we need to count how often we have entered an InBlockComment
                        if (mdx.SafeSubstring(iPos, 2) == "/*")
                        {
                            blockCommentCount += 1;
                            iPos += 1;
                        }
                        if (mdx.SafeSubstring(iPos, 2) == "*/")
                        {
                            // check if its the last InBlockComment-Block open
                            if (blockCommentCount == 1)
                            {
                                status = MDXSplitStatus.InMDX;
                                blockCommentCount = 0;
                                iPos += 1;
                            }
                            else // close the last InBlockComment-Block
                            {
                                blockCommentCount -= 1;
                                iPos += 1;
                            }
                        }
                    }
                    iPos++;
                }
                catch (Exception ex)
                {
                }
            }
            return ret.ToString().Trim(Environment.NewLine.ToCharArray());
        }
        #endregion
    }
    #region Enumerators
    public enum MdxScriptCommandType
    {
        CREATE_MEMBER,
        CREATE_SET,
        SCOPE,
        END_SCOPE,
        ALTER_CUBE,
        THIS_ASSIGNMENT,
        DIRECT_ASSIGNMENT,
        FORMATTING,
        FREEZE,
        CALCULATE,
        CELL_CALCULATION,
        CLEAR_CALCULATIONS,
        Other
    }
    #endregion

    //Extension methods must be defined in a static class 
    public static class StringExtension
    {
        // This is the extension method. 
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined. 
        public static string SafeSubstring(this string str, int startIndex, int length)
        {
            if (startIndex + length > str.Length)
                return str.Substring(startIndex);

            return str.Substring(startIndex, length);
        }

        // This is the extension method. 
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined. 
        public static string SafeLeft(this string str, int length)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            return str.Substring(0, Math.Min(str.Length, length));
        }
    }
}
