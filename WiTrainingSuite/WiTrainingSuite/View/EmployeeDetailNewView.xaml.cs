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
    /// Interaction logic for EmployeeDetailNewView.xaml
    /// </summary>
    public partial class EmployeeDetailNewView : UserControl, IViewFor<EmployeeDetailNewViewModel>
    {
        public EmployeeDetailNewView()
        {
            InitializeComponent();
            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);
        }

        public EmployeeDetailNewViewModel ViewModel
        {
            get { return (EmployeeDetailNewViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(EmployeeDetailNewViewModel), typeof(EmployeeDetailNewView), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (EmployeeDetailNewViewModel)value; }
        }
    }
}
