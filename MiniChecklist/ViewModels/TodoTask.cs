using System;
using System.Collections;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;

namespace MiniChecklist.ViewModels
{
    public class TodoTask : BindableBase, ICollection
    {
        static uint _id = 0;

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

        public DelegateCommandBase CheckTaskCommand { get; }

        public string Task { get; set; }

        public ObservableCollection<TodoTask> SubList { get; } = new ObservableCollection<TodoTask>();

        /// <summary> For Previewer Only</summary>
        public TodoTask()
        {

            Task = "Task";
            SubList.Add(new TodoTask("SubTask1"));
            SubList.Add(new TodoTask("SubTask2"));
        }

        public TodoTask(string task)
        {
            Task = task;

            CheckTaskCommand = new DelegateCommand(OnCheckTask);
        }

        private void OnCheckTask()
        {
            Done = !Done;
            //Hide = Done && HideFinished; // @todo
        }

        public int Count => SubList.Count;

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public void Add(TodoTask subtask)
        {
            SubList.Add(subtask);
        }

        public void CopyTo(Array array, int index) => SubList.CopyTo((TodoTask[])array, index);

        public IEnumerator GetEnumerator() => SubList.GetEnumerator();
    }
}
