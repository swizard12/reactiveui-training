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
    /// Interaction logic for ConfigMenuVarMgmtView.xaml
    /// </summary>
    public partial class ConfigMenuVarMgmtView : UserControl, IViewFor<ConfigMenuVarMgmtViewModel>
    {
        public ConfigMenuVarMgmtView()
        {
            InitializeComponent();
            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);
        }

        public ConfigMenuVarMgmtViewModel ViewModel
        {
            get { return (ConfigMenuVarMgmtViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(ConfigMenuVarMgmtViewModel), typeof(ConfigMenuVarMgmtView), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ConfigMenuVarMgmtViewModel)value; }
        }
    }
}
