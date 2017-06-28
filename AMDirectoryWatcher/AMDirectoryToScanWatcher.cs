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
        ICustomerImportReposetory _ImportRepo;
        IScannerRepository _ScannerRepo;

        public AMDirectoryToScanWatcher()
        {
            InitializeComponent();
            _ImportRepo = new CustomerImportReposetory(new CustomerImportRetrievalService(), new EmailService());
            
        }

        protected override void OnStart(string[] args)
        {
            FileSystemWatcher watcher = new FileSystemWatcher(ConfigurationManager.AppSettings["DirectoryToWatch"]);
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
                    }
                    else
                    {
                        //move
                    }
                }
            }
        }

        protected override void OnStop()
        {
        }
    }
}
