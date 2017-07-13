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
using FileUtilityLibrary.Model.ScannerFile;

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
        private object lockObject = new object();
        private bool _AlwaysMailSupport;

        public DirectoryScannerReposetory(
            ICustomerImportReposetory customerImportReposetory, IScannerRepository scannerReposetory, ILog logHandler,
            string importDirecotery, string verifiedFileDirectory, string[] supportEmailAddresses, bool alwaysMailSupport)
        {
            _ImportRepo = customerImportReposetory;
            _ScannerRepo = scannerReposetory;
            _LogHandler = logHandler;
            _ImportDirecotry = importDirecotery;
            _VerifiedFileDirectory = verifiedFileDirectory;
            _SupportEmailAddresses = supportEmailAddresses;
            _AlwaysMailSupport = alwaysMailSupport;
        }

        public void ScannCreatedFile(string fullFileName)
        {
            lock (lockObject)
            {
                _LogHandler.Debug("It's a new file");
                var customerImportDef = _ImportRepo.GetImportDefinisionFromFileName(fullFileName);
                if (customerImportDef != null)
                {
                    _LogHandler.Debug("Found Client Configuration");
                    _LogHandler.Debug("ClientDatabase :" + customerImportDef.ClientDatabase);
                    _LogHandler.Debug("ImportPath :" + customerImportDef.ImportPath);
                    _LogHandler.Debug("Delimiter :" + customerImportDef.Delimiter);
                    var direcotryToMoveTo = _ImportRepo.GetMoveToDirecotry(
                        fullFileName.Substring(0, fullFileName.LastIndexOf(@"\")), _ImportDirecotry, _VerifiedFileDirectory);
                    _LogHandler.Debug(direcotryToMoveTo);
                    var fileMaskToScannerFile = new FileMaskToScannerFile(
                            customerImportDef.FileMask,
                            customerImportDef.Delimiter[0],
                            customerImportDef.HasHeader,
                            customerImportDef.ImportFormat,
                            _LogHandler);
                    _ScannerRepo.MoverService = new MoverService(direcotryToMoveTo, _LogHandler);
                    _ScannerRepo.FileMaskToScannerFile = fileMaskToScannerFile;
                    _ScannerRepo.ExceptionsToScanFor =
                        new List<IExceptionOccurrence>() { new HeaderColumnLineCountExceptionOccurrence(
                                    "There is an error in the following line: ")};
                    var scannerFile = _ScannerRepo.FileMaskToScannerFile.GetScannerFileInstance(new FileInfo(fullFileName));
                    var hasExceptions = _ScannerRepo.ScanForExceptions(scannerFile);
                    var TempFile = new FileInfo(fullFileName);
                    if (hasExceptions == true)
                    {
                        _LogHandler.Debug("Email Faulty File");
                        if (_AlwaysMailSupport)
                        {
                            customerImportDef.FailureEmailList = _SupportEmailAddresses[0];
                            _LogHandler.Debug("sending mail to support");
                        }
                        _ImportRepo.EmailFaultyFile(fullFileName, customerImportDef,
                            ((List<string>)scannerFile.ExceptionList).ToArray());
                        _LogHandler.Debug("Delete Faulty File");
                        _ScannerRepo.DeleteFaultyFile(fullFileName);
                        _LogHandler.Debug("deleted");
                    }
                    else
                    {
                        _LogHandler.Debug("Move File after scan");
                        _ScannerRepo.MoveFileAfterScan(fullFileName);
                        _LogHandler.Debug("moved");
                    }
                }
                else
                {
                    _LogHandler.Debug("The file is orphaned");
                    _ImportRepo.EMailOrphenedFileToSupport(fullFileName, _SupportEmailAddresses);
                    _ScannerRepo.DeleteOrphanedFile(fullFileName);
                    _LogHandler.Debug("The file " + fullFileName + " has been deleted.");
                }
            }
        }
    }
}
