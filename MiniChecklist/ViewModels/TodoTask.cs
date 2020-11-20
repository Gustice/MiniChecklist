using System;
using System.Collections;
using System.Collections.ObjectModel;
using MiniChecklist.Events;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace MiniChecklist.ViewModels
{
    public class TodoTask : BindableBase, IList
    {
        private int? _index;
        private readonly IEventAggregator _eventAggregator;
        private readonly NewInkrementEvent _newInkrementEvent;

        private bool _done;
        private bool _implicitDone;
        private bool _hide;
        private string _task;
        private IList _parent;


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

        public DelegateCommandBase CheckTaskCommand { get; }
        public DelegateCommandBase CheckTaskBoxCommand { get; }
        public DelegateCommandBase ManipulateTaskCommand { get; }

        public ObservableCollection<TodoTask> SubList { get; } = new ObservableCollection<TodoTask>();

        /// <summary> For Previewer Only</summary>
        public TodoTask()
        {
            Task = "First level Task";
            Description = "Description for first level Task";
            SubList.Add(new TodoTask("Second level Task", "", null));
            var subTask = new TodoTask("Another second level Task", "", null);
            SubList.Add(subTask);
            subTask.Done = true;
        }

        public TodoTask(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _newInkrementEvent = eventAggregator.GetEvent<NewInkrementEvent>();
        }

        public TodoTask(string task, string description, IEventAggregator ea) : this(ea)
        {
            Task = task;
            Description = description;
            CheckTaskCommand = new DelegateCommand(OnCheckTask);
            CheckTaskBoxCommand = new DelegateCommand(OnCheckTaskBox);
            ManipulateTaskCommand = new DelegateCommand<string>(OnManipulateTask);
        }

        public void SetParent(IList parent)
        {
            _parent = parent;
        }

        private void OnManipulateTask(string command)
        {
            _newInkrementEvent.Publish();
            switch (command)
            {
                case "Sibling":
                    var sibling = new TodoTask("", "", _eventAggregator);
                    sibling.SetParent(_parent);
                    _parent.Insert(_parent.IndexOf(this) + 1, sibling);
                    break;

                case "Child":
                    var child = new TodoTask("", "", _eventAggregator);
                    child.SetParent(this);

                    this.Insert(0, child);
                    break;

                case "Remove":
                    _parent.Remove(this);
                    break;

                case "Up":
                    {
                        int index = _parent.IndexOf(this);
                        if (index == 0)
                            break;

                        _parent.Remove(this);
                        _parent.Insert(--index, this);
                    }
                    break;

                case "Down":
                    {
                        int index = _parent.IndexOf(this);
                        if (index >= _parent.Count - 1)
                            break;

                        _parent.Remove(this);
                        _parent.Insert(++index, this);
                    }
                    break;

                default:
                    throw new Exception($"Unknown Command '{command}'");
            }
        }


        private void OnCheckTask()
        {
            Done = !Done;
            Hide = Done && HideFinished;
        }
        private void OnCheckTaskBox()
        {
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
                _index = (int?)value;
            }
        }


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

        public override string ToString() => $"{Task} # {Description}";
    }
}
