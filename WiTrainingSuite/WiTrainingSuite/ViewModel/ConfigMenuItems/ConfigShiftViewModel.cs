using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiTrainingSuite.Model;

namespace WiTrainingSuite.ViewModel.ConfigMenuItems
{
    public class ConfigShiftViewModel : ConfigBaseViewModel
    {
        public ConfigShiftViewModel()
        {
            SaveConfig = ReactiveCommand.Create(async () =>
            {
                switch (SelectedConfig.SHIFT_ID)
                {
                    case null:
                        // Create New Department
                        await DbInterface.fnShiftCreate(SelectedConfig);
                        ConfigList = new ReactiveList<ShiftResult>(await DbInterface.fnShiftList());
                        SelectedConfigIndex = -1;
                        SelectedConfig = new ShiftResult();
                        break;
                    default:
                        // Update Existing Department
                        await DbInterface.fnShiftUpdate(SelectedConfig);
                        ConfigList = new ReactiveList<ShiftResult>(await DbInterface.fnShiftList());
                        SelectedConfigIndex = -1;
                        SelectedConfig = new ShiftResult();
                        break;
                }
            }, this.WhenAny(x => x.SelectedConfig.SHIFT_NAME,
                (a) => !string.IsNullOrWhiteSpace(a.Value)));

            DeleteConfig = ReactiveCommand.Create(async () =>
            {
                await DbInterface.fnShiftDelete(SelectedConfig);
                ConfigList = new ReactiveList<ShiftResult>(await DbInterface.fnShiftList());
                SelectedConfigIndex = -1;
                SelectedConfig = new ShiftResult();
            }, this.WhenAny(x => x.SelectedConfigIndex,
                (i) => i.Value != -1));

            ClearConfig = ReactiveCommand.Create(() =>
            {
                SelectedConfigIndex = -1;
                SelectedConfig = new ShiftResult();
            });

            Task.Run(async () =>
            {
                ConfigList = new ReactiveList<ShiftResult>(await DbInterface.fnShiftList());
            });
        }

        private ReactiveList<ShiftResult> _ConfigList;
        public ReactiveList<ShiftResult> ConfigList
        {
            get { return _ConfigList; }
            set { this.RaiseAndSetIfChanged(ref _ConfigList, value); }
        }

        private ShiftResult _SelectedConfig = new ShiftResult();
        public ShiftResult SelectedConfig
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
