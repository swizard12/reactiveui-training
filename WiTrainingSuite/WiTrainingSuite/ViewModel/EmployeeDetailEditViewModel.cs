using MaterialDesignThemes.Wpf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using WiTrainingSuite.Model;

namespace WiTrainingSuite.ViewModel
{
    public class EmployeeDetailEditViewModel : ReactiveObject, IRoutableViewModel
    {
        #region IRoutableViewModel
        public string UrlPathSegment
        {
            get { return "employeedetailedit"; }
        }
        public IScreen HostScreen { get; protected set; }
        #endregion

        public SnackbarMessageQueue SnackBarQueue { get; set; }

        public EmployeeDetailEditViewModel(IScreen screen, FnTEMPLOYEE_LISTResult employee)
        {
            HostScreen = screen;

            Employee = employee;

            SnackBarQueue = new SnackbarMessageQueue();

            using (Wi_training_suite db = new Wi_training_suite(App.ConString))
            {
                SelectedRole = db.TROLE.First(x => x.ROLE_ID == Employee.ROLE_ID);

                VarList = BuildVarList();

                var rq = from r in db.TROLE orderby r.ROLE_NAME select r;
                foreach (TROLE r in rq) { RoleList.Add(r); }

                var dq = from d in db.TDEPARTMENT orderby d.DEPT_NAME select d;
                foreach (TDEPARTMENT d in dq) { DeptList.Add(d); }

                var sq = from s in db.TSHIFT orderby s.SHIFT_NAME select s;
                foreach (TSHIFT s in sq) { ShiftList.Add(s); }
            }

            BackCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.NavigateBack.Execute();
            });

            SaveCommand = ReactiveCommand.Create(() =>
            {
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    db.FnTEMPLOYEE_UPDATE(Employee.EMP_ID, 
                                          Employee.EMP_FNAME, 
                                          Employee.EMP_LNAME, 
                                          Employee.EMP_INITIAL, 
                                          Employee.DEPT_ID, 
                                          Employee.VAR_ID, 
                                          Employee.SHIFT_ID, 
                                          Employee.EMP_NOTE);
                    HostScreen.Router.FindViewModelInStack<EmployeeMasterViewModel>().EmployeeList =
                        new ReactiveList<FnTEMPLOYEE_LISTResult>(db.FnTEMPLOYEE_LIST().OrderBy("EMP_LNAME", this).AsEnumerable());
                    HostScreen.Router.FindViewModelInStack<EmployeeMasterViewModel>().SelectedEmployee =
                        new FnTEMPLOYEE_LISTResult();

                    HostScreen.Router.NavigateAndReset.Execute(HostScreen.Router.FindViewModelInStack<EmployeeMasterViewModel>());
                }
            });
        }

        public ReactiveCommand BackCommand { get; set; }
        public ReactiveCommand SaveCommand { get; set; }

        private ReactiveList<TROLEVAR> _VarList = new ReactiveList<TROLEVAR>();
        public ReactiveList<TROLEVAR> VarList
        {
            get { return _VarList; }
            set { this.RaiseAndSetIfChanged(ref _VarList, value); }
        }

        public ReactiveList<TROLEVAR> BuildVarList()
        {
            ReactiveList<TROLEVAR> l = new ReactiveList<TROLEVAR>();
            using (Wi_training_suite db = new Wi_training_suite(App.ConString))
            {
                var q = from x in db.TROLEVAR
                        where x.ROLE_ID == SelectedRole.ROLE_ID
                        orderby x.VAR_NAME
                        select x;
                foreach(TROLEVAR r in q)
                {
                    l.Add(r);
                }
                return l;
            }
        }

        private TROLE _SelectedRole = new TROLE();
        public TROLE SelectedRole
        {
            get { return _SelectedRole; }
            set
            {
                this.RaiseAndSetIfChanged(ref _SelectedRole, value);
                VarList = BuildVarList();
                Employee.VAR_ID = VarList.OrderBy(x => x.VAR_NAME).First().VAR_ID;
            }
        }

        private ReactiveList<TROLE> _RoleList = new ReactiveList<TROLE>();
        public ReactiveList<TROLE> RoleList
        {
            get { return _RoleList; }
            set { this.RaiseAndSetIfChanged(ref _RoleList, value); }
        }

        private ReactiveList<TDEPARTMENT> _DeptList = new ReactiveList<TDEPARTMENT>();
        public ReactiveList<TDEPARTMENT> DeptList
        {
            get { return _DeptList; }
            set { this.RaiseAndSetIfChanged(ref _DeptList, value); }
        }

        private ReactiveList<TSHIFT> _ShiftList = new ReactiveList<TSHIFT>();
        public ReactiveList<TSHIFT> ShiftList
        {
            get { return _ShiftList; }
            set { this.RaiseAndSetIfChanged(ref _ShiftList, value); }
        }

        private FnTEMPLOYEE_LISTResult _Employee;
        public FnTEMPLOYEE_LISTResult Employee
        {
            get { return _Employee; }
            set { this.RaiseAndSetIfChanged(ref _Employee, value); }
        }
    }
}
