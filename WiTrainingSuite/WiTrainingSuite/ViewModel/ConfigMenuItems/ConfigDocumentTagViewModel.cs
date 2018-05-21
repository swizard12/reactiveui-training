using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiTrainingSuite.Model;

namespace WiTrainingSuite.ViewModel.ConfigMenuItems
{
    public class ConfigDocumentTagViewModel : ConfigBaseViewModel
    {
        public ConfigDocumentTagViewModel()
        {
            SaveConfig = ReactiveCommand.Create(async () =>
            {
                switch (SelectedConfig.DOCTAG_ID)
                {
                    case null:
                        // Create New Department
                        await DbInterface.fnDocTagCreate(SelectedConfig);
                        ConfigList = new ReactiveList<DocTagResult>(await DbInterface.fnDocTagList());
                        SelectedConfigIndex = -1;
                        SelectedConfig = new DocTagResult();
                        break;
                    default:
                        // Update Existing Department
                        await DbInterface.fnDocTagUpdate(SelectedConfig);
                        ConfigList = new ReactiveList<DocTagResult>(await DbInterface.fnDocTagList());
                        SelectedConfigIndex = -1;
                        SelectedConfig = new DocTagResult();
                        break;
                }
            }, this.WhenAny(x => x.SelectedConfig.DOCTAG_NAME,
                (a) => !string.IsNullOrWhiteSpace(a.Value)));

            DeleteConfig = ReactiveCommand.Create(async () =>
            {
                await DbInterface.fnDocTagDelete(SelectedConfig);
                ConfigList = new ReactiveList<DocTagResult>(await DbInterface.fnDocTagList());
                SelectedConfigIndex = -1;
                SelectedConfig = new DocTagResult();
            }, this.WhenAny(x => x.SelectedConfigIndex,
                (i) => i.Value != -1));

            ClearConfig = ReactiveCommand.Create(() =>
            {
                SelectedConfigIndex = -1;
                SelectedConfig = new DocTagResult();
            });

            Task.Run(async () =>
            {
                ConfigList = new ReactiveList<DocTagResult>(await DbInterface.fnDocTagList());
            });
        }

        private ReactiveList<DocTagResult> _ConfigList;
        public ReactiveList<DocTagResult> ConfigList
        {
            get { return _ConfigList; }
            set { this.RaiseAndSetIfChanged(ref _ConfigList, value); }
        }

        private DocTagResult _SelectedConfig = new DocTagResult();
        public DocTagResult SelectedConfig
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

