using FileUtilityLibrary.Interface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Excel;
using System.Data;
using System.Data.OleDb;

namespace FileUtilityLibrary.Model.ScannerFile
{
    public class ExcelScannerFile : IScannerFile
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public char Delimiter { get; set; }
        public bool HasHeader { get; set; }
        public bool HasException { get; set; }
        public IList<string> ExceptionList { get; set; }
        public bool HasSubStructures()
        {
            _CurrentStructureNumber++;
            bool hasSubStructures = false;
            if (_ResultDataSet != null)
            {
                hasSubStructures = _ResultDataSet.Tables.Count > 1;
            }
            _StreamReader = new StreamReader(_StreamForSheets[_CurrentStructureNumber]);
            return hasSubStructures;
        }
        private int _CurrentStructureNumber;
        private StreamReader _StreamReader;
        private DataSet _ResultDataSet;
        private MemoryStream[] _StreamForSheets;

        public ExcelScannerFile(string fileName, string filePath, char delimiter, bool hasHeader)
        {
            FileName = fileName;
            FilePath = filePath;
            Delimiter = delimiter;
            HasHeader = hasHeader;
            var fullFileName = FilePath + @"\" + FileName;
            ExceptionList = new List<string>();
            //TODO: convert the file in memory to CSV before creating the stream
            _StreamForSheets = getExcelStreamsForSheets(fullFileName);
            _CurrentStructureNumber = -1;
        }

        private string GetConnectionString(string fileName)
        {
            Dictionary<string, string> props = new Dictionary<string, string>();

            //// XLSX - Excel 2007, 2010, 2012, 2013
            props["Provider"] = "Microsoft.ACE.OLEDB.12.0";
            props["Extended Properties"] = "\"Excel 12.0 xml;HDR=YES\"";
            //props["Data Source"] = fileName;

            // XLS - Excel 2003 and Older
            //props["Provider"] = "Microsoft.Jet.OLEDB.4.0";
            //props["Extended Properties"] = "\"Excel 8.0;HDR=NO;IMEX=1\"";
            props["Data Source"] = fileName;

            StringBuilder sb = new StringBuilder();

            foreach (KeyValuePair<string, string> prop in props)
            {
                sb.Append(prop.Key);
                sb.Append('=');
                sb.Append(prop.Value);
                sb.Append(';');
            }

            return sb.ToString();
        }

        //TODO:Test
        private MemoryStream[] getExcelStreamsForSheets(string fullFileName)
        {
            var connStr = GetConnectionString(fullFileName);
            OleDbConnection conn = new OleDbConnection(connStr);
            conn.Open();
            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            DataSet _ResultDataSet = new DataSet();
            var dtSheet = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            foreach (DataRow dr in dtSheet.Rows)
            {
                string sheetName = dr["TABLE_NAME"].ToString();
                if (!sheetName.EndsWith("$"))
                    continue;
                // Get all rows from the Sheet
                cmd.CommandText = "SELECT * FROM [" + sheetName + "]";
                DataTable dt = new DataTable();
                dt.TableName = sheetName;
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                da.Fill(dt);
                _ResultDataSet.Tables.Add(dt);
            }

            cmd = null;
            conn.Close();



            //_ResultDataSet = getExcelDataSet(fullFileName);
            StringBuilder sbRowData = new StringBuilder();
            MemoryStream[] csvStreamsToScan = new MemoryStream[_ResultDataSet.Tables.Count];

            for (int tableCount = 0; tableCount < _ResultDataSet.Tables.Count; tableCount++)
            {
                MemoryStream csvStream = new MemoryStream();
                StreamWriter csvStreamWriter = new StreamWriter(csvStream);
                for (int i = 0; i < _ResultDataSet.Tables[tableCount].Rows.Count; i++)
                {
                    for (int j = 0; j < _ResultDataSet.Tables[tableCount].Columns.Count; j++)
                    {
                        sbRowData.Append("|" + _ResultDataSet.Tables[tableCount].Rows[i].ItemArray[j]);
                    }
                    sbRowData.Remove(0, 1);
                    csvStreamWriter.WriteLine(sbRowData);
                }
                csvStream.Flush();
                csvStream.Position = 0;
                csvStreamsToScan[tableCount] = csvStream;
            }

            return csvStreamsToScan;
        }

        //TODO:Test
        private DataSet getExcelDataSet(string fullFileName)
        {
            FileStream stream = File.Open(fullFileName, FileMode.Open, FileAccess.Read);
            IExcelDataReader excelReader = null;
            var fileNameArray = fullFileName.Split('.');
            if (fileNameArray.Last() == "xls" || fileNameArray.Last() == "xlsb")
            {
                excelReader = ExcelReaderFactory.CreateBinaryReader(stream, false, ReadOption.Loose);
            }
            else
            {
                excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            }

            
            excelReader.IsFirstRowAsColumnNames = HasHeader;
            var result = excelReader.AsDataSet();
            return result;
        }

        public void Close()
        {
            _StreamReader.Close();
        }

        public FileInfo GetFileInfo()
        {
            return new FileInfo(FilePath + @"\" + FileName);
        }

        public int Peek()
        {
            return _StreamReader.Peek();
        }

        public int Read()
        {
            return _StreamReader.Read();
        }

        public string ReadLine()
        {
            return _StreamReader.ReadLine();
        }
    }
}
