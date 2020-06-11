using MiniChecklist.ViewModels;
using System.Collections.ObjectModel;

namespace MiniChecklist.Interfaces
{
    public interface ITaskListRepo
    {
        ObservableCollection<TodoTask> GetTaskList();
    }
}
