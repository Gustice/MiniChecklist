using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MiniChecklist.Services
{
    class PathIndex
    {
        public string CurrentPath { get; private set; } 

        public PathIndex()
        {
            CurrentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        public void UpdateBase(string newPath)
        {
            CurrentPath = newPath;
            try
            {
                if (Directory.Exists(CurrentPath))
                    return;

                var file = new FileInfo(CurrentPath);
                CurrentPath = file.Directory.FullName;
            }
            catch (Exception)
            {
                CurrentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
        }
    }
}
