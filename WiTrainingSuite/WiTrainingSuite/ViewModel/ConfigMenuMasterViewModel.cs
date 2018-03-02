using MaterialDesignThemes.Wpf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiTrainingSuite.ViewModel
{
    public class ConfigMenuMasterViewModel : ReactiveObject, IRoutableViewModel
    {
        #region IRoutableViewModel
        public string UrlPathSegment
        {
            get { return "configmaster"; }
        }
        public IScreen HostScreen { get; protected set; }
        #endregion

        public SnackbarMessageQueue SnackBarQueue { get; set; }

        public ConfigMenuMasterViewModel(IScreen screen)
        {
            HostScreen = screen;

            SnackBarQueue = new SnackbarMessageQueue();

            GeneratePaperworkCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.Navigate.Execute(new ConfigMenuTrainingPaperWorkViewModel(HostScreen));
            });

            RoleMgmtCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.Navigate.Execute(new ConfigMenuRoleMgmtViewModel(HostScreen));
            });

            VarMgmtCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.Navigate.Execute(new ConfigMenuVarMgmtViewModel(HostScreen));
            });

            SkillMgmtCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.Navigate.Execute(new ConfigMenuSkillMgmtViewModel(HostScreen));
            });

            ShiftMgmtCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.Navigate.Execute(new ConfigMenuShiftMgmtViewModel(HostScreen));
            });
        }

        public ReactiveCommand GeneratePaperworkCommand { get; set; }
        public ReactiveCommand ShiftMgmtCommand { get; set; }
        public ReactiveCommand SkillMgmtCommand { get; set; }
        public ReactiveCommand VarMgmtCommand { get; set; }
        public ReactiveCommand RoleMgmtCommand { get; set; }
    }
}
