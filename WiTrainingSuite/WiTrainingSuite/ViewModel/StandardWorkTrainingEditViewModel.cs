using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using WiTrainingSuite.Model;
using WiTrainingSuite.View;

namespace WiTrainingSuite.ViewModel
{
    public class StandardWorkTrainingEditViewModel : ReactiveObject, IRoutableViewModel
    {
        #region IRoutableViewModel

        public string UrlPathSegment
        {
            get { return "standardworktrainingedit"; }
        }
        public IScreen HostScreen { get; protected set; }

        #endregion

        #region StandardWorkTrainingEditViewModel

        public SnackbarMessageQueue SnackBarQueue { get; set; }

        public StandardWorkTrainingEditViewModel(IScreen screen, FnTSTANDARDWORK_LISTResult standardwork)
        {
            HostScreen = screen;

            StandardWork = standardwork;

            SnackBarQueue = new SnackbarMessageQueue();

            PrepLists();

            ////// Define Reactive Commands

            QueueAddCommand = ReactiveCommand.Create(() =>
            {
                bWorking = true;
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    db.FnTEMPTRAINING_QUEUEADD(
                        SelectedEmployee.EMP_ID,
                        StandardWork.SW_ID,
                        1,
                        VALID_DATE,
                        "Full Training Declared");
                    EmployeeIndex = -1;
                    fEMP = string.Empty;

                    RefreshTrainingLists();
                }
            }, this.WhenAny(
                x => x.DateSet,
                x => x.EmployeeIndex,
                (d, e) => d.Value && e.Value != -1));

            QueueDelCommand = ReactiveCommand.Create(() =>
            {
                bWorking = true;
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    db.FnTEMPTRAINING_QUEUEDELETE(
                        SelectedQueue.EMP_ID,
                        StandardWork.SW_ID);
                    EmployeeIndex = -1;
                    fEMP = string.Empty;

                    RefreshTrainingLists();
                }
            }, this.WhenAny(
                x => x.DateSet,
                x => x.QueueIndex,
                (d, e) => d.Value && e.Value != -1));

            ClearEMPFilter = ReactiveCommand.Create(() =>
            {
                bWorking = true;
                EmployeeList = EmployeeOriginalList;
                fEMP = string.Empty;
                EmployeeIndex = -1;
            }, this.WhenAny(
                x => x.fEMP,
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
                                db.FnTEMPTRAINING_QUEUECOMMITS(StandardWork.SW_ID);
                            });

                            await DialogManager.ShowMessageAsync(App.TopWindow, "Action Confirmed", "Records saved, please review the training list");

