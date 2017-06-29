using AMCustomerImportInspector.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AMCustomerImportInspector.Service
{
    public class EMailTemplateService : IEMailTemplateService
    {
        public XmlDocument EMailTemplateFile { get; }

        public EMailTemplateService()
        {
            EMailTemplateFile = new XmlDocument();
        }

        private string getResourceString(string templateFileName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(templateFileName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return  reader.ReadToEnd();
            }

        }
        public string GetBodyPreReplacementText(string templateFileName, string xpathToPreErrorText)
        {
            EMailTemplateFile.LoadXml(getResourceString(templateFileName));
            return EMailTemplateFile.SelectSingleNode(xpathToPreErrorText).InnerXml;
        }

        public string GetSubjectText(string templateFileName, string xpathToPreErrorText)
        {
            var xmlText = getResourceString(templateFileName);
            EMailTemplateFile.LoadXml(xmlText);
            return EMailTemplateFile.SelectSingleNode(xpathToPreErrorText).InnerXml;
        }

        public string GetWholeEmailBody(
            string templateFileName, string xpathToPreReplacementText, string replaceTagName, string replacementValue)
        {
            var bodyString = GetBodyPreReplacementText(templateFileName, xpathToPreReplacementText);
            var fullBodySting = bodyString.Replace(replaceTagName, replacementValue);

            return fullBodySting;
        }

        public string GetWholeEmailBodyWithErrors(
            string templateFileName, string xpathToPreErrorText, string errorListTagName, string[] errorList)
        {
            StringBuilder bodyBuilder = new StringBuilder();
            foreach(string error in errorList)
            {
                bodyBuilder.AppendLine(error);
            }
            var bodyString = GetBodyPreReplacementText(templateFileName, xpathToPreErrorText);
            var fullBodySting = bodyString.Replace(errorListTagName, bodyBuilder.ToString());

            return fullBodySting;
        }
    }
}
