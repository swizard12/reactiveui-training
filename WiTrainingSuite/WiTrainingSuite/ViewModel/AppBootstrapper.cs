using MahApps.Metro.Controls.Dialogs;
using MaterialDesignThemes.Wpf;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiTrainingSuite.View;

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

            // Bind 
            RegisterParts(dependencyResolver);

            // TODO: This is a good place to set up any other app 
            // startup tasks, like setting the logging level
            LogHost.Default.Level = LogLevel.Debug;
        }

        private void RegisterParts(IMutableDependencyResolver dependencyResolver)
        {
            dependencyResolver.RegisterConstant(this, typeof(IScreen));

            dependencyResolver.Register(() => new EmployeeMasterView(), typeof(IViewFor<EmployeeMasterViewModel>));
            dependencyResolver.Register(() => new EmployeeDetailNewView(), typeof(IViewFor<EmployeeDetailNewViewModel>));
            dependencyResolver.Register(() => new EmployeeDetailEditView(), typeof(IViewFor<EmployeeDetailEditViewModel>));

            dependencyResolver.Register(() => new StandardWorkMasterView(), typeof(IViewFor<StandardWorkMasterViewModel>));
            dependencyResolver.Register(() => new StandardWorkDetailNewView(), typeof(IViewFor<StandardWorkDetailNewViewModel>));
            dependencyResolver.Register(() => new StandardWorkDetailEditView(), typeof(IViewFor<StandardWorkDetailEditViewModel>));

            LoadEmployeeList = ReactiveCommand.Create(() => Router.NavigateAndReset.Execute(new EmployeeMasterViewModel(this)));
            LoadStandardWorkList = ReactiveCommand.Create(() => Router.NavigateAndReset.Execute(new StandardWorkMasterViewModel(this)));
        }

        public ReactiveCommand LoadEmployeeList { get; protected set; }
        public ReactiveCommand LoadStandardWorkList { get; protected set; }
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
