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

        public MainWindowViewModel(IRegionManager regionManagerm, IEventAggregator eventAggregator, ITaskFileReader TaksFeilReader, ITaskListRepo taskListRepo) : this()
        {
            _taskFileReader = TaksFeilReader;
            _regionManager = regionManagerm;
            _taskList = taskListRepo.GetTaskList();

            eventAggregator.GetEvent<LoadFileEvent>().Subscribe(OpenNewFileEvent);
            eventAggregator.GetEvent<NewInkrementEvent>().Subscribe(OnNewInkrement);
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
            _regionManager.RequestNavigate(RegionNames.MainRegion, nameof(ChecklistView));
            CanFinish = false;
            CanEdit = true;
            CanSave = true;

            UndoStack.Clear();
            RedoStack.Clear();
        }

        private void OnEdit()
        {
            _regionManager.RequestNavigate(RegionNames.MainRegion, nameof(EditListView));
            CanFinish = true;
            CanEdit = false;
        }
               
        private void OnUndo()
        {
            RedoStack.Push(CollectAllTaskItems());
            var inkrement = UndoStack.Pop();
            
            var result = _taskFileReader.ReadTasksFromList(inkrement);
            UpdateView(result);
        }

        private void OnRedo()
        {
            var inkrement = RedoStack.Pop();
            UndoStack.Push(inkrement);

            var result = _taskFileReader.ReadTasksFromList(inkrement);
            UpdateView(result);
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
        }

        private void OnNew()
        {
            _taskList.Clear();
            _regionManager.RequestNavigate(RegionNames.MainRegion, nameof(EditListView));
            CanFinish = true;
            CanEdit = false;

            UndoStack.Clear();
            RedoStack.Clear();
        }

        private void OpenNewFileEvent(string path)
        {
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
