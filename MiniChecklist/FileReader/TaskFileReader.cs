using MiniChecklist.DataModels;
using System.Collections.Generic;
using System.IO;

namespace MiniChecklist.FileReader
{
    public class TaskFileReader : ITaskFileReader
    {
        public TaskFileResult ReadTasksFromFile(string path)
        {
            var list = new List<TodoTask>();
            var result = new TaskFileResult(path, ReadResult.FileNotFound);

            if (!File.Exists(path))
                return result;

            result.ChangeStatus(ReadResult.NoContent);
            var lines = File.ReadAllLines(path);
            if ((lines.Length == 0) || (lines.Length == 1 && string.IsNullOrEmpty(lines[0])))
                return result;
            
            foreach (var item in lines)
                list.Add(new TodoTask(item));

            result.ChangeStatus(ReadResult.ReadSuccess);
            result.AddData(list);

            return result;
        }
    }
}
