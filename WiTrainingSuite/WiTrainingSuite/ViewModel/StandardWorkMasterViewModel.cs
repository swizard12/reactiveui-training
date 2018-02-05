using MaterialDesignThemes.Wpf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
using System.Threading.Tasks;

namespace WiTrainingSuite.ViewModel
{
    public class StandardWorkMasterViewModel : ExtendedViewModelBase, IRoutableViewModel
    {
        #region IRoutableViewModel
        public string UrlPathSegment
        {
            get { return "standardworkmaster"; }
        }
        public IScreen HostScreen { get; protected set; }
        #endregion

        #region StandardWorkViewModel

        public SnackbarMessageQueue SnackBarQueue { get; set; }

        public StandardWorkMasterViewModel(IScreen screen)
        {
            HostScreen = screen;

            SnackBarQueue = new SnackbarMessageQueue();

            using (Wi_training_suite db = new Wi_training_suite(App.ConString))
            {
                StandardWorkList = new ReactiveList<FnTSTANDARDWORK_LISTResult>(db.FnTSTANDARDWORK_LIST().OrderBy("SW_CODE", this).AsEnumerable());
            }
            SnackBarQueue.Enqueue("Load Successful");

            CloseDrawerCommand = ReactiveCommand.Create(() =>
            {
                IsBtmOpen = false;

                string f = BuildFilterString(SortList);

                if (!string.IsNullOrWhiteSpace(f) && StandardWorkList != null)
                    StandardWorkList = new ReactiveList<FnTSTANDARDWORK_LISTResult>(StandardWorkList.OrderBy(f, this).AsEnumerable());
            });

            NewStandardWorkCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.Navigate.Execute(new StandardWorkDetailNewViewModel(HostScreen));
            });

            EditStandardWorkCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.Navigate.Execute(new StandardWorkDetailEditViewModel(HostScreen, SelectedStandardWork));
            },
            this.WhenAnyValue(
                x => x.SelectedStandardWork,
                (s) => s.SW_ID > 0));

            // Key = SQL Column Header; Value = Friendly Name
            ColumnHeaders.Add("SW_CODE", "Code");
            ColumnHeaders.Add("SW_DESCRIPTION", "Description");
            ColumnHeaders.Add("SW_ISSUE", "Issue #");
            ColumnHeaders.Add("SW_ISSUEDATE", "Issue Date");
            ColumnHeaders.Add("SW_RA", "Risk Assessment #");
        }

        public ReactiveCommand NewStandardWorkCommand { get; set; }
        public ReactiveCommand EditStandardWorkCommand { get; set; }

        private ReactiveList<FnTSTANDARDWORK_LISTResult> _StandardWorkList;
        public ReactiveList<FnTSTANDARDWORK_LISTResult> StandardWorkList
        {
            get { return _StandardWorkList; }
            set { this.RaiseAndSetIfChanged(ref _StandardWorkList, value); }
        }

        private FnTSTANDARDWORK_LISTResult _SelectedStandardWork = new FnTSTANDARDWORK_LISTResult();
        public FnTSTANDARDWORK_LISTResult SelectedStandardWork
        {
            get { return _SelectedStandardWork; }
            set { this.RaiseAndSetIfChanged(ref _SelectedStandardWork, value); }
        }

        #endregion
    }
}
