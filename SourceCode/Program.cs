using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AnalysisServices;
using Microsoft.AnalysisServices.AdomdClient;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Configuration;

namespace gbrueckl.AnalysisServices.MdxScriptDebugger
{
    class Program
    {
        public const string DevDBName = "AdventureWorksDW2012";
        public const string DevServer = "localhost";
        public const string DevCubeName = "AdventureWorks";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string server = null;
            string database = null;
            string cube = null;

            if (args.Length >= 1) server = args[0];
            if (args.Length >= 2) database = args[1];
            if (args.Length >= 3) cube = args[2];


            if (string.IsNullOrEmpty(server))
                server = Properties.Settings.Default.LastServer;
            if (string.IsNullOrEmpty(database))
                database = Properties.Settings.Default.LastDatabase;
            if (string.IsNullOrEmpty(cube))
                cube = Properties.Settings.Default.LastCube;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MdxScriptDebuggerGUI(server, database, cube));
        }
    }
}
