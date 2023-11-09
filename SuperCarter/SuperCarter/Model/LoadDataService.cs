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
using System.Collections;
using CsvHelper.Configuration;

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
                    var FOLDER_RESULT = System.Windows.Forms.Application.StartupPath + @"\result\Dynamic";
                    defaultpath = string.Format("{0}\\{1}_{2}", FOLDER_RESULT, DateTime.Now.ToString("yyyyMMddHHmmss"), "_dataOutput.csv");

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
                        //csv.WriteHeader<UnifiedHostCommandSettypeBeta>();
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
        public class csvVerticaltype {

            [Index(0)]
            public string tital { get; set; }
            [Index(1)]
            public string msg { get; set; }
        }
        public void AppendToCsv2(UnifiedHostCommandSettype data)
        {
            try
            {
                // Now, append the record
                var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false };

                using (var stream = File.Open(defaultpath, FileMode.Append))
                using (var writer = new StreamWriter(stream))
                using (var csv = new CsvWriter(writer, config))
                {
                    string DUT1cur = null, DUT2cur = null, DUT3cur = null;
                    while (data.DUT1CurrentList.TryDequeue(out string strdata))
                    {
                        DUT1cur += strdata + ", ";
                    }

                    while (data.DUT2CurrentList.TryDequeue(out string strdata))
                    {
                        DUT2cur += strdata + ", ";
                    }
                    while (data.DUT3CurrentList.TryDequeue(out string strdata))
                    {
                        DUT3cur += strdata + ", ";
                    }
                    var writeRecords1 = new List<csvVerticaltype>
                    {
                        // for DUT1
                        new csvVerticaltype { tital = "#1 SWversion", msg = data.DUT1SWversion },
                        new csvVerticaltype { tital = "#1 HWversion", msg = data.DUT1HWversion },
                        new csvVerticaltype { tital = "#1 PowerMode", msg = data.DUT2PowerMode },
                        new csvVerticaltype { tital = "#1 InputVoltage", msg = data.DUT1Voltage },
                        new csvVerticaltype { tital = "#1 Current(A/uA)", msg = data.DUT1NormalCurrent },
                        new csvVerticaltype { tital = "#1 Current(uA)", msg = data.DUT1SleepCurrent },
                        new csvVerticaltype { tital = "#1 Diagnostic", msg = data.DUT1Diagnostic },
                        new csvVerticaltype { tital = "#1 Lightsensor", msg = data.DUT1Lightsensor },
                        new csvVerticaltype { tital = "#1 Touch_finger", msg = data.DUT1Touchfinger },
                        new csvVerticaltype { tital = "#1 Touch_XY", msg = data.DUT1Touch_XY },
                        new csvVerticaltype { tital = "#1 Brightness(%)", msg = data.DUT1Brightness },
                        new csvVerticaltype { tital = "#1 T_chamber", msg = data.DUT1T_chamber },
                        new csvVerticaltype { tital = "#1 T_LED1, msg = 2PCB", msg = data.DUT1T_LED1_2PCB },
                        new csvVerticaltype { tital = "#1 Diagnostic_raw", msg = data.DUT1Diagnostic_raw },
                        new csvVerticaltype { tital = "#1 CurrentList", msg = DUT1cur },
                    };
                        var writeRecords2 = new List<csvVerticaltype>
                    {
                        // for DUT2
                        new csvVerticaltype { tital = "#2 SWversion", msg = data.DUT2SWversion },
                        new csvVerticaltype { tital = "#2 HWversion", msg = data.DUT2HWversion },
                        new csvVerticaltype { tital = "#2 PowerMode", msg = data.DUT2PowerMode },
                        new csvVerticaltype { tital = "#2 InputVoltage", msg = data.DUT2Voltage },
                        new csvVerticaltype { tital = "#2 Current(A/uA)", msg = data.DUT2NormalCurrent },
                        new csvVerticaltype { tital = "#2 Current(uA)", msg = data.DUT2SleepCurrent },
                        new csvVerticaltype { tital = "#2 Diagnostic", msg = data.DUT2Diagnostic },
                        new csvVerticaltype { tital = "#2 Lightsensor", msg = data.DUT2Lightsensor },
                        new csvVerticaltype { tital = "#2 Touch_finger", msg = data.DUT2Touchfinger },
                        new csvVerticaltype { tital = "#2 Touch_XY", msg = data.DUT2Touch_XY },
                        new csvVerticaltype { tital = "#2 Brightness(%)", msg = data.DUT2Brightness },
                        new csvVerticaltype { tital = "#2 T_chamber", msg = data.DUT2T_chamber },
                        new csvVerticaltype { tital = "#2 T_LED1, msg = 2PCB", msg = data.DUT2T_LED1_2PCB },
                        new csvVerticaltype { tital = "#2 Diagnostic_raw", msg = data.DUT2Diagnostic_raw },
                        new csvVerticaltype { tital = "#2 CurrentList", msg = DUT2cur },
                    };
                        var writeRecords3 = new List<csvVerticaltype>
                    {
                        new csvVerticaltype { tital = "#3 SWversion", msg = data.DUT3SWversion },
                        new csvVerticaltype { tital = "#3 HWversion", msg = data.DUT3HWversion },
                        new csvVerticaltype { tital = "#3 PowerMode", msg = data.DUT3PowerMode },
                        new csvVerticaltype { tital = "#3 InputVoltage", msg = data.DUT3Voltage },
                        new csvVerticaltype { tital = "#3 Current(A/uA)", msg = data.DUT3NormalCurrent },
                        new csvVerticaltype { tital = "#3 Current(uA)", msg = data.DUT3SleepCurrent },
                        new csvVerticaltype { tital = "#3 Diagnostic", msg = data.DUT3Diagnostic },
                        new csvVerticaltype { tital = "#3 Lightsensor", msg = data.DUT3Lightsensor },
                        new csvVerticaltype { tital = "#3 Touch_finger", msg = data.DUT3Touchfinger },
                        new csvVerticaltype { tital = "#3 Touch_XY", msg = data.DUT3Touch_XY },
                        new csvVerticaltype { tital = "#3 Brightness(%)", msg = data.DUT3Brightness },
                        new csvVerticaltype { tital = "#3 T_chamber", msg = data.DUT3T_chamber },
                        new csvVerticaltype { tital = "#3 T_LED1, msg = 2PCB", msg = data.DUT3T_LED1_2PCB },
                        new csvVerticaltype { tital = "#3 Diagnostic_raw", msg = data.DUT3Diagnostic_raw },
                        new csvVerticaltype { tital = "#3 CurrentList", msg = DUT3cur },
                    };

                    csv.WriteRecords(writeRecords1);
                    writer.WriteLine();
                    csv.WriteRecords(writeRecords2);
                    writer.WriteLine();
                    csv.WriteRecords(writeRecords3);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.StackTrace);
                MessageBox.Show(ex.Message);
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
