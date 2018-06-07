using CommandLine;
using PicPick.Forms;
using PicPick.Helpers;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using TalUtils;

[assembly: log4net.Config.XmlConfigurator(Watch=true)]

namespace PicPick
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

            

            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(opts => RunOptionsAndReturnExitCode(opts))
                .WithNotParsed<Options>((errs) => HandleParseError((IEnumerable<Error>)errs));


        }
        

        public static void m1()
        {
            Msg.Show("Method 1");
        }

        public static void m2()
        {
            Msg.Show("Method 2");
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
                //ProjectRunner projectRunner = new ProjectRunner(opts.Project);
                //if (projectRunner.Init())
                //    projectRunner.Run();
            }
        }
    }
}
