using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileUtilityTests.Sundry
{
    [TestClass]
    public class LogTests
    {
        [TestMethod]
        public void Test_LogMessage_LogsMessage()
        {
            log4net.ILog log =
               log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            log.Info("Application is working");

            Assert.IsTrue(true);
        }
    }
}
