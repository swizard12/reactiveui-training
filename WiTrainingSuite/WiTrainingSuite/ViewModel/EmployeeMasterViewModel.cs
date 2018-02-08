using MaterialDesignThemes.Wpf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using WiTrainingSuite.Model;

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

        public SnackbarMessageQueue SnackBarQueue { get; set; }

        readonly ObservableAsPropertyHelper<bool> isAdmin;
        public bool IsAdmin
        {
            get { return isAdmin.Value; }
        }

        public bool admin;

        public EmployeeMasterViewModel(IScreen screen)
        {
            admin = App._admin;
            this.WhenAnyValue(x => x.admin).ToProperty(this, x => x.IsAdmin, out isAdmin);

            HostScreen = screen;

            SnackBarQueue = new SnackbarMessageQueue();

            using (Wi_training_suite db = new Wi_training_suite(App.ConString))
            {
                EmployeeList = new ReactiveList<FnTEMPLOYEE_LISTResult>(db.FnTEMPLOYEE_LIST().OrderBy("EMP_LNAME"));
            }
            SnackBarQueue.Enqueue("Load Successful");

            CloseDrawerCommand = ReactiveCommand.Create(() =>
            {
                IsBtmOpen = false;

                string f = BuildFilterString(SortList);

                if (!string.IsNullOrWhiteSpace(f) && EmployeeList != null)
                    EmployeeList = new ReactiveList<FnTEMPLOYEE_LISTResult>(EmployeeList.OrderBy(f, this).AsEnumerable());
            });

            NewEmployeeCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.Navigate.Execute(new EmployeeDetailNewViewModel(HostScreen));
            }, this.WhenAny(
                x => x.IsAdmin,
                (a) => a.Value == true));

            EditEmployeeCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.Navigate.Execute(new EmployeeDetailEditViewModel(HostScreen, SelectedEmployee));
            }, this.WhenAny(
                x => x.EmployeeIndex,
                x => x.IsAdmin,
                (i, a) => i.Value > -1 && a.Value == true));

            EditTrainingCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.Navigate.Execute(new EmployeeTrainingEditViewModel(HostScreen, SelectedEmployee));
            }, this.WhenAny(
                x => x.EmployeeIndex,
                x => x.IsAdmin,
                (i, a) => i.Value > -1 && a.Value == true));

            // Key = SQL Column Header; Value = Friendly Name
            ColumnHeaders.Add("EMP_FNAME", "First Name");
            ColumnHeaders.Add("EMP_LNAME", "Last Name");
            ColumnHeaders.Add("DEPT_NAME", "Department");
            ColumnHeaders.Add("ROLE_NAME", "Role");
            ColumnHeaders.Add("VAR_NAME", "Variant");
            ColumnHeaders.Add("SHIFT_NAME", "Shift");
        }

        private int _EmployeeIndex = -1;
        public int EmployeeIndex
        {
            get { return _EmployeeIndex; }
            set { this.RaiseAndSetIfChanged(ref _EmployeeIndex, value); }
        }

        public ReactiveCommand NewEmployeeCommand { get; set; }
        public ReactiveCommand EditEmployeeCommand { get; set; }
        public ReactiveCommand EditTrainingCommand { get; set; }

        private ReactiveList<FnTEMPLOYEE_LISTResult> _employeeList;
        public ReactiveList<FnTEMPLOYEE_LISTResult> EmployeeList
        {
            get { return _employeeList; }
            set { this.RaiseAndSetIfChanged(ref _employeeList, value); }
        }
        
        private FnTEMPLOYEE_LISTResult _selectedEmployee = new FnTEMPLOYEE_LISTResult();
        public FnTEMPLOYEE_LISTResult SelectedEmployee
        {
            get { return _selectedEmployee; }
            set { this.RaiseAndSetIfChanged(ref _selectedEmployee, value); }
        }
    }
}
