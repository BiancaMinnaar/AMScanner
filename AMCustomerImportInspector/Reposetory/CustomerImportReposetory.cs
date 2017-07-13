using AMCustomerImportInspector.Interface;
using System.Collections.Generic;
using AMCustomerImportInspector.Model;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Linq;
using log4net;

namespace AMCustomerImportInspector.Reposetory
{
    public class CustomerImportReposetory : ICustomerImportReposetory
    {
        private ICustomerImportRetrievalService _DataService;
        public IEmailService EmailService { get; set; }
        private IEMailTemplateService _EMailTemplateService;
        private ILog _LogHandler;
        private string _RootPath;

        public CustomerImportReposetory(string rootPath,
            ICustomerImportRetrievalService dataService, 
            IEmailService emailService, 
            IEMailTemplateService emailTemplateService,
            ILog logHandler)
        {
            _RootPath = rootPath;
            _DataService = dataService;
            EmailService = emailService;
            _EMailTemplateService = emailTemplateService;
            _LogHandler = logHandler;
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
                EmailService.SendEmailToRecipient(emailAddress, faultyEmailSubject, faultyEmailBody, fullFileName);
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
                EmailService.SendEmailToRecipient(
                    emailAddress, orphanedEmailSubject, orphanedEmailBody, fullFileName);
            }
        }

        private bool isNameInMask(string testMask, string fileName)
        {
            Regex mask = new Regex(
                        '^' +
                        testMask
                            .Replace(".", "[.]")
                            .Replace("*", ".*")
                            .Replace("?", ".")
                        + '$',
                        RegexOptions.IgnoreCase);
            return mask.IsMatch(fileName);
        }

        private ImportDefinision GetMostLightlyImportDefinision(string rootPath, string fullFileName, 
            IList<ImportDefinision> importDefinisionList)
        {
            string[] fileParts = fullFileName.Split('\\');
            var fileName = fileParts[fileParts.Length - 1].ToUpper();
            List<ImportDefinisionTestFrame> direcoryPartsList = new List<ImportDefinisionTestFrame>();
            foreach(ImportDefinision definision in importDefinisionList)
            {
                var testSet = new ImportDefinisionTestFrame()
                {
                    ClientName = definision.ClientDatabase,
                    DirectoryParts = definision.ImportPath.Split('\\'),
                    ID = definision.ID,
                    Probability = 0,
                    FileMask = definision.FileMask
                };
                direcoryPartsList.Add(testSet);

                var rootPathParts = _RootPath.Split('\\');
                var rootPathLastDirectory = rootPathParts[rootPathParts.Length - 1].ToUpper();
                if (isNameInMask(testSet.DirectoryParts[testSet.DirectoryParts.Length - 1].ToUpper(), fileName))
                {
                    var DirecotryCounter = testSet.DirectoryParts.Count() - 2;
                    for (int filePartsCounter = fileParts.Length - 2; filePartsCounter > 0; filePartsCounter--)
                    {
                        var pathPartToTest = fileParts[filePartsCounter].ToUpper();
                        var testDirectoryPart = testSet.DirectoryParts[DirecotryCounter].ToUpper();

                        if (pathPartToTest.Equals(testDirectoryPart))
                        {
                            testSet.Probability++;
                            if (DirecotryCounter > 0)
                                DirecotryCounter--;
                            testSet.ReachedRootFolder = pathPartToTest == rootPathLastDirectory;
                        }
                        else
                            break;
                    }
                }
                       
            }
            var maxPriority = direcoryPartsList.Max(p => p.Probability);
            _LogHandler.Debug("Max Priority :" + maxPriority);
            var setsWithMaxPriority = direcoryPartsList.Where(p => p.ReachedRootFolder && p.Probability == maxPriority);
            if (maxPriority == 0 || setsWithMaxPriority.Count() == 0)
            {
                return null;
            }
            if (setsWithMaxPriority.Count() > 1)
            {
                foreach(ImportDefinisionTestFrame frame in setsWithMaxPriority)
                {
                    if (isNameInMask(frame.FileMask, fileName))
                    {
                        return importDefinisionList.Where(p => p.ID == frame.ID).First();
                    }
                }
            }

            return importDefinisionList.Where(p => p.ID == setsWithMaxPriority.First().ID).First();
        }

        public ImportDefinision GetImportDefinisionFromFileName(string fullFileName)
        {
            var importList = GetImportDefinitionsFromDatabase();
            var importDef = GetMostLightlyImportDefinision(_RootPath, fullFileName, importList);
            if (importDef == null)
            {
                return null;
            }
            string[] fileParts = fullFileName.Split('\\');
            var fileName = fileParts[fileParts.Length - 1];
            if (isNameInMask(importDef.FileMask, fileName))
            {
                return importDef;
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
                _LogHandler.Fatal(sqlExe.Message);
            }
            return null;
        }

        public string GetMoveToDirecotry(string importPath, string importDirectory, string checkedDirectory)
        {
            return importPath.Replace(importDirectory, checkedDirectory);
        }
    }
}