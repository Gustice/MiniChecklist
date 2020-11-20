﻿using MiniChecklist.ViewModels;
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

            int indent = 0;
            ICollection<TodoTask> lastIdx = null;
            Stack<ICollection<TodoTask>> idxStack = new Stack<ICollection<TodoTask>>();
            idxStack.Push(list);
            foreach (var item in lines)
            {
                var ci = TabPreambel.Match(item).Value.Length;
                var cleared = TabPreambel.Replace(item, "");

                TodoTask task = ProcessLine(cleared);

                if (task == null)
                {
                    continue;
                }

                // Ident stays on same level
                if (ci == indent)
                {
                    idxStack.Peek().Add(task);
                    lastIdx = task.SubList;
                }
                // Ident increases by one
                else if (ci == indent + 1)
                {
                    indent = ci;
                    idxStack.Push(lastIdx);
                    lastIdx.Add(task);
                }
                // Ident decreses by any level in range of stack
                else if (ci < indent)
                {
                    if ((indent - ci) > idxStack.Count)
                        return list;

                    for (int i = 0; i < indent - ci; i++)
                    {
                        idxStack.Pop();
                    }

                    indent = ci;
                    idxStack.Peek().Add(task);
                }
                // Else format error
                else
                {
                    return list;
                }
            }

            return list;
        }

        private TodoTask ProcessLine(string cleared)
        {
            TodoTask task = null;
            var parts = cleared.Split('#');
            if (parts.Length == 2)
            {
                task = new TodoTask(parts[0], parts[1], _eventAggregator);

            }
            else if (parts.Length == 1)
            {
                task = new TodoTask(parts[0], "", _eventAggregator);
            }

            return task;
        }
    }
}
