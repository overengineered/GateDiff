using GateDiff.Properties;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GateDiff
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string[] args = Environment.GetCommandLineArgs();

            var left = args.Length >= 3 ? new FileInfo(args[1]) : null;
            var right = args.Length >= 3 ? new FileInfo(args[2]) : null;

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
                ProcessStartInfo command = new ProcessStartInfo(diffProgram, BuildCommand(left.FullName, right.FullName));
                command.UseShellExecute = false;
                command.RedirectStandardOutput = true;
                Process app = Process.Start(command);

                app.StandardOutput.ReadToEnd();
                app.WaitForExit();

                Application.Current.Shutdown();
                return;
            }

            var data = Tuple.Create(left, right);
            layoutRoot.DataContext = data;
        }

        private static string BuildCommand(params string[] args)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string a in args)
            {
                if (string.IsNullOrEmpty(a))
                    continue;

                if (builder.Length > 0)
                    builder.Append(" ");
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

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            WindowPlacement.SetPlacement(new WindowInteropHelper(this).Handle, Settings.Default.MainWindowPlacement);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.MainWindowPlacement = WindowPlacement.GetPlacement(new WindowInteropHelper(this).Handle);
            Settings.Default.Save();
        }
    }
}
