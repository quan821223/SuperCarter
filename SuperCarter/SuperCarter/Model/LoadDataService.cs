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
using SuperCarter.Services;
using System.Windows.Markup;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using CsvHelper.Configuration.Attributes;
using System.Reflection;

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
        //private static readonly CSVfile _instance = new CSVfile();
       // public static CSVfile Instance => _instance;
        private string defaultpath { get; set; }

        public CSVfile(string _path = null)
        {

            try
            {

                if (string.IsNullOrEmpty(_path))
                {
                    var FOLDER_RESULT = System.Windows.Forms.Application.StartupPath + @"result\";
                    defaultpath = string.Format("{0}\\{1}_{2}", FOLDER_RESULT, DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"), "-outputtestdata.csv");

                }
                else
                {
                    defaultpath = ConfigModel.Instance.GetStrScriptpath();
                }


                if (!File.Exists(defaultpath))
                {
                    using (var stream = File.Create(defaultpath))
                    using (var writer = new StreamWriter(stream))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.WriteHeader<UnifiedHostCommandSettypeBeta>();
                        //csv.NextRecord();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }

            //// Initialize CSV if it does not exist
            //if (!File.Exists(_path))
            //{
            //    using (StreamWriter sw = new StreamWriter(_path, true))
            //    {
            //        sw.WriteLine("Time,Port,Type,Data"); // You can adjust these headers based on your requirements
            //    }
            //}
            // Initialize CSV if it does not exist

        }
        public void SetCSVfileStoragepath(string _path)
        {
            defaultpath = _path;
        }
        public void AppendToCsv2(UnifiedHostCommandSettypeBeta data, string functionName)
        {

            // 根据传入的 functionName 创建不同功能的对象
            UnifiedHostCommandSettypeBeta dataA = null;



            dataA = new UnifiedHostCommandSettypeBeta
            {
                // 设置 FunctionA 需要的属性值
            };

            // 配置 FunctionA 的属性选择器
            data.Selector = new FunctionSelector
            {
                IncludeDUT1SWversion = false,
                IncludeLoop = false,
                IncludeBlockloop = false,
                IncludeBlockphase = false
            };

         
     

            //if (!File.Exists(defaultpath))
            //{
            //    // If the file does not exist, create it and write the header
            //    using (var stream = File.Create(defaultpath))
            //    using (var writer = new StreamWriter(stream))
            //    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            //    {
            //        csv.WriteHeader(data.GetType()); // Use the specific type for the header
            //        writer.WriteLine();
            //    }
            //}

            // Now, append the record
            using (var stream = File.Open(defaultpath, FileMode.Append))
            using (var writer = new StreamWriter(stream))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                //csv.WriteRecord(data);
                //writer.WriteLine(); // Write new line
                var properties = data.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var customAttribute = property.GetCustomAttribute<NameAttribute>();
                    var propertyName = customAttribute?.Names?.FirstOrDefault(name => !string.IsNullOrEmpty(name)) ?? property.Name;

                    var propertyValue = property.GetValue(data)?.ToString() ?? string.Empty;
                    writer.WriteLine($"{propertyName}, {propertyValue}");
                }
            }
        }

        public void AppendToCsv(UnifiedHostCommandSettype data, string functionName)
        {

      
          
            //if (!File.Exists(defaultpath))
            //{
            //    using (var stream = File.Create(defaultpath))
            //    using (var writer = new StreamWriter(stream))
            //    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            //    {
            //        csv.WriteHeader<UnifiedHostCommandSettype>();
            //        csv.NextRecord();
            //    }
            //}



            ////if (!File.Exists(_path))
            ////{
            ////    // If the file does not exist, create it and write the header
            ////    using (var stream = File.Create(_path))
            ////    using (var writer = new StreamWriter(stream))
            ////    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            ////    {
            ////        var headerRecord = data.ToCsvRecord(functionName);
            ////        csv.WriteRecords(headerRecord);
            ////        writer.WriteLine();
            ////    }
            ////}

            //// Now, append the record
            //using (var stream = File.Open(_path, FileMode.Append))
            //using (var writer = new StreamWriter(stream))
            //using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            //{
            //    var record = data.ToCsvRecord("Function2");
            //    csv.WriteRecords(record);
            //    writer.WriteLine(); // Write new line
            //}
        }
    }
}
