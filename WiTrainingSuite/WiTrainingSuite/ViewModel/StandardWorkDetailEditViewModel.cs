using ReactiveUI;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        private FnTSTANDARDWORK_LISTResult _StandardWork;
        public FnTSTANDARDWORK_LISTResult StandardWork
        {
            get { return _StandardWork; }
            set { this.RaiseAndSetIfChanged(ref _StandardWork, value); }
        }
    }
}
