using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SuperCarter.Services;
namespace SuperCarter.Model
{
    public class PortCandidatelist
    {
        [JsonProperty("PortID")]
        public int PortID { get; set; }
        [JsonProperty("PortName")]
        public string PortName { get; set; }
        [JsonProperty("BaudRateValue")]
        public int BaudRateValue { get; set; }
        [JsonProperty("Reccase")]
        public int Reccase { get; set; }
    }
    public class ConfigbyJSON : SingletonBase<ConfigbyJSON>
    {

        public readonly string FOLDER_config = System.Windows.Forms.Application.StartupPath + @"\config\";

        public void WriteMultiScheduledscript()
        {
            try 
            { 

            }
            catch(Exception ex) 
            {
                System.Windows.MessageBox.Show(ex.Message);
                System.Windows.MessageBox.Show(ex.StackTrace);
            }
        }
        /// <summary>
        /// 寫入 COM 資訊
        /// </summary>
        /// <param name="_SerialCandidator"></param>
        public void WritePortCandidatelist(ObservableCollection<Portdetectedtype> _SerialCandidator)
        {

            try
            {
                string JSONfilePath = FOLDER_config + "PortCandidatelist.json"; // 路徑為你的JSON文件的實際路徑
                if (_SerialCandidator.Count > 0)
                {
                    if (System.IO.File.Exists(JSONfilePath))
                    {
                        System.IO.File.Delete(JSONfilePath);
                    }
                    using (System.IO.StreamWriter filestream = new System.IO.StreamWriter(JSONfilePath))
                    {
                        List<PortCandidatelist> lstStuModel = new List<PortCandidatelist>();
                        foreach (var item in _SerialCandidator)
                        {
                            lstStuModel.Add(new PortCandidatelist()
                            {
                                PortID = item.PortID,
                                PortName = item.PortName,
                                BaudRateValue = item.BaudRateValue,
                                Reccase = item.DataReceivedCasenum
                            });
                        }

                        string JSONStreamdata = JsonConvert.SerializeObject(lstStuModel, Newtonsoft.Json.Formatting.Indented);
                        filestream.Write(JSONStreamdata);

                    }
                }
                else
                {
                    LoadDataService<PortCandidatelist>.Writefile(JSONfilePath);
                }
                nlogMessageAggregator.Instance.SendMessage(new nlogtype { LogLevel = NLog.LogLevel.Debug, Msg = JSONfilePath });
                nlogMessageAggregator.Instance.SendMessage(new nlogtype { LogLevel = NLog.LogLevel.Debug, Msg = "WritePortCandidatelist" });

            }
            catch (Exception ex)
            {
                nlogMessageAggregator.Instance.SendMessage(new nlogtype { LogLevel = NLog.LogLevel.Debug, Msg = "WritePortCandidatelistErr" });
                nlogMessageAggregator.Instance.SendMessage(new nlogtype { LogLevel = NLog.LogLevel.Debug, Msg = ex.StackTrace });
                throw;
            }
        }
        /// <summary>
        /// 讀取COM紀錄
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Portdetectedtype> ReadPortCandidatelist()
        {

            try
            {
                string JSONfilePath = FOLDER_config + "PortCandidatelist.json"; // 路徑為你的JSON文件的實際路徑

                if (System.IO.File.Exists(JSONfilePath))
                {
                    //System.IO.FileStream fs = new System.IO.FileStream (JSONfilePath, System.IO.FileAccess)

                    List<PortCandidatelist> dataset = LoadDataService<PortCandidatelist>.ReadFile(JSONfilePath);
                    var list = new ObservableCollection<Portdetectedtype>();

                    foreach (var item in dataset)
                    {
                        list.Add(new Portdetectedtype()
                        {
                            PortID = item.PortID,
                            PortName = item.PortName,
                            BaudRateValue = item.BaudRateValue,
                            DataReceivedCasenum = item.Reccase

                        });
                    }
                 
                    nlogMessageAggregator.Instance.SendMessage(new nlogtype { LogLevel = NLog.LogLevel.Debug, Msg = JSONfilePath });
                    nlogMessageAggregator.Instance.SendMessage(new nlogtype { LogLevel = NLog.LogLevel.Debug, Msg = "ReadPortCandidatelist" });
                    return list;

                }
                else
                {
                    LoadDataService<PortCandidatelist>.Writefile(JSONfilePath);
                    return new ObservableCollection<Portdetectedtype>() { };
                }

            }
            catch (Exception ex)
            {
                nlogMessageAggregator.Instance.SendMessage(new nlogtype { LogLevel = NLog.LogLevel.Debug, Msg = "ReadPortCandidatelist" });
                nlogMessageAggregator.Instance.SendMessage(new nlogtype { LogLevel = NLog.LogLevel.Debug, Msg = ex.StackTrace });
          
                return new ObservableCollection<Portdetectedtype>() { };
                throw;
            }

        }

