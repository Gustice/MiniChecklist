using Prism.Commands;
using Prism.Mvvm;
using System;

namespace MiniChecklist.ViewModels
{

    class MainWindowViewModel : BindableBase
    {
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

        public MainWindowViewModel()
        {
            NewCommand = new DelegateCommand(OnNew);
            LoadCommand = new DelegateCommand(OnLoad);
            SaveCommand = new DelegateCommand(OnSave).ObservesCanExecute(() => CanSave);
            EditCommand = new DelegateCommand(OnEdit).ObservesCanExecute(() => CanEdit);

            CanSave = false;
            CanEdit = false;
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
            throw new NotImplementedException();
        }

        private void OnNew()
        {
            throw new NotImplementedException();
        }
    }
}
