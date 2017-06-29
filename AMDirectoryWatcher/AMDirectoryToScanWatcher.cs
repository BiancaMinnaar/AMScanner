using AMCustomerImportInspector.Interface;
using AMCustomerImportInspector.Reposetory;
using AMCustomerImportInspector.Service;
using FileUtilityLibrary.ExpetionOccurrences;
using FileUtilityLibrary.Interface.Model;
using FileUtilityLibrary.Interface.Repository;
using FileUtilityLibrary.Model;
using FileUtilityLibrary.Reposetory;
using FileUtilityLibrary.Service;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.ServiceProcess;

namespace AMDirectoryWatcher
{
    public partial class AMDirectoryToScanWatcher : ServiceBase
    {
        private ICustomerImportReposetory _ImportRepo;
        private IScannerRepository _ScannerRepo;
        public const string MyServiceName = "AMFolderWatcher";
        private FileSystemWatcher watcher = null;

        public AMDirectoryToScanWatcher()
        {
            InitializeComponent();
            _ImportRepo = new CustomerImportReposetory(
                new CustomerImportRetrievalService(), new EmailService(), new EMailTemplateService());
        }

        protected override void OnStart(string[] args)
        {
            watcher = new FileSystemWatcher(ConfigurationManager.AppSettings["DirectoryToWatch"]);
            watcher.IncludeSubdirectories = true;
            watcher.Created += WatcherFoundCreation;
        }

        private void WatcherFoundCreation(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                if (!Directory.Exists(e.FullPath))
                {
                    //file
                    var customerImportDef = _ImportRepo.GetImportDefinisionFromFileName(e.FullPath);
                    if (customerImportDef != null)
                    {
                        
                        var direcotryToMoveTo = _ImportRepo.GetMoveToDirecotry(
                            customerImportDef.ImportPath, 
                            ConfigurationManager.AppSettings["ScannedDirectoryFound"],
                            ConfigurationManager.AppSettings["ScannedDirectoryReplace"]);
                        var fileMaskToScannerFile = new FileMaskToScannerFile(
                                customerImportDef.FileMask,
                                customerImportDef.Delimiter[0],
                                customerImportDef.HasHeader,
                                customerImportDef.ImportFormat);
                        _ScannerRepo = new ScannerRepository(
                            new MoverService(direcotryToMoveTo),
                            fileMaskToScannerFile,
                            new List<IExceptionOccurrence>() { new HeaderColumnLineCountExceptionOccurrence(
                                "There is an error in the following line: ")}
                            );
                        var scannerFile = fileMaskToScannerFile.GetScannerFileInstance(new FileInfo(e.FullPath));
                        //scan
                        var isSuccesfullScan = _ScannerRepo.ScanForExceptions(scannerFile);
                        //if scan fails 
                        //email
                        //delete
                        //or move
                        if (isSuccesfullScan)
                        {
                            _ScannerRepo.MoveFileAfterScan(scannerFile);
                        }
                        else
                        {
                            _ImportRepo.EmailFaultyFile(e.FullPath, customerImportDef, 
                                ((List<string>)scannerFile.ExceptionList).ToArray());
                            _ScannerRepo.DeleteFaultyFile(scannerFile);
                        }
                    }
                    else
                    {
                        _ImportRepo.EMailOrphenedFileToSupport(
                                e.FullPath, new string[] { ConfigurationManager.AppSettings["EmailSupportAddress"] });
                        _ScannerRepo.DeleteOrphanedFile(e.FullPath);
                    }
                }
            }
        }

        protected override void OnStop()
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }
    }
}
