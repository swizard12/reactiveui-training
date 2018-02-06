using ReactiveUI;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiTrainingSuite.ViewModel
{
    public class StandardWorkDetailNewViewModel : ReactiveObject, IRoutableViewModel
    {
        #region IRoutableViewModel
        public string UrlPathSegment
        {
            get { return "standardworkdetailnew"; }
        }
        public IScreen HostScreen { get; protected set; }
        #endregion

        public SnackbarMessageQueue SnackBarQueue { get; set; }

        public StandardWorkDetailNewViewModel(IScreen screen)
        {
            HostScreen = screen;

            SnackBarQueue = new SnackbarMessageQueue();

            BackCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.NavigateBack.Execute();
            });

            SaveCommand = ReactiveCommand.Create(() =>
            {
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    int? sw_id = 0;
                    db.FnTSTANDARDWORK_CREATE(StandardWork.SW_CODE, StandardWork.SW_DESCRIPTION, StandardWork.SW_ISSUE, StandardWork.SW_ISSUEDATE, StandardWork.SW_RA, ref sw_id);
                    HostScreen.Router.NavigateAndReset.Execute(new StandardWorkDetailEditViewModel(HostScreen, (FnTSTANDARDWORK_LISTResult)db.FnTSTANDARDWORK_SELECT(sw_id).First()));
                }
            }, this.WhenAnyValue(
                x => x.StandardWork.SW_CODE,
                x => x.StandardWork.SW_DESCRIPTION,
                x => x.StandardWork.SW_ISSUE,
                x => x.StandardWork.SW_ISSUEDATE,
                    (c, d, i, id) => !string.IsNullOrEmpty(c) &&
                                 !string.IsNullOrEmpty(d) &&
                                 i > 0 &&
                                 id.HasValue));
        }

        public ReactiveCommand BackCommand { get; set; }
        public ReactiveCommand SaveCommand { get; set; }

        private FnTSTANDARDWORK_LISTResult _StandardWork = new FnTSTANDARDWORK_LISTResult();
        public FnTSTANDARDWORK_LISTResult StandardWork
        {
            get { return _StandardWork; }
            set { this.RaiseAndSetIfChanged(ref _StandardWork, value); }
        }
    }
}
