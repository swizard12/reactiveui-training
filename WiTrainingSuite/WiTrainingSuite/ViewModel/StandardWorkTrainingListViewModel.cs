using MaterialDesignThemes.Wpf;
using ReactiveUI;
using Saxon.Api;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WiTrainingSuite.Model;
using WiTrainingSuite.View;

namespace WiTrainingSuite.ViewModel
{
    public class StandardWorkTrainingListViewModel : ReactiveObject, IRoutableViewModel
    {
        #region IRoutableViewModel
        public string UrlPathSegment
        {
            get { return "standardworktraininglist"; }
        }
        public IScreen HostScreen { get; protected set; }
        #endregion

        public SnackbarMessageQueue SnackBarQueue { get; set; }

        public StandardWorkTrainingListViewModel(IScreen screen, FnTSTANDARDWORK_LISTResult standardwork)
        {
            HostScreen = screen;

            StandardWork = standardwork;

            SnackBarQueue = new SnackbarMessageQueue();

            PrepLists();

            ////// Reactive Command Definitions

            // Opening Filter Panels

            FLevelCommand = ReactiveCommand.Create(() =>
            {
                bLevel = true;
                IsTopOpen = true;
            });

            FVarCommand = ReactiveCommand.Create(() =>
            {
                bVar = true;
                IsTopOpen = true;
            });

            FShiftCommand = ReactiveCommand.Create(() =>
            {
                bShift = true;
                IsTopOpen = true;
            });

            FClearCommand = ReactiveCommand.Create(() =>
            {
                bWorking = true;
                foreach (CBListModel x in LevelList) { x.ISTICKED = false; }
                foreach (CBListModel x in VarList) { x.ISTICKED = false; }
                foreach (CBListModel x in ShiftList) { x.ISTICKED = false; }
                IsTopOpen = false;
                ApplyFilters();
            });

            // Level Filter Buttons

            FLevelApplyCommand = ReactiveCommand.Create(() =>
            {
                bWorking = true;
                IsTopOpen = false;
                ApplyFilters();
            });

            FLevelClearCommand = ReactiveCommand.Create(() =>
            {
                bWorking = true;
                foreach (CBListModel x in LevelList)
                {
                    x.ISTICKED = false;
                }
                IsTopOpen = false;
                ApplyFilters();
            });

            // Variant Filter Buttons

            FVarApplyCommand = ReactiveCommand.Create(() =>
            {
                bWorking = true;
                IsTopOpen = false;
                ApplyFilters();
            });

            FVarClearCommand = ReactiveCommand.Create(() =>
            {
                bWorking = true;
                foreach (CBListModel x in VarList)
                {
                    x.ISTICKED = false;
                }
                IsTopOpen = false;
                ApplyFilters();
            });

            // Shift Filter Buttons

            FShiftApplyCommand = ReactiveCommand.Create(() =>
            {
                IsTopOpen = false;
                bWorking = true;
                ApplyFilters();
            });

            FShiftClearCommand = ReactiveCommand.Create(() =>
            {
                foreach (CBListModel x in ShiftList)
                {
                    x.ISTICKED = false;
                }
                IsTopOpen = false;
                ApplyFilters();
            });

            PrintCommand = ReactiveCommand.Create(() =>
            {
                XmlSerializer xml = new XmlSerializer(typeof(ReactiveList<FnTEMPTRAINING_SELECTSResult>));

                var inE = App.CreateTmpFile();
                var outE = Path.ChangeExtension(App.CreateTmpFile(),".html");

                using (FileStream fs = new FileStream(inE, FileMode.Create))
                {
                    xml.Serialize(fs, TrainingList);
                }

                // Embedded xslt
                using (var xsltE = Assembly.GetExecutingAssembly().GetManifestResourceStream("WiTrainingSuite.Model.StandardWorkTrainingReport.xslt"))
                using (var rdr = new StreamReader(xsltE))
                {
                    // Compile stylesheet
                    var processor = new Processor();
                    var compiler = processor.NewXsltCompiler();
                    var executable = compiler.Compile(rdr);


                    // Do transformation to a destination
                    var destination = new DomDestination();
                    using (var inputStream = new FileInfo(inE).OpenRead())
                    {
                        var transformer = executable.Load();
                        transformer.SetInputStream(inputStream, new Uri(new FileInfo(inE).DirectoryName));
                        transformer.Run(destination);
                    }

                    // Save result to a file (or whatever else you wanna do)
                    destination.XmlDocument.Save(outE);
                }

                Process.Start("excel.exe", outE);
            }, this.WhenAny(
                x => x.bWorking,
                (b) => !(b.Value)));
        }

