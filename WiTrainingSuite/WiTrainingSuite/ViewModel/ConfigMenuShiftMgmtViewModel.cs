using MaterialDesignThemes.Wpf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiTrainingSuite.Model;

namespace WiTrainingSuite.ViewModel
{
    public class ConfigMenuShiftMgmtViewModel : ReactiveObject, IRoutableViewModel
    {
        #region IRoutableViewModel
        public string UrlPathSegment
        {
            get { return "configmenushiftmgmt"; }
        }
        public IScreen HostScreen { get; protected set; }
        #endregion

        public SnackbarMessageQueue SnackBarQueue { get; set; }

        public ConfigMenuShiftMgmtViewModel(IScreen screen)
        {
            HostScreen = screen;

            SnackBarQueue = new SnackbarMessageQueue();
            PrepList();

            NewCommand = ReactiveCommand.Create(() =>
            {
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    db.TSHIFT.InsertOnSubmit(new TSHIFT() { SHIFT_NAME = String.Format("NEW SHIFT {0}", DateTime.Now.ToString("ddMMMyyyy")) });
                    db.SubmitChanges();
                    PrepList();
                }
            });

            DelCommand = ReactiveCommand.Create(() =>
            {
                SnackBarQueue.Enqueue(String.Format("Confirm Delete : TSHIFT : {0}", SelectedItem.SHIFT_NAME),
                        "YES",
                        () =>
                        {
                            using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                            {
                                db.TSHIFT.Attach(SelectedItem);
                                db.TSHIFT.DeleteOnSubmit(SelectedItem);
                                db.SubmitChanges();
                                PrepList();
                            }
                        });
            }, this.WhenAny(
                x => x.SelectedIndex,
                (i) => i.Value != -1));

            UpdCommand = ReactiveCommand.Create(() =>
            {
                SnackBarQueue.Enqueue("Confirm Update : TSHIFT",
                        "YES",
                        () =>
                        {
                            using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                            {
                                db.SubmitChanges();
                            }
                        });
            }, this.WhenAny(
                x => x.SelectedIndex,
                (i) => i.Value != -1));
        }

        public async void PrepList()
        {
            await Task.Run(() =>
            {
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    LabelList = new ReactiveList<TSHIFT>(db.TSHIFT);
                }
            });
        }

        public ReactiveCommand NewCommand { get; set; }
        public ReactiveCommand UpdCommand { get; set; }
        public ReactiveCommand DelCommand { get; set; }
        public ReactiveCommand BackCommand { get; set; }

        private ReactiveList<TSHIFT> _LabelList;
        public ReactiveList<TSHIFT> LabelList
        {
            get { return _LabelList; }
            set { this.RaiseAndSetIfChanged(ref _LabelList, value); }
        }

        private TSHIFT _SelectedItem;
        public TSHIFT SelectedItem
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