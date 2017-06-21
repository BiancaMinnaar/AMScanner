using System.ServiceProcess;

namespace AMDirectoryWatcher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new AMDirectoryToScanWatcher()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
