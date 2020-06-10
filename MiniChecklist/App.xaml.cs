using System;
using System.Collections.Generic;
using System.Windows;
using MiniChecklist.Defines;
using MiniChecklist.Events;
using MiniChecklist.FileReader;
using MiniChecklist.ViewModels;
using MiniChecklist.Views;
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

        /// <inheritdoc /> // 2. This will be called second
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ITaskFileReader, TaskFileReader>();
            containerRegistry.RegisterSingleton<List<TodoTask>>();
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
    }
}