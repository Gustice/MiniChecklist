using MiniChecklist.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MiniChecklist.FileReader
{
    public class TaskFileReader : ITaskFileReader
    {
        Regex TabPreambel = new Regex("^\t+");

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

            result.ChangeStatus(ReadResult.WrongFormat);
            if (TabPreambel.Match(lines[0]).Value.Length > 0)
                return result;

            int indent = 0;
            ICollection<TodoTask> lastIdx = null;
            Stack<ICollection<TodoTask>> idxStack = new Stack<ICollection<TodoTask>>();
            idxStack.Push(list);
            foreach (var item in lines)
            {
                var ci = TabPreambel.Match(item).Value.Length;
                var cleared = TabPreambel.Replace(item, "");
                var task = new TodoTask(cleared);
                if (ci == indent)
                {
                    idxStack.Peek().Add(task);
                    lastIdx = task.SubList;
                }
                else if (ci == indent + 1)
                {
                    indent = ci;
                    idxStack.Push(lastIdx);
                    lastIdx.Add(task);
                }
                else if (ci < indent)
                {
                    if ((indent - ci) > idxStack.Count)
                        return result;

                    for (int i = 0; i < indent - ci; i++)
                    {
                        idxStack.Pop();
                    }

                    indent = ci;
                    idxStack.Peek().Add(task);
                }
                else
                {
                    return result;
                }
            }

            result.ChangeStatus(ReadResult.ReadSuccess);
            result.AddData(list);

            return result;
        }
    }
}
