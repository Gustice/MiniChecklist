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

        private bool _canFinish;

        public bool CanFinish
        {
            get { return _canFinish; }
            set { SetProperty(ref _canFinish, value); }
        }

        public DelegateCommand NewCommand { get; }
        public DelegateCommand LoadCommand { get; }
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand EditCommand { get; }
        public DelegateCommand FinishCommand { get; }

        public ITaskFileReader _taskFileReader { get; }
        SetTasksEvent _setTasksEvent;
        GetTasksEvent _getTasksEvent;
        IRegionManager _regionManager;

        public MainWindowViewModel()
        {
            Caption = "DemoConstructor";

            NewCommand = new DelegateCommand(OnNew);
            LoadCommand = new DelegateCommand(OnLoad);
            SaveCommand = new DelegateCommand(OnSave).ObservesCanExecute(() => CanSave);
            EditCommand = new DelegateCommand(OnEdit).ObservesCanExecute(() => CanEdit);
            FinishCommand = new DelegateCommand(OnFinish).ObservesCanExecute(() => CanFinish);
        }

        public MainWindowViewModel(IRegionManager regionManagerm, IEventAggregator eventAggregator, ITaskFileReader TaksFeilReader) : this()
        {
            _taskFileReader = TaksFeilReader;
            _regionManager = regionManagerm;

            eventAggregator.GetEvent<LoadFileEvent>().Subscribe(OpenNewFileEvent);
            _setTasksEvent = eventAggregator.GetEvent<SetTasksEvent>();
            _getTasksEvent = eventAggregator.GetEvent<GetTasksEvent>();
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

        private void OnSave()
        {
            var saveFile = new SaveFileDialog
            {
                InitialDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (saveFile.ShowDialog() == true)
            {
                List<string> list = new List<string>();
                List<TodoTask> todos = new List<TodoTask>();
                _getTasksEvent.Publish(todos);
                int level = 1;
                foreach (var item in todos)
                {
                    list.Add($"{item.Task} # {item.Description}");
                    list.AddRange(PrcessSublistsRecursively(item.SubList, level));
                }

                using (StreamWriter saved = new StreamWriter(saveFile.FileName)) 
                {
                    if (saved == null)
                        return;

                    foreach (var item in list)
                    {
                        saved.WriteLine(item);
                    }
                    saved.Close();
                }
            }
        }

        private List<string> PrcessSublistsRecursively(ObservableCollection<TodoTask> subList, int level)
        {
            string prepend = "".PadLeft(level, '\t');
            List<string> list = new List<string>();
            foreach (var item in subList)
            {
                list.Add($"{prepend}{item.Task} # {item.Description}");
                list.AddRange(PrcessSublistsRecursively(item.SubList, level));
            }
            return list;
        }

        private void OnLoad()
        {
            var openFile = new OpenFileDialog
            {
                InitialDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (openFile.ShowDialog() == true)
            {
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
