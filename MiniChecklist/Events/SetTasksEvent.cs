using MiniChecklist.ViewModels;
using Prism.Events;
using System.Collections.Generic;

namespace MiniChecklist.Events
{
    // todo: This can be removed if not further used
    class SetTasksEvent : PubSubEvent<List<TodoTask>> { }
}

