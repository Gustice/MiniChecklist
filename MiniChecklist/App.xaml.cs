using System;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using MiniChecklist.Defines;
using MiniChecklist.Events;
using MiniChecklist.FileReader;
using MiniChecklist.Interfaces;
using MiniChecklist.Repositories;
using MiniChecklist.Views;
using NLog;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;

namespace MiniChecklist
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        // Note 1. Initialize(); will be called first, 5. OnInitialized(); will be called last

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public override void Initialize()
        {
            base.Initialize();
            Application.Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(OnDispatcherUnhandledException);
        }

        /// <inheritdoc /> // 2. This will be called second
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ITaskFileReader, TaskFileReader>();
            containerRegistry.RegisterSingleton<ITaskListRepo, TaskListRepo>();
            containerRegistry.RegisterInstance<ILogger>(_logger);
        }

        /// <inheritdoc /> // 3. This will be called third
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        /// <inheritdoc /> // 4. This will be called fourth
        protected override void InitializeShell(Window shell)
        {
            base.InitializeShell(shell);
            RegisterViews(Container.Resolve<IRegionManager>());

            string[] args = Environment.GetCommandLineArgs();

            if (args.Length >= 2)
            {
                var targetFile = args[1];
                var ea = Container.Resolve<IEventAggregator>();
                ea.GetEvent<LoadFileEvent>().Publish(targetFile);
            }
        }

        private void RegisterViews(IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(ChecklistView));
            regionManager.RegisterViewWithRegion(RegionNames.MainRegion, typeof(EditListView));

            regionManager.RequestNavigate(RegionNames.MainRegion, nameof(ChecklistView));
        }


        protected override void OnInitialized()
        {
            base.OnInitialized();

            _logger.Info($"############ App Version '{Assembly.GetExecutingAssembly().GetName().Version}' Initialized ################");
        }

        void OnDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            //e.Handled = true; // Not handled

            _logger.Fatal(e.Exception, e.Exception.ToString());
            MessageBox.Show("Sorry for the inconvenience. The programm went into an error. See Log for more details", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}