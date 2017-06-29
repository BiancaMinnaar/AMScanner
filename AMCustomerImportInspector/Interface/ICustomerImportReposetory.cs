using AMCustomerImportInspector.Model;
using System.Collections.Generic;

namespace AMCustomerImportInspector.Interface
{
    public interface ICustomerImportReposetory
    {
        IList<ImportDefinision> GetImportDefinitionsFromDatabase();
        ImportDefinision GetImportDefinisionFromFileName(string fullFileName);
        void EmailFaultyFile(string fullFileName, ImportDefinision definition, string[] errorList);
        void EMailOrphenedFileToSupport(string fullFileName, string[] emailAddressList);
        string GetMoveToDirecotry(string importPath, string importDirectory, string checkedDirectory);
    }
}
