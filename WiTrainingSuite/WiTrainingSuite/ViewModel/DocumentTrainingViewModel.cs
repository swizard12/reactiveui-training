using MaterialDesignThemes.Wpf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using WiTrainingSuite.Model;

namespace WiTrainingSuite.ViewModel
{
    public class DocumentTrainingViewModel : ReactiveObject, IRoutableViewModel
    {
        #region IRoutableViewModel

        public string UrlPathSegment
        {
            get { return "documenttraining"; }
        }

        public IScreen HostScreen { get; protected set; }

        public SnackbarMessageQueue SnackBarQueue { get; set; }

        #endregion

        public DocumentTrainingViewModel(IScreen screen, DocumentResult document)
        {
            HostScreen = screen;

            SnackBarQueue = new SnackbarMessageQueue();

            SelectedDocument = document;

            TrainingSave = ReactiveCommand.Create(() =>
            {
                SnackBarQueue.Enqueue(String.Format("About to Commit {0} Records, Continue?", TrainingQueue.Count), "YES", async () =>
                {
                    await DbInterface.fnDocumentTrainingSave(SelectedDocument, TrainingQueue, ValidFromDate);
                    await HostScreen.Router.NavigateBack.Execute();
                });
            }, this.WhenAny(x => x.TrainingQueue.Count, x => x.ValidFromDate, (i, j) => i.Value > 0 && j.Value > DateTime.Now.AddYears(-1)));

            TrainingCancel = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.NavigateBack.Execute();
            });

            TrainingQueueAdd = ReactiveCommand.Create(() =>
            {
                TrainingQueue.Add(TrainingPoolSelected);
                TrainingPool.Remove(TrainingPoolSelected);
                TrainingQueue = new ReactiveList<TrainingResult>(TrainingQueue.OrderBy(x => x.EMP_LNAME));
                TrainingPool = new ReactiveList<TrainingResult>(TrainingPool.OrderBy(x => x.EMP_LNAME));
            },
                this.WhenAny(x => x.TrainingPoolIndex,
                (i) => i.Value != -1));

            TrainingQueueDel = ReactiveCommand.Create(() =>
            {
                TrainingPool.Add(TrainingQueueSelected);
                TrainingQueue.Remove(TrainingQueueSelected);
                TrainingQueue = new ReactiveList<TrainingResult>(TrainingQueue.OrderBy(x => x.EMP_LNAME));
                TrainingPool = new ReactiveList<TrainingResult>(TrainingPool.OrderBy(x => x.EMP_LNAME));
            },
                this.WhenAny(x => x.TrainingQueueIndex,
                (i) => i.Value != -1));

            FilterCommand = ReactiveCommand.Create(() =>
            {
                IsWorking = true;
                if (!String.IsNullOrWhiteSpace(FilterText))
                {
                    var fList = new ReactiveList<TrainingResult>(
                        OriginalList.Where(x => x.EMP_FNAME.ToUpper().Contains(FilterText.ToUpper()) ||
                                                x.EMP_LNAME.ToUpper().Contains(FilterText.ToUpper())).OrderBy(x => x.EMP_LNAME));
                    TrainingPool = fList;
                    IsWorking = false;
                }
                else
                {
                    TrainingPool = OriginalList;
                    IsWorking = false;
                }
            });

            ClearFilterCommand = ReactiveCommand.Create(() =>
            {
                FilterText = string.Empty;
            }, this.WhenAny(x => x.FilterText,
                (f) => !string.IsNullOrWhiteSpace(f.Value)));

            Task LoadList = Task.Run(async () =>
            {
                TrainingPool = new ReactiveList<TrainingResult>(await DbInterface.fnDocumentTraining(SelectedDocument));
                OriginalList = TrainingPool;
            }).ContinueWith((Unit) =>
            {
                IsWorking = false;
            });

            this.WhenAnyValue(x => x.FilterText)
                .Throttle(TimeSpan.FromSeconds(.25))
                .Select(_ => Unit.Default)
                .InvokeCommand(FilterCommand);
        }

        // Command Binding

        public ReactiveCommand TrainingSave { get; set; }
        public ReactiveCommand TrainingCancel { get; set; }
        public ReactiveCommand TrainingQueueAdd { get; set; }
        public ReactiveCommand TrainingQueueDel { get; set; }

        public ReactiveCommand FilterCommand { get; set; }
        public ReactiveCommand ClearFilterCommand { get; set; }

        // Header Information

        private DocumentResult _SelectedDocument;
        public DocumentResult SelectedDocument
        {
            get { return _SelectedDocument; }
            set { this.RaiseAndSetIfChanged(ref _SelectedDocument, value); }
        }

        // Original Training Pool

        private ReactiveList<TrainingResult> _OriginalList;
        public ReactiveList<TrainingResult> OriginalList
        {
            get { return _OriginalList; }
            set { this.RaiseAndSetIfChanged(ref _OriginalList, value); }
        }

        // Training Pool

        private ReactiveList<TrainingResult> _TrainingPool = new ReactiveList<TrainingResult>();
        public ReactiveList<TrainingResult> TrainingPool
        {
            get { return _TrainingPool; }
            set { this.RaiseAndSetIfChanged(ref _TrainingPool, value); }
        }

        private int _TrainingPoolIndex = -1;
        public int TrainingPoolIndex
        {
            get { return _TrainingPoolIndex; }
            set { this.RaiseAndSetIfChanged(ref _TrainingPoolIndex, value);
                if (value != -1)
                    TrainingQueueIndex = -1;
            }
        }

        private TrainingResult _TrainingPoolSelected;
        public TrainingResult TrainingPoolSelected
        {
            get { return _TrainingPoolSelected; }
            set { this.RaiseAndSetIfChanged(ref _TrainingPoolSelected, value); }
        }

        // Training Queue

        private ReactiveList<TrainingResult> _TrainingQueue = new ReactiveList<TrainingResult>();
        public ReactiveList<TrainingResult> TrainingQueue
        {
            get { return _TrainingQueue; }
            set { this.RaiseAndSetIfChanged(ref _TrainingQueue, value); }
        }

        private int _TrainingQueueIndex = -1;
        public int TrainingQueueIndex
        {
            get { return _TrainingQueueIndex; }
            set { this.RaiseAndSetIfChanged(ref _TrainingQueueIndex, value);
                if (value != -1)
                    TrainingPoolIndex = -1;
            }
        }

        private TrainingResult _TrainingQueueSelected;
        public TrainingResult TrainingQueueSelected
        {
            get { return _TrainingQueueSelected; }
            set { this.RaiseAndSetIfChanged(ref _TrainingQueueSelected, value); }
        }

        // Employee Filter

        private string _FilterText;
        public string FilterText
        {
            get { return _FilterText; }
            set { this.RaiseAndSetIfChanged(ref _FilterText, value); }
        }

        // Valid From Date

        private DateTime _ValidFromDate = DateTime.Today;
        public DateTime ValidFromDate
        {
            get { return _ValidFromDate; }
            set { this.RaiseAndSetIfChanged(ref _ValidFromDate, value); }
        }

        // Loading Bar

        private bool _IsWorking = true;
        public bool IsWorking
        {
            get { return _IsWorking; }
            set { this.RaiseAndSetIfChanged(ref _IsWorking, value); }
        }
    }
}
