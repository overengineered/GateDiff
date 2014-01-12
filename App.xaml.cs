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
        public Tuple<FileInfo, FileInfo> DiffData
        {
            get;
            set;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var left = e.Args.Length >= 2 ? new FileInfo(e.Args[0]) : null;
            var right = e.Args.Length >= 2 ? new FileInfo(e.Args[1]) : null;

            String diffProgram = null;
            if (left != null && right != null)
            {
                foreach (var key in ConfigurationManager.AppSettings.AllKeys)
                {
                    var extensions = key.Split(' ').Where(x => !String.IsNullOrWhiteSpace(x));
                    var foundDiffer = extensions.Contains(left.Extension)
                        || extensions.Contains(right.Extension)
                        || extensions.Contains(".*");
                    if (foundDiffer)
                    {
                        diffProgram = ConfigurationManager.AppSettings[key];
                        diffProgram = String.IsNullOrWhiteSpace(diffProgram) ? null : diffProgram;
                        break;
                    }
                }
            }

            if (diffProgram != null)
            {
                using (DiffFile leftFile = new DiffFile(left), rightFile = new DiffFile(right))
                {
                    ProcessStartInfo command = new ProcessStartInfo(diffProgram, BuildCommand(leftFile.Path, rightFile.Path));
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
