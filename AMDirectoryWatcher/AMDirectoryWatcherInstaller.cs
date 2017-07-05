using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace AMDirectoryWatcher
{
    [RunInstaller(true)]
    public partial class AMDirectoryWatcherInstaller : Installer
    {
        private ServiceProcessInstaller serviceProcessInstaller;
        private ServiceInstaller serviceInstaller;
        public AMDirectoryWatcherInstaller()
        {
            //InitializeComponent();
            serviceProcessInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();
            // Here you can set properties on serviceProcessInstaller                                          
            //or register event handlers                                          
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;

            serviceInstaller.ServiceName = AMDirectoryToScanWatcher.MyServiceName;
            this.Installers.AddRange(new Installer[] {
                serviceProcessInstaller, serviceInstaller });
        }
    }
}
