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
    public class ConfigMenuSkillMgmtViewModel : ReactiveObject, IRoutableViewModel
    {
        #region IRoutableViewModel
        public string UrlPathSegment
        {
            get { return "configmenuskillmgmt"; }
        }
        public IScreen HostScreen { get; protected set; }
        #endregion

        public SnackbarMessageQueue SnackBarQueue { get; set; }

        public ConfigMenuSkillMgmtViewModel(IScreen screen)
        {
            HostScreen = screen;

            SnackBarQueue = new SnackbarMessageQueue();

            PrepList();

            NewCommand = ReactiveCommand.Create(() =>
            {
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    db.TSKILL.InsertOnSubmit(new TSKILL() { SKILL_NAME = String.Format("NEW SKILL {0}", DateTime.Now.ToString("ddMMMyyyy")) });
                    db.SubmitChanges();
                    PrepList();
                }
            });

            DelCommand = ReactiveCommand.Create(() =>
            {
                SnackBarQueue.Enqueue(String.Format("Confirm Delete : TSKILL : {0}", SelectedItem.SKILL_NAME),
                        "YES",
                        () =>
                        {
                            using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                            {
                                db.TSKILL.Attach(SelectedItem);
                                db.TSKILL.DeleteOnSubmit(SelectedItem);
                                db.SubmitChanges();
                                PrepList();
                            }
                        });
            }, this.WhenAny(
                x => x.SelectedIndex,
                (i) => i.Value != -1));

            UpdCommand = ReactiveCommand.Create(() =>
            {
                SnackBarQueue.Enqueue("Confirm Update : TSKILL",
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
                    LabelList = new ReactiveList<TSKILL>(db.TSKILL);
                }
            });
        }

        public ReactiveCommand NewCommand { get; set; }
        public ReactiveCommand UpdCommand { get; set; }
        public ReactiveCommand DelCommand { get; set; }
        public ReactiveCommand BackCommand { get; set; }

        private ReactiveList<TSKILL> _LabelList;
        public ReactiveList<TSKILL> LabelList
        {
            get { return _LabelList; }
            set { this.RaiseAndSetIfChanged(ref _LabelList, value); }
        }

        private TSKILL _SelectedItem;
        public TSKILL SelectedItem
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
