using System.Collections.ObjectModel;
using Prism.Mvvm;

namespace MiniChecklist.DataModels
{
    public class TodoTask : BindableBase
    {
        static uint _id = 0;

        public string Id { get; set; }

        private bool _done;
        public bool Done
        {
            get => _done;
            set => SetProperty(ref _done, value);
        }

        private bool _hide;
        public bool Hide
        {
            get => _hide;
            set => SetProperty(ref _hide, value);
        }

        public string Task { get; set; }

        public ObservableCollection<TodoTask> SubList { get; } = new ObservableCollection<TodoTask>();

        /// <summary> For Previewer Only</summary>
        public TodoTask()
        {
            Id = $"{++_id}";
        }


        public TodoTask(string task)
        {
            Id = $"{++_id}";
            Task = task;
        }

        public void AddSubtasks(TodoTask subtask)
        {
            SubList.Add(subtask);
        }
 
    }
}
