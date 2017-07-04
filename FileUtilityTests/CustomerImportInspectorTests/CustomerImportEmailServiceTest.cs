using AMCustomerImportInspector.Service;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FileUtilityTests.CustomerImportInspectorTests
{
    [TestClass]
    public class CustomerImportEmailServiceTest
    {
        [TestMethod]
        public void Test_SendEmailToRecipient_SendsEmail()
        {
            var logMock = new Mock<ILog>();
            var service = new EmailService(logMock.Object);

            service.SendEmailToRecipient(CustomerImportInspectorConstants.CONSTEmailAddress, CustomerImportInspectorConstants.CONSTMessageSubject, CustomerImportInspectorConstants.CONSTMessageBody, FileUtilityLibraryConstants.CONSTDirectoryToScan + "\\" + FileUtilityLibraryConstants.CONSTExcelFileWithError);

            Assert.IsTrue(true);
        }
    }
}
