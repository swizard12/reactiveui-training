using MaterialDesignThemes.Wpf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using WiTrainingSuite.Model;

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

        readonly ObservableAsPropertyHelper<bool> isAdmin;
        public bool IsAdmin
        {
            get { return isAdmin.Value; }
        }

        public bool admin;

        public StandardWorkMasterViewModel(IScreen screen)
        {
            admin = App._admin;
            this.WhenAnyValue(x => x.admin).ToProperty(this, x => x.IsAdmin, out isAdmin);

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
            }, this.WhenAny(
                x => x.IsAdmin,
                (a) => a.Value == true));

            EditStandardWorkCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.Navigate.Execute(new StandardWorkDetailEditViewModel(HostScreen, SelectedStandardWork));
            }, this.WhenAny(
                x => x.StandardWorkIndex,
                x => x.IsAdmin, 
                (i, a) => i.Value > -1 && a.Value == true));

            // Key = SQL Column Header; Value = Friendly Name
            ColumnHeaders.Add("SW_CODE", "Code");
            ColumnHeaders.Add("SW_DESCRIPTION", "Description");
            ColumnHeaders.Add("SW_ISSUE", "Issue #");
            ColumnHeaders.Add("SW_ISSUEDATE", "Issue Date");
            ColumnHeaders.Add("SW_RA", "Risk Assessment #");
        }

        private int _StandardWorkIndex = -1;
        public int StandardWorkIndex
        {
            get { return _StandardWorkIndex; }
            set { this.RaiseAndSetIfChanged(ref _StandardWorkIndex, value); }
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
