﻿using AMCustomerImportInspector.DataAccess;
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
                var blanketModel = new Blankets();
                var returnObj = blanketModel.GetCustomerImportParameters().Select(x => new ImportDefinision
                {
                    Delimiter = x.delimeter,
                    FailureEmailAddresses = x.failure_email_addresses.Split(';'),
                    ImportFormat = x.data_format,
                    ImportName = x.name,
                    ImportPath = x.vfs_path,
                    HasHeader = x.data_has_header
                }).ToList();

                return returnObj;
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
