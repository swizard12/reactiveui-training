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
        }

        private FnTSTANDARDWORK_LISTResult _StandardWork = new FnTSTANDARDWORK_LISTResult();
        public FnTSTANDARDWORK_LISTResult StandardWork
        {
            get { return _StandardWork; }
            set { this.RaiseAndSetIfChanged(ref _StandardWork, value); }
        }
    }
}
