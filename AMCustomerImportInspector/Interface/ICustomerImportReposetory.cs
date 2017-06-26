using AMCustomerImportInspector.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCustomerImportInspector.Interface
{
    public interface ICustomerImportReposetory
    {
        IList<ImportDefinision> GetImportDefinitionsFromDatabase();
        bool IsFileInImportDefinition(string fullFileName, IList<ImportDefinision> definisionList);
        void EmailFaultyFile(string fullFileName, ImportDefinision definition);
    }
}
