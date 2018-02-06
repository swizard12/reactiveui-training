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

            BackCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.NavigateBack.Execute();
            });

            SaveCommand = ReactiveCommand.Create(() =>
            {
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    int? emp_id = 0;
                    db.FnTEMPLOYEE_CREATE(Employee.EMP_FNAME, Employee.EMP_LNAME, Employee.EMP_INITIAL, ref emp_id);
                    HostScreen.Router.NavigateAndReset.Execute(new EmployeeDetailEditViewModel(HostScreen, (FnTEMPLOYEE_LISTResult)db.FnTEMPLOYEE_SELECT(emp_id).First()));
                }
            }, this.WhenAnyValue(
                x => x.Employee.EMP_FNAME, 
                x => x.Employee.EMP_LNAME, 
                x => x.Employee.EMP_INITIAL, 
                    (f, s, i) => !string.IsNullOrEmpty(f) && 
                                 !string.IsNullOrEmpty(s) && 
                                 !string.IsNullOrEmpty(i)));
        }

        public ReactiveCommand BackCommand { get; set; }
        public ReactiveCommand SaveCommand { get; set; }

        private FnTEMPLOYEE_LISTResult _Employee = new FnTEMPLOYEE_LISTResult();
        public FnTEMPLOYEE_LISTResult Employee
        {
            get { return _Employee; }
            set { this.RaiseAndSetIfChanged(ref _Employee, value); }
        }
    }
}
