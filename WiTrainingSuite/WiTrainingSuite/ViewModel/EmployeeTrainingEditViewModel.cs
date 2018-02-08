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

            SnackBarQueue = new SnackbarMessageQueue();

            using (Wi_training_suite db = new Wi_training_suite(App.ConString))
            {
                // Purge WIP Table for this employee
                var deleteEmployeeTraining =
                    from w in db.TEMPTRAINING_WIP
                    where w.EMP_ID == Employee.EMP_ID
                    select w;

                foreach(var w in deleteEmployeeTraining)
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

                StandardWorkOriginalList = new ReactiveList<FnTEMPTRAINING_SELECTEResult>(from t in db.FnTEMPTRAINING_SELECTE(Employee.EMP_ID) select t);

                StandardWorkList = StandardWorkOriginalList;
            }

            QueueAddCommand = ReactiveCommand.Create(() =>
            {
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    db.FnTEMPTRAINING_QUEUEADD(
                        employee.EMP_ID,
                        SelectedStandardWork.SW_ID,
                        1,
                        VALID_DATE,
                        "Full Training Declared");

                    RefreshTrainingLists();
                }
            });

            QueueDelCommand = ReactiveCommand.Create(() =>
            {
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    db.FnTEMPTRAINING_QUEUEDELETE(
                        SelectedQueue.EMP_ID,
                        SelectedQueue.SW_ID);

                    RefreshTrainingLists();
                }
            });
        }

        public void RefreshTrainingLists()
        {
            using (Wi_training_suite db = new Wi_training_suite(App.ConString))
            {
                QueueList = new ReactiveList<VTRAINING>(
                    from t in db.VTRAINING.Where(x => x.EMP_ID == Employee.EMP_ID)
                    where (from w in db.TEMPTRAINING_WIP.Where(x => x.EMP_ID == Employee.EMP_ID)
                           select w.SW_ID).Contains(t.SW_ID)
                    select t);

                StandardWorkOriginalList = new ReactiveList<FnTEMPTRAINING_SELECTEResult>(from t in db.FnTEMPTRAINING_SELECTE(Employee.EMP_ID)
                                                                                          where !(from q in QueueList
                                                                                                  select q.SW_ID).ToList()
                                                                                                  .Contains(t.SW_ID.GetValueOrDefault())
                                                                                          select t);

                StandardWorkList = StandardWorkOriginalList;
            }
        }


        private ReactiveList<FnTEMPTRAINING_SELECTEResult> _XX;
        public ReactiveList<FnTEMPTRAINING_SELECTEResult> XX
        {
            get { return _XX; }
            set { this.RaiseAndSetIfChanged(ref _XX, value); }
        }


        private FnTEMPLOYEE_LISTResult _Employee;
        public FnTEMPLOYEE_LISTResult Employee
        {
            get { return _Employee; }
            set { this.RaiseAndSetIfChanged(ref _Employee, value); }
        }

        private String _CodeFilter;
        public String CodeFilter
        {
            get { return _CodeFilter; }
            set
            {
                this.RaiseAndSetIfChanged(ref _CodeFilter, value);
                if (!string.IsNullOrWhiteSpace(value))
                {
                    StandardWorkList = new ReactiveList<FnTEMPTRAINING_SELECTEResult>(StandardWorkOriginalList.Where(x => x.SW_CODE.Contains(CodeFilter)).OrderBy("SW_CODE"));
                }
            }
        }

        private DateTime _VALID_DATE = DateTime.Today;
        public DateTime VALID_DATE
        {
            get { return _VALID_DATE; }
            set { this.RaiseAndSetIfChanged(ref _VALID_DATE, value); }
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

        private ReactiveList<VTRAINING> _QueueList;
        public ReactiveList<VTRAINING>  QueueList
        {
            get { return _QueueList; }
            set { this.RaiseAndSetIfChanged(ref _QueueList, value); }
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
    }
}
