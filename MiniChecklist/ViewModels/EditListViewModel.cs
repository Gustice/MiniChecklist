using MiniChecklist.Events;
using MiniChecklist.Interfaces;
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

        public EditListViewModel(IEventAggregator ea, ITaskListRepo taskListRepo)
        {
            ea.GetEvent<SetTasksEvent>().Subscribe(OnSetTasks);
            TodoList = taskListRepo.GetTaskList();
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

        void ClearEmptyRecusively(ICollection<TodoTask> list)
        {
            List<TodoTask> temp = new List<TodoTask>();
            temp.AddRange(list);
            foreach (var item in temp)
            {
                if (string.IsNullOrEmpty( item.Task ) && item.SubList.Count == 0)
                {
                    list.Remove(item);
                }
                ClearEmptyRecusively(item.SubList);
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
            ClearEmptyRecusively(TodoList);
            //_taskList.Clear();
            //_taskList.AddRange(TodoList);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    }
}
