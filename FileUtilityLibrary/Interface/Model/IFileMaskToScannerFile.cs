using FileUtilityLibrary.Interface.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUtilityLibrary.Interface.Model
{
    public interface IFileMaskToScannerFile
    {
        string FileMask { get; }
        char Delimiter { get; }
        bool HasHeader { get; }
        string ImportFormat { get; }
        IScannerFile GetScannerFileInstance(FileInfo file);
    }
}
