using GateDiff.Properties;
using System;
using System.Collections.Generic;
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

            var data = Tuple.Create(left, right);

            layoutRoot.DataContext = data;
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
