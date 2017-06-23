using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCustomerImportInspector.Model
{
    public class ImportDefinision
    {
        public string ImportName { get; set; }
        public string ImportPath { get; set; }
        public string FileMask { get; set; }
        public string ImportFormat { get; set; }
        public string Delimiter { get; set; }
        public string[] FailureEmailAddresses { get; set; }
    }
}
