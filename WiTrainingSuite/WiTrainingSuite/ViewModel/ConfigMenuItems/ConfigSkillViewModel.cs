using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiTrainingSuite.Model;

namespace WiTrainingSuite.ViewModel.ConfigMenuItems
{
    public class ConfigSkillViewModel : ConfigBaseViewModel
    {
        public ConfigSkillViewModel()
        {
            SaveConfig = ReactiveCommand.Create(async () =>
            {
                switch (SelectedConfig.SKILL_ID)
                {
                    case null:
                        // Create New Department
                        await DbInterface.fnSkillCreate(SelectedConfig);
                        ConfigList = new ReactiveList<SkillResult>(await DbInterface.fnSkillList());
                        SelectedConfigIndex = -1;
                        SelectedConfig = new SkillResult();
                        break;
                    default:
                        // Update Existing Department
                        await DbInterface.fnSkillUpdate(SelectedConfig);
                        ConfigList = new ReactiveList<SkillResult>(await DbInterface.fnSkillList());
                        SelectedConfigIndex = -1;
                        SelectedConfig = new SkillResult();
                        break;
                }
            }, this.WhenAny(x => x.SelectedConfig.SKILL_NAME,
                (a) => !string.IsNullOrWhiteSpace(a.Value)));

            DeleteConfig = ReactiveCommand.Create(async () =>
            {
                await DbInterface.fnSkillDelete(SelectedConfig);
                ConfigList = new ReactiveList<SkillResult>(await DbInterface.fnSkillList());
                SelectedConfigIndex = -1;
                SelectedConfig = new SkillResult();
            }, this.WhenAny(x => x.SelectedConfigIndex,
                            (i) => i.Value != -1));

            ClearConfig = ReactiveCommand.Create(() =>
            {
                SelectedConfigIndex = -1;
                SelectedConfig = new SkillResult();
            });

            Task.Run(async () =>
            {
                ConfigList = new ReactiveList<SkillResult>(await DbInterface.fnSkillList());
            });
        }

        private ReactiveList<SkillResult> _ConfigList;
        public ReactiveList<SkillResult> ConfigList
        {
            get { return _ConfigList; }
            set { this.RaiseAndSetIfChanged(ref _ConfigList, value); }
        }

        private SkillResult _SelectedConfig = new SkillResult();
        public SkillResult SelectedConfig
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
