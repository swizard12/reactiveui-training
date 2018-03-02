using ReactiveUI;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;
using WiTrainingSuite.Model;

namespace WiTrainingSuite.ViewModel
{
    public class StandardWorkDetailEditViewModel : ReactiveObject, IRoutableViewModel
    {
        #region IRoutableViewModel
        public string UrlPathSegment
        {
            get { return "standardworkdetailedit"; }
        }
        public IScreen HostScreen { get; protected set; }
        #endregion

        public SnackbarMessageQueue SnackBarQueue { get; set; }

        public StandardWorkDetailEditViewModel(IScreen screen, FnTSTANDARDWORK_LISTResult standardwork)
        {
            HostScreen = screen;

            SnackBarQueue = new SnackbarMessageQueue();

            StandardWork = standardwork;

            PrepWIP();

            PrepLists();

            BackCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.NavigateAndReset.Execute(HostScreen.Router.FindViewModelInStack<StandardWorkMasterViewModel>());
            });

            SaveCommand = ReactiveCommand.Create(() =>
            {
                // Show Confirmation Dialog in Top Right Corner
                SnackBarQueue.Enqueue(
                    String.Format("Commit Changes to Database?"),
                    "CONFIRM",
                    // Confirmation Method Here
                    async () =>
                    {
                        using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                        {
                            await Task.Run(() =>
                            {
                                // Update SW Details
                                db.FnTSTANDARDWORK_UPDATE(StandardWork.SW_ID,
                                                          StandardWork.SW_CODE,
                                                          StandardWork.SW_DESCRIPTION,
                                                          StandardWork.SW_ISSUE,
                                                          StandardWork.SW_ISSUEDATE,
                                                          StandardWork.SW_RA);
                                // Update SW Skill Assignment
                                db.FnTSKILLSW_COMMIT(StandardWork.SW_ID);
                            });
                            HostScreen.Router.FindViewModelInStack<StandardWorkMasterViewModel>().StandardWorkList =
                                new ReactiveList<FnTSTANDARDWORK_LISTResult>(db.FnTSTANDARDWORK_LIST().OrderBy("SW_CODE", this).AsEnumerable());
                            HostScreen.Router.FindViewModelInStack<StandardWorkMasterViewModel>().SelectedStandardWork =
                                StandardWork;

                            HostScreen.Router.NavigateAndReset.Execute(HostScreen.Router.FindViewModelInStack<StandardWorkMasterViewModel>());
                        }
                    });
            });

            ListAddCommand = ReactiveCommand.Create(() =>
            {
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    db.FnTSKILLSW_ADD(StandardWork.SW_ID, SelectedPool.SKILL_ID);
                    PrepLists();
                }
            }, this.WhenAny(
                x => x.PoolIndex,
                (i) => i.Value != -1));

            ListDelCommand = ReactiveCommand.Create(() =>
            {
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    db.FnTSKILLSW_DEL(StandardWork.SW_ID, SelectedList.SKILL_ID);
                    PrepLists();
                }
            }, this.WhenAny(
                x => x.ListIndex,
                (i) => i.Value != -1));
        }

        public async void PrepWIP()
        {
            await Task.Run(() =>
            {
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    db.FnTSKILLSW_PREP(StandardWork.SW_ID);
                }
            });
        }
            
        public async void PrepLists()
        {
            await Task.Run(() =>
            {
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    SkillList = new ReactiveList<TSKILL>(from k in db.TSKILL
                                                         where (from x in db.TSKILLSW_WIP
                                                                where x.SW_ID == StandardWork.SW_ID
                                                                select x.SKILL_ID).Contains(k.SKILL_ID)
                                                         select k);
                    SkillPool = new ReactiveList<TSKILL>(from k in db.TSKILL
                                                         where !(from x in SkillList
                                                                 select x.SKILL_ID).Contains(k.SKILL_ID)
                                                         select k);
                }
            });
        }

        public ReactiveCommand BackCommand { get; set; }
        public ReactiveCommand SaveCommand { get; set; }
        public ReactiveCommand ListAddCommand { get; set; }
        public ReactiveCommand ListDelCommand { get; set; }

        private FnTSTANDARDWORK_LISTResult _StandardWork;
        public FnTSTANDARDWORK_LISTResult StandardWork
        {
            get { return _StandardWork; }
            set { this.RaiseAndSetIfChanged(ref _StandardWork, value); }
        }

        private ReactiveList<TSKILL> _SkillList;
        public ReactiveList<TSKILL> SkillList
        {
            get { return _SkillList; }
            set { this.RaiseAndSetIfChanged(ref _SkillList, value); }
        }

        private TSKILL _SelectedList;
        public TSKILL SelectedList
        {
            get { return _SelectedList; }
            set { this.RaiseAndSetIfChanged(ref _SelectedList, value); }
        }

        private int _ListIndex = -1;
        public int ListIndex
        {
            get { return _ListIndex; }
            set { this.RaiseAndSetIfChanged(ref _ListIndex, value);
                if (value != -1) PoolIndex = -1; }
        }

        private ReactiveList<TSKILL> _SkillPool;
        public ReactiveList<TSKILL> SkillPool
        {
            get { return _SkillPool; }
            set { this.RaiseAndSetIfChanged(ref _SkillPool, value); }
        }

        private TSKILL _SelectedPool;
        public TSKILL SelectedPool
        {
            get { return _SelectedPool; }
            set { this.RaiseAndSetIfChanged(ref _SelectedPool, value); }
        }

        private int _PoolIndex = -1;
        public int PoolIndex
        {
            get { return _PoolIndex; }
            set { this.RaiseAndSetIfChanged(ref _PoolIndex, value);
                if (value != -1) ListIndex = -1; }
        }
    }
}
