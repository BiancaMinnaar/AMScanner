using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCustomerImportInspector.Model
{
    public class ImportDefinisionTestFrame
    {
        public int ID { get; set; }
        public string ClientName {get;set;}
        public string[] DirectoryParts { get; set; }
        public int Probability { get; set; }
        public string FileMask { get; set; }
        public bool ReachedRootFolder { get; set; }
    }
}
