using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiTrainingSuite.Model;

namespace WiTrainingSuite.ViewModel.ConfigMenuItems
{
    public class ConfigDeptViewModel : ConfigBaseViewModel
    {
        public ConfigDeptViewModel()
        {
            SaveConfig = ReactiveCommand.Create(async () =>
            {
                switch(SelectedConfig.DEPT_ID)
                {
                    case null:
                        // Create New Department
                        await DbInterface.fnDeptCreate(SelectedConfig);
                        ConfigList = new ReactiveList<DeptResult>(await DbInterface.fnDeptList());
                        SelectedConfigIndex = -1;
                        SelectedConfig = new DeptResult();
                        break;
                    default:
                        // Update Existing Department
                        await DbInterface.fnDeptUpdate(SelectedConfig);
                        ConfigList = new ReactiveList<DeptResult>(await DbInterface.fnDeptList());
                        SelectedConfigIndex = -1;
                        SelectedConfig = new DeptResult();
                        break;
                }
            }, this.WhenAny(x => x.SelectedConfig.DEPT_NAME,
                (a) => !string.IsNullOrWhiteSpace(a.Value)));

            DeleteConfig = ReactiveCommand.Create(async () =>
            {
                await DbInterface.fnDeptDelete(SelectedConfig);
                ConfigList = new ReactiveList<DeptResult>(await DbInterface.fnDeptList());
                SelectedConfigIndex = -1;
                SelectedConfig = new DeptResult();
            }, this.WhenAny(x => x.SelectedConfigIndex,
                (i) => i.Value != -1));

            ClearConfig = ReactiveCommand.Create(() =>
            {
                SelectedConfigIndex = -1;
                SelectedConfig = new DeptResult();
            });

            Task.Run(async () =>
            {
                ConfigList = new ReactiveList<DeptResult>(await DbInterface.fnDeptList());
            });
        }

        private ReactiveList<DeptResult> _ConfigList;
        public ReactiveList<DeptResult> ConfigList
        {
            get { return _ConfigList; }
            set { this.RaiseAndSetIfChanged(ref _ConfigList, value); }
        }

        private DeptResult _SelectedConfig = new DeptResult();
        public DeptResult SelectedConfig
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
