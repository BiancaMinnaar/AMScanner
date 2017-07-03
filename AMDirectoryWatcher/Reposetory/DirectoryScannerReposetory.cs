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
                    customerImportDef.ImportPath, _ImportDirecotry, _VerifiedFileDirectory);
                var fileMaskToScannerFile = new FileMaskToScannerFile(
                        customerImportDef.FileMask,
                        customerImportDef.Delimiter[0],
                        customerImportDef.HasHeader,
                        customerImportDef.ImportFormat);
                _ScannerRepo.MoverService = new MoverService(direcotryToMoveTo);
                _ScannerRepo.FileMaskToScannerFile = fileMaskToScannerFile;
                _ScannerRepo.ExceptionsToScanFor =
                    new List<IExceptionOccurrence>() { new HeaderColumnLineCountExceptionOccurrence(
                                    "There is an error in the following line: ")};
                var scannerFile = _ScannerRepo.FileMaskToScannerFile.GetScannerFileInstance(new FileInfo(fullFileName));
                var isSuccessfullScan = _ScannerRepo.ScanForExceptions(scannerFile);
                if (isSuccessfullScan != null && isSuccessfullScan.Value)
                {
                    _ScannerRepo.MoveFileAfterScan(scannerFile);
                }
                else
                {
                    _ImportRepo.EmailFaultyFile(fullFileName, customerImportDef,
                        ((List<string>)scannerFile.ExceptionList).ToArray());
                    _ScannerRepo.DeleteFaultyFile(scannerFile);
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
