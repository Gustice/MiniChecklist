using MiniChecklist.Events;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MiniChecklist.ViewModels
{
    public class EditListViewModel : BindableBase, INavigationAware
    {
        public ObservableCollection<TodoTask> TodoList { get; } = new ObservableCollection<TodoTask>();

        /// <summary> For Preview only </summary>
        public EditListViewModel()
        {

        }

        public EditListViewModel(IEventAggregator ea)
        {
            ea.GetEvent<SetTasksEvent>().Subscribe(OnSetTasks);
        }

        private void OnSetTasks(List<TodoTask> obj)
        {
            TodoList.Clear();
            TodoList.AddRange(obj);
        }

        void AppendEmptyRecusively(ICollection<TodoTask> list)
        {
            foreach (var item in list)
            {
                AppendEmptyRecusively(item.SubList);
            }
            list.Add(new TodoTask("", ""));
        }

        void SetParenthoodRecursively(TodoTask task)
        {
            foreach (var item in task.SubList)
            {
                item.SetParent(task);
                SetParenthoodRecursively(item);
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            foreach (var item in TodoList)
            {
                item.SetParent(TodoList);
                SetParenthoodRecursively(item);
            }
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            throw new System.NotImplementedException();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    }
}
