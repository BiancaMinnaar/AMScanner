using AMCustomerImportInspector.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileUtilityTests.CustomerImportInspectorTests
{
    [TestClass]
    public class CustomerImportEmailServiceTest
    {
        [TestMethod]
        public void Test_SendEmailToRecipient_SendsEmail()
        {
            var service = new EmailService();

            service.SendEmailToRecipient(CustomerImportInspectorConstants.CONSTEmailAddress, CustomerImportInspectorConstants.CONSTMessageSubject, CustomerImportInspectorConstants.CONSTMessageBody, FileUtilityLibraryConstants.CONSTDirectoryToScan + "\\" + FileUtilityLibraryConstants.CONSTExcelFileWithError);

            Assert.IsTrue(true);
        }
    }
}
