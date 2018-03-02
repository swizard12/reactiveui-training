using MaterialDesignThemes.Wpf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using WiTrainingSuite.Model;

namespace WiTrainingSuite.ViewModel
{
    public class ConfigMenuRoleMgmtViewModel : ReactiveObject, IRoutableViewModel
    {
        #region IRoutableViewModel
        public string UrlPathSegment
        {
            get { return "configmenurolemgmt"; }
        }
        public IScreen HostScreen { get; protected set; }
        #endregion

        public SnackbarMessageQueue SnackBarQueue { get; set; }

        public ConfigMenuRoleMgmtViewModel(IScreen screen)
        {
            HostScreen = screen;

            SnackBarQueue = new SnackbarMessageQueue();

            PrepList();

            NewCommand = ReactiveCommand.Create(() =>
            {
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    db.TROLE.InsertOnSubmit(new TROLE() { ROLE_NAME = String.Format("NEW ROLE {0}", DateTime.Now.ToString("ddMMMyyyy")) });
                    db.SubmitChanges();
                    PrepList();
                }
            });

            DelCommand = ReactiveCommand.Create(() =>
            {
                SnackBarQueue.Enqueue(String.Format("Confirm Delete : TROLE : {0}", SelectedItem.ROLE_NAME),
                        "YES",
                        () =>
                        {
                            using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                            {
                                db.TROLE.Attach(SelectedItem);
                                db.TROLE.DeleteOnSubmit(SelectedItem);
                                db.SubmitChanges();
                                PrepList();
                            }
                        });
            }, this.WhenAny(
                x => x.SelectedIndex,
                (i) => i.Value != -1));

            UpdCommand = ReactiveCommand.Create(() =>
            {
                SnackBarQueue.Enqueue("Confirm Update : TROLE",
                        "YES",
                        () =>
                        {
                            using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                            {
                                db.SubmitChanges();
                                PrepList();
                            }
                        });
            });
        }

        public async void PrepList()
        {
            await Task.Run(() =>
            {
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    LabelList = new ReactiveList<TROLE>(db.TROLE);
                }
            });
        }

        public ReactiveCommand NewCommand { get; set; }
        public ReactiveCommand UpdCommand { get; set; }
        public ReactiveCommand DelCommand { get; set; }
        public ReactiveCommand BackCommand { get; set; }

        private ReactiveList<TROLE> _LabelList;
        public ReactiveList<TROLE> LabelList
        {
            get { return _LabelList; }
            set { this.RaiseAndSetIfChanged(ref _LabelList, value); }
        }

        private TROLE _SelectedItem;
        public TROLE SelectedItem
        {
            get { return _SelectedItem; }
            set { this.RaiseAndSetIfChanged(ref _SelectedItem, value); }
        }

        private int _SelectedIndex = -1;
        public int SelectedIndex
        {
            get { return _SelectedIndex; }
            set { this.RaiseAndSetIfChanged(ref _SelectedIndex, value); }
        }
    }
}