using MiniChecklist.Interfaces;
using MiniChecklist.ViewModels;
using System.Collections.ObjectModel;

namespace MiniChecklist.Repositories
{
    public class TaskListRepo : ITaskListRepo
    {
        readonly ObservableCollection<TodoTask> TaskList = new ObservableCollection<TodoTask>();

        public ObservableCollection<TodoTask> GetTaskList() => TaskList;
    }
}
