using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AMCustomerImportInspector.Service;
using System.Text;

namespace FileUtilityTests.CustomerImportInspectorTests
{
    [TestClass]
    public class EMailTemplateServiceTests
    {
        [TestMethod]
        public void Test_GetSubjectText_ReturnsCorrectText()
        {
            var templateService = new EMailTemplateService();

            var subject = templateService.GetSubjectText(
                "AMCustomerImportInspector.EmailTemplates.FaultyImportFIleTemplate.xml",
                "//FaultyImportFileEMail/EMailSubject");

            Assert.AreEqual("Hi, Your Import File has been found faulty.", subject);
        }

        [TestMethod]
        public void Test_GetBodyPreErrorText_ReturnsCorrectText()
        {
            var templateService = new EMailTemplateService();

            var subject = templateService.GetBodyPreReplacementText(
                "AMCustomerImportInspector.EmailTemplates.FaultyImportFIleTemplate.xml",
                "//FaultyImportFileEMail/EMailBody/PreErrorText");

            Assert.AreEqual("Please review the errors below and contact support if necesary.\r\n<ErrorText></ErrorText>", subject);
        }

        [TestMethod]
        public void Test_GetWholeEmailBodyWithErrors_ReturnsCorrectText()
        {
            var templateService = new EMailTemplateService();

            var subject = templateService.GetWholeEmailBodyWithErrors(
                "AMCustomerImportInspector.EmailTemplates.FaultyImportFIleTemplate.xml",
                "//FaultyImportFileEMail/EMailBody/PreErrorText",
                "<ErrorText></ErrorText>",
                new string[] { "Error on Line 1", "Error on Line 2"});
            StringBuilder sb = new StringBuilder("Please review the errors below and contact support if necesary.\r\n");
            sb.AppendLine("Error on Line 1");
            sb.AppendLine("Error on Line 2");

            Assert.AreEqual(sb.ToString(), subject);
        }
    }
}
