using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WiTrainingSuite.ViewModel;

namespace WiTrainingSuite.View
{
    /// <summary>
    /// Interaction logic for ConfigMenuTrainingPaperWorkView.xaml
    /// </summary>
    public partial class ConfigMenuTrainingPaperWorkView : UserControl, IViewFor<ConfigMenuTrainingPaperWorkViewModel>
    {
        public ConfigMenuTrainingPaperWorkView()
        {
            InitializeComponent();
            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);
        }

        public ConfigMenuTrainingPaperWorkViewModel ViewModel
        {
            get { return (ConfigMenuTrainingPaperWorkViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ConfigMenuTrainingPaperWorkViewModel), typeof(ConfigMenuTrainingPaperWorkView), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ConfigMenuTrainingPaperWorkViewModel)value; }
        }

    }
}
