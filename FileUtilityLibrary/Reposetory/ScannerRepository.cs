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
        private IScannerService _ScannerService;
        private IMoverService _MoverService;
        private IFileMaskToScannerFile<IScannerFile> _FileMaskToScannerFile;
        private IList<IExceptionOccurrence> _ExceptionsToScanFor;
        
        
        public ScannerRepository(IScannerService scannerService, IMoverService moverService, IFileMaskToScannerFile<IScannerFile> fileMaskToScannerFile, IList<IExceptionOccurrence> exceptionsToScanFor)
        {
            _ScannerService = scannerService;
            _MoverService = moverService;
            _FileMaskToScannerFile = fileMaskToScannerFile;
            _ExceptionsToScanFor = exceptionsToScanFor;
        }
        public ScannerFileCollection<IScannerFile> FindFilesToScan()
        {
            if (_ScannerService.DirectoryHasFile(_FileMaskToScannerFile.GetFileMask()))
            {
                return _ScannerService.GetFilesToScan(_FileMaskToScannerFile);
            }
            return null;
        }
        public bool ScanFileForException(IScannerFile fileToScan)
        {
            foreach(IExceptionOccurrence rule in _ExceptionsToScanFor)
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

    }
}
