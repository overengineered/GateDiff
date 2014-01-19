using System;
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

                    ProcessStartInfo command = new ProcessStartInfo(program.Path, BuildCommand(leftFile.Path, rightFile.Path));
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

        private static string BuildCommand(params string[] args)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string a in args)
            {
                if (string.IsNullOrEmpty(a))
                    continue;

                if (builder.Length > 0)
                    builder.Append(@" ");
                builder.Append(ArgumentEscape(a));
            }
            return builder.ToString();
        }

        private static string ArgumentEscape(string arg)
        {
            if (arg.Contains(@" "))
                return @"""" + arg + @"""";
            return arg;
        }
    }
}
