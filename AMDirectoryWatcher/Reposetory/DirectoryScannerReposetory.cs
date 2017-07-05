using AMCustomerImportInspector.Interface;
using AMDirectoryWatcher.Interface;
using FileUtilityLibrary.Interface.Repository;
using log4net;
using FileUtilityLibrary.Model;
using FileUtilityLibrary.Service;
using System.Collections.Generic;
using FileUtilityLibrary.Interface.Model;
using System.IO;
using FileUtilityLibrary.ExpetionOccurrences;

namespace AMDirectoryWatcher.Reposetory
{
    public class DirectoryScannerReposetory : IDirecotryScannerReposetory
    {
        private ICustomerImportReposetory _ImportRepo;
        private IScannerRepository _ScannerRepo;
        private ILog _LogHandler;
        private string _ImportDirecotry;
        private string _VerifiedFileDirectory;
        private string[] _SupportEmailAddresses;

        public DirectoryScannerReposetory(
            ICustomerImportReposetory customerImportReposetory, IScannerRepository scannerReposetory, ILog logHandler,
            string importDirecotery, string verifiedFileDirectory, string[] supportEmailAddresses)
        {
            _ImportRepo = customerImportReposetory;
            _ScannerRepo = scannerReposetory;
            _LogHandler = logHandler;
            _ImportDirecotry = importDirecotery;
            _VerifiedFileDirectory = verifiedFileDirectory;
            _SupportEmailAddresses = supportEmailAddresses;
        }

        public void ScannCreatedFile(string fullFileName)
        {
            _LogHandler.Info("It's a new file");
            var customerImportDef = _ImportRepo.GetImportDefinisionFromFileName(fullFileName);
            if (customerImportDef != null)
            {
                _LogHandler.Info("Found Client Configuration");
                var direcotryToMoveTo = _ImportRepo.GetMoveToDirecotry(
                    fullFileName.Substring(0, fullFileName.LastIndexOf(@"\")), _ImportDirecotry, _VerifiedFileDirectory);
                _LogHandler.Info(direcotryToMoveTo);
                var fileMaskToScannerFile = new FileMaskToScannerFile(
                        customerImportDef.FileMask,
                        customerImportDef.Delimiter[0],
                        customerImportDef.HasHeader,
                        customerImportDef.ImportFormat);
                _ScannerRepo.MoverService = new MoverService(direcotryToMoveTo, _LogHandler);
                _ScannerRepo.FileMaskToScannerFile = fileMaskToScannerFile;
                _ScannerRepo.ExceptionsToScanFor =
                    new List<IExceptionOccurrence>() { new HeaderColumnLineCountExceptionOccurrence(
                                    "There is an error in the following line: ")};
                var scannerFile = _ScannerRepo.FileMaskToScannerFile.GetScannerFileInstance(new FileInfo(fullFileName));
                var isSuccessfullScan = _ScannerRepo.ScanForExceptions(scannerFile);
                //TODO: Test Properly
                if (isSuccessfullScan != null && !isSuccessfullScan.Value)
                {
                    _LogHandler.Info("Move File after scan");
                    _ScannerRepo.MoveFileAfterScan(scannerFile);
                    _LogHandler.Info("moved");
                }
                else
                {
                    _LogHandler.Info("Email Faulty File");
                    //TODO: REMOVE!!
                    customerImportDef.FailureEmailList = "bminnaar@gmail.com";
                    _ImportRepo.EmailFaultyFile(fullFileName, customerImportDef,
                        ((List<string>)scannerFile.ExceptionList).ToArray());
                    _LogHandler.Info("Delete Faulty File");
                    _ScannerRepo.DeleteFaultyFile(scannerFile);
                    _LogHandler.Info("deleted");
                }
            }
            else
            {
                _LogHandler.Info("The file is orphaned");
                _ImportRepo.EMailOrphenedFileToSupport(fullFileName, _SupportEmailAddresses);
                _ScannerRepo.DeleteOrphanedFile(fullFileName);
                _LogHandler.Info("The file " + fullFileName + " has been deleted.");
            }
        }
    }
}
