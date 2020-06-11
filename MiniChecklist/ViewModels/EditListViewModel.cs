using MiniChecklist.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MiniChecklist.ViewModels
{
    public class EditListViewModel : BindableBase, INavigationAware
    {
        public ObservableCollection<TodoTask> TodoList { get; } = new ObservableCollection<TodoTask>();

        public DelegateCommand InsertFirstCommand { get; }

        /// <summary> For Preview only </summary>
        public EditListViewModel()
        {

        }

        public EditListViewModel(ITaskListRepo taskListRepo)
        {
            TodoList = taskListRepo.GetTaskList();
            InsertFirstCommand = new DelegateCommand(OnInsertFirst);
        }

        private void OnInsertFirst()
        {
            var item = new TodoTask("New", "");
            item.SetParent(TodoList);
            TodoList.Insert(0, item);
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
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    }
}
