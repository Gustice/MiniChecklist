using MiniChecklist.FileReader;
using MiniChecklist.DataModels;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.IO;
using Prism.Events;
using MiniChecklist.Events;
using System.Linq;

namespace MiniChecklist.ViewModels
{
    public class ChecklistViewModel : BindableBase
    {
        public ObservableCollection<TodoTask> TodoList { get; } = new ObservableCollection<TodoTask>();

        private string _caption;
        public string Caption
        {
            get => _caption;
            set => SetProperty(ref _caption, value);
        }

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
        public DelegateCommandBase OpenNewFileCommand { get; }
        public DelegateCommandBase CheckTaskCommand { get; }

        public ITaskFileReader TaskFileReader { get; }

        /// <summary> For Preview only </summary>
        public ChecklistViewModel()
        {
            Caption = "DemoConstructor";

            TodoList.Add(new TodoTask("CheckMe"));
            TodoList.Add(new TodoTask("DoMe"));

            var task = new TodoTask("FinishMe");
            task.AddSubtasks(new TodoTask("Me also"));
            task.AddSubtasks(new TodoTask("And Me"));
            task.AddSubtasks(new TodoTask("And not to forget me"));
            TodoList.Add(task);
            TodoList.Add(new TodoTask("I'm Done") { Done = true});
        }

        public ChecklistViewModel(IEventAggregator ea, ITaskFileReader taskFileReader)
        {
            TaskFileReader = taskFileReader;

            ea.GetEvent<LoadFileEvent>().Subscribe(OpenNewFileEvent);

            FinishCommand = new DelegateCommand(OnFinish);
            OpenNewFileCommand = new DelegateCommand(OnOpenNewFile);
            CheckTaskCommand = new DelegateCommand<string>(OnCheckTask);
        }

        private void OnCheckTask(string id)
        {
            var item = TodoList.Single(x => x.Id == id);
            item.Done = !item.Done;
            item.Hide = item.Done && HideFinished;
        }

        private void OnOpenNewFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var result = TaskFileReader.ReadTasksFromFile(openFileDialog.FileName);
                UpdateView(result);
            }
        }

        private void UpdateView(TaskFileResult result)
        {
            string fileName = Path.GetFileName(result.Path);
            if (result.Status == ReadResult.FileNotFound)
            {
                Caption = $"File {fileName} not found";
                return;
            }

            if (result.Status == ReadResult.NoContent)
            {
                Caption = $"File {fileName} is empty";
                return;
            }

            Caption = fileName;
            TodoList.Clear();
            TodoList.AddRange(result.Todos);
        }


        private void OpenNewFileEvent(string path)
        {
            var result = TaskFileReader.ReadTasksFromFile(path);
            UpdateView(result);
        }

        private void OnFinish()
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
