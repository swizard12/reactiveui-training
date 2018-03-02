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
        public static string ConString { get; set; }
        public static string ConStringReadOnly = "User ID=traininguser;Password=WItra1n;Persist Security Info=False;Initial Catalog=wi_training_suite;Data Source=10.42.0.25";
        public static bool _admin { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ConString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=wi_training_suite;Data Source=10.42.0.25";
            TopWindow = new MainWindow();
            TopWindow.Show();
        }

        public static void OnUI(Action action) // Wrapper for invoking action on the UI Thread, typically passing long running data back to the ViewModel;
        {
            Application.Current.Dispatcher.Invoke(() => { action(); });
        }

        public static string CreateTmpFile()
        {
            string fName = string.Empty;

            try
            {
                fName = Path.GetTempFileName();

                FileInfo fI = new FileInfo(fName);

                fI.Attributes = FileAttributes.Temporary;

                Console.WriteLine("TEMP file created at: {0}", fName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to create TEMP file or set its attributes: {0}", ex.Message);
            }

            return fName;
        }
    }
}
