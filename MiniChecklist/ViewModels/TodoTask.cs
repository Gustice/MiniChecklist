using System;
using System.Collections;
using System.Collections.ObjectModel;
using Prism.Commands;
using Prism.Mvvm;

namespace MiniChecklist.ViewModels
{
    public class TodoTask : BindableBase, IList
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
        public DelegateCommandBase ManipulateTaskCommand { get; }

        private string _task;

        private IList _parent;

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
            ManipulateTaskCommand = new DelegateCommand<string>(OnManipulateTask);
        }

        public void SetParent(IList parent)
        {
            _parent = parent;
        }

        private void OnManipulateTask(string command)
        {
            switch (command)
            {
                case "Sibling":
                    var sibling = new TodoTask("", "");
                    sibling.SetParent(_parent);
                    _parent.Insert(_parent.IndexOf(this)+1, sibling);
                    break;

                case "Child":
                    var child = new TodoTask("", "");
                    child.SetParent(this);

                    this.Insert(0, child);
                    break;

                case "Remove":
                    _parent.Remove(this);
                    break;

                case "Up":

                    break;

                case "Down":

                    break;

                default:
                    break;
            }
        }

        private void OnCheckTask()
        {
            Done = !Done;
            Hide = Done && HideFinished;
        }

        public int Count => SubList.Count;

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        public object this[int index]
        {
            get => _index;
            set
            {
                _index = (int?)value; }
        }

        private int? _index;

        public int Add(TodoTask subtask)
        {
            SubList.Add(subtask);
            return 0;
        }

        public void CopyTo(Array array, int index) => SubList.CopyTo((TodoTask[])array, index);

        public IEnumerator GetEnumerator() => SubList.GetEnumerator();

        public int Add(object value) => Add((TodoTask)value);
        public void Clear() => SubList.Clear();
        public bool Contains(object value) => SubList.Contains((TodoTask)value);
        public int IndexOf(object value) => SubList.IndexOf((TodoTask)value);
        public void Insert(int index, object value) => SubList.Insert(index, (TodoTask)value);
        public void Remove(object value) => SubList.Remove((TodoTask)value);
        public void RemoveAt(int index) => SubList.RemoveAt(index);
    }
}
