using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WiTrainingSuite
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static MainWindow TopWindow { get; set; }
        public static string ConString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=wi_training_suite;Data Source=WI-SQL-01";

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            TopWindow = new MainWindow();
            TopWindow.Show();
        }
    }
}
