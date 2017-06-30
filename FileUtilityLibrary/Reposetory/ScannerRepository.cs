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
        public IMoverService MoverService { get; set; }
        public IFileMaskToScannerFile FileMaskToScannerFile { get; set; }
        public IList<IExceptionOccurrence> ExceptionsToScanFor { get; set; }

        public ScannerRepository(IMoverService moverService)
        {
            MoverService = moverService;
        }
        
        public ScannerRepository(IMoverService moverService, 
            IFileMaskToScannerFile fileMaskToScannerFile, 
            IList<IExceptionOccurrence> exceptionsToScanFor)
            :this(moverService)
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
                var log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Fatal("No rules were configured for scan!");
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
