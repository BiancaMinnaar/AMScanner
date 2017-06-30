using AMCustomerImportInspector.Interface;
using System.Collections.Generic;
using AMCustomerImportInspector.Model;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace AMCustomerImportInspector.Reposetory
{
    public class CustomerImportReposetory : ICustomerImportReposetory
    {
        private ICustomerImportRetrievalService _DataService;
        private IEmailService _EmailService;
        private IEMailTemplateService _EMailTemplateService;

        public CustomerImportReposetory(ICustomerImportRetrievalService dataService, IEmailService emailService, IEMailTemplateService emailTemplateService)
        {
            _DataService = dataService;
            _EmailService = emailService;
            _EMailTemplateService = emailTemplateService;
        }

        public void EmailFaultyFile(string fullFileName, ImportDefinision definition, string[] errorList)
        {
            var faultyEmailSubject = _EMailTemplateService.GetSubjectText(
                "AMCustomerImportInspector.EmailTemplates.FaultyImportFIleTemplate.xml",
                "//FaultyImportFileEMail/EMailSubject");
            var faultyEmailBody = _EMailTemplateService.GetWholeEmailBodyWithErrors(
                "AMCustomerImportInspector.EmailTemplates.FaultyImportFIleTemplate.xml",
                "//FaultyImportFileEMail/EMailBody/PreErrorText", "<ErrorText></ErrorText>", errorList);
            foreach (string emailAddress in definition.FailureEmailAddresses)
            {
                _EmailService.SendEmailToRecipient(emailAddress, faultyEmailSubject, faultyEmailBody, fullFileName);
            }
        }

        public void EMailOrphenedFileToSupport(string fullFileName, string[] emailAddressList)
        {
            var orphanedEmailSubject = _EMailTemplateService.GetSubjectText(
                "AMCustomerImportInspector.EmailTemplates.OrphanedImportFileTemplate.xml",
                "//OrphanedImportFileEMail/EMailSubject");
            var orphanedEmailBody = _EMailTemplateService.GetWholeEmailBody(
                "AMCustomerImportInspector.EmailTemplates.OrphanedImportFileTemplate.xml",
                "//OrphanedImportFileEMail/EMailBody/PreErrorText", 
                "<MissPlacedFileDirectory></MissPlacedFileDirectory>", fullFileName);

            foreach (string emailAddress in emailAddressList)
            {
                _EmailService.SendEmailToRecipient(
                    emailAddress, orphanedEmailSubject, orphanedEmailBody, fullFileName);
            }
        }

        public ImportDefinision GetImportDefinisionFromFileName(string fullFileName)
        {
            foreach (ImportDefinision definition in GetImportDefinitionsFromDatabase())
            {
                if (fullFileName.Contains(definition.ImportPath))
                {
                    Regex mask = new Regex(
                        '^' +
                        definition.FileMask
                            .Replace(".", "[.]")
                            .Replace("*", ".*")
                            .Replace("?", ".")
                        + '$',
                        RegexOptions.IgnoreCase);
                    if (mask.IsMatch(fullFileName))
                    {
                        return definition;
                    }
                }
            }

            return null;
        }

        public IList<ImportDefinision> GetImportDefinitionsFromDatabase()
        {
            try
            {
                return _DataService.GetCustomerImports();
            }
            catch(SqlException sqlExe)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Fatal(sqlExe.Message);
            }
            return null;
        }

        public string GetMoveToDirecotry(string importPath, string importDirectory, string checkedDirectory)
        {
            return importPath.Replace(importDirectory, checkedDirectory);
        }
    }
}