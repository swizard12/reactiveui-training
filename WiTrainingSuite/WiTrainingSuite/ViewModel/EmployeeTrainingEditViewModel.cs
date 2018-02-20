using MaterialDesignThemes.Wpf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using WiTrainingSuite.Model;
using MahApps.Metro.Controls.Dialogs;

namespace WiTrainingSuite.ViewModel
{
    public class EmployeeTrainingEditViewModel : ReactiveObject, IRoutableViewModel
    {
        #region IRoutableViewModel
        public string UrlPathSegment
        {
            get { return "employeetrainingedit"; }
        }
        public IScreen HostScreen { get; protected set; }
        #endregion

        public SnackbarMessageQueue SnackBarQueue { get; set; }

        public EmployeeTrainingEditViewModel(IScreen screen, FnTEMPLOYEE_LISTResult employee)
        {
            HostScreen = screen;

            Employee = employee;

            EMP_FULLNAME = Employee.EMP_FNAME + " " + Employee.EMP_LNAME;

            SnackBarQueue = new SnackbarMessageQueue();

            PrepLists();    

            QueueAddCommand = ReactiveCommand.Create(() =>
            {
                bWorking = true;
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    db.FnTEMPTRAINING_QUEUEADD(
                        Employee.EMP_ID,
                        SelectedStandardWork.SW_ID,
                        1,
                        VALID_DATE,
                        "Full Training Declared");
                    StandardWorkIndex = -1;
                    fSW = string.Empty;

                    RefreshTrainingLists();
                }
            }, this.WhenAny(
                x => x.DateSet,
                x => x.StandardWorkIndex,
                (y, z) => y.Value && z.Value != -1));

            QueueDelCommand = ReactiveCommand.Create(() =>
            {
                bWorking = true;
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    db.FnTEMPTRAINING_QUEUEDELETE(
                        SelectedQueue.EMP_ID,
                        SelectedQueue.SW_ID);
                    StandardWorkIndex = -1;
                    fSW = string.Empty;

                    RefreshTrainingLists();
                }
            }, this.WhenAny(
                x => x.DateSet,
                x => x.QueueIndex,
                (y, z) => y.Value && z.Value != -1));

            ClearSWFilter = ReactiveCommand.Create(() =>
            {
                bWorking = true;
                StandardWorkList = StandardWorkOriginalList;
                fSW = string.Empty;
                StandardWorkIndex = -1;
            }, this.WhenAny(
                x => x.fSW,
                (f) => !string.IsNullOrWhiteSpace(f.Value)));

            BackCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.NavigateBack.Execute();
            });

            SaveCommand = ReactiveCommand.Create(() =>
            {
                // Show Confirmation Dialog in Top Right Corner
                SnackBarQueue.Enqueue(
                    String.Format("Commit {0} Records to Database?", QueueList.Count), 
                    "CONFIRM", 
                    // Confirmation Method Here
                    async () => 
                    {
                        using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                        {
                            await Task.Run(() =>
                            {
                                db.FnTEMPTRAINING_QUEUECOMMITE(Employee.EMP_ID);
                            });

                            await DialogManager.ShowMessageAsync(App.TopWindow, "Action Confirmed", "Records saved, please review the training list");

                            HostScreen.Router.NavigateAndReset.Execute(new EmployeeTrainingListViewModel(HostScreen, Employee));
                        }
                    });
            },
            this.WhenAny(
                x => x.QueueHasItems,
                (q) => q.Value));
        }

        private bool _bWorking = true;
        public bool bWorking
        {
            get { return _bWorking; }
            set { this.RaiseAndSetIfChanged(ref _bWorking, value); }
        }

        public async void PrepLists()
        {
            bWorking = true;
            await Task.Run(() =>
            {
                // Purge TEMPTRAINING_WIP
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    // Purge WIP Table for this employee
                    var deleteEmployeeTraining =
                        from w in db.TEMPTRAINING_WIP
                        where w.EMP_ID == Employee.EMP_ID
                        select w;

                    foreach (var w in deleteEmployeeTraining)
                    {
                        db.TEMPTRAINING_WIP.DeleteOnSubmit(w);
                    }
                    try
                    {
                        db.SubmitChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }

                    StandardWorkOriginalList = new ReactiveList<FnTEMPTRAINING_SELECTEResult>(db.FnTEMPTRAINING_SELECTE(Employee.EMP_ID));

                    StandardWorkList = StandardWorkOriginalList;
                }

                bWorking = false;
            });
        }

        public async void RefreshTrainingLists()
        {
            bWorking = true;
            await Task.Run(() =>
            {
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    // Get Queue List
                    QueueList = new ReactiveList<VTRAINING>(from t in db.VTRAINING.Where(x => x.EMP_ID == Employee.EMP_ID)
                                                            where (from w in db.TEMPTRAINING_WIP.Where(x => x.EMP_ID == Employee.EMP_ID)
                                                                   select w.SW_ID).Contains(t.SW_ID)
                                                            select t);

                    // Get StandardWork Original List, less what is in the Queue
                    StandardWorkOriginalList = new ReactiveList<FnTEMPTRAINING_SELECTEResult>(from t in db.FnTEMPTRAINING_SELECTE(Employee.EMP_ID)
                                                                                                where !(from q in QueueList
                                                                                                        select q.SW_ID).ToList()
                                                                                                        .Contains(t.SW_ID.GetValueOrDefault())
                                                                                                select t);
                }

                StandardWorkList = StandardWorkOriginalList;

                bWorking = false;
            });
        }

        public ReactiveCommand ClearSWFilter { get; set; }
        public ReactiveCommand BackCommand { get; set; }
        public ReactiveCommand SaveCommand { get; set; }

        private FnTEMPLOYEE_LISTResult _Employee;
        public FnTEMPLOYEE_LISTResult Employee
        {
            get { return _Employee; }
            set { this.RaiseAndSetIfChanged(ref _Employee, value); }
        }

        private string _EMP_FULLNAME;
        public string EMP_FULLNAME
        {
            get { return _EMP_FULLNAME; }
            set { this.RaiseAndSetIfChanged(ref _EMP_FULLNAME, value); }
        }

        // Default value is 1st January of the current year
        private DateTime _VALID_DATE = new DateTime(DateTime.Now.Year, 01, 01);
        public DateTime VALID_DATE
        {
            get { return _VALID_DATE; }
            set
            {
                this.RaiseAndSetIfChanged(ref _VALID_DATE, value);
                if (DateSet == false)
                    DateSet = true;
            }
        }

        private bool _DateSet = false;
        public bool DateSet
        {
            get { return _DateSet; }
            set { this.RaiseAndSetIfChanged(ref _DateSet, value); }
        }

        private ReactiveList<FnTEMPTRAINING_SELECTEResult> _StandardWorkList = new ReactiveList<FnTEMPTRAINING_SELECTEResult>();
        public ReactiveList<FnTEMPTRAINING_SELECTEResult> StandardWorkList
        {
            get { return _StandardWorkList; }
            set { this.RaiseAndSetIfChanged(ref _StandardWorkList, value); }
        }

        private ReactiveList<FnTEMPTRAINING_SELECTEResult> _StandardWorkOriginalList;
        public ReactiveList<FnTEMPTRAINING_SELECTEResult> StandardWorkOriginalList
        {
            get { return _StandardWorkOriginalList; }
            set { this.RaiseAndSetIfChanged(ref _StandardWorkOriginalList, value); }
        }

        private int _StandardWorkIndex = -1;
        public int StandardWorkIndex
        {
            get { return _StandardWorkIndex; }
            set
            {
                this.RaiseAndSetIfChanged(ref _StandardWorkIndex, value);
                if (value != -1)
                {
                    QueueIndex = -1;
                }
            }
        }

        private FnTEMPTRAINING_SELECTEResult _SelectedStandardWork = new FnTEMPTRAINING_SELECTEResult();
        public FnTEMPTRAINING_SELECTEResult SelectedStandardWork
        {
            get { return _SelectedStandardWork; }
            set { this.RaiseAndSetIfChanged(ref _SelectedStandardWork, value); }
        }

        private ReactiveList<VTRAINING> _QueueList = new ReactiveList<VTRAINING>();
        public ReactiveList<VTRAINING> QueueList
        {
            get { return _QueueList; }
            set
            {
                this.RaiseAndSetIfChanged(ref _QueueList, value);
                App.OnUI(() => { QueueHasItems = QueueList.Count > 0; });
            }
        }

    private bool _QueueHasItems = false;
    public bool QueueHasItems
    {
        get { return _QueueHasItems; }
        set
        {
            this.RaiseAndSetIfChanged(ref _QueueHasItems, value);
        }
    }

    private int _QueueIndex = -1;
        public int QueueIndex
        {
            get { return _QueueIndex; }
            set
            {
                this.RaiseAndSetIfChanged(ref _QueueIndex, value);
                if (value != -1)
                {
                    StandardWorkIndex = -1;
                }
            }
        }

        private VTRAINING _SelectedQueue;
        public VTRAINING SelectedQueue
        {
            get { return _SelectedQueue; }
            set { this.RaiseAndSetIfChanged(ref _SelectedQueue, value); }
        }

        public ReactiveCommand QueueAddCommand { get; set; }
        public ReactiveCommand QueueDelCommand { get; set; }

        private string _fSW;
        public string fSW
        {
            get { return _fSW; }
            set
            {
                this.RaiseAndSetIfChanged(ref _fSW, value);
                ApplyFilter();
            }
        }

        private async void ApplyFilter()
        {
            bWorking = true;
            await Task.Run(() =>
            { 
                if (!string.IsNullOrWhiteSpace(fSW)) { StandardWorkList = new ReactiveList<FnTEMPTRAINING_SELECTEResult>(StandardWorkOriginalList.Where(x => x.SW_CODE.ToUpper().Contains(fSW.ToUpper()) || x.SW_DESCRIPTION.ToUpper().Contains(fSW.ToUpper()))); }
                else { StandardWorkList = StandardWorkOriginalList; }
                StandardWorkIndex = -1;
                bWorking = false;
                return 0;
            });
        }
    }
}
