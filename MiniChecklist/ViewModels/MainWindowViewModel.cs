using System;
using MiniChecklist.FileReader;
using System.IO;
using Prism.Events;
using MiniChecklist.Events;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System.Windows;

namespace MiniChecklist.ViewModels
{

    public class MainWindowViewModel : BindableBase
    {
        private string _caption;
        public string Caption
        {
            get => _caption;
            set => SetProperty(ref _caption, value);
        }


        private bool _canSave;

        public bool CanSave
        {
            get { return _canSave; }
            set { SetProperty(ref _canSave, value); }
        }

        private bool _canEdit;

        public bool CanEdit
        {
            get { return _canEdit; }
            set { SetProperty(ref _canEdit, value); }
        }


        public DelegateCommand NewCommand { get; }
        public DelegateCommand LoadCommand { get; }
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand EditCommand { get; }

        public ITaskFileReader TaskFileReader { get; }
        SetTasksEvent _setTasksEvent;

        public MainWindowViewModel()
        {
            Caption = "DemoConstructor";

            NewCommand = new DelegateCommand(OnNew);
            LoadCommand = new DelegateCommand(OnLoad);
            SaveCommand = new DelegateCommand(OnSave).ObservesCanExecute(() => CanSave);
            EditCommand = new DelegateCommand(OnEdit).ObservesCanExecute(() => CanEdit);

            CanSave = false;
            CanEdit = false;
        }

        public MainWindowViewModel(IEventAggregator ea, ITaskFileReader taskFileReader) : this()
        {
            TaskFileReader = taskFileReader;

            ea.GetEvent<LoadFileEvent>().Subscribe(OpenNewFileEvent);
            _setTasksEvent = ea.GetEvent<SetTasksEvent>();
        }

        private void OnEdit()
        {
            throw new NotImplementedException();
        }

        private void OnSave()
        {
            throw new NotImplementedException();
        }

        private void OnLoad()
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

            _setTasksEvent.Publish(result.Todos);
        }

        private void OnNew()
        {
            // throw new NotImplementedException();
            MessageBox.Show("Not implemented yet, sorry");
        }

        private void OpenNewFileEvent(string path)
        {
            var result = TaskFileReader.ReadTasksFromFile(path);
            UpdateView(result);
        }
    }
}
