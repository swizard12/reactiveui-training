using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ReactiveUI;
using System.Reactive.Linq;
using System.IO;

namespace WiTrainingSuite
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>

    public partial class App : Application
    {
        public static MainWindow TopWindow { get; set; }
        public static string ConString = "Integrated Security=SSPI;Initial Catalog=wi_training_db;Data Source=10.42.0.25";

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ConString = "Integrated Security=SSPI;Initial Catalog=wi_training_db;Data Source=10.42.0.25";
            TopWindow = new MainWindow();
            TopWindow.Show();
        }

        public static void OnUI(Action action) // Wrapper for invoking action on the UI Thread, typically passing long running data back to the ViewModel;
        {
            Application.Current.Dispatcher.Invoke(() => { action(); });
        }
    }
}
