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
    public class DocumentMasterViewModel : ReactiveObject, IRoutableViewModel
    {
        #region IRoutableViewModel
        public string UrlPathSegment
        {
            get { return "documentmaster"; }
        }
        public IScreen HostScreen { get; protected set; }

        public SnackbarMessageQueue SnackBarQueue { get; set; }

        #endregion

        public DocumentMasterViewModel(IScreen screen)
        {
            HostScreen = screen;

            SnackBarQueue = new SnackbarMessageQueue();

            // Init Commands
            DocumentAdd = ReactiveCommand.Create(() => 
            {
                HostScreen.Router.Navigate.Execute(new DocumentDetailViewModel(HostScreen));
            });
            DocumentEdit = ReactiveCommand.Create(() => 
            {
                HostScreen.Router.Navigate.Execute(new DocumentDetailViewModel(HostScreen, SelectedDocument));
            },
                this.WhenAny(x => x.SelectedDocumentIndex,
                             x => x.IsWorking,
                (i, w) => i.Value != -1 && w.Value == false));
            DocumentDelete = ReactiveCommand.Create(() =>
            {
                SnackBarQueue.Enqueue(String.Format("Are you sure you want to delete {0}?", SelectedDocument.DOC_CODE), "YES", async () =>
                {
                    IsWorking = true;
                    await DbInterface.fnDocumentDelete(SelectedDocument);
                    DocumentList = new ReactiveList<DocumentResult>(await DbInterface.fnDocumentList());
                    IsWorking = false;
                });
            }, this.WhenAny(x => x.SelectedDocumentIndex, x => x.IsWorking, (i, w) => i.Value != -1 && w.Value == false));
            DocumentTraining = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.Navigate.Execute(new DocumentTrainingViewModel(HostScreen, SelectedDocument));
            }, 
                this.WhenAny(x => x.SelectedDocumentIndex,
                             x => x.IsWorking,
                (i, w) => i.Value != -1 && w.Value == false));

            FilterCommand = ReactiveCommand.Create(() =>
            {
                IsWorking = true;
                if (!String.IsNullOrWhiteSpace(FilterText))
                {
                    var fList = new ReactiveList<DocumentResult>(
                        OriginalList.Where(
                            x => x.DOC_CODE.ToUpper().Contains(FilterText.ToUpper()) ||
                                 x.DOC_TITLE.ToUpper().Contains(FilterText.ToUpper())));
                    DocumentList = fList;
                }
                else
                {
                    DocumentList = OriginalList;
                }
                IsWorking = false;
            });
            ClearFilterCommand = ReactiveCommand.Create(() =>
            {
                FilterText = string.Empty;
            }, this.WhenAny(x => x.FilterText,
                (f) => !string.IsNullOrWhiteSpace(f.Value)));

            // Get Documents

            Task.Run(async () =>
            {
                DocumentList = new ReactiveList<DocumentResult>(new ReactiveList<DocumentResult>(await DbInterface.fnDocumentList()).OrderBy(x => x.DOC_CODE));
                OriginalList = DocumentList;
            }).ContinueWith((Unit) =>
            {
                IsWorking = false;
            });

            this.WhenAnyValue(x => x.FilterText)
                .Throttle(TimeSpan.FromSeconds(.25))
                .Select(_ => Unit.Default)
                .InvokeCommand(FilterCommand);
        }

        public ReactiveCommand DocumentAdd { get; set; }
        public ReactiveCommand DocumentEdit { get; set; }
        public ReactiveCommand DocumentDelete { get; set; }
        public ReactiveCommand DocumentTraining { get; set; }

        public ReactiveCommand ClearFilterCommand { get; set; }
        public ReactiveCommand FilterCommand { get; set; }

        private ReactiveList<DocumentResult> _DocumentList;
        public ReactiveList<DocumentResult> DocumentList
        {
            get { return _DocumentList; }
            set { this.RaiseAndSetIfChanged(ref _DocumentList, value); }
        }

        private ReactiveList<DocumentResult> _OriginalList;
        public ReactiveList<DocumentResult> OriginalList
        {
            get { return _OriginalList; }
            set { this.RaiseAndSetIfChanged(ref _OriginalList, value); }
        }

        private DocumentResult _SelectedDocument;
        public DocumentResult SelectedDocument
        {
            get { return _SelectedDocument; }
            set { this.RaiseAndSetIfChanged(ref _SelectedDocument, value); }
        }

        private int _SelectedDocumentIndex = -1;
        public int SelectedDocumentIndex
        {
            get { return _SelectedDocumentIndex; }
            set { this.RaiseAndSetIfChanged(ref _SelectedDocumentIndex, value); }
        }

        private string _FilterText;
        public string FilterText
        {
            get { return _FilterText; }
            set { this.RaiseAndSetIfChanged(ref _FilterText, value); }
        }

        private bool _IsWorking = true;
        public bool IsWorking
        {
            get { return _IsWorking; }
            set { this.RaiseAndSetIfChanged(ref _IsWorking, value); }
        }
    }
}
