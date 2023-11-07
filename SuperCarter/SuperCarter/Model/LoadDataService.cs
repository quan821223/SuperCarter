using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CsvHelper;

namespace SuperCarter.Model
{
    public static class LoadDataService<T>
    {

        public static List<T> ReadFile(string _path)
        {

            if (System.IO.File.Exists(_path))
            {
                string jsonString = System.IO.File.ReadAllText(_path);
                List<T> userData = JsonConvert.DeserializeObject<List<T>>(jsonString);
                return userData;
            }
            else
            {
                MessageBox.Show("Cannt find " + _path);
                return new List<T>() { };
            }

        }

        public static void Writefile(string _path)
        {
            using (System.IO.StreamWriter filestream = new System.IO.StreamWriter(_path))
            {
                List<T> lstStuModel = new List<T>();

                string JSONStreamdata = JsonConvert.SerializeObject(lstStuModel, Newtonsoft.Json.Formatting.Indented);
                filestream.Write(JSONStreamdata);

            }
        }  
    }
    public class CSVfile
    {
        private static object _lock = new object();
        private string _path;

        public CSVfile(string path)
        {
            _path = path;

            //// Initialize CSV if it does not exist
            //if (!File.Exists(_path))
            //{
            //    using (StreamWriter sw = new StreamWriter(_path, true))
            //    {
            //        sw.WriteLine("Time,Port,Type,Data"); // You can adjust these headers based on your requirements
            //    }
            //}
            // Initialize CSV if it does not exist
            if (!File.Exists(_path))
            {
                using (var stream = File.Create(_path))
                using (var writer = new StreamWriter(stream))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteHeader<UnifiedHostCommandSettype>();
                    csv.NextRecord();
                }
            }
        }
        public void AppendToCsv(UnifiedHostCommandSettype data, string functionName)
        {
            if (!File.Exists(_path))
            {
                // If the file does not exist, create it and write the header
                using (var stream = File.Create(_path))
                using (var writer = new StreamWriter(stream))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    var headerRecord = data.ToCsvRecord(functionName);
                    csv.WriteRecords(headerRecord);
                    writer.WriteLine();
                }
            }

            // Now, append the record
            using (var stream = File.Open(_path, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                var record = data.ToCsvRecord(functionName);
                csv.WriteRecords(record);
                writer.WriteLine(); // Write new line
            }
        }
    }
}