        public async void PrepLists()
        {
            await Task.Run(() =>
            {
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    // Import Training Matrix for this SW
                    TrainingList = new ReactiveList<FnTEMPTRAINING_SELECTSResult>(db.FnTEMPTRAINING_SELECTS(StandardWork.SW_ID).OrderBy("EMP_LNAME"));

                    // Hold Copy of Original
                    OriginalTrainingList = TrainingList;

                    // Level Selection List
                    LevelList = new ReactiveList<CBListModel>()
                    {
                        new CBListModel() { ID = 0, LABEL = "Need Full Training", ISTICKED = false },
                        new CBListModel() { ID = 2, LABEL = "Need Update Training", ISTICKED = false },
                        new CBListModel() { ID = 1, LABEL = "Fully Trained", ISTICKED = false }
                    };

                    // Variant Selection List
                    VarList = new ReactiveList<CBListModel>();
                    var vars = from v in db.TROLEVAR
                               where (from k in db.TVARSKILL
                                      where (from s in db.TSKILLSW
                                             where s.SW_ID == StandardWork.SW_ID
                                             select s.SKILL_ID).Contains(k.SKILL_ID)
                                      select k.VAR_ID).Contains(v.VAR_ID)
                               select v;
                    foreach (TROLEVAR v in vars)
                    {
                        App.OnUI(() => { VarList.Add(new CBListModel() { ID = v.VAR_ID, LABEL = v.VAR_NAME, ISTICKED = true }); });
                    }

                    // Shift Selection List
                    ShiftList = new ReactiveList<CBListModel>();
                    var shifts = from s in db.TSHIFT
                                 where (from x in db.TEMPSHIFT
                                        where (from y in db.TEMPVAR
                                               where (from z in vars
                                                      select z.VAR_ID).Contains(y.VAR_ID)
                                               select y.EMP_ID).Contains(x.EMP_ID)
                                        select x.SHIFT_ID).Contains(s.SHIFT_ID)
                                 select s;
                    foreach (TSHIFT s in shifts)
                    {
                        App.OnUI(() => { ShiftList.Add(new CBListModel() { ID = s.SHIFT_ID, LABEL = s.SHIFT_NAME, ISTICKED = true }); });
                    }
                }
                // Filter to All Levels, Employees of Valid Variant and All Shifts containing an employee 
                ApplyFilters();
            });
        }

        public ReactiveCommand FLevelCommand { get; set;}
        public ReactiveCommand FLevelApplyCommand { get; set; }
        public ReactiveCommand FLevelClearCommand { get; set; }

        public ReactiveCommand FVarCommand { get; set; }
        public ReactiveCommand FVarApplyCommand { get; set; }
        public ReactiveCommand FVarClearCommand { get; set; }

        public ReactiveCommand FShiftCommand { get; set; }
        public ReactiveCommand FShiftApplyCommand { get; set; }
        public ReactiveCommand FShiftClearCommand { get; set; }

        public ReactiveCommand FClearCommand { get; set; }

        public ReactiveCommand BackCommand { get; set; }
        public ReactiveCommand PrintCommand { get; set; }

        public ReactiveList<FnTEMPTRAINING_SELECTSResult> ApplyVariantFilter()
        {
            using (Wi_training_suite db = new Wi_training_suite(App.ConString))
            {
                return new ReactiveList<FnTEMPTRAINING_SELECTSResult>(from e in OriginalTrainingList
                                                                      where (from x in db.TEMPVAR
                                                                             where (from v in VarList
                                                                                    where v.ISTICKED == true
                                                                                    select v.ID).Contains(x.VAR_ID)
                                                                             select x.EMP_ID).Contains(e.EMP_ID.Value)
                                                                      select e);

            }
        }

        public ReactiveList<FnTEMPTRAINING_SELECTSResult> ApplyShiftFilter()
        {
            using (Wi_training_suite db = new Wi_training_suite(App.ConString))
            {
                return new ReactiveList<FnTEMPTRAINING_SELECTSResult>(from e in OriginalTrainingList
                                                                      where (from x in db.TEMPSHIFT
                                                                             where (from s in ShiftList
                                                                                    where s.ISTICKED == true
                                                                                    select s.ID).Contains(x.SHIFT_ID)
                                                                             select x.EMP_ID).Contains(e.EMP_ID.Value)
                                                                      select e);
            }
        }

