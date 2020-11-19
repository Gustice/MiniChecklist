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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MiniChecklist.Interfaces;
using MiniChecklist.Services;
using System.Windows;

namespace MiniChecklist.ViewModels
{

    public class MainWindowViewModel : BindableBase
    {
        private readonly ITaskFileReader _taskFileReader;
        private readonly IRegionManager _regionManager;
        private readonly ObservableCollection<TodoTask> _taskList;
        private readonly PathIndex _currentPath = new PathIndex();

        private string _caption;
        private bool _canSave;
        private bool _canEdit;
        private bool _canFinish;
        private bool _canUndo;
        private bool _canRedo;

        public string Caption
        {
            get => _caption;
            set => SetProperty(ref _caption, value);
        }
        
        public bool CanSave
        {
            get { return _canSave; }
            set { SetProperty(ref _canSave, value); }
        }

        public bool CanEdit
        {
            get { return _canEdit; }
            set { SetProperty(ref _canEdit, value); }
        }

        public bool CanFinish
        {
            get { return _canFinish; }
            set { SetProperty(ref _canFinish, value); }
        }

        public bool CanUndo
        {
            get { return _canUndo; }
            set { SetProperty(ref _canUndo, value); }
        }

        public bool CanRedo
        {
            get { return _canRedo; }
            set { SetProperty(ref _canRedo, value); }
        }

        public DelegateCommand NewCommand { get; }
        public DelegateCommand LoadCommand { get; }
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand EditCommand { get; }
        public DelegateCommand FinishCommand { get; }
        public DelegateCommand UndoCommand { get; }
        public DelegateCommand RedoCommand { get; }
        
        public DelegateCommand SelectNextCommand { get; }
        public DelegateCommand SelectPreviousCommand { get; }
        public DelegateCommand CheckUncheckCommand { get; }

        public MainWindowViewModel()
        {
            Caption = "DemoConstructor";

            NewCommand = new DelegateCommand(OnNew);
            LoadCommand = new DelegateCommand(OnLoad);
            SaveCommand = new DelegateCommand(OnSave).ObservesCanExecute(() => CanSave);
            EditCommand = new DelegateCommand(OnEdit).ObservesCanExecute(() => CanEdit);
            FinishCommand = new DelegateCommand(OnFinish).ObservesCanExecute(() => CanFinish);
            UndoCommand = new DelegateCommand(OnUndo).ObservesCanExecute(() => CanUndo);
            RedoCommand = new DelegateCommand(OnRedo).ObservesCanExecute(() => CanRedo);
            SelectNextCommand = new DelegateCommand(OnSelectNext);
            SelectPreviousCommand = new DelegateCommand(OnSelectPrevious);
            CheckUncheckCommand = new DelegateCommand(OnCheckUncheck);

            CanUndo = false;
            CanRedo = false;
        }

        private void OnSelectPrevious()
        {
        }

        private void OnSelectNext()
        {
        }

        private void OnCheckUncheck()
        {
        }

        public MainWindowViewModel(IRegionManager regionManagerm, IEventAggregator eventAggregator, ITaskFileReader TaksFeilReader, ITaskListRepo taskListRepo) : this()
        {
            _taskFileReader = TaksFeilReader;
            _regionManager = regionManagerm;
            _taskList = taskListRepo.GetTaskList();

            eventAggregator.GetEvent<LoadFileEvent>().Subscribe(OpenNewFileEvent);
        }

        private void OnFinish()
        {
            _regionManager.RequestNavigate(RegionNames.MainRegion, nameof(ChecklistView));
            CanFinish = false;
            CanEdit = true;
            CanSave = true;
        }

        private void OnEdit()
        {
            _regionManager.RequestNavigate(RegionNames.MainRegion, nameof(EditListView));
            CanFinish = true;
            CanEdit = false;
        }

        private void OnUndo()
        { 

        }

        private void OnRedo()
        { 

        }

        private void OnSave()
        {
            var saveFile = new SaveFileDialog
            {
                InitialDirectory = _currentPath.CurrentPath,
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (saveFile.ShowDialog() == true)
            {
                List<string> list = new List<string>();
                int level = 1;
                foreach (var item in _taskList)
                {
                    list.Add($"{item.Task} # {item.Description}");
                    list.AddRange(PrcessSublistsRecursively(item.SubList, level));
                }

                using StreamWriter saved = new StreamWriter(saveFile.FileName);
                if (saved == null)
                    return;

                foreach (var item in list)
                {
                    saved.WriteLine(item);
                }
                saved.Close();
            }
        }

        private List<string> PrcessSublistsRecursively(ObservableCollection<TodoTask> subList, int level)
        {
            string prepend = "".PadLeft(level, '\t');
            List<string> list = new List<string>();
            foreach (var item in subList)
            {
                list.Add($"{prepend}{item.Task} # {item.Description}");
                list.AddRange(PrcessSublistsRecursively(item.SubList, level+1));
            }
            return list;
        }

        private void OnLoad()
        {
            var openFile = new OpenFileDialog
            {
                InitialDirectory = _currentPath.CurrentPath,
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (openFile.ShowDialog() == true)
            {
                _currentPath.UpdateBase(openFile.FileName);
                var result = _taskFileReader.ReadTasksFromFile(openFile.FileName);
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

            _taskList.Clear();
            _taskList.AddRange(result.Todos);
            CanEdit = true;
            CanUndo = false;
            CanRedo = false;
        }

        private void OnNew()
        {
            _taskList.Clear();
            _regionManager.RequestNavigate(RegionNames.MainRegion, nameof(EditListView));
            CanFinish = true;
            CanEdit = false;
        }

        private void OpenNewFileEvent(string path)
        {
            try
            {
                _currentPath.UpdateBase(path);
                var result = _taskFileReader.ReadTasksFromFile(path);
                UpdateView(result);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error loading file {path}:\n" + e.Message, "Loading error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
