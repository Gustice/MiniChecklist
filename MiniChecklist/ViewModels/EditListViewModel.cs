using MiniChecklist.Events;
using Prism.Events;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MiniChecklist.ViewModels
{
    public class EditListViewModel : BindableBase
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
    }
}
