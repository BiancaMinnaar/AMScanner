using FileUtilityLibrary.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUtilityLibrary.Interface.Model
{
    public interface IScannerFileCollection<T> where T:IScannerFile
    {
        IList<T> GetScannerFilesWithExceptions();
        IList<T> GetScannerFilesWithNoExceptions();
    }
}
