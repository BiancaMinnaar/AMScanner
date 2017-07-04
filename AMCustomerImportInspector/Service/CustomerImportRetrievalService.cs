using AMCustomerImportInspector.DataAccess;
using AMCustomerImportInspector.Interface;
using AMCustomerImportInspector.Model;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AMCustomerImportInspector.Service
{
    public class CustomerImportRetrievalService : ICustomerImportRetrievalService
    {
        private ILog _LogHandler;

        public CustomerImportRetrievalService(ILog logHandler)
        {
            _LogHandler = logHandler;
        }

        public IList<ImportDefinision> GetCustomerImports()
        {
            try
            {
                CLIENTLISTEntities ent = new CLIENTLISTEntities();
                var importDefinisions = ent.IMPEX_CONFIGURATIONS.Select(x => new ImportDefinision
                {
                    ID = x.id,
                    ClientDatabase = x.DatabaseName,
                    Delimiter = x.Delimeter,
                    FailureEmailList = x.Failure_Email_Addresses,
                    ImportFormat = x.Data_Format,
                    ImportName = x.Name,
                    ImportPath = x.vfs_path,
                    HasHeader = x.Data_Has_Header ?? x.Data_Has_Header.Value,
                    IsEnabled = x.Enabled ?? x.Enabled.Value
                }) .ToList();

                return importDefinisions;
            }
            catch(Exception excp)
            {
                _LogHandler.Fatal(excp.Message);
            }
            return null;
        }
    }
}
