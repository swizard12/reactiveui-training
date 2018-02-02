using MaterialDesignThemes.Wpf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace WiTrainingSuite.ViewModel
{
    public class EmployeeMasterViewModel : ExtendedViewModelBase, IRoutableViewModel
    {
        #region IRoutableViewModel
        public string UrlPathSegment
        {
            get { return "employeemaster"; }
        }
        public IScreen HostScreen { get; protected set; }
        #endregion

        #region WelcomeViewModel

        public SnackbarMessageQueue SnackBarQueue { get; set; }

        public EmployeeMasterViewModel(IScreen screen)
        {
            HostScreen = screen;

            SnackBarQueue = new SnackbarMessageQueue();

            using (Wi_training_suite db = new Wi_training_suite(App.ConString))
            {
                EmployeeList = new ReactiveList<FnTEMPLOYEE_LISTResult>(db.FnTEMPLOYEE_LIST().OrderBy("EMP_LNAME").AsEnumerable());
            }
            SnackBarQueue.Enqueue("Load Successful");

            CloseDrawerCommand = ReactiveCommand.Create(() =>
            {
                IsBtmOpen = false;

                string f = BuildFilterString(SortList);

                if (!string.IsNullOrWhiteSpace(f) && EmployeeList != null)
                    EmployeeList = new ReactiveList<FnTEMPLOYEE_LISTResult>(EmployeeList.OrderBy(f, this).AsEnumerable());
            });

            // Key = SQL Column Header; Value = Friendly Name
            ColumnHeaders.Add("EMP_FNAME", "First Name");
            ColumnHeaders.Add("EMP_LNAME", "Last Name");
            ColumnHeaders.Add("DEPT_NAME", "Department");
            ColumnHeaders.Add("ROLE_NAME", "Role");
            ColumnHeaders.Add("VAR_NAME", "Variant");
            ColumnHeaders.Add("SHIFT_NAME", "Shift");
        }

        private ReactiveList<FnTEMPLOYEE_LISTResult> _employeeList;
        public ReactiveList<FnTEMPLOYEE_LISTResult> EmployeeList
        {
            get { return _employeeList; }
            set { this.RaiseAndSetIfChanged(ref _employeeList, value); }
        }

        private FnTEMPLOYEE_LISTResult _selectedEmployee;
        public FnTEMPLOYEE_LISTResult SelectedEmployee
        {
            get { return _selectedEmployee; }
            set { this.RaiseAndSetIfChanged(ref _selectedEmployee, value); }
        }

        #endregion
    }
}