                            HostScreen.Router.NavigateAndReset.Execute(new StandardWorkTrainingListViewModel(HostScreen, StandardWork));
                        }
                    });
            },
            this.WhenAny(
                x => x.QueueHasItems,
                (q) => q.Value));

            //////
        }

        public ReactiveCommand QueueAddCommand { get; set; }
        public ReactiveCommand QueueDelCommand { get; set; }

        public ReactiveCommand BackCommand { get; set; }
        public ReactiveCommand SaveCommand { get; set; }

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
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    // Purge TEMPTRAINING_WIP
                    var deleteStandardWorkTraining =
                        from w in db.TEMPTRAINING_WIP
                        where w.SW_ID == StandardWork.SW_ID
                        select w;

                    foreach (var w in deleteStandardWorkTraining)
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

                    // Get Original Employee List
                    EmployeeOriginalList = new ReactiveList<FnTEMPTRAINING_SELECTSResult>(db.FnTEMPTRAINING_SELECTS(StandardWork.SW_ID));

                    EmployeeList = EmployeeOriginalList;
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
                    QueueList = new ReactiveList<VTRAINING>(from t in db.VTRAINING.Where(x => x.SW_ID == StandardWork.SW_ID)
                                                            where (from w in db.TEMPTRAINING_WIP.Where(x => x.SW_ID == StandardWork.SW_ID)
                                                                   select w.EMP_ID).Contains(t.EMP_ID)
                                                            select t);

                    // Get Original Employee List, less what is in the Queue
                    EmployeeOriginalList = new ReactiveList<FnTEMPTRAINING_SELECTSResult>(from t in db.FnTEMPTRAINING_SELECTS(StandardWork.SW_ID)
                                                                                          where !(from q in QueueList
                                                                                                  select q.EMP_ID).ToList()
                                                                                                  .Contains(t.EMP_ID.GetValueOrDefault())
                                                                                          select t);
                }

                EmployeeList = EmployeeOriginalList;

                bWorking = false;
            });
        }

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

        #region EmployeeNameFilter

        private string _fEMP;
        public string fEMP
        {
            get { return _fEMP; }
            set { this.RaiseAndSetIfChanged(ref _fEMP, value);
                ApplyFilter();
            }
        }

        public ReactiveCommand ClearEMPFilter { get; set; }

        public async void ApplyFilter()
        {
            bWorking = true;
            await Task.Run(() =>
            {
                if(!string.IsNullOrWhiteSpace(fEMP))
                {
                    EmployeeList = new ReactiveList<FnTEMPTRAINING_SELECTSResult>(
                      EmployeeOriginalList.Where(x => 
                          x.EMP_FNAME.ToUpper().Contains(fEMP.ToUpper()) || 
                          x.EMP_LNAME.ToUpper().Contains(fEMP.ToUpper()))
                    );
                }
                else
                {
                    EmployeeList = EmployeeOriginalList;
                }
                EmployeeIndex = -1;
                bWorking = false;
            });
        }

        #endregion  

        private FnTSTANDARDWORK_LISTResult _StandardWork;
        public FnTSTANDARDWORK_LISTResult StandardWork
        {
            get { return _StandardWork; }
            set { this.RaiseAndSetIfChanged(ref _StandardWork, value); }
        }

        private FnTEMPTRAINING_SELECTSResult _SelectedEmployee = new FnTEMPTRAINING_SELECTSResult();
        public FnTEMPTRAINING_SELECTSResult SelectedEmployee
        {
            get { return _SelectedEmployee; }
            set { this.RaiseAndSetIfChanged(ref _SelectedEmployee, value); }
        }

        private int _EmployeeIndex = -1;
        public int EmployeeIndex
        {
            get { return _EmployeeIndex; }
            set { this.RaiseAndSetIfChanged(ref _EmployeeIndex, value);
                if (value != -1)
                    QueueIndex = -1;
            }
        }

        private ReactiveList<FnTEMPTRAINING_SELECTSResult> _EmployeeList = new ReactiveList<FnTEMPTRAINING_SELECTSResult>();
        public ReactiveList<FnTEMPTRAINING_SELECTSResult> EmployeeList
        {
            get { return _EmployeeList; }
            set { this.RaiseAndSetIfChanged(ref _EmployeeList, value); }
        }

        private ReactiveList<FnTEMPTRAINING_SELECTSResult> _EmployeeOriginalList;
        public ReactiveList<FnTEMPTRAINING_SELECTSResult> EmployeeOriginalList
        {
            get { return _EmployeeOriginalList; }
            set { this.RaiseAndSetIfChanged(ref _EmployeeOriginalList, value); }
        }

        private VTRAINING _SelectedQueue;
        public VTRAINING SelectedQueue
        {
            get { return _SelectedQueue; }
            set { this.RaiseAndSetIfChanged(ref _SelectedQueue, value); }
        }

        private int _QueueIndex = -1;
        public int QueueIndex
        {
            get { return _QueueIndex; }
            set { this.RaiseAndSetIfChanged(ref _QueueIndex, value);
                if (value != -1)
                    EmployeeIndex = -1;
            }
        }

        private ReactiveList<VTRAINING> _QueueList = new ReactiveList<VTRAINING>();
        public ReactiveList<VTRAINING> QueueList
        {
            get { return _QueueList; }
            set { this.RaiseAndSetIfChanged(ref _QueueList, value);
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

        #endregion
    }
}