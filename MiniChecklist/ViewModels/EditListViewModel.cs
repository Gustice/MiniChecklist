﻿using MiniChecklist.Events;
using MiniChecklist.Interfaces;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MiniChecklist.ViewModels
{
    public class EditListViewModel : BindableBase, INavigationAware
    {
        private readonly IEventAggregator _eventAggregator;

        public ObservableCollection<TodoTask> TodoList { get; } = new ObservableCollection<TodoTask>();

        public DelegateCommandBase InsertFirstCommand { get; }

        /// <summary> For Preview only </summary>
        public EditListViewModel()
        {

        }

        public EditListViewModel(IEventAggregator eventAggregator, ITaskListRepo taskListRepo)
        {
            _eventAggregator = eventAggregator;
            TodoList = taskListRepo.GetTaskList();
            InsertFirstCommand = new DelegateCommand<string>(OnInsertFirst);

            eventAggregator.GetEvent<RefreshConnectionsEvent>().Subscribe(OnRefreshConnections);
        }

        private void OnRefreshConnections()
        {
            RefreshConnections();
        }

        private void OnInsertFirst(string command)
        {
            _eventAggregator.GetEvent<NewInkrementEvent>().Publish();

            TodoTask item;
            switch (command)
            {
                case "Append":
                    item = new TodoTask("", "", _eventAggregator);
                    item.SetParent(TodoList);
                    TodoList.Add(item);
                    break;

                case "Prepend":
                    item = new TodoTask("", "", _eventAggregator);
                    item.SetParent(TodoList);
                    TodoList.Insert(0, item);
                    break;

                default:
                    throw new Exception($"Unknown Command '{command}'");
            }
        }

        void AppendEmptyRecusively(ICollection<TodoTask> list)
        {
            foreach (var item in list)
            {
                AppendEmptyRecusively(item.SubList);
            }
            list.Add(new TodoTask("", "", _eventAggregator));
        }

        void SetParenthoodRecursively(TodoTask task)
        {
            foreach (var item in task.SubList)
            {
                item.SetParent(task);
                SetParenthoodRecursively(item);
            }
        }

        void ClearEmptyRecusively(ICollection<TodoTask> list)
        {
            List<TodoTask> temp = new List<TodoTask>();
            temp.AddRange(list);
            foreach (var item in temp)
            {
                if (string.IsNullOrEmpty( item.Task ) && item.SubList.Count == 0)
                {
                    list.Remove(item);
                }
                ClearEmptyRecusively(item.SubList);
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RefreshConnections();
        }

        private void RefreshConnections()
        {
            foreach (var item in TodoList)
            {
                item.SetParent(TodoList);
                SetParenthoodRecursively(item);
            }
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            ClearEmptyRecusively(TodoList);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;
    }
}
