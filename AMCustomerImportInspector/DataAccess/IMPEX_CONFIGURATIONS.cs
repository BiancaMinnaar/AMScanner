//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AMCustomerImportInspector.DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class IMPEX_CONFIGURATIONS
    {
        public int id { get; set; }
        public string DatabaseName { get; set; }
        public string Name { get; set; }
        public string vfs_path { get; set; }
        public string Data_Format { get; set; }
        public string Delimeter { get; set; }
        public Nullable<bool> Data_Has_Header { get; set; }
        public string Failure_Email_Addresses { get; set; }
        public string Approved_Folder { get; set; }
        public Nullable<bool> Enabled { get; set; }
        public string Cron_Trigger { get; set; }
    }
}
