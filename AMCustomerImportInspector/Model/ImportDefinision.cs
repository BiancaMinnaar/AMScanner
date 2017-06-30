using System.Text.RegularExpressions;

namespace AMCustomerImportInspector.Model
{
    public class ImportDefinision
    {
        public string ClientDatabase { get; set; }
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
        private string failureEmailList;
        public string FailureEmailList
        {
            set
            {
                failureEmailList = value;
            }
        }
        public string[] FailureEmailAddresses
        {
            get { return failureEmailList.Split(';'); }
        }
        public bool IsEnabled { get; set; }
    }
}
