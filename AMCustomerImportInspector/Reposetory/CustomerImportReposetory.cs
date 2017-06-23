using AMCustomerImportInspector.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCustomerImportInspector.Model;

namespace AMCustomerImportInspector.Reposetory
{
    public class CustomerImportReposetory : ICustomerImportReposetory
    {
        private ICustomerImportRetrievalService _DataService;

        public CustomerImportReposetory(ICustomerImportRetrievalService dataService)
        {
            _DataService = dataService;
        }

        public IList<ImportDefinision> GetImportDefinitionsFromDatabase()
        {
            return _DataService.GetCustomerImports();
        }
    }
}