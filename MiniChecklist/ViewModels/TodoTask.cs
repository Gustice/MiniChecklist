using System;
using System.Collections;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;

namespace MiniChecklist.ViewModels
{
    public class TodoTask : BindableBase, ICollection
    {
        private bool _done;
        public bool Done
        {
            get => _done || _implicitDone;
            set
            {
                SetProperty(ref _done, value);
                // todo parent undo
                foreach (var item in SubList)
                {
                    item.ImplicitDone = _done;
                }
            }
        }

        private bool _implicitDone;

        public bool ImplicitDone
        {
            get => _implicitDone;
            set
            {
                _implicitDone = value;

                RaisePropertyChanged(nameof(Done));

                foreach (var item in SubList)
                {
                    item.ImplicitDone = value;
                }
            }
        }

        private bool _hide;
        public bool Hide
        {
            get => _hide;
            set => SetProperty(ref _hide, value);
        }

        private bool _hideFinished;
        public bool HideFinished
        {
            get => _hideFinished;
            set
            {
                _hideFinished = value;
                Hide = Done && _hideFinished;

                foreach (var item in SubList)
                    item.HideFinished = value;
            }
        }

        public DelegateCommandBase CheckTaskCommand { get; }

        private string _task;
        public string Task
        {
            get => _task;
            set => SetProperty(ref _task, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public ObservableCollection<TodoTask> SubList { get; } = new ObservableCollection<TodoTask>();

        /// <summary> For Previewer Only</summary>
        public TodoTask()
        {

            Task = "First level Task";
            Description = "Description for first level Task";
            SubList.Add(new TodoTask("Second level Task"));
            var subTask = new TodoTask("Second level Task");
            SubList.Add(subTask);
            subTask.Done = true;
        }

        public TodoTask(string task) : this(task, "")
        {

        }

        public TodoTask(string task, string description)
        {
            Task = task;
            Description = description;
            CheckTaskCommand = new DelegateCommand(OnCheckTask);
        }

        private void OnCheckTask()
        {
            Done = !Done;
            Hide = Done && HideFinished;
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
