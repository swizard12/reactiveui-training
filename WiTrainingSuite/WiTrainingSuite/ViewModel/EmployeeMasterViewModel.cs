using MaterialDesignThemes.Wpf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
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
                OriginalList = EmployeeList;
            }
            SnackBarQueue.Enqueue("Load Successful");

            CloseDrawerCommand = ReactiveCommand.Create(() =>
            {
                IsBtmOpen = false;

                ApplySort();
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

            ClearNameFilter = ReactiveCommand.Create(() =>
            {
                fNAME = string.Empty;
            }, this.WhenAny(
                x => x.fNAME,
                (y) => !string.IsNullOrWhiteSpace(y.Value)));

            ClearDeptFilter = ReactiveCommand.Create(() =>
            {
                fDEPT = string.Empty;
            }, this.WhenAny(
                x => x.fDEPT,
                (y) => !string.IsNullOrWhiteSpace(y.Value)));

            ClearRoleFilter = ReactiveCommand.Create(() =>
            {
                fROLE = string.Empty;
            }, this.WhenAny(
                x => x.fROLE,
                (y) => !string.IsNullOrWhiteSpace(y.Value)));

            ClearAll = ReactiveCommand.Create(() =>
            {
                fNAME = string.Empty;
                fDEPT = string.Empty;
                fROLE = string.Empty;
            }, this.WhenAny(
                x => x.fNAME,
                x => x.fDEPT,
                x => x.fROLE,
                (n, d, r) => !string.IsNullOrWhiteSpace(n.Value) || 
                             !string.IsNullOrWhiteSpace(d.Value) || 
                             !string.IsNullOrWhiteSpace(r.Value)));
        }

        public void ApplySort()
        {
            string f = BuildFilterString(SortList);

            if (!string.IsNullOrWhiteSpace(f) && EmployeeList != null)
                EmployeeList = new ReactiveList<FnTEMPLOYEE_LISTResult>(EmployeeList.OrderBy(f, this).AsEnumerable());
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

        public ReactiveCommand ClearNameFilter { get; set; }
        public ReactiveCommand ClearDeptFilter { get; set; }
        public ReactiveCommand ClearRoleFilter { get; set; }
        public ReactiveCommand ClearAll { get; set; }

        private ReactiveList<FnTEMPLOYEE_LISTResult> _OriginalList = new ReactiveList<FnTEMPLOYEE_LISTResult>();
        public ReactiveList<FnTEMPLOYEE_LISTResult> OriginalList
        {
            get { return _OriginalList; }
            set { this.RaiseAndSetIfChanged(ref _OriginalList, value); }
        }

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

        private string _fNAME;
        public string fNAME
        {
            get { return _fNAME; }
            set
            {
                this.RaiseAndSetIfChanged(ref _fNAME, value);

                var task = Task.Run(() => ApplyFilter());
                bWorking = true;
                task.ContinueWith((x) => { bWorking = false; });
            }
        }

        private string _fDEPT;
        public string fDEPT
        {
            get { return _fDEPT; }
            set
            {
                this.RaiseAndSetIfChanged(ref _fDEPT, value);

                var task = Task.Run(() => ApplyFilter());
                bWorking = true;
                task.ContinueWith((x) => { bWorking = false; });
            }
        }

        private string _fROLE;
        public string fROLE
        {
            get { return _fROLE; }
            set
            {
                this.RaiseAndSetIfChanged(ref _fROLE, value);

                var task = Task.Run(() => ApplyFilter());
                bWorking = true;
                task.ContinueWith((x) => { bWorking = false; });
            }
        }

        private async void ApplyFilter()
        {
            await Task.Run(() => 
            {
                var tList = new ReactiveList<FnTEMPLOYEE_LISTResult>(OriginalList);

                // Pass One - First Name / Last Name
                if (!string.IsNullOrWhiteSpace(fNAME)) { tList = new ReactiveList<FnTEMPLOYEE_LISTResult>(OriginalList.Where(x => x.EMP_FNAME.ToUpper().Contains(fNAME.ToUpper()) || x.EMP_LNAME.ToUpper().Contains(fNAME.ToUpper()))); }
                else { tList = OriginalList; }

                // Pass Two - Department
                if (!string.IsNullOrWhiteSpace(fDEPT)) { tList = new ReactiveList<FnTEMPLOYEE_LISTResult>(tList.Where(x => x.DEPT_NAME.ToUpper().Contains(fDEPT.ToUpper()))); }

                // Pass Three - Role / Var
                if (!string.IsNullOrEmpty(fROLE)) { tList = new ReactiveList<FnTEMPLOYEE_LISTResult>(tList.Where(x => x.ROLE_NAME.ToUpper().Contains(fROLE.ToUpper()) || x.VAR_NAME.ToUpper().Contains(fROLE.ToUpper()))); }

                EmployeeList = tList;

                ApplySort();

                EmployeeIndex = -1;

                return 0;
            });
        }


        private bool _bWorking = false;
        public bool bWorking
        {
            get { return _bWorking; }
            set { this.RaiseAndSetIfChanged(ref _bWorking, value); }
        }
    }
}
