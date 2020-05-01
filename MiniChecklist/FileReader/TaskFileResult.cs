using MiniChecklist.DataModels;
using System.Collections.Generic;

namespace MiniChecklist.FileReader
{
    public class TaskFileResult
    {
        public List<TodoTask> Todos { get; private set; } = new List<TodoTask>();

        public ReadResult Status { get; private set; }
        public string Path { get; }

        public TaskFileResult(string path, ReadResult status = ReadResult.FileNotFound)
        {
            Status = status;
            Path = path;
        }

        public void ChangeStatus(ReadResult status)
        {
            Status = status;
        }

        public void AddData(List<TodoTask> todos)
        {
            Todos.AddRange(todos);
        }
    }
}
