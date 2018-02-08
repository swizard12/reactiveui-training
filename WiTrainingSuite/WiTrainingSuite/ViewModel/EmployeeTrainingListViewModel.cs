using MaterialDesignThemes.Wpf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiTrainingSuite.Model;

namespace WiTrainingSuite.ViewModel
{
    public class EmployeeTrainingListViewModel : ReactiveObject, IRoutableViewModel
    {
        #region IRoutableViewModel
        public string UrlPathSegment
        {
            get { return "employeetrainingedit"; }
        }
        public IScreen HostScreen { get; protected set; }
        #endregion

        public SnackbarMessageQueue SnackBarQueue { get; set; }

        public EmployeeTrainingListViewModel(IScreen screen, FnTEMPLOYEE_LISTResult employee)
        {
            HostScreen = screen;

            Employee = employee;

            SnackBarQueue = new SnackbarMessageQueue();
        }

        private FnTEMPLOYEE_LISTResult _Employee;
        public FnTEMPLOYEE_LISTResult Employee
        {
            get { return _Employee; }
            set { this.RaiseAndSetIfChanged(ref _Employee, value); }
        }


    }
}
