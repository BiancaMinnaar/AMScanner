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
        
        public bool ScanForExceptions(FileInfo fileToScan, out IList<string> exceptionList)
        {
            var scannerFile =_FileMaskToScannerFile.GetScannerFileInstance(fileToScan);
            foreach (IExceptionOccurrence rule in _ExceptionsToScanFor)
            {
                rule.ScanFile(scannerFile);                
            }

            exceptionList = scannerFile.ExceptionList;
            return scannerFile.HasException;
        }

        public void MoveFileAfterScan(IScannerFile fileToScan)
        {
            if (!fileToScan.HasException)
            {
                _MoverService.MoveFilesInList(new FileInfo[] { fileToScan.GetFileInfo() });
            }
        }

    }
}
