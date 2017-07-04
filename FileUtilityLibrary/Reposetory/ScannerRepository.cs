﻿using FileUtilityLibrary.Interface.Model;
using FileUtilityLibrary.Interface.Repository;
using FileUtilityLibrary.Interface.Service;
using log4net;
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

                return fileToScan.HasException;
            }
            else
            {
                _LogHandler.Fatal("No rules were configured for scan!");
            }

            return null;
        }

        public void MoveFileAfterScan(IScannerFile fileToScan)
        {
            if (!fileToScan.HasException)
            {
                MoverService.MoveFilesInList(new FileInfo[] { fileToScan.GetFileInfo() });
            }
        }

        public void DeleteFaultyFile(IScannerFile fileToDelete)
        {
            if (fileToDelete.HasException)
            {
                MoverService.DeleteFilesInList(new FileInfo[] { fileToDelete.GetFileInfo() });
            }
        }

        public void DeleteOrphanedFile(string fullFileName)
        {
            MoverService.DeleteFilesInList(new FileInfo[] { new FileInfo(fullFileName) });
        }
    }
}
