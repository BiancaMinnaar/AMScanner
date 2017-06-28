﻿using AMCustomerImportInspector.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCustomerImportInspector.Model;
using System.Configuration;
using System.Text.RegularExpressions;

namespace AMCustomerImportInspector.Reposetory
{
    public class CustomerImportReposetory : ICustomerImportReposetory
    {
        private ICustomerImportRetrievalService _DataService;
        private IEmailService _EmailService;

        public CustomerImportReposetory(ICustomerImportRetrievalService dataService, IEmailService emailService)
        {
            _DataService = dataService;
            _EmailService = emailService;
        }

        public void EmailFaultyFile(string fullFileName, ImportDefinision definition)
        {
            var faultyEmailSubject = ConfigurationManager.AppSettings["FaultyEmailSubject"];
            var faultyEmailBody = ConfigurationManager.AppSettings["FaultyEmailBody"];
            _EmailService.SendEmailToRecipient(definition.FailureEmailAddresses[0], faultyEmailSubject, faultyEmailBody, fullFileName);
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
            return _DataService.GetCustomerImports();
        }

        public string GetMoveToDirecotry(string importPath, string importDirectory, string checkedDirectory)
        {
            return importPath.Replace(importDirectory, checkedDirectory);
        }
    }
}