using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniChecklist.ViewModels
{
    
    class MainWindowViewModel : BindableBase
    {
        DelegateCommand NewCommand;
        DelegateCommand LoadCommand;
        DelegateCommand SaveCommand;
        DelegateCommand EditCommand;

        public MainWindowViewModel()
        {
            NewCommand = new DelegateCommand(OnNew);
            LoadCommand = new DelegateCommand(OnLoad);
            SaveCommand = new DelegateCommand(OnSave);
            EditCommand = new DelegateCommand(OnEdit);
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
