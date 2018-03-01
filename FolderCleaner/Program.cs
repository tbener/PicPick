using CommandLine;
using FolderCleaner.Forms;
using FolderCleaner.Helpers;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using TalUtils;

namespace FolderCleaner
{
    static class Program
    {
        class Options
        {
            [Option('p', "project", HelpText = "Project name to be processed.")]
            public string Project { get; set; }

            [Value(0)]
            public string ProjectFallback { get; set; }
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            //string f = @"C:\Users\Tal\source\repos\FolderCleaner\FolderCleaner\Test\2018-01-11 09.36.58.jpg";
            //Debug.Print( Configuration.FolderCleanerConfigTaskDestination.GetDateTaken(f).ToString());

            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(opts => RunOptionsAndReturnExitCode(opts))
                .WithNotParsed<Options>((errs) => HandleParseError((IEnumerable<Error>)errs));


        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            string msg = "Error in arguments. ";
            //if (errs.Count > 0) msg += "(more than one error ocurred)";
            Msg.ShowE(msg);
        }

        private static void RunOptionsAndReturnExitCode(Options opts)
        {
            if (string.IsNullOrWhiteSpace(opts.Project) && !string.IsNullOrWhiteSpace(opts.ProjectFallback))
                opts.Project = opts.ProjectFallback;

            if (opts.Project == null)
                Application.Run(new MainForm());
            else
            {
                ProjectRunner projectRunner = new ProjectRunner(opts.Project);
                if (projectRunner.Init())
                    projectRunner.Run();
            }
        }
    }
}
