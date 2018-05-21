using MaterialDesignThemes.Wpf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using WiTrainingSuite.Model;

namespace WiTrainingSuite.ViewModel
{
    public class EmployeeMasterViewModel : ReactiveObject, IRoutableViewModel
    {
        #region IRoutableViewModel
        public string UrlPathSegment
        {
            get { return "employeemaster"; }
        }
        public IScreen HostScreen { get; protected set; }
        #endregion

        public SnackbarMessageQueue SnackBarQueue { get; set; }

        public EmployeeMasterViewModel(IScreen screen)
        {
            HostScreen = screen;

            SnackBarQueue = new SnackbarMessageQueue();

            EmployeeAdd = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.Navigate.Execute(new EmployeeDetailViewModel(HostScreen));
            });
            EmployeeEdit = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.Navigate.Execute(new EmployeeDetailViewModel(HostScreen, SelectedEmployee));
            },
                this.WhenAny(x => x.SelectedEmployeeIndex,
                             x => x.IsWorking,
                (i,w) => i.Value != -1 && w.Value == false));
            EmployeeDelete = ReactiveCommand.Create(() =>
            {
                SnackBarQueue.Enqueue(String.Format("Are you sure you want to delete {0} {1}", SelectedEmployee.EMP_FNAME, SelectedEmployee.EMP_LNAME), "YES", async () =>
                {
                    IsWorking = true;
                    await DbInterface.fnEmployeeDelete(SelectedEmployee);
                    EmployeeList = new ReactiveList<EmployeeResult>(await DbInterface.fnEmployeeList());
                    OriginalList = EmployeeList;
                    IsWorking = false;
                });
            }, this.WhenAny(x => x.SelectedEmployeeIndex, x => x.IsWorking, (i, w) => i.Value != -1 && w.Value == false));
            EmployeeTraining = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.Navigate.Execute(new EmployeeTrainingViewModel(screen, SelectedEmployee));
            },
                this.WhenAny(x => x.SelectedEmployeeIndex,
                             x => x.IsWorking,
                (i, w) => i.Value != -1 && w.Value == false));
            FilterCommand = ReactiveCommand.Create(() =>
            {
                IsWorking = true;
                if (!String.IsNullOrWhiteSpace(FilterText))
                {
                    var fList = new ReactiveList<EmployeeResult>(
                        OriginalList.Where(
                            x => x.EMP_FNAME.ToUpper().Contains(FilterText.ToUpper()) ||
                                 x.EMP_LNAME.ToUpper().Contains(FilterText.ToUpper()) ||
                                 x.EMP_INITIAL.ToUpper().Contains(FilterText.ToUpper()) ||
                                 x.DEPT_NAME.ToUpper().Contains(FilterText.ToUpper()) ||
                                 x.SHIFT_NAME.ToUpper().Contains(FilterText.ToUpper()) ||
                                 x.VAR_NAME.ToUpper().Contains(FilterText.ToUpper())));
                    EmployeeList = fList;
                }
                else
                {
                    EmployeeList = OriginalList;
                }
                IsWorking = false;
            });
            ClearFilterCommand = ReactiveCommand.Create(() =>
            {
                FilterText = string.Empty;
            }, this.WhenAny(x => x.FilterText,
                (f) => !string.IsNullOrWhiteSpace(f.Value)));

            Task.Run(async () =>
            {
                EmployeeList = new ReactiveList<EmployeeResult>(new ReactiveList<EmployeeResult>(await DbInterface.fnEmployeeList()).OrderBy(x => x.EMP_LNAME));
                OriginalList = EmployeeList;
            }).ContinueWith((Unit) =>
            {
                IsWorking = false;
            });

            this.WhenAnyValue(x => x.FilterText)
                .Throttle(TimeSpan.FromSeconds(.25))
                .Select(_ => Unit.Default)
                .InvokeCommand(FilterCommand);
        }

        private string _FilterText;
        public string FilterText
        {
            get { return _FilterText; }
            set { this.RaiseAndSetIfChanged(ref _FilterText, value); }
        }

        public ReactiveCommand EmployeeAdd { get; set; }
        public ReactiveCommand EmployeeEdit { get; set; }
        public ReactiveCommand EmployeeDelete { get; set; }
        public ReactiveCommand EmployeeTraining { get; set; }
        public ReactiveCommand FilterCommand { get; set; }
        public ReactiveCommand ClearFilterCommand { get; set; }

        private ReactiveList<EmployeeResult> _OriginalList = new ReactiveList<EmployeeResult>();
        public ReactiveList<EmployeeResult> OriginalList
        {
            get { return _OriginalList; }
            set { this.RaiseAndSetIfChanged(ref _OriginalList, value); }
        }

        private ReactiveList<EmployeeResult> _EmployeeList;
        public ReactiveList<EmployeeResult> EmployeeList
        {
            get { return _EmployeeList; }
            set { this.RaiseAndSetIfChanged(ref _EmployeeList, value); }
        }

        private EmployeeResult _SelectedEmployee;
        public EmployeeResult SelectedEmployee
        {
            get { return _SelectedEmployee; }
            set { this.RaiseAndSetIfChanged(ref _SelectedEmployee, value); }
        }

        private int _SelectedEmployeeIndex = -1;
        public int SelectedEmployeeIndex
        {
            get { return _SelectedEmployeeIndex; }
            set { this.RaiseAndSetIfChanged(ref _SelectedEmployeeIndex, value); }
        }

        private bool _IsWorking = true;
        public bool IsWorking
        {
            get { return _IsWorking; }
            set { this.RaiseAndSetIfChanged(ref _IsWorking, value); }
        }
    }
}
