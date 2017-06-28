using FileUtilityLibrary.Interface.Model;
using FileUtilityLibrary.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUtilityLibrary.Interface.Repository
{
    public interface IScannerRepository
    {
        void MoveFileAfterScan(IScannerFile fileToScan);
        bool ScanForExceptions(FileInfo fileToScan, out IList<string> exceptionList);
    }
}