        public async void ApplyFilters()
        {
            await Task.Run(() =>
            {
            ReactiveList<FnTEMPTRAINING_SELECTSResult> tList = new ReactiveList<FnTEMPTRAINING_SELECTSResult>();
            ReactiveList<FnTEMPTRAINING_SELECTSResult> lS = null;
            ReactiveList<FnTEMPTRAINING_SELECTSResult> lV = null;
            bool bS = false;
            bool bV = false;

            // Variant Filtering
            if (VarList.Count(x => x.ISTICKED == true) > 0)
            {
                lV = ApplyVariantFilter();
            }
            else
                bV = true;

            // Shift Filtering
            if (ShiftList.Count(x => x.ISTICKED == true) > 0)
            {
                lS = ApplyShiftFilter();
            }
            else
                bS = true;

            if (bS && bV) // Nothing in either, use original Matrix
                tList = OriginalTrainingList;
            if (bS && !bV) // Nothing in Shift, some in Var : Use Var
                tList = lV;
            if (!bS && bV) // Nothing in Var, some in Shift : Use Shift
                tList = lS;
            if (!bS && !bV) // Something in Both : Union for Distinct Rows
            {
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    tList = new ReactiveList<FnTEMPTRAINING_SELECTSResult>(from e in lS.Union(lV)
                                                                           where (from v in lV
                                                                                  select v.EMP_ID).Contains(e.EMP_ID) &&
                                                                                  (from s in lS
                                                                                   select s.EMP_ID).Contains(e.EMP_ID)
                                                                           select e);
                }
            }
            // Pass tList to Level Filter and Present to UI
            if (LevelList.Count(x => x.ISTICKED == true) > 0)
            {
                TrainingList = new ReactiveList<FnTEMPTRAINING_SELECTSResult>(from s in tList
                                                                              where (from l in LevelList
                                                                                     where l.ISTICKED == true
                                                                                     select l.ID).Contains(s.SW_LEVEL.Value)
                                                                              select s);
            }
            else
            {
                TrainingList = tList;
            }
                // Close Drawer
                App.OnUI(() => bWorking = false);
            });
        }

        private ReactiveList<FnTEMPTRAINING_SELECTSResult> _TrainingList;
        public ReactiveList<FnTEMPTRAINING_SELECTSResult> TrainingList
        {
            get { return _TrainingList; }
            set { this.RaiseAndSetIfChanged(ref _TrainingList, value); }
        }

        private ReactiveList<FnTEMPTRAINING_SELECTSResult> _OriginalTrainingList;
        public ReactiveList<FnTEMPTRAINING_SELECTSResult> OriginalTrainingList
        {
            get { return _OriginalTrainingList; }
            set { this.RaiseAndSetIfChanged(ref _OriginalTrainingList, value); }
        }

        private FnTEMPTRAINING_SELECTSResult _SelectedTraining;
        public FnTEMPTRAINING_SELECTSResult SelectedTraining
        {
            get { return _SelectedTraining; }
            set { this.RaiseAndSetIfChanged(ref _SelectedTraining, value); }
        }

        private int _TrainingIndex = -1;
        public int TrainingIndex
        {
            get { return _TrainingIndex; }
            set { this.RaiseAndSetIfChanged(ref _TrainingIndex, value); }
        }

        private FnTSTANDARDWORK_LISTResult _StandardWork;
        public FnTSTANDARDWORK_LISTResult StandardWork
        {
            get { return _StandardWork; }
            set { this.RaiseAndSetIfChanged(ref _StandardWork, value); }
        }

        private ReactiveList<CBListModel> _LevelList;
        public ReactiveList<CBListModel> LevelList
        {
            get { return _LevelList; }
            set { this.RaiseAndSetIfChanged(ref _LevelList, value); }
        }

        private ReactiveList<CBListModel> _VarList;
        public ReactiveList<CBListModel> VarList
        {
            get { return _VarList; }
            set { this.RaiseAndSetIfChanged(ref _VarList, value); }
        }

        private ReactiveList<CBListModel> _ShiftList;
        public ReactiveList<CBListModel> ShiftList
        {
            get { return _ShiftList; }
            set { this.RaiseAndSetIfChanged(ref _ShiftList, value); }
        }

        private bool _bLevel;
        public bool bLevel
        {
            get { return _bLevel; }
            set
            {
                this.RaiseAndSetIfChanged(ref _bLevel, value);
                if (bLevel)
                {
                    bVar = false;
                    bShift = false;
                }
            }
        }

        private bool _bVar;
        public bool bVar
        {
            get { return _bVar; }
            set
            {
                this.RaiseAndSetIfChanged(ref _bVar, value);
                if (bVar)
                {
                    bLevel = false;
                    bShift = false;
                }
            }
        }

        private bool _bShift;
        public bool bShift
        {
            get { return _bShift; }
            set
            {
                this.RaiseAndSetIfChanged(ref _bShift, value);
                if (bShift)
                {
                    bLevel = false;
                    bVar = false;
                }
            }
        }

        private bool _IsTopOpen = false;
        public bool IsTopOpen
        {
            get { return _IsTopOpen; }
            set { this.RaiseAndSetIfChanged(ref _IsTopOpen, value); }
        }

        private bool _bWorking = true;
        public bool bWorking
        {
            get { return _bWorking; }
            set { this.RaiseAndSetIfChanged(ref _bWorking, value); }
        }
    }
}
