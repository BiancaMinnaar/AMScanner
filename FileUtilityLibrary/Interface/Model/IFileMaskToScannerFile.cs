using FileUtilityLibrary.Interface.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUtilityLibrary.Interface.Model
{
    public interface IFileMaskToScannerFile<T> where T:IScannerFile
    {
        string GetFileMask();
        char GetDelimiter();
        T GetScannerFileInstance(FileInfo file);
    }
}
