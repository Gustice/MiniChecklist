using MiniChecklist.ViewModels;
using Prism.Events;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MiniChecklist.FileReader
{
    public class TaskFileReader : ITaskFileReader
    {
        readonly Regex TabPreambel = new Regex("^\t+");
        private readonly IEventAggregator _eventAggregator;

        public TaskFileReader(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public TaskFileResult ReadTasksFromFile(string path)
        {
            var list = new List<TodoTask>();

            if (!File.Exists(path))
                return new TaskFileResult(path, ReadResult.FileNotFound);

            var lines = File.ReadAllLines(path);
            if ((lines.Length == 0) || (lines.Length == 1 && string.IsNullOrEmpty(lines[0])))
                return new TaskFileResult(path, ReadResult.NoContent);

            if (TabPreambel.Match(lines[0]).Value.Length > 0)
                return new TaskFileResult(path, ReadResult.WrongFormat);

            list.AddRange(ProcessLines(lines));

            var result = new TaskFileResult(path, ReadResult.ReadSuccess);
            result.AddData(list);
            return result;
        }

        public TaskFileResult ReadTasksFromList(List<string> list)
        {
            var result = new TaskFileResult("", ReadResult.ReadSuccess);
            result.AddData(ProcessLines(list));

            return result;
        }
        
        private List<TodoTask> ProcessLines(IEnumerable<string> lines)
        {
            var list = new List<TodoTask>();

            int lastIndent = 0;
            ICollection<TodoTask> appendList = list;
            ICollection<TodoTask> insertList = null;
            Stack<ICollection<TodoTask>> idxStack = new Stack<ICollection<TodoTask>>();
            idxStack.Push(list);
            foreach (var item in lines)
            {
                var indent = TabPreambel.Match(item).Value.Length;
                var cleared = TabPreambel.Replace(item, "");
                if (string.IsNullOrEmpty(cleared))
                    continue;

                TodoTask task = ProcessLine(cleared);
                if (task == null)
                    continue;

                // Ident stays on same level
                if (indent == lastIndent)
                {
                    appendList.Add(task);
                }
                // Ident increases by one
                else if (indent == lastIndent + 1)
                {
                    idxStack.Push(insertList);
                    insertList.Add(task);
                }
                // Ident decreses by any level in range of stack
                else if (indent < lastIndent)
                {
                    if ((lastIndent - indent) > idxStack.Count)
                    { 
                        return list; // Format error
                    }

                    for (int i = 0; i < lastIndent - indent; i++)
                        idxStack.Pop();

                    idxStack.Peek().Add(task);
                }
                // Else format error
                else
                {
                    return list;
                }

                appendList = idxStack.Peek();
                insertList = task.SubList;
                lastIndent = indent;
            }

            return list;
        }

        private TodoTask ProcessLine(string cleared)
        {
            TodoTask task = null;
            var parts = cleared.Split('#');
            
            if (parts.Length == 2)
            {
                task = new TodoTask(parts[0].Trim(), parts[1].Trim(), _eventAggregator);

            }
            else if (parts.Length == 1)
            {
                task = new TodoTask(parts[0].Trim(), "", _eventAggregator);
            }

            return task;
        }
    }
}
