using AMCustomerImportInspector.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCustomerImportInspector.Interface
{
    public interface ICustomerImportRetrievalService
    {
        IList<ImportDefinision> GetCustomerImports();
    }
}
