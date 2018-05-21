using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiTrainingSuite.Model;

namespace WiTrainingSuite.ViewModel.ConfigMenuItems
{
    public class ConfigVariantViewModel : ConfigBaseViewModel
    {
        public ConfigVariantViewModel()
        {
            SaveConfig = ReactiveCommand.Create(async () =>
            {
                switch(SelectedConfig.VAR_ID)
                {
                    case null:
                        await DbInterface.fnVarCreate(SelectedConfig);
                        ConfigList = new ReactiveList<VariantResult>(await DbInterface.fnVarList());
                        SelectedConfigIndex = -1;
                        SelectedConfig = new VariantResult();
                        break;
                    default:
                        await DbInterface.fnVarUpdate(SelectedConfig);
                        ConfigList = new ReactiveList<VariantResult>(await DbInterface.fnVarList());
                        SelectedConfigIndex = -1;
                        SelectedConfig = new VariantResult();
                        break;
                }
            }, this.WhenAny(x => x.SelectedConfig.VAR_NAME, 
                            x => x.SelectedConfig.ROLE_ID,
                            (a, b) => !string.IsNullOrWhiteSpace(a.Value) && b.Value != -1));

            DeleteConfig = ReactiveCommand.Create(async () =>
            {
                await DbInterface.fnVarDelete(SelectedConfig);
                ConfigList = new ReactiveList<VariantResult>(await DbInterface.fnVarList());
                SelectedConfigIndex = -1;
                SelectedConfig = new VariantResult();
            }, this.WhenAny(x => x.SelectedConfigIndex,
                (i) => i.Value != -1));

            ClearConfig = ReactiveCommand.Create(() =>
            {
                SelectedConfigIndex = -1;
                SelectedConfig = new VariantResult();
            });

            Task.Run(async () =>
            {
                ConfigList = new ReactiveList<VariantResult>(await DbInterface.fnVarList());
                RoleList = new ReactiveList<RoleResult>(await DbInterface.fnRoleList());
            });
        }

        private ReactiveList<RoleResult> _RoleList;
        public ReactiveList<RoleResult> RoleList
        {
            get { return _RoleList; }
            set { this.RaiseAndSetIfChanged(ref _RoleList, value); }
        }

        private ReactiveList<VariantResult> _ConfigList;
        public ReactiveList<VariantResult> ConfigList
        {
            get { return _ConfigList; }
            set { this.RaiseAndSetIfChanged(ref _ConfigList, value); }
        }

        private VariantResult _SelectedConfig = new VariantResult();
        public VariantResult SelectedConfig
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
