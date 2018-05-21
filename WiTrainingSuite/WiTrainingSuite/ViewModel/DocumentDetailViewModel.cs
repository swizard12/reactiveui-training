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
    public class DocumentDetailViewModel : ReactiveObject, IRoutableViewModel
    {
        #region IRoutableViewModel

        public string UrlPathSegment
        {
            get { return "documentdetail"; }
        }

        public IScreen HostScreen { get; protected set; }

        #endregion

        public SnackbarMessageQueue SnackBarQueue { get; set; }

        // New Document
        public DocumentDetailViewModel(IScreen screen)
        {
            HostScreen = screen;

            SnackBarQueue = new SnackbarMessageQueue();
                        
            // Init Document
            SelectedDocument = new DocumentResult();

            CanSaveWithoutTraining = false;

            DocumentSaveNoTraining = ReactiveCommand.Create(() =>
            {
            }, this.WhenAny(x => x.CanSaveWithoutTraining,
                (c) => c.Value));

            // Bind Commands
            DocumentSave = ReactiveCommand.Create(() =>
            {
                SnackBarQueue.Enqueue($"Are you sure you want to save {SelectedDocument.DOC_CODE}? Training WILL BE UPDATED.", "YES", async () =>
                 {
                     await DbInterface.fnDocumentCreate(SelectedDocument, SkillTags);
                     await HostScreen.Router.NavigateAndReset.Execute(new DocumentMasterViewModel(HostScreen));
                 });
            }, this.WhenAny(x => x.SelectedDocument.DOC_CODE,
                            x => x.SelectedDocument.DOC_TITLE,
                            x => x.SelectedDocument.DOC_ISSUE,
                            x => x.SelectedDocument.DOC_ISSUEDATE,
                            (a, b, c, d) => !String.IsNullOrWhiteSpace(a.Value) &&
                                         !String.IsNullOrWhiteSpace(b.Value) &&
                                         c.Value > 0 &&
                                         d.Value > new DateTime(DateTime.Today.Year, 1, 1)));

            BindCommonCommands();

            // Load Skill Pool & Tags
            Task.Run(async () =>
            {
                SkillTags = new ReactiveList<SkillResult>(await DbInterface.fnDocumentSelectSkills(SelectedDocument, 0));
                SkillPool = new ReactiveList<SkillResult>(await DbInterface.fnDocumentSelectSkills(SelectedDocument, 1));
                IsWorking = false;
            });
        }

        // Edit Document
        public DocumentDetailViewModel(IScreen screen, DocumentResult document)
        {
            HostScreen = screen;

            SnackBarQueue = new SnackbarMessageQueue();

            // Init Document
            SelectedDocument = document;

            // Bind Commands
            DocumentSave = ReactiveCommand.Create(() =>
            {
                SnackBarQueue.Enqueue($"Are you sure you want to save {SelectedDocument.DOC_CODE}? Training WILL BE UPDATED.", "YES", async () =>
                {
                    await DbInterface.fnDocumentUpdate(SelectedDocument, SkillTags);
                    await HostScreen.Router.NavigateAndReset.Execute(new DocumentMasterViewModel(HostScreen));
                });
            }, this.WhenAny(x => x.SelectedDocument.DOC_CODE,
                            x => x.SelectedDocument.DOC_TITLE,
                            x => x.SelectedDocument.DOC_ISSUE,
                            (a, b, c) => !String.IsNullOrWhiteSpace(a.Value) &&
                                         !String.IsNullOrWhiteSpace(b.Value) &&
                                         c.Value >= 0));

            DocumentSaveNoTraining = ReactiveCommand.Create(() =>
            {
                SnackBarQueue.Enqueue($"Are you sure you want to save {SelectedDocument.DOC_CODE}? Training WILL NOT be affected.", "YES", async () =>
                {
                    await DbInterface.fnDocumentUpdateNoTraining(SelectedDocument);
                    await HostScreen.Router.NavigateAndReset.Execute(new DocumentMasterViewModel(HostScreen));
                });
            }, this.WhenAny(x => x.CanSaveWithoutTraining,
                (c) => c.Value));

            BindCommonCommands();

            // Load Skill Pool & Tags
            Task.Run(async () =>
            {
                DocTagList = new ReactiveList<DocTagResult>(await DbInterface.fnDocTagList());
                SkillTags = new ReactiveList<SkillResult>(await DbInterface.fnDocumentSelectSkills(SelectedDocument, 0));
                SkillPool = new ReactiveList<SkillResult>(await DbInterface.fnDocumentSelectSkills(SelectedDocument, 1));
                IsWorking = false;
            });
        }

        public void BindCommonCommands()
        {
            DocumentCancel = ReactiveCommand.Create(() => { HostScreen.Router.NavigateBack.Execute(); });

            DocTagClear = ReactiveCommand.Create(() =>
            {
                SelectedDocument.DOCTAG_ID = null;
            }, this.WhenAny(x => x.SelectedDocument.DOCTAG_ID,
                (i) => i.Value != null));

            TagListAdd = ReactiveCommand.Create(() =>
            {
                SkillTags.Add(PoolSelected);
                SkillPool.Remove(PoolSelected);
                SkillTags = new ReactiveList<SkillResult>(SkillTags.OrderBy(x => x.SKILL_NAME));
            }, this.WhenAny(
                x => x.PoolIndex,
                (i) => i.Value != -1));

            TagListDelete = ReactiveCommand.Create(() =>
            {
                SkillPool.Add(TagSelected);
                SkillTags.Remove(TagSelected);
                SkillPool = new ReactiveList<SkillResult>(SkillPool.OrderBy(x => x.SKILL_NAME));
            }, this.WhenAny(
                x => x.TagIndex,
                (i) => i.Value != -1));
        }

        private bool _CanSaveWithoutTraining = true;
        public bool CanSaveWithoutTraining {
            get { return _CanSaveWithoutTraining; }
            set { this.RaiseAndSetIfChanged(ref _CanSaveWithoutTraining, value); }
        }

        public ReactiveCommand DocumentCancel { get; set; }
        public ReactiveCommand DocumentSave { get; set; }
        public ReactiveCommand DocumentSaveNoTraining{ get; set; }
        public ReactiveCommand DocTagClear { get; set; }
        public ReactiveCommand TagListAdd { get; set; }
        public ReactiveCommand TagListDelete { get; set; }

        private ReactiveList<DocTagResult> _DocTagList = new ReactiveList<DocTagResult>();
        public ReactiveList<DocTagResult> DocTagList
        {
            get { return _DocTagList; }
            set { this.RaiseAndSetIfChanged(ref _DocTagList, value); }
        }

        private ReactiveList<SkillResult> _SkillTags = new ReactiveList<SkillResult>();
        public ReactiveList<SkillResult> SkillTags
        {
            get { return _SkillTags; }
            set { this.RaiseAndSetIfChanged(ref _SkillTags, value); }
        }

        private ReactiveList<SkillResult> _SkillPool = new ReactiveList<SkillResult>();
        public ReactiveList<SkillResult> SkillPool
        {
            get { return _SkillPool; }
            set { this.RaiseAndSetIfChanged(ref _SkillPool, value); }
        }

        private SkillResult _PoolSelected;
        public SkillResult PoolSelected
        {
            get { return _PoolSelected; }
            set { this.RaiseAndSetIfChanged(ref _PoolSelected, value); }
        }

        private int _PoolIndex = -1;
        public int PoolIndex
        {
            get { return _PoolIndex; }
            set { this.RaiseAndSetIfChanged(ref _PoolIndex, value);
                if (value != -1)
                    TagIndex = -1;
            }
        }

        private int _TagIndex = -1;
        public int TagIndex
        {
            get { return _TagIndex; }
            set { this.RaiseAndSetIfChanged(ref _TagIndex, value);
                if (value != -1)
                    PoolIndex = -1;
            }
        }

        private SkillResult _TagSelected;
        public SkillResult TagSelected
        {
            get { return _TagSelected; }
            set { this.RaiseAndSetIfChanged(ref _TagSelected, value); }
        }

        private DocumentResult _SelectedDocument;
        public DocumentResult SelectedDocument
        {
            get { return _SelectedDocument; }
            set { this.RaiseAndSetIfChanged(ref _SelectedDocument, value); }
        }

        private bool _IsWorking = true;
        public bool IsWorking
        {
            get { return _IsWorking; }
            set { this.RaiseAndSetIfChanged(ref _IsWorking, value); }
        }
    }
}