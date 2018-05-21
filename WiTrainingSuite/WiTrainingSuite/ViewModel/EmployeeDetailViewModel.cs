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
    public class EmployeeDetailViewModel : ReactiveObject, IRoutableViewModel
    {
        #region IRoutableViewModel
        public string UrlPathSegment
        {
            get { return "documentmaster"; }
        }
        public IScreen HostScreen { get; protected set; }
        #endregion

        public SnackbarMessageQueue SnackBarQueue { get; set; }

        // New
        public EmployeeDetailViewModel(IScreen screen)
        {
            HostScreen = screen;

            SnackBarQueue = new SnackbarMessageQueue();

            SelectedEmployee = new EmployeeResult();

            EmployeeSave = ReactiveCommand.Create(() =>
            {
                SnackBarQueue.Enqueue($"Are you sure you want to save {SelectedEmployee.EMP_FNAME} {SelectedEmployee.EMP_LNAME}?", "YES", async () =>
                  {
                      await DbInterface.fnEmployeeCreate(SelectedEmployee);
                      HostScreen.Router.NavigateAndReset.Execute(new EmployeeMasterViewModel(HostScreen));
                  });
            }, this.WhenAny(x => x.SelectedEmployee.EMP_FNAME,
                            x => x.SelectedEmployee.EMP_LNAME,
                            x => x.SelectedEmployee.EMP_INITIAL,
                            (a, b, c) => !string.IsNullOrWhiteSpace(a.Value) &&
                                         !string.IsNullOrWhiteSpace(a.Value) &&
                                         !string.IsNullOrWhiteSpace(a.Value)));

            BindCommonCommands();

            Task.Run(() =>
            {
                SetupLists();
                IsWorking = false;
            });
        }

        // Edit
        public EmployeeDetailViewModel(IScreen screen, EmployeeResult employee)
        {
            HostScreen = screen;

            SnackBarQueue = new SnackbarMessageQueue();

            SelectedEmployee = employee;

            EmployeeSave = ReactiveCommand.Create(() =>
            {
                SnackBarQueue.Enqueue($"Are you sure you want to save {SelectedEmployee.EMP_FNAME} {SelectedEmployee.EMP_LNAME}?", "YES", async () =>
                {
                    await DbInterface.fnEmployeeUpdate(SelectedEmployee);
                    HostScreen.Router.NavigateAndReset.Execute(new EmployeeMasterViewModel(HostScreen));
                });
            }, this.WhenAny(x => x.SelectedEmployee.EMP_FNAME,
                            x => x.SelectedEmployee.EMP_LNAME,
                            x => x.SelectedEmployee.EMP_INITIAL,
                            (a, b, c) => !string.IsNullOrWhiteSpace(a.Value) &&
                                         !string.IsNullOrWhiteSpace(a.Value) &&
                                         !string.IsNullOrWhiteSpace(a.Value)));

            BindCommonCommands();

            Task.Run(() =>
            {
                SetupLists();
                IsWorking = false;
            });
        }

        public void BindCommonCommands()
        {
            EmployeeCancel = ReactiveCommand.Create(() => { HostScreen.Router.NavigateBack.Execute(); });

            DepartmentClear = ReactiveCommand.Create(() => { SelectedEmployee.DEPT_ID = null; });
            ShiftClear= ReactiveCommand.Create(() => { SelectedEmployee.SHIFT_ID = null; });
            VariantClear = ReactiveCommand.Create(() => { SelectedEmployee.VAR_ID = null; });
            VariantTrainingClear = ReactiveCommand.Create(() => { SelectedEmployee.VAR_ID_TRAINING = null; });
        }

        public async void SetupLists()
        {
            DepartmentList = new ReactiveList<DeptResult>(await DbInterface.fnDeptList());
            ShiftList = new ReactiveList<ShiftResult>(await DbInterface.fnShiftList());
            VariantList = new ReactiveList<VariantResult>(await DbInterface.fnVarList());
        }

        public ReactiveCommand EmployeeCancel { get; set; }
        public ReactiveCommand EmployeeSave { get; set; }
        public ReactiveCommand DepartmentClear { get; set; }
        public ReactiveCommand ShiftClear { get; set; }
        public ReactiveCommand VariantClear { get; set; }
        public ReactiveCommand VariantTrainingClear { get; set; }

        private EmployeeResult _SelectedEmployee;
        public EmployeeResult SelectedEmployee
        {
            get { return _SelectedEmployee; }
            set { this.RaiseAndSetIfChanged(ref _SelectedEmployee, value); }
        }

        private ReactiveList<ShiftResult> _ShiftList;
        public ReactiveList<ShiftResult> ShiftList
        {
            get { return _ShiftList; }
            set { this.RaiseAndSetIfChanged(ref _ShiftList, value); }
        }

        private ReactiveList<DeptResult> _DepartmentList;
        public ReactiveList<DeptResult> DepartmentList
        {
            get { return _DepartmentList; }
            set { this.RaiseAndSetIfChanged(ref _DepartmentList, value); }
        }

        private ReactiveList<VariantResult> _VariantList;
        public ReactiveList<VariantResult> VariantList
        {
            get { return _VariantList; }
            set { this.RaiseAndSetIfChanged(ref _VariantList, value); }
        }

        private bool _IsWorking = true;
        public bool IsWorking
        {
            get { return _IsWorking; }
            set { this.RaiseAndSetIfChanged(ref _IsWorking, value); }
        }
    }
}
