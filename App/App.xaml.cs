using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace GateDiff
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly String GATEDIFF = "GateDiff";

        public Tuple<FileInfo, FileInfo> DiffData
        {
            get;
            set;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            CustomizationSection config = ConfigurationManager.GetSection("customization") as CustomizationSection;

            bool swappingEnabled = false;
            var args = e.Args.Where(arg => { bool m=(arg=="--swapping"); if(m)swappingEnabled=true; return!m; }).ToList();

            var left = e.Args.Length >= 2 ? new FileInfo(e.Args[0]) : null;
            var right = e.Args.Length >= 2 ? new FileInfo(e.Args[1]) : null;

            string toolKey = null;
            if (left != null && right != null)
            {
                foreach (Case item in config.Rules)
                {
                    var extensions = item.Extensions.Split(' ').Where(x => !String.IsNullOrWhiteSpace(x));
                    var foundDiffer = extensions.Contains(left.Extension)
                        || extensions.Contains(right.Extension)
                        || extensions.Contains(".*");
                    if (foundDiffer)
                    {
                        toolKey = item.Tool;
                        break;
                    }
                }
            }

            if (String.IsNullOrEmpty(toolKey))
                toolKey = config.Rules.Default;

            if (!String.IsNullOrEmpty(toolKey) && toolKey != GATEDIFF)
            {
                using (DiffFile leftFile = new DiffFile(left), rightFile = new DiffFile(right))
                {
                    Executable program = config.Tools[toolKey];

                    var arguments = new List<string>(3);
                    arguments.Add(ArgumentEscape(leftFile.Path));
                    arguments.Add(ArgumentEscape(rightFile.Path));

                    if (swappingEnabled && !String.IsNullOrEmpty(program.SwappingParameter))
                        arguments.Add(program.SwappingParameter);

                    ProcessStartInfo command = new ProcessStartInfo(program.Path, String.Join(@" ", arguments));
                    command.UseShellExecute = false;
                    command.RedirectStandardOutput = true;
                    Process app = Process.Start(command);

                    app.StandardOutput.ReadToEnd();
                    app.WaitForExit();

                    Shutdown();
                    return;
                }
            }

            StartupUri = new System.Uri(@"MainWindow.xaml", System.UriKind.Relative);
            DiffData = Tuple.Create(left, right);
        }

        private static string ArgumentEscape(string arg)
        {
            if (arg.Contains(@" "))
                return @"""" + arg + @"""";
            return arg;
        }
    }
}
