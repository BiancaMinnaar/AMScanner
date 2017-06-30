using AMCustomerImportInspector.DataAccess;
using AMCustomerImportInspector.Interface;
using AMCustomerImportInspector.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCustomerImportInspector.Service
{
    public class CustomerImportRetrievalService : ICustomerImportRetrievalService
    {
         
        public IList<ImportDefinision> GetCustomerImports()
        {
            try
            {
                CLIENTLISTEntities ent = new CLIENTLISTEntities();
                var importDefinisions = ent.IMPEX_CONFIGURATIONS.Select(x => new ImportDefinision
                {
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
                log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                log.Fatal(excp.Message);
            }
            return null;
        }
    }
}
