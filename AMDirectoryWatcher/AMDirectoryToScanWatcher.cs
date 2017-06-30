using AMCustomerImportInspector.Interface;
using AMCustomerImportInspector.Reposetory;
using AMCustomerImportInspector.Service;
using FileUtilityLibrary.ExpetionOccurrences;
using FileUtilityLibrary.Interface.Model;
using FileUtilityLibrary.Interface.Repository;
using FileUtilityLibrary.Model;
using FileUtilityLibrary.Reposetory;
using FileUtilityLibrary.Service;
using System;
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
        private log4net.ILog log;

        public AMDirectoryToScanWatcher()
        {
            InitializeComponent();
            log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info("DirectoryWatcher Has been installed");
            _ImportRepo = new CustomerImportReposetory(
                new CustomerImportRetrievalService(), new EmailService(), new EMailTemplateService());
            _ScannerRepo = new ScannerRepository(new MoverService());
        }

        protected override void OnStart(string[] args)
        {
            log.Info("DirectoryWatcher Has been Started");
            watcher = new FileSystemWatcher(ConfigurationManager.AppSettings["DirectoryToWatch"]);
            log.Info("The watcher has been initialised with directory :" + ConfigurationManager.AppSettings["DirectoryToWatch"]);
            watcher.IncludeSubdirectories = true;
            
            watcher.NotifyFilter = NotifyFilters.LastAccess
               | NotifyFilters.LastWrite
               | NotifyFilters.FileName
               | NotifyFilters.DirectoryName;
            watcher.Created += WatcherFoundCreation;
            watcher.EnableRaisingEvents = true;
        }

        private void WatcherFoundCreation(object sender, FileSystemEventArgs e)
        {
            log.Info("A File " + e.Name + " in the path " + e.FullPath + " has been changed");
            try
            {
                if (e.ChangeType == WatcherChangeTypes.Created)
                {
                    if (!Directory.Exists(e.FullPath))
                    {
                        log.Info("It's a new file");
                        var fullFileName = e.FullPath;
                        var customerImportDef = _ImportRepo.GetImportDefinisionFromFileName(fullFileName);
                        if (customerImportDef != null)
                        {
                            log.Info("Found Client Configuration");
                            var direcotryToMoveTo = _ImportRepo.GetMoveToDirecotry(
                                customerImportDef.ImportPath,
                                ConfigurationManager.AppSettings["ScannedDirectoryFound"],
                                ConfigurationManager.AppSettings["ScannedDirectoryReplace"]);
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
                            var scannerFile = fileMaskToScannerFile.GetScannerFileInstance(new FileInfo(fullFileName));
                            //scan
                            var isSuccesfullScan = _ScannerRepo.ScanForExceptions(scannerFile);
                            //if scan fails 
                            //email
                            //delete
                            //or move
                            if (isSuccesfullScan != null && isSuccesfullScan.Value)
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
                            log.Info("The file is orphaned");
                            _ImportRepo.EMailOrphenedFileToSupport(
                                    e.FullPath, new string[] { ConfigurationManager.AppSettings["EmailSupportAddress"] });
                            _ScannerRepo.DeleteOrphanedFile(fullFileName);
                            log.Info("The file " + fullFileName + " has been deleted.");
                        }
                    }
                }
            }
            catch(Exception excp)
            {
                log.Fatal(excp.Message);
                log.Fatal(excp.StackTrace);
            }
        }

        protected override void OnStop()
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }
    }
}
