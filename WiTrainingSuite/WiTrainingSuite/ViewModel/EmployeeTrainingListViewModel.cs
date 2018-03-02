using MaterialDesignThemes.Wpf;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using WiTrainingSuite.Model;

using System.IO;
using System.Net;
using System.Xml.Serialization;
using System.Xml;
using System.Reflection;
using SelectPdf;
using Saxon;
using Saxon.Api;
using System.Diagnostics;

namespace WiTrainingSuite.ViewModel
{
    public class EmployeeTrainingListViewModel : ReactiveObject, IRoutableViewModel
    {
        #region IRoutableViewModel
        public string UrlPathSegment
        {
            get { return "employeetrainingedit"; }
        }
        public IScreen HostScreen { get; protected set; }
        #endregion

        public SnackbarMessageQueue SnackBarQueue { get; set; }

        public EmployeeTrainingListViewModel(IScreen screen, FnTEMPLOYEE_LISTResult employee, bool fromInput = false)
        {
            HostScreen = screen;

            Employee = employee;

            SnackBarQueue = new SnackbarMessageQueue();

            PrepLists(fromInput);

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

            FSkillCommand = ReactiveCommand.Create(() =>
            {
                bSkill = true;
                IsTopOpen = true;
            });

            FClearCommand = ReactiveCommand.Create(() =>
            {
                bWorking = true;
                foreach (CBListModel x in LevelList) { x.ISTICKED = false; }
                foreach (CBListModel x in VarList) { x.ISTICKED = false; }
                foreach (CBListModel x in SkillList) { x.ISTICKED = false; }
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

            // Skill Filter Buttons

            FSkillDefaultCommand = ReactiveCommand.Create(() =>
            {
                bWorking = true;
                foreach (CBListModel x in SkillList)
                {
                    if (TrainingSkills.Select(s => s.SKILL_ID).Contains(x.ID))
                    {
                        x.ISTICKED = true;
                    }
                    else
                    {
                        x.ISTICKED = false;
                    }
                }
                IsTopOpen = false;
                ApplyFilters();
            });

            FSkillApplyCommand = ReactiveCommand.Create(() =>
            {
                IsTopOpen = false;
                bWorking = true;
                ApplyFilters();
            });

            FSkillClearCommand = ReactiveCommand.Create(() =>
            {
                foreach (CBListModel x in SkillList)
                {
                    x.ISTICKED = false;
                }
                IsTopOpen = false;
                ApplyFilters();
            });

            // Variant Filter Buttons

            FVarDefaultCommand = ReactiveCommand.Create(() =>
            {
                bWorking = true;
                foreach (CBListModel x in VarList)
                {
                    if (x.ID == Employee.VAR_ID)
                    {
                        x.ISTICKED = true;
                    }
                    else
                    {
                        x.ISTICKED = false;
                    }
                }
                IsTopOpen = false;
                ApplyFilters();
            });

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

            // Navigation Bar Commands

            BackCommand = ReactiveCommand.Create(() =>
            {
                HostScreen.Router.NavigateBack.Execute();
            });

            PrintCommand = ReactiveCommand.Create(() =>
            {
                XmlSerializer xml = new XmlSerializer(typeof(ReactiveList<FnTEMPTRAINING_SELECTEResult>));

                var inE = App.CreateTmpFile();
                var outE = Path.ChangeExtension(App.CreateTmpFile(), ".html");

                using (FileStream fs = new FileStream(inE, FileMode.Create))
                {
                    xml.Serialize(fs, TrainingList);
                }

                // Embedded xslt
                using (var xsltE = Assembly.GetExecutingAssembly().GetManifestResourceStream("WiTrainingSuite.Model.EmployeeVariantTrainingReport.xslt"))
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
                (b) => !b.Value));
        }

        public async void PrepLists(bool fI)
        {
            await Task.Run(() =>
            {
                ////// List Setup
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    // Training Matrix Import

                    TrainingList = new ReactiveList<FnTEMPTRAINING_SELECTEResult>(db.FnTEMPTRAINING_SELECTE(Employee.EMP_ID).OrderBy("SW_CODE"));

                    // Hold a Copy of Full Matrix

                    OriginalTrainingList = TrainingList;

                    // Employee Skill Training Programme

                    TrainingSkills = new ReactiveList<TEMPSKILL>(db.TEMPSKILL.Where(x => x.EMP_ID == Employee.EMP_ID));

                    // Level Selection List

                    LevelList = new ReactiveList<CBListModel>()
                    {
                        new CBListModel() { ID = 0, LABEL = "Need Full Training", ISTICKED = false },
                        new CBListModel() { ID = 2, LABEL = "Need Update Training", ISTICKED = false },
                        new CBListModel() { ID = 1, LABEL = "Fully Trained", ISTICKED = false }
                    };

                    // Variant Selection List

                    VarList = new ReactiveList<CBListModel>();
                    var vars = db.TROLEVAR.AsEnumerable();
                    foreach (TROLEVAR v in vars)
                    {
                        if (v.VAR_ID == Employee.VAR_ID)
                        {
                            App.OnUI(() => { VarList.Add(new CBListModel() { ID = v.VAR_ID, LABEL = v.VAR_NAME, ISTICKED = fI ? false : true }); });
                        }
                        else
                        {
                            App.OnUI(() => { VarList.Add(new CBListModel() { ID = v.VAR_ID, LABEL = v.VAR_NAME, ISTICKED = false }); });
                        }
                    }

                    // Skill Selection List

                    SkillList = new ReactiveList<CBListModel>();
                    var skills = db.TSKILL.AsEnumerable();
                    foreach (TSKILL s in skills)
                    {
                        if (TrainingSkills.Select(x => x.SKILL_ID).Contains(s.SKILL_ID))
                        {
                            App.OnUI(() => { SkillList.Add(new CBListModel() { ID = s.SKILL_ID, LABEL = s.SKILL_NAME, ISTICKED = fI ? false : true }); });
                        }
                        else
                        {
                            App.OnUI(() => { SkillList.Add(new CBListModel() { ID = s.SKILL_ID, LABEL = s.SKILL_NAME, ISTICKED = false }); });
                        }
                    }

                    // Filter to All Levels, Default Role & any Skill Training
                    ApplyFilters();
                }
            });
        }

        public ReactiveList<FnTEMPTRAINING_SELECTEResult> ApplySkillFilter()
        {
            using (Wi_training_suite db = new Wi_training_suite(App.ConString))
            {

                    return new ReactiveList<FnTEMPTRAINING_SELECTEResult>(from s in OriginalTrainingList // Training List
                                                                                  where (from k in db.TSKILLSW // Skill to SW
                                                                                         where (from l in SkillList // Skill Tick List
                                                                                                where l.ISTICKED == true
                                                                                                select l.ID).Contains(k.SKILL_ID)
                                                                                         select k.SW_ID).Contains(s.SW_ID.Value)
                                                                                  select s);
                    

            }
        }

        public ReactiveList<FnTEMPTRAINING_SELECTEResult> ApplyVariantFilter()
        {
            using (Wi_training_suite db = new Wi_training_suite(App.ConString))
            {

                    return new ReactiveList<FnTEMPTRAINING_SELECTEResult>(from s in OriginalTrainingList // Training List
                                                                          where (from k in db.TSKILLSW // Skill to SW
                                                                                 where (from v in db.TVARSKILL // Var to Skill
                                                                                        where (from l in VarList // Variant Tick List
                                                                                               where l.ISTICKED == true
                                                                                               select l.ID).Contains(v.VAR_ID)
                                                                                        select v.SKILL_ID).Contains(k.SKILL_ID)
                                                                                 select k.SW_ID).Contains(s.SW_ID.Value)
                                                                          select s);

            }
        }

        public async void ApplyFilters()
        {
            await Task.Run(() =>
            {
                ReactiveList<FnTEMPTRAINING_SELECTEResult> tList = new ReactiveList<FnTEMPTRAINING_SELECTEResult>();
                ReactiveList<FnTEMPTRAINING_SELECTEResult> lS = null;
                ReactiveList<FnTEMPTRAINING_SELECTEResult> lV = null;
                bool bS = false;
                bool bV = false;

                // Skill Filtering
                if (SkillList.Count(x => x.ISTICKED == true) > 0)
                {
                    lS = ApplySkillFilter();
                }
                else
                    bS = true;
                if (VarList.Count(x => x.ISTICKED == true) > 0)
                {
                    lV = ApplyVariantFilter();
                }
                else
                    bV = true;

                if (bS && bV) // Nothing in either, use original Matrix
                    tList = OriginalTrainingList;
                if (bS && !bV) // Nothing in Skill, some in Var : Use Var
                    tList = lV;
                if (!bS && bV) // Nothing in Var, some in Skill, : Use Skill
                    tList = lS;
                if (!bS && !bV) // Something in Both : Union for Distinct Rows
                    tList = new ReactiveList<FnTEMPTRAINING_SELECTEResult>(lS.Union(lV));

                // Pass tList to Level Filter and Present to UI
                if (LevelList.Count(x => x.ISTICKED == true) > 0)
                {
                    TrainingList = new ReactiveList<FnTEMPTRAINING_SELECTEResult>(from s in tList
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

        public ReactiveCommand FLevelCommand { get; set; }
        public ReactiveCommand FVarCommand { get; set; }
        public ReactiveCommand FSkillCommand { get; set; }
        public ReactiveCommand FClearCommand { get; set; }

        public ReactiveCommand FLevelApplyCommand { get; set; }
        public ReactiveCommand FLevelClearCommand { get; set; }

        public ReactiveCommand FSkillDefaultCommand { get; set; }
        public ReactiveCommand FSkillApplyCommand { get; set; }
        public ReactiveCommand FSkillClearCommand { get; set; }

        public ReactiveCommand FVarDefaultCommand { get; set; }
        public ReactiveCommand FVarApplyCommand { get; set; }
        public ReactiveCommand FVarClearCommand { get; set; }

        public ReactiveCommand BackCommand { get; set; }
        public ReactiveCommand PrintCommand { get; set; }

        private ReactiveList<FnTEMPTRAINING_SELECTEResult> _TrainingList;
        public ReactiveList<FnTEMPTRAINING_SELECTEResult> TrainingList
        {
            get { return _TrainingList; }
            set { this.RaiseAndSetIfChanged(ref _TrainingList, value); }
        }

        private ReactiveList<FnTEMPTRAINING_SELECTEResult> _OriginalTrainingList;
        public ReactiveList<FnTEMPTRAINING_SELECTEResult> OriginalTrainingList
        {
            get { return _OriginalTrainingList; }
            set { this.RaiseAndSetIfChanged(ref _OriginalTrainingList, value); }
        }

        private FnTEMPTRAINING_SELECTEResult _SelectedTraining = new FnTEMPTRAINING_SELECTEResult();
        public FnTEMPTRAINING_SELECTEResult SelectedTraining
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

        private FnTEMPLOYEE_LISTResult _Employee;
        public FnTEMPLOYEE_LISTResult Employee
        {
            get { return _Employee; }
            set { this.RaiseAndSetIfChanged(ref _Employee, value); }
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

        private ReactiveList<CBListModel> _SkillList;
        public ReactiveList<CBListModel> SkillList
        {
            get { return _SkillList; }
            set { this.RaiseAndSetIfChanged(ref _SkillList, value); }
        }

        private ReactiveList<TEMPSKILL> _TrainingSkills;
        public ReactiveList<TEMPSKILL> TrainingSkills
        {
            get { return _TrainingSkills; }
            set { this.RaiseAndSetIfChanged(ref _TrainingSkills, value); }
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
                    bSkill = false;
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
                if(bVar)
                {
                    bLevel = false;
                    bSkill = false;
                }
            }
        }

        private bool _bSkill;
        public bool bSkill
        {
            get { return _bSkill; }
            set
            {
                this.RaiseAndSetIfChanged(ref _bSkill, value);
                if(bSkill)
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
