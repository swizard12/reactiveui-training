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
    /// Interaction logic for StandardWorkTrainingListView.xaml
    /// </summary>
    public partial class StandardWorkTrainingListView : UserControl, IViewFor<StandardWorkTrainingListViewModel>
    {
        public StandardWorkTrainingListView()
        {
            InitializeComponent();
            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);
        }

        public StandardWorkTrainingListViewModel ViewModel
        {
            get { return (StandardWorkTrainingListViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(StandardWorkTrainingListViewModel), typeof(StandardWorkTrainingListView), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (StandardWorkTrainingListViewModel)value; }
        }
    }
}
