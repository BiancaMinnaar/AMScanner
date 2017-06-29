namespace AMCustomerImportInspector.Interface
{
    public interface IEMailTemplateService
    {
        string GetSubjectText(string templateFileName, string xpathToPreErrorText);
        string GetBodyPreReplacementText(string templateFileName, string xpathToPreReplacementText);
        string GetWholeEmailBody(string templateFileName, string xpathToPreReplacementText, string replaceTagName, string replacementValue);
        string GetWholeEmailBodyWithErrors(string templateFileName, string xpathToPreErrorText, string errorListTagName, string[] errorList);
    }
}
