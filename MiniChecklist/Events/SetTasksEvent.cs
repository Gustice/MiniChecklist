using MiniChecklist.DataModels;
using Prism.Events;
using System.Collections.Generic;

namespace MiniChecklist.Events
{
    class SetTasksEvent : PubSubEvent<List<TodoTask>> { }
}

