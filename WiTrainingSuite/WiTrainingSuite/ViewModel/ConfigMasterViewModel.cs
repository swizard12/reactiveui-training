using MaterialDesignThemes.Wpf;
using ReactiveUI;
using System;
using WiTrainingSuite.Model;
using WiTrainingSuite.ViewModel.ConfigMenuItems;

namespace WiTrainingSuite.ViewModel
{
    public class ConfigMasterViewModel : ReactiveObject, IRoutableViewModel
    {
        #region IRoutableViewModel

        public string UrlPathSegment
        {
            get { return "configmaster"; }
        }

        public IScreen HostScreen { get; protected set; }

        #endregion

        public SnackbarMessageQueue SnackBarQueue { get; set; }

        public ConfigMasterViewModel(IScreen screen)
        {
            HostScreen = screen;

            SnackBarQueue = new SnackbarMessageQueue();

            // Initialise Config Menu
            ConfigMenu = new ReactiveList<ConfigMenuItem>();

            // Add Config Panes
            ConfigMenu.Add(new ConfigMenuItem("Departments", ReactiveCommand.Create(() => 
            {
                SelectedConfigMenuItem = new ConfigDeptViewModel();
            })));
            ConfigMenu.Add(new ConfigMenuItem("Document Tags", ReactiveCommand.Create(() =>
            {
                SelectedConfigMenuItem = new ConfigDocumentTagViewModel();
            })));
            ConfigMenu.Add(new ConfigMenuItem("Role", ReactiveCommand.Create(() =>
            {
                SelectedConfigMenuItem = new ConfigRoleViewModel();
            })));
            ConfigMenu.Add(new ConfigMenuItem("Variant", ReactiveCommand.Create(() =>
            {
                SelectedConfigMenuItem = new ConfigVariantViewModel();
            })));
            ConfigMenu.Add(new ConfigMenuItem("Shift", ReactiveCommand.Create(() =>
            {
                SelectedConfigMenuItem = new ConfigShiftViewModel();
            })));
            ConfigMenu.Add(new ConfigMenuItem("Skill", ReactiveCommand.Create(() =>
            {
                SelectedConfigMenuItem = new ConfigSkillViewModel();
            })));
        }

        private ConfigBaseViewModel _SelectedConfigMenuItem = new ConfigBaseViewModel();
        public ConfigBaseViewModel SelectedConfigMenuItem
        {
            get { return _SelectedConfigMenuItem; }
            set { this.RaiseAndSetIfChanged(ref _SelectedConfigMenuItem, value); }
        }

        private ReactiveList<ConfigMenuItem> _ConfigMenu;
        public ReactiveList<ConfigMenuItem> ConfigMenu
        {
            get { return _ConfigMenu; }
            set { this.RaiseAndSetIfChanged(ref _ConfigMenu, value); }
        }
    }

    public class ConfigMenuItem
    {
        public ConfigMenuItem(string itemName, ReactiveCommand itemAction)
        {
            ITEM_NAME = itemName;
            ITEM_ACTION = itemAction;
        }
        public string ITEM_NAME { get; set; }
        public ReactiveCommand ITEM_ACTION { get; set; }
    }
}