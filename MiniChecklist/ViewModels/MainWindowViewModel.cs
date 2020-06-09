using System;
using MiniChecklist.FileReader;
using System.IO;
using Prism.Events;
using MiniChecklist.Events;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using MiniChecklist.Defines;
using MiniChecklist.Views;

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

        public ITaskFileReader _taskFileReader { get; }
        SetTasksEvent _setTasksEvent;
        IRegionManager _regionManager;

        public MainWindowViewModel()
        {
            Caption = "DemoConstructor";

            NewCommand = new DelegateCommand(OnNew);
            LoadCommand = new DelegateCommand(OnLoad);
            SaveCommand = new DelegateCommand(OnSave).ObservesCanExecute(() => CanSave);
            EditCommand = new DelegateCommand(OnEdit).ObservesCanExecute(() => CanEdit);
        }

        public MainWindowViewModel(IRegionManager regionManagerm, IEventAggregator eventAggregator, ITaskFileReader TaksFeilReader) : this()
        {
            _taskFileReader = TaksFeilReader;
            _regionManager = regionManagerm;

            eventAggregator.GetEvent<LoadFileEvent>().Subscribe(OpenNewFileEvent);
            _setTasksEvent = eventAggregator.GetEvent<SetTasksEvent>();
        }

        private void OnEdit()
        {
            _regionManager.RequestNavigate(RegionNames.MainRegion, nameof(EditListView));
            CanSave = true;
            CanEdit = false;
        }

        private void OnSave()
        {
            _regionManager.RequestNavigate(RegionNames.MainRegion, nameof(ChecklistView));
            CanSave = false;
            CanEdit = true;
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
                var result = _taskFileReader.ReadTasksFromFile(openFileDialog.FileName);
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
            CanEdit = true;
        }

        private void OnNew()
        {
            throw new NotImplementedException();
        }

        private void OpenNewFileEvent(string path)
        {
            var result = _taskFileReader.ReadTasksFromFile(path);
            UpdateView(result);
        }
    }
}
