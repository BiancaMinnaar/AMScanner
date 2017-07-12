using FileUtilityLibrary.Interface.Model;
using FileUtilityLibrary.Interface.Repository;
using FileUtilityLibrary.Interface.Service;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;

namespace FileUtilityLibrary.Reposetory
{
    public class ScannerRepository : IScannerRepository
    {
        public IMoverService MoverService { get; set; }
        public IFileMaskToScannerFile FileMaskToScannerFile { get; set; }
        public IList<IExceptionOccurrence> ExceptionsToScanFor { get; set; }
        private ILog _LogHandler;

        public ScannerRepository(IMoverService moverService, ILog logHandler)
        {
            MoverService = moverService;
            _LogHandler = logHandler;
        }
        
        public ScannerRepository(IMoverService moverService, 
            IFileMaskToScannerFile fileMaskToScannerFile, 
            IList<IExceptionOccurrence> exceptionsToScanFor, ILog logHandler)
            :this(moverService, logHandler)
        {
            FileMaskToScannerFile = fileMaskToScannerFile;
            ExceptionsToScanFor = exceptionsToScanFor;
        }

        
        public bool? ScanForExceptions(IScannerFile fileToScan)
        {
            if (ExceptionsToScanFor != null)
            {
                foreach (IExceptionOccurrence rule in ExceptionsToScanFor)
                {
                    rule.ScanFile(fileToScan);
                }
                _LogHandler.Debug(fileToScan.FileName + " HasError = " + fileToScan.HasException.ToString());
                var returnHasException = fileToScan.HasException;
                fileToScan.Dispose();
                return returnHasException;
            }
            else
            {
                _LogHandler.Fatal("No rules were configured for scan!");
            }

            return null;
        }

        public void MoveFileAfterScan(string fullFileName)
        {
            var fileToMove = new FileInfo(fullFileName);
            MoverService.MoveFilesInList(new FileInfo[] { fileToMove });
        }

        public void DeleteFaultyFile(string fullFileName)
        {
            var fileToDelete = new FileInfo(fullFileName);
            MoverService.DeleteFilesInList(new FileInfo[] { fileToDelete });
        }

        public void DeleteOrphanedFile(string fullFileName)
        {
            MoverService.DeleteFilesInList(new FileInfo[] { new FileInfo(fullFileName) });
        }
    }
}
