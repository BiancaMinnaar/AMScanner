using System.Collections.Generic;
using System.IO;

namespace FileUtilityLibrary.Interface.Model
{
    public interface IScannerFile : IScannerFileStream
    {
        string FileName { get; set; }
        string FilePath { get; set; }
        char Delimiter { get; set; }
        bool HasHeader { get; set; }
        bool HasException { get; set; }
        IList<string> ExceptionList
        { get; set; }
        bool HasSubStructures();
        FileInfo GetFileInfo();
    }
}
