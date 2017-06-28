using AMCustomerImportInspector.Interface;
using AMCustomerImportInspector.Reposetory;
using AMCustomerImportInspector.Service;
using FileUtilityLibrary.ExpetionOccurrences;
using FileUtilityLibrary.Interface.Model;
using FileUtilityLibrary.Interface.Repository;
using FileUtilityLibrary.Model;
using FileUtilityLibrary.Model.ScannerFile;
using FileUtilityLibrary.Reposetory;
using FileUtilityLibrary.Service;
using FileUtilityLibrary.Service.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

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
                        _ScannerRepo = new ScannerRepository(
                            new MoverService(direcotryToMoveTo),
                            new FileMaskToScannerFile(
                                customerImportDef.FileMask,
                                customerImportDef.Delimiter[0],
                                customerImportDef.HasHeader,
                                customerImportDef.ImportFormat),
                            new List<IExceptionOccurrence>() { new HeaderColumnLineCountExceptionOccurrence(
                                "There is an error in the following line: ")}
                            );
                        //scan
                        IList<string> exceptionList;
                        _ScannerRepo.ScanForExceptions(new FileInfo(e.FullPath), out exceptionList);
                        //if scan fails 
                        //email
                        //delete
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
