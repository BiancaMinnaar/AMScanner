using FileUtilityLibrary.Interface.Model;
using FileUtilityLibrary.Interface.Repository;
using FileUtilityLibrary.Interface.Service;
using FileUtilityLibrary.Model;
using System.Collections.Generic;
using System.IO;

namespace FileUtilityLibrary.Reposetory
{
    public class ScannerRepository : IScannerRepository
    {
        private IMoverService _MoverService;
        private IFileMaskToScannerFile _FileMaskToScannerFile;
        private IList<IExceptionOccurrence> _ExceptionsToScanFor;
        
        public ScannerRepository(IMoverService moverService, 
            IFileMaskToScannerFile fileMaskToScannerFile, 
            IList<IExceptionOccurrence> exceptionsToScanFor)
        {
            _MoverService = moverService;
            _FileMaskToScannerFile = fileMaskToScannerFile;
            _ExceptionsToScanFor = exceptionsToScanFor;
        }
        
        public bool ScanForExceptions(IScannerFile fileToScan)
        {
            foreach (IExceptionOccurrence rule in _ExceptionsToScanFor)
            {
                rule.ScanFile(fileToScan);                
            }

            return fileToScan.HasException;
        }

        public void MoveFileAfterScan(IScannerFile fileToScan)
        {
            if (!fileToScan.HasException)
            {
                _MoverService.MoveFilesInList(new FileInfo[] { fileToScan.GetFileInfo() });
            }
        }

        public void DeleteFaultyFile(IScannerFile fileToDelete)
        {
            if (fileToDelete.HasException)
            {
                _MoverService.DeleteFilesInList(new FileInfo[] { fileToDelete.GetFileInfo() });
            }
        }

        public void DeleteOrphanedFile(string fullFileName)
        {
            _MoverService.DeleteFilesInList(new FileInfo[] { new FileInfo(fullFileName) });
        }
    }
}
