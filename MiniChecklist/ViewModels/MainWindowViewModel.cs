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
using NLog;

namespace MiniChecklist.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly ITaskFileReader _taskFileReader;
        private readonly ILogger _logger;
        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly ObservableCollection<TodoTask> _taskList;
        private readonly PathIndex _currentPath = new PathIndex();

        private string _caption;
        private bool _canSave;
        private bool _canEdit;
        private bool _canFinish;

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

        public InkrementHistory UndoStack { get; } = new InkrementHistory();
        public InkrementHistory RedoStack { get; } = new InkrementHistory();


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
            UndoCommand = new DelegateCommand(OnUndo).ObservesCanExecute(() => UndoStack.CanPop);
            RedoCommand = new DelegateCommand(OnRedo).ObservesCanExecute(() => RedoStack.CanPop);
            SelectNextCommand = new DelegateCommand(OnSelectNext);
            SelectPreviousCommand = new DelegateCommand(OnSelectPrevious);
            CheckUncheckCommand = new DelegateCommand(OnCheckUncheck);
        }

        public MainWindowViewModel(ILogger logger, IRegionManager regionManagerm, IEventAggregator eventAggregator, ITaskFileReader TaksFeilReader, ITaskListRepo taskListRepo) : this()
        {
            _logger = logger;
            _regionManager = regionManagerm;
            _eventAggregator = eventAggregator;
            _taskFileReader = TaksFeilReader;
            _taskList = taskListRepo.GetTaskList();

            _eventAggregator.GetEvent<LoadFileEvent>().Subscribe(OpenNewFileEvent);
        }

        private void OnNewInkrement()
        {
            var list = CollectAllTaskItems();
            UndoStack.Push(list);
            RedoStack.Clear();
        }

        private List<string> CollectAllTaskItems()
        {
            List<string> list = new List<string>();
            foreach (var item in _taskList) 
            {
                list.Add(item.ToString());
                list.AddRange(PrcessSublistsRecursively(item.SubList));
            }

            return list;
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

        private void OnFinish()
        {
            _eventAggregator.GetEvent<NewInkrementEvent>().Unsubscribe(OnNewInkrement);
            _regionManager.RequestNavigate(RegionNames.MainRegion, nameof(ChecklistView));
            CanFinish = false;
            CanEdit = true;
            CanSave = true;

            UndoStack.Clear();
            RedoStack.Clear();
        }

        private void OnEdit()
        {
            _eventAggregator.GetEvent<NewInkrementEvent>().Subscribe(OnNewInkrement);
            _regionManager.RequestNavigate(RegionNames.MainRegion, nameof(EditListView));
            CanFinish = true;
            CanEdit = false;
        }
               
        private void OnUndo()
        {
            _eventAggregator.GetEvent<NewInkrementEvent>().Unsubscribe(OnNewInkrement);

            RedoStack.Push(CollectAllTaskItems());
            var inkrement = UndoStack.Pop();
            
            var result = _taskFileReader.ReadTasksFromList(inkrement);
            UpdateView(result);
            _eventAggregator.GetEvent<RefreshConnectionsEvent>().Publish();
            _eventAggregator.GetEvent<NewInkrementEvent>().Subscribe(OnNewInkrement);
        }

        private void OnRedo()
        {
            _eventAggregator.GetEvent<NewInkrementEvent>().Unsubscribe(OnNewInkrement);
            var inkrement = RedoStack.Pop();
            UndoStack.Push(inkrement);

            var result = _taskFileReader.ReadTasksFromList(inkrement);
            UpdateView(result);
            _eventAggregator.GetEvent<RefreshConnectionsEvent>().Publish();
            _eventAggregator.GetEvent<NewInkrementEvent>().Subscribe(OnNewInkrement);
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
                var list = CollectAllTaskItems();
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

        private List<string> PrcessSublistsRecursively(ObservableCollection<TodoTask> subList, int level = 1)
        {
            string prepend = "".PadLeft(level, '\t');
            List<string> list = new List<string>();
            foreach (var item in subList)
            {
                list.Add($"{prepend}{item.ToString()}");
                list.AddRange(PrcessSublistsRecursively(item.SubList, level+1));
            }
            return list;
        }

        private void OnLoad()
        {
            _logger.Debug("Starting File-Load Dialogue");

            var openFile = new OpenFileDialog
            {
                InitialDirectory = _currentPath.CurrentPath,
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (openFile.ShowDialog() == true)
            {
                _logger.Debug($"Loading new File '{openFile.FileName}'");
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
        }

        private void OnNew()
        {
            _logger.Debug("Starting new File");
            _taskList.Clear();
            _regionManager.RequestNavigate(RegionNames.MainRegion, nameof(EditListView));

            UndoStack.Clear();
            RedoStack.Clear();

            CanFinish = true;
            CanEdit = false;
        }

        private void OpenNewFileEvent(string path)
        {
            _logger.Debug($"Open new File '{path}'");

            try
            {
                _currentPath.UpdateBase(path);
                var result = _taskFileReader.ReadTasksFromFile(path);
                UpdateView(result);

                UndoStack.Clear();
                RedoStack.Clear();
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error loading file {path}:\n" + e.Message, "Loading error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
