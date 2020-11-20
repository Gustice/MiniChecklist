using System.Collections.Generic;

namespace MiniChecklist.FileReader
{
    public interface ITaskFileReader
    {

        TaskFileResult ReadTasksFromFile(string path);
        TaskFileResult ReadTasksFromList(List<string> list);
    }
}
