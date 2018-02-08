using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiTrainingSuite.Model;
using WiTrainingSuite.View;

namespace WiTrainingSuite.ViewModel
{
    public class AppBootstrapper : ReactiveObject, IScreen
    {
        public RoutingState Router { get; private set; }

        public SnackbarMessageQueue SnackBarQueue { get; set; }

        readonly ObservableAsPropertyHelper<bool> isAdmin;
        public bool IsAdmin
        {
            get { return isAdmin.Value; }
        }

        public bool admin;

        public AppBootstrapper(IMutableDependencyResolver dependencyResolver = null, RoutingState testRouter = null)
        {
            SnackBarQueue = new SnackbarMessageQueue();

            Router = testRouter ?? new RoutingState();
            dependencyResolver = dependencyResolver ?? Locator.CurrentMutable;

            // Bind VM to V in Splat
            RegisterParts(dependencyResolver);

            // TODO: This is a good place to set up any other app 
            // startup tasks, like setting the logging level
            LogHost.Default.Level = LogLevel.Debug;

            // Test Username for Admin
            try
            {
                using (Wi_training_suite db = new Wi_training_suite(App.ConStringReadOnly))
                {
                    string user = Environment.UserName;
                    var q = from x in db.TADMIN
                            select x;
                    foreach (TADMIN x in q)
                    {
                        AdminList.Add(x);
                    }
                    if (AdminList.Any(x => x.USERNAME == user))
                    {
                        Console.WriteLine("User Is Admin");
                        App._admin= true;
                    }
                    else
                    {
                        Console.WriteLine("User Is Not Admin");
                        App._admin = false;
                    }
                }
            }
            catch(Exception e)
            {
                this.Log().ErrorException("Failed to test username for admin", e);
                App._admin = false;
                App.ConString = App.ConStringReadOnly;
            }

            admin = App._admin;
            this.WhenAnyValue(x => x.admin).ToProperty(this, x => x.IsAdmin, out isAdmin);

            // Register Window Button Commands
            LoadEmployeeList = ReactiveCommand.Create(() => Router.NavigateAndReset.Execute(new EmployeeMasterViewModel(this)));
            LoadStandardWorkList = ReactiveCommand.Create(() => Router.NavigateAndReset.Execute(new StandardWorkMasterViewModel(this)));
            LoadConfigMenu = ReactiveCommand.Create(() => Router.NavigateAndReset.Execute(new ConfigMenuMasterViewModel(this)), this.WhenAnyValue(x => x.IsAdmin, (a) => a == true));
        }

        private void RegisterParts(IMutableDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterConstant(this, typeof(IScreen));

            dependencyResolver.Register(() => new EmployeeMasterView(), typeof(IViewFor<EmployeeMasterViewModel>));
            dependencyResolver.Register(() => new EmployeeDetailNewView(), typeof(IViewFor<EmployeeDetailNewViewModel>));
            dependencyResolver.Register(() => new EmployeeDetailEditView(), typeof(IViewFor<EmployeeDetailEditViewModel>));
            dependencyResolver.Register(() => new EmployeeTrainingEditView(), typeof(IViewFor<EmployeeTrainingEditViewModel>));

            dependencyResolver.Register(() => new StandardWorkMasterView(), typeof(IViewFor<StandardWorkMasterViewModel>));
            dependencyResolver.Register(() => new StandardWorkDetailNewView(), typeof(IViewFor<StandardWorkDetailNewViewModel>));
            dependencyResolver.Register(() => new StandardWorkDetailEditView(), typeof(IViewFor<StandardWorkDetailEditViewModel>));

            dependencyResolver.Register(() => new ConfigMenuMasterView(), typeof(IViewFor<ConfigMenuMasterViewModel>));
        }

        public ReactiveCommand LoadEmployeeList { get; protected set; }
        public ReactiveCommand LoadStandardWorkList { get; protected set; }
        public ReactiveCommand LoadConfigMenu { get; set; }

        private ReactiveList<TADMIN> _AdminList = new ReactiveList<TADMIN>();
        public ReactiveList<TADMIN> AdminList
        {
            get { return _AdminList; }
            set { this.RaiseAndSetIfChanged(ref _AdminList, value); }
        }
    }

    public class fsItem
    {
        public string ColumnHeader { get; set; }
        public string FriendlyName { get; set; }
        public bool SortDesc { get; set; }

        public fsItem()
        {
            ColumnHeader = String.Empty;
            FriendlyName = String.Empty;
            SortDesc = false;
        }
    }

    public class IntMsg
    {
        public string title { get; set; }
        public string msg { get; set; }
        public MessageDialogStyle ms { get; set; }
        public MetroDialogSettings md { get; set; }
    }
}
