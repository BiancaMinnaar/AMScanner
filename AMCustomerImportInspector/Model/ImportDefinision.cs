using System.Text.RegularExpressions;

namespace AMCustomerImportInspector.Model
{
    public class ImportDefinision
    {
        public string ImportName { get; set; }
        public string ImportPath { get; set; }
        public string FileMask
        {
            get
            {
                switch (ImportFormat)
                {
                    case "EXCEL":
                        return "*.xls|*.xlsx|*.xlsb";
                    case "CSV":
                        return "*.txt|*.csv";
                    default:
                        return "*.*";
                }
            }
        }
        public string ImportFormat { get; set; }
        public bool HasHeader { get; set; }
        public string Delimiter { get; set; }
        public string[] FailureEmailAddresses { get; set; }
    }
}
