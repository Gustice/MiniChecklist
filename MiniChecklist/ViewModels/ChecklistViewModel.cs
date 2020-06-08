using MiniChecklist.ViewModels;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using Prism.Events;
using System.Linq;
using MiniChecklist.Events;
using System.Collections.Generic;

namespace MiniChecklist.ViewModels
{
    public class ChecklistViewModel : BindableBase
    {
        public ObservableCollection<TodoTask> TodoList { get; } = new ObservableCollection<TodoTask>();

        private bool _hide;
        public bool HideFinished
        {
            get => _hide;
            set
            {
                SetProperty(ref _hide, value);
                foreach (var item in TodoList)
                    item.Hide = item.Done && _hide;
            }
        }
        

        public DelegateCommandBase FinishCommand { get; }


        /// <summary> For Preview only </summary>
        public ChecklistViewModel()
        {
            TodoList.Add(new TodoTask("CheckMe"));
            TodoList.Add(new TodoTask("DoMe"));

            var task = new TodoTask("FinishMe");
            task.Add(new TodoTask("Me also"));
            task.Add(new TodoTask("And Me"));
            task.Add(new TodoTask("And not to forget me"));
            TodoList.Add(task);
            TodoList.Add(new TodoTask("I'm Done") { Done = true});
        }

        public ChecklistViewModel(IEventAggregator ea)
        {
            FinishCommand = new DelegateCommand(OnFinish);

            ea.GetEvent<SetTasksEvent>().Subscribe(OnSetTasks);
        }

        private void OnSetTasks(List<TodoTask> obj)
        {
            TodoList.Clear();
            TodoList.AddRange(obj);
        }

        private void OnFinish()
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
