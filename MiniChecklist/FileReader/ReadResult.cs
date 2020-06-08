using System;
using System.Collections.Generic;
using System.Text;

namespace MiniChecklist.FileReader
{
    public enum ReadResult
    {
        FileNotFound,
        NoContent,
        WrongFormat,
        ReadSuccess,
    }
}
