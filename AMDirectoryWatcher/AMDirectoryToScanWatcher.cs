using AMCustomerImportInspector.Interface;
using AMCustomerImportInspector.Reposetory;
using AMCustomerImportInspector.Service;
using AMDirectoryWatcher.Interface;
using AMDirectoryWatcher.Reposetory;
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
        public const string MyServiceName = "AMFolderWatcher";
        private FileSystemWatcher watcher = null;
        private IDirecotryScannerReposetory _DirecotryWatcher;
        private log4net.ILog log;

        public AMDirectoryToScanWatcher()
        {
            InitializeComponent();
            log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info("DirectoryWatcher Has been installed");
            var importRepo = new CustomerImportReposetory(
                new CustomerImportRetrievalService(), new EmailService(), new EMailTemplateService());
            var scannerRepo = new ScannerRepository(new MoverService());
            _DirecotryWatcher = new DirectoryScannerReposetory(
                importRepo, scannerRepo, log,
                ConfigurationManager.AppSettings["ScannedDirectoryFound"], 
                ConfigurationManager.AppSettings["ScannedDirectoryReplace"], 
                ConfigurationManager.AppSettings["EmailSupportAddress"].Split(';'));
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
                        _DirecotryWatcher.ScannCreatedFile(e.FullPath);
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
