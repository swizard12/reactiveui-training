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

            StandardWork = standardwork;

            BackCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.NavigateBack.Execute();
            });

            SaveCommand = ReactiveCommand.Create(() =>
            {
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    db.FnTSTANDARDWORK_UPDATE(StandardWork.SW_ID,
                                              StandardWork.SW_CODE,
                                              StandardWork.SW_DESCRIPTION,
                                              StandardWork.SW_ISSUE,
                                              StandardWork.SW_ISSUEDATE,
                                              StandardWork.SW_RA);
                    HostScreen.Router.FindViewModelInStack<StandardWorkMasterViewModel>().StandardWorkList = 
                        new ReactiveList<FnTSTANDARDWORK_LISTResult>(db.FnTSTANDARDWORK_LIST().OrderBy("SW_CODE", this).AsEnumerable());
                    HostScreen.Router.FindViewModelInStack<StandardWorkMasterViewModel>().SelectedStandardWork =
                        StandardWork;

                    HostScreen.Router.NavigateAndReset.Execute(HostScreen.Router.FindViewModelInStack<StandardWorkMasterViewModel>());
                }
            });
        }

        public ReactiveCommand BackCommand { get; set; }
        public ReactiveCommand SaveCommand { get; set; }

        private FnTSTANDARDWORK_LISTResult _StandardWork;
        public FnTSTANDARDWORK_LISTResult StandardWork
        {
            get { return _StandardWork; }
            set { this.RaiseAndSetIfChanged(ref _StandardWork, value); }
        }
    }
}
