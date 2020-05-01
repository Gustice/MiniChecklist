using MiniChecklist.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniChecklist.FileReader
{
    public interface ITaskFileReader
    {

        TaskFileResult ReadTasksFromFile(string path);

    }
}
