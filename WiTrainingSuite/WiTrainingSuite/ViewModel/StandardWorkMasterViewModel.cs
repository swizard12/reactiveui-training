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
                OriginalList = StandardWorkList;
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

            EditTrainingCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.Navigate.Execute(new StandardWorkTrainingEditViewModel(HostScreen, SelectedStandardWork));
            }, this.WhenAny(
                x => x.StandardWorkIndex,
                x => x.IsAdmin,
                (i, a) => i.Value > -1 && a.Value == true));
            
            ListTrainingCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.Navigate.Execute(new StandardWorkTrainingListViewModel(HostScreen, SelectedStandardWork));
            }, this.WhenAny(
                x => x.StandardWorkIndex,
                (i) => i.Value != -1));

            ClearCodeFilter = ReactiveCommand.Create(() =>
            {
                fCODE = string.Empty;
            }, this.WhenAny(
                x => x.fCODE,
                (y) => !string.IsNullOrWhiteSpace(y.Value)));

            ClearDescFilter = ReactiveCommand.Create(() =>
            {
                fDESC = string.Empty;
            }, this.WhenAny(
                x => x.fDESC,
                (y) => !string.IsNullOrWhiteSpace(y.Value)));

            ClearAll = ReactiveCommand.Create(() =>
            {
                fCODE = string.Empty;
                fDESC = string.Empty;
            }, this.WhenAny(
                x => x.fCODE,
                x => x.fDESC,
                (c, d) => !string.IsNullOrWhiteSpace(c.Value) ||
                          !string.IsNullOrWhiteSpace(d.Value)));

            // Key = SQL Column Header; Value = Friendly Name
            ColumnHeaders.Add("SW_CODE", "Code");
            ColumnHeaders.Add("SW_DESCRIPTION", "Description");
            ColumnHeaders.Add("SW_ISSUE", "Issue #");
            ColumnHeaders.Add("SW_ISSUEDATE", "Issue Date");
            ColumnHeaders.Add("SW_RA", "Risk Assessment #");
        }

        public void ApplySort()
        {
            string f = BuildFilterString(SortList);

            if (!string.IsNullOrWhiteSpace(f) && StandardWorkList != null)
                StandardWorkList = new ReactiveList<FnTSTANDARDWORK_LISTResult>(StandardWorkList.OrderBy(f, this).AsEnumerable());
        }

        private int _StandardWorkIndex = -1;
        public int StandardWorkIndex
        {
            get { return _StandardWorkIndex; }
            set { this.RaiseAndSetIfChanged(ref _StandardWorkIndex, value); }
        }

        public ReactiveCommand NewStandardWorkCommand { get; set; }
        public ReactiveCommand EditStandardWorkCommand { get; set; }
        public ReactiveCommand ListTrainingCommand { get; set; }
        public ReactiveCommand EditTrainingCommand { get; set; }
        public ReactiveCommand ClearCodeFilter { get; set; }
        public ReactiveCommand ClearDescFilter { get; set; }
        public ReactiveCommand ClearAll { get; set; }

        private bool _bWorking = false;
        public bool bWorking
        {
            get { return _bWorking; }
            set { this.RaiseAndSetIfChanged(ref _bWorking, value); }
        }

        private string _fCODE;
        public string fCODE
        {
            get { return _fCODE; }
            set
            {
                this.RaiseAndSetIfChanged(ref _fCODE, value);
                var task = Task.Run(() => ApplyFilter());
                bWorking = true;
                task.ContinueWith((x) => { bWorking = false; });
            }
        }

        private string _fDESC;
        public string fDESC
        {
            get { return _fDESC; }
            set
            {
                this.RaiseAndSetIfChanged(ref _fDESC, value);
                var task = Task.Run(() => ApplyFilter());
                bWorking = true;
                task.ContinueWith((x) => { bWorking = false; });
            }
        }

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

        private ReactiveList<FnTSTANDARDWORK_LISTResult> _OriginalList = new ReactiveList<FnTSTANDARDWORK_LISTResult>();
        public ReactiveList<FnTSTANDARDWORK_LISTResult> OriginalList
        {
            get { return _OriginalList; }
            set { this.RaiseAndSetIfChanged(ref _OriginalList, value); }
        }

        private async void ApplyFilter()
        {
            await Task.Run(() =>
            {
                var tList = new ReactiveList<FnTSTANDARDWORK_LISTResult>(OriginalList);

                // Pass One - SW Code
                if (!string.IsNullOrWhiteSpace(fCODE)) { tList = new ReactiveList<FnTSTANDARDWORK_LISTResult>(OriginalList.Where(x => x.SW_CODE.ToUpper().Contains(fCODE.ToUpper()))); }
                else { tList = OriginalList; }

                // Pass Two - Description
                if (!string.IsNullOrWhiteSpace(fDESC)) { tList = new ReactiveList<FnTSTANDARDWORK_LISTResult>(tList.Where(x => x.SW_DESCRIPTION.ToUpper().Contains(fCODE.ToUpper()))); }

                StandardWorkList = tList;

                ApplySort();

                StandardWorkIndex = -1;

                return 0;
            });
        }

        #endregion
    }
}