namespace MiniChecklist.FileReader
{
    public interface ITaskFileReader
    {

        TaskFileResult ReadTasksFromFile(string path);

    }
}
