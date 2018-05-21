using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiTrainingSuite.Model;

namespace WiTrainingSuite.ViewModel.ConfigMenuItems
{
    public class ConfigRoleViewModel : ConfigBaseViewModel
    {
        public ConfigRoleViewModel()
        {
            SaveConfig = ReactiveCommand.Create(async () =>
            {
                switch(SelectedConfig.ROLE_ID)
                {
                    case null:
                        await DbInterface.fnRoleCreate(SelectedConfig);
                        ConfigList = new ReactiveList<RoleResult>(await DbInterface.fnRoleList());
                        SelectedConfigIndex = -1;
                        SelectedConfig = new RoleResult();
                        break;
                    default:
                        await DbInterface.fnRoleUpdate(SelectedConfig);
                        ConfigList = new ReactiveList<RoleResult>(await DbInterface.fnRoleList());
                        SelectedConfigIndex = -1;
                        SelectedConfig = new RoleResult();
                        break;
                }
            }, this.WhenAny(x => x.SelectedConfig.ROLE_NAME,
                (a) => !string.IsNullOrWhiteSpace(a.Value)));

            DeleteConfig = ReactiveCommand.Create(async () =>
            {
                await DbInterface.fnRoleDelete(SelectedConfig);
                ConfigList = new ReactiveList<RoleResult>(await DbInterface.fnRoleList());
                SelectedConfigIndex = -1;
                SelectedConfig = new RoleResult();
            }, this.WhenAny(x => x.SelectedConfigIndex,
                (i) => i.Value != -1));

            ClearConfig = ReactiveCommand.Create(() =>
            {
                SelectedConfigIndex = -1;
                SelectedConfig = new RoleResult();
            });

            Task.Run(async () =>
            {
                ConfigList = new ReactiveList<RoleResult>(await DbInterface.fnRoleList());
            });
        }

        private ReactiveList<RoleResult> _ConfigList;
        public ReactiveList<RoleResult> ConfigList
        {
            get { return _ConfigList; }
            set { this.RaiseAndSetIfChanged(ref _ConfigList, value); }
        }

        private RoleResult _SelectedConfig = new RoleResult();
        public RoleResult SelectedConfig
        {
            get { return _SelectedConfig; }
            set { this.RaiseAndSetIfChanged(ref _SelectedConfig, value); }
        }

        private int _SelectedConfigIndex = -1;
        public int SelectedConfigIndex
        {
            get { return _SelectedConfigIndex; }
            set { this.RaiseAndSetIfChanged(ref _SelectedConfigIndex, value); }
        }

        public ReactiveCommand SaveConfig { get; set; }
        public ReactiveCommand DeleteConfig { get; set; }
        public ReactiveCommand ClearConfig { get; set; }
    }
}