        public void WriteJSONfile()
        {
            string JSONfilePath = FOLDER_config + "SMSconfig.json"; // 路徑為你的JSON文件的實際路徑
            if (System.IO.File.Exists(JSONfilePath))
            {
            }
            using (System.IO.StreamWriter filestream = new System.IO.StreamWriter(JSONfilePath))
            {
                var lists = new SMSSystemGroup[]
                {
                new SMSSystemGroup(){

                    IntrospectIntervalTimer = 5000,
                    BaudRates = new List<int>() { 9600, 115200 },
                    DataReceivedCasenum = new List<int>() { 0, 1 },
                    AutoDetectPortdevices = true,
                    DataReceivedCase = 0
                }
                };
                string JSONStreamdata = JsonConvert.SerializeObject(lists, Newtonsoft.Json.Formatting.Indented);
                filestream.Write(JSONStreamdata);

            }

        }
        public void ReadJSONfile()
        {
            try
            {
                string JSONfilePath = FOLDER_config + "SMSconfig.json"; // 路徑為你的JSON文件的實際路徑

                // check the system file if the json exits in local path or not.
                if (!System.IO.File.Exists(JSONfilePath)) WriteJSONfile();
                string jsonString = System.IO.File.ReadAllText(JSONfilePath);

                //轉成物件
                //SMSSystemGroup JSONreader = JsonConvert.DeserializeObject<SMSSystemGroup>(JSONfilePath);
                //DataReceivedCase = JSONreader.DataReceivedCase;
                //AutoDetectPortdevices = JSONreader.AutoDetectPortdevices;
                //IntrospectIntervalTimer = JSONreader.IntrospectIntervalTimer;
                //BaudRates = JSONreader.BaudRates;
                //DataReceivedCasenum = JSONreader.DataReceivedCasenum;

                List<SMSSystemGroup> JSONreader = JsonConvert.DeserializeObject<List<SMSSystemGroup>>(jsonString);
                IntrospectIntervalTimer = JSONreader[0].IntrospectIntervalTimer;
                DataReceivedCase = JSONreader[0].DataReceivedCase;
                DataReceivedCasenum = JSONreader[0].BaudRates;
                AutoDetectPortdevices = JSONreader[0].AutoDetectPortdevices;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        public List<int> DataReceivedCasenum { get; set; } = new List<int>() { 0, 1 };
        public List<int> BaudRates { get; set; } = new List<int>() { 9600, 115200 };
        public int IntrospectIntervalTimer { get; set; } = 5000;
        public bool AutoDetectPortdevices { get; set; } = true;
        public int DataReceivedCase { get; set; } = 0;

    }
    public class SMSSystemGroup
    {
        [JsonProperty("IntrospectIntervalTimer")]
        public int IntrospectIntervalTimer { get; set; } = 5000;
        [JsonProperty("BaudRates")]
        public List<int> BaudRates { get; set; } = new List<int>() { 9600, 115200 };
        [JsonProperty("DataReceivedCasenum")]
        public List<int> DataReceivedCasenum { get; set; } = new List<int>() { 0, 1 };
        [JsonProperty("AutoDetectPortdevices")]
        public bool AutoDetectPortdevices { get; set; } = true;
        [JsonProperty("DataReceivedCase")]
        public int DataReceivedCase { get; set; } = 0;

    }
}
