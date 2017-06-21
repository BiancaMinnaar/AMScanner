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
        //TODO: Get Client Specified Masks and delimeter and email addresses.
        public AMDirectoryToScanWatcher()
        {
            InitializeComponent();
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
                }
            }
        }

        protected override void OnStop()
        {
        }
    }
}
