﻿using ReactiveUI;
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
    /// Interaction logic for StandardWorkDetailNewView.xaml
    /// </summary>
    public partial class StandardWorkDetailNewView : UserControl, IViewFor<StandardWorkDetailNewViewModel>
    {
        public StandardWorkDetailNewView()
        {
            InitializeComponent();
            this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext);
        }

        public StandardWorkDetailNewViewModel ViewModel
        {
            get { return (StandardWorkDetailNewViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(StandardWorkDetailNewViewModel), typeof(StandardWorkDetailNewView), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (StandardWorkDetailNewViewModel)value; }
        }
    }
}
