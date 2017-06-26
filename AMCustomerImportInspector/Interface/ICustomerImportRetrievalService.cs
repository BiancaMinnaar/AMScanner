using AMCustomerImportInspector.Model;
using System.Collections.Generic;

namespace AMCustomerImportInspector.Interface
{
    public interface ICustomerImportRetrievalService
    {
        IList<ImportDefinision> GetCustomerImports();
    }
}
