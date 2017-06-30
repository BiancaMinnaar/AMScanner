using FileUtilityLibrary.Interface.Model;
using FileUtilityLibrary.Interface.Service;
using System.Collections.Generic;

namespace FileUtilityLibrary.Interface.Repository
{
    public interface IScannerRepository
    {
        IMoverService MoverService { get; set; }
        IFileMaskToScannerFile FileMaskToScannerFile { get; set; }
        IList<IExceptionOccurrence> ExceptionsToScanFor { get; set; }

        void DeleteFaultyFile(IScannerFile fileToDelete);
        void DeleteOrphanedFile(string fullFileName);
        void MoveFileAfterScan(IScannerFile fileToScan);
        bool? ScanForExceptions(IScannerFile fileToScan);
    }
}
