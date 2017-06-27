using AMCustomerImportInspector.Model;
using System.Collections.Generic;

namespace AMCustomerImportInspector.Interface
{
    public interface ICustomerImportReposetory
    {
        IList<ImportDefinision> GetImportDefinitionsFromDatabase();
        ImportDefinision GetImportDefinisionFromFileName(string fullFileName, IList<ImportDefinision> definisionList);
        void EmailFaultyFile(string fullFileName, ImportDefinision definition);
        string GetMoveToDirecotry(string importPath);
    }
}
