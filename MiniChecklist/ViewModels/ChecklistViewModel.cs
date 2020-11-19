using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using Prism.Events;
using MiniChecklist.Events;
using System.Collections.Generic;
using Prism.Regions;
using MiniChecklist.Interfaces;

namespace MiniChecklist.ViewModels
{
    public class ChecklistViewModel : BindableBase, INavigationAware
    {
        public ObservableCollection<TodoTask> TodoList { get; } = new ObservableCollection<TodoTask>();

        private bool _hideFinished;

        public bool HideFinished
        {
            get => _hideFinished;
            set
            {
                SetProperty(ref _hideFinished, value);
                foreach (var item in TodoList)
                {
                    item.HideFinished = _hideFinished;
                    // item.Hide = item.Done && _hideFinished;
                }
            }
        }
        

        public DelegateCommandBase FinishCommand { get; }


        /// <summary> For Preview only </summary>
        public ChecklistViewModel()
        {
            TodoList.Add(new TodoTask("CheckMe", "", null));
            TodoList.Add(new TodoTask("DoMe", "", null));

            var task = new TodoTask("FinishMe", "", null)
            {
                new TodoTask("Me also", "", null),
                new TodoTask("And Me", "", null),
                new TodoTask("And not to forget me", "", null)
            };
            TodoList.Add(task);
            TodoList.Add(new TodoTask("I'm Done", "", null) { Done = true});
        }

        public ChecklistViewModel(ITaskListRepo taskListRepo)
        {
            FinishCommand = new DelegateCommand(OnFinish);

            TodoList = taskListRepo.GetTaskList();
        }

        private void OnFinish()
        {
            System.Windows.Application.Current.Shutdown();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }
    }
}
