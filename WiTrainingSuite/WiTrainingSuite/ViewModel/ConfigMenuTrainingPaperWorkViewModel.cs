using MaterialDesignThemes.Wpf;
using ReactiveUI;
using Saxon.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WiTrainingSuite.Model;

namespace WiTrainingSuite.ViewModel
{
    public class ConfigMenuTrainingPaperWorkViewModel : ReactiveObject, IRoutableViewModel
    {
        #region IRoutableViewModel
        public string UrlPathSegment
        {
            get { return "configmenutrainingpaperwork"; }
        }
        public IScreen HostScreen { get; protected set; }
        #endregion

        public SnackbarMessageQueue SnackBarQueue { get; set; }

        public ConfigMenuTrainingPaperWorkViewModel(IScreen screen)
        {
            HostScreen = screen;

            SnackBarQueue = new SnackbarMessageQueue();

            PrepLists();

            SelectAllDeptsCommand = ReactiveCommand.Create(() =>
            {
                foreach(CBListModel x in DeptList)
                {
                    x.ISTICKED = true;
                }
            });

            SelectAllShiftsCommand = ReactiveCommand.Create(() =>
            {
                foreach (CBListModel x in ShiftList)
                {
                    x.ISTICKED = true;
                }
            });

            GeneratePaperworkCommand = ReactiveCommand.Create(async () =>
            {
                await Task.Run(() =>
                {
                    using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                    {
                        var fileList = new ReactiveList<string>();

                        foreach (CBListModel X in DeptList.Where(d => d.ISTICKED == true))
                        {
                            foreach (CBListModel Y in ShiftList.Where(s => s.ISTICKED == true))
                            {
                                var emp = from e in db.TEMPLOYEE
                                          where (from d in db.TEMPDEPT
                                                 where d.DEPT_ID == X.ID
                                                 select d.EMP_ID).Contains(e.EMP_ID) &&
                                                (from s in db.TEMPSHIFT
                                                 where s.SHIFT_ID == Y.ID
                                                 select s.EMP_ID).Contains(e.EMP_ID)
                                          select e.EMP_ID;
                                foreach (int e in emp)
                                {
                                    XmlSerializer xml = new XmlSerializer(typeof(ReactiveList<FnTEMPTRAINING_SELECTEResult>));

                                    var inE = App.CreateTmpFile();
                                    var outE = Path.ChangeExtension(App.CreateTmpFile(), ".html");

                                    using (FileStream fs = new FileStream(inE, FileMode.Create))
                                    {
                                        xml.Serialize(fs, new ReactiveList<FnTEMPTRAINING_SELECTEResult>(db.FnTEMPTRAINING_SELECTE(e)));
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
                                        fileList.Add(outE);
                                    }
                                }
                            }
                        }

                        foreach(string f in fileList)
                        {
                            Process.Start("excel.exe", f);
                        }
                    }
                });
            });
        }

        public async void PrepLists()
        {
            await Task.Run(() =>
            {
                using (Wi_training_suite db = new Wi_training_suite(App.ConString))
                {
                    DeptList = new ReactiveList<CBListModel>();
                    var depts = db.TDEPARTMENT.Where(x => x.DEPT_NAME.Contains("Production"));
                    foreach(TDEPARTMENT x in depts)
                    {
                        App.OnUI(() => { DeptList.Add(new CBListModel() { ID = x.DEPT_ID, LABEL = x.DEPT_NAME }); });
                    }

                    ShiftList = new ReactiveList<CBListModel>();
                    var shifts = db.TSHIFT.Where(x => new string[] { "A", "B", "C", "D" }.Contains(x.SHIFT_NAME));
                    foreach (TSHIFT x in shifts)
                    {
                        App.OnUI(() => { ShiftList.Add(new CBListModel() { ID = x.SHIFT_ID, LABEL = x.SHIFT_NAME }); });
                    }
                }
            });
        }

        public ReactiveCommand SelectAllDeptsCommand { get; set; }
        public ReactiveCommand SelectAllShiftsCommand { get; set; }
        public ReactiveCommand GeneratePaperworkCommand { get; set; }

        private ReactiveList<CBListModel> _DeptList;
        public ReactiveList<CBListModel> DeptList
        {
            get { return _DeptList; }
            set { this.RaiseAndSetIfChanged(ref _DeptList, value); }
        }

        private ReactiveList<CBListModel> _ShiftList;
        public ReactiveList<CBListModel> ShiftList
        {
            get { return _ShiftList; }
            set { this.RaiseAndSetIfChanged(ref _ShiftList, value); }
        }
    }
}
