using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;

namespace WiTrainingSuite.ViewModel
{
    public class EmployeeDetailNewViewModel : ReactiveObject, IRoutableViewModel
    {
        #region IRoutableViewModel
        public string UrlPathSegment
        {
            get { return "employeedetailnew"; }
        }
        public IScreen HostScreen { get; protected set; }
        #endregion

        public SnackbarMessageQueue SnackBarQueue { get; set; }

        public EmployeeDetailNewViewModel(IScreen screen)
        {
            HostScreen = screen;

            SnackBarQueue = new SnackbarMessageQueue();
        }

        private FnTEMPLOYEE_LISTResult _Employee = new FnTEMPLOYEE_LISTResult();
        public FnTEMPLOYEE_LISTResult Employee
        {
            get { return _Employee; }
            set { this.RaiseAndSetIfChanged(ref _Employee, value); }
        }
    }
}
