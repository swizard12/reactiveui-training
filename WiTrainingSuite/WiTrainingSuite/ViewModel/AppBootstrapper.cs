using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using WiTrainingSuite.Model;
using WiTrainingSuite.View;
using System.Data;

namespace WiTrainingSuite.ViewModel
{
    public class AppBootstrapper : ReactiveObject, IScreen
    {
        public RoutingState Router { get; private set; }

        public SnackbarMessageQueue SnackBarQueue { get; set; }

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

            // Bind Window Commands
            DocumentMaster = ReactiveCommand.Create(() =>
            {
                Router.NavigateAndReset.Execute(new DocumentMasterViewModel(this));
            });
            CourseMaster = ReactiveCommand.Create(() =>
            {
                SnackBarQueue.Enqueue("Coming Soon");
            });
            EmployeeMaster = ReactiveCommand.Create(() =>
            {
                Router.NavigateAndReset.Execute(new EmployeeMasterViewModel(this));
            });
            ConfigMaster = ReactiveCommand.Create(() =>
            {
                Router.NavigateAndReset.Execute(new ConfigMasterViewModel(this));
            });          
        }
        
        private void RegisterParts(IMutableDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterConstant(this, typeof(IScreen));

            dependencyResolver.Register(() => new DocumentMasterView(), typeof(IViewFor<DocumentMasterViewModel>));
            dependencyResolver.Register(() => new DocumentDetailView(), typeof(IViewFor<DocumentDetailViewModel>));
            dependencyResolver.Register(() => new DocumentTrainingView(), typeof(IViewFor<DocumentTrainingViewModel>));

            dependencyResolver.Register(() => new EmployeeMasterView(), typeof(IViewFor<EmployeeMasterViewModel>));
            dependencyResolver.Register(() => new EmployeeDetailView(), typeof(IViewFor<EmployeeDetailViewModel>));
            dependencyResolver.Register(() => new EmployeeTrainingView(), typeof(IViewFor<EmployeeTrainingViewModel>));

            dependencyResolver.Register(() => new ConfigMasterView(), typeof(IViewFor<ConfigMasterViewModel>));
        }

        public ReactiveCommand EmployeeMaster { get; protected set; }
        public ReactiveCommand DocumentMaster { get; protected set; }
        public ReactiveCommand CourseMaster { get; protected set; }

        public ReactiveCommand ConfigMaster { get; set; }
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
}
