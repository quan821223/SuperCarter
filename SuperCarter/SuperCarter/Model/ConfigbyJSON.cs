using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SuperCarter.Services;
using System.IO;
using static SuperCarter.Model.Monitoringmodescripttype;

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
    public class Monitoringmodescripttype {
        [JsonProperty("Loop")]
        public int Loop { get; set; }
        [JsonProperty("BlockALoop")]
        public int BlockALoop { get; set; }
        [JsonProperty("BlockBLoop")]
        public int BlockBLoop { get; set; }
        [JsonProperty("BlockA1Interval")]
        public int BlockA1Interval { get; set; }
        [JsonProperty("BlockA2Interval")]
        public int BlockA2Interval { get; set; }
        [JsonProperty("BlockB1Interval")]
        public int BlockB1Interval { get; set; }
        [JsonProperty("BlockB2Interval")]
        public int BlockB2Interval { get; set; }
        [JsonProperty("MonitoringIntervaltime")]
        public int MonitoringIntervaltime { get; set; }
        public IList<subscript> TestSuiteA1init { get; set; }
        public IList<subscript> TestSuiteA1 { get; set; }
        public IList<subscript> TestSuiteA2init { get; set; }
        public IList<subscript> TestSuiteA2 { get; set; }
        public IList<subscript> TestSuiteB1init { get; set; }
        public IList<subscript> TestSuiteB1 { get; set; }
        public IList<subscript> TestSuiteB2init { get; set; }
        public IList<subscript> TestSuiteB2 { get; set; }

        // there is ThresholdSetting parameters  
        public IList<ThresholdSettingtype> DetectDiag { get; set; }
        public IList<ThresholdSettingtype> NormCurrent { get; set; }
        public IList<ThresholdSettingtype> SleepCurrent { get; set; }
        public IList<ThresholdSettingtype> Lightsensor { get; set; }
        public IList<ThresholdSettingtype> Touchfinger { get; set; }
        public IList<ThresholdSettingtype> TouchXY { get; set; }
        public class subscript
        {
            public string ScriptPath { get; set; }
            public ObservableCollection<ScriptItemtype> Command { get; set; }
        }
        public class ThresholdSettingtype
        {
            public bool ISEnable { get; set; }
            public int Upper { get; set; }
            public int Lower { get; set; }
        }
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
                    if (System.IO.File.Exists(JSONfilePath))        // 查看檔案是否存在
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
        public void ReadMonitoringmodeScriptfromJson(ScheduledScriptEditor _ScriptEditor)
        {

            try
            {
                string JSONfilePath = _ScriptEditor.OpenedBlockScriptPath; // 路徑為你的JSON文件的實際路徑

                if (System.IO.File.Exists(JSONfilePath))
                {
                    List<Monitoringmodescripttype> dataset = LoadDataService<Monitoringmodescripttype>.ReadFile(JSONfilePath);
                    _ScriptEditor.Fullloop = dataset[0].Loop;
                    _ScriptEditor.BlockALoop = dataset[0].BlockALoop;
                    _ScriptEditor.BlockBLoop = dataset[0].BlockBLoop;
                    _ScriptEditor.BlockB1Interval = dataset[0].BlockB1Interval;
                    _ScriptEditor.BlockB2Interval = dataset[0].BlockB2Interval;
                    _ScriptEditor.BlockA2Interval = dataset[0].BlockA2Interval;
                    _ScriptEditor.BlockA1Interval = dataset[0].BlockA1Interval;
                    _ScriptEditor.MonitoringIntervaltime = dataset[0].MonitoringIntervaltime;
                    _ScriptEditor.MonitoringIntervaltime = dataset[0].MonitoringIntervaltime;
                    _ScriptEditor.BlockA1initscriptPath = dataset[0].TestSuiteA1init[0].ScriptPath;
                    _ScriptEditor.BlockA1initObsColSequences = dataset[0].TestSuiteA1init[0].Command;
                    _ScriptEditor.BlockA1scriptPath = dataset[0].TestSuiteA1[0].ScriptPath;
                    _ScriptEditor.BlockA1ObsColSequences = dataset[0].TestSuiteA1[0].Command;
                    _ScriptEditor.BlockA2initscriptPath = dataset[0].TestSuiteA2init[0].ScriptPath;
                    _ScriptEditor.BlockA2initObsColSequences = dataset[0].TestSuiteA2init[0].Command;
                    _ScriptEditor.BlockA2scriptPath = dataset[0].TestSuiteA2[0].ScriptPath;
                    _ScriptEditor.BlockA2ObsColSequences = dataset[0].TestSuiteA2[0].Command;
                    _ScriptEditor.BlockB1initscriptPath = dataset[0].TestSuiteB1init[0].ScriptPath;
                    _ScriptEditor.BlockB1initObsColSequences = dataset[0].TestSuiteB1init[0].Command;
                    _ScriptEditor.BlockB1scriptPath = dataset[0].TestSuiteB1[0].ScriptPath;
                    _ScriptEditor.BlockB1ObsColSequences = dataset[0].TestSuiteB1[0].Command;
                    _ScriptEditor.BlockB2initscriptPath = dataset[0].TestSuiteB2init[0].ScriptPath;
                    _ScriptEditor.BlockB2initObsColSequences = dataset[0].TestSuiteB2init[0].Command;
                    _ScriptEditor.BlockB2scriptPath = dataset[0].TestSuiteB2[0].ScriptPath;
                    _ScriptEditor.BlockB2ObsColSequences = dataset[0].TestSuiteB2[0].Command;

                    _ScriptEditor.IsEnableDetectDiag = dataset[0].DetectDiag[0].ISEnable;
                    _ScriptEditor.IsEnableDetectnormCurrent = dataset[0].NormCurrent[0].ISEnable;
                    _ScriptEditor.LowerLimitnormCurrentValue = dataset[0].NormCurrent[0].Lower;
                    _ScriptEditor.UpperLimitnormCurrentValue = dataset[0].NormCurrent[0].Upper;
                    _ScriptEditor.IsEnableDetectsleepCurrent = dataset[0].SleepCurrent[0].ISEnable;
                    _ScriptEditor.UpperLimitsleepCurrentValue = dataset[0].SleepCurrent[0].Upper;
                    _ScriptEditor.IsEnableDetectLightsensor = dataset[0].Lightsensor[0].ISEnable;
                    _ScriptEditor.LowerLimitLightsensorValue = dataset[0].Lightsensor[0].Lower;
                    _ScriptEditor.UpperLimitLightsensorValue = dataset[0].Lightsensor[0].Upper;
                    _ScriptEditor.IsEnableTouchfinger = dataset[0].Touchfinger[0].ISEnable;
                    _ScriptEditor.IsEnableTouchXY = dataset[0].TouchXY[0].ISEnable;
                    nlogMessageAggregator.Instance.SendMessage(new nlogtype { LogLevel = NLog.LogLevel.Debug, Msg = JSONfilePath });
                    nlogMessageAggregator.Instance.SendMessage(new nlogtype { LogLevel = NLog.LogLevel.Debug, Msg = "ReadPortCandidatelist" });

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
                throw;
            }
        }
        public void ReadMonitoringmodeScriptfromJson(CustomScriptEditor _ScriptEditor) {

            try
            {
                string JSONfilePath = _ScriptEditor.Scheduledscriptpath; // 路徑為你的JSON文件的實際路徑

                if (System.IO.File.Exists(JSONfilePath))
                {
                    List<Monitoringmodescripttype> dataset = LoadDataService<Monitoringmodescripttype>.ReadFile(JSONfilePath);
                    _ScriptEditor.Fullloop = dataset[0].Loop;
                    _ScriptEditor.BlockALoop = dataset[0].BlockALoop;
                    _ScriptEditor.BlockBLoop = dataset[0].BlockBLoop;
                    _ScriptEditor.BlockB1Interval = dataset[0].BlockB1Interval;
                    _ScriptEditor.BlockB2Interval = dataset[0].BlockB2Interval;
                    _ScriptEditor.BlockA2Interval = dataset[0].BlockA2Interval;
                    _ScriptEditor.BlockA1Interval = dataset[0].BlockA1Interval;
                    _ScriptEditor.MonitoringIntervaltime = dataset[0].MonitoringIntervaltime;
                    _ScriptEditor.MonitoringIntervaltime = dataset[0].MonitoringIntervaltime;
                    _ScriptEditor.blockA1initscriptpath = dataset[0].TestSuiteA1init[0].ScriptPath;
                    _ScriptEditor.BlockA1initSequencesList = ConvertOCBlockscript( dataset[0].TestSuiteA1init[0].Command);
                    _ScriptEditor.blockA1scriptpath = dataset[0].TestSuiteA1[0].ScriptPath;
                    _ScriptEditor.BlockA1SequencesList = ConvertOCBlockscript(dataset[0].TestSuiteA1[0].Command);
                    _ScriptEditor.blockA2initscriptpath = dataset[0].TestSuiteA2init[0].ScriptPath;
                    _ScriptEditor.BlockA2initSequencesList = ConvertOCBlockscript(dataset[0].TestSuiteA2init[0].Command);
                    _ScriptEditor.blockA2scriptpath = dataset[0].TestSuiteA2[0].ScriptPath;
                    _ScriptEditor.BlockA2SequencesList = ConvertOCBlockscript(dataset[0].TestSuiteA2[0].Command);
                    _ScriptEditor.blockB1initscriptpath = dataset[0].TestSuiteB1init[0].ScriptPath;
                    _ScriptEditor.BlockB1initSequencesList = ConvertOCBlockscript(dataset[0].TestSuiteB1init[0].Command);
                    _ScriptEditor.blockB1scriptpath = dataset[0].TestSuiteB1[0].ScriptPath;
                    _ScriptEditor.BlockB1SequencesList = ConvertOCBlockscript(dataset[0].TestSuiteB1[0].Command);
                    _ScriptEditor.blockB2initscriptpath = dataset[0].TestSuiteB2init[0].ScriptPath;
                    _ScriptEditor.BlockB2initSequencesList = ConvertOCBlockscript(dataset[0].TestSuiteB2init[0].Command);
                    _ScriptEditor.blockB2scriptpath = dataset[0].TestSuiteB2[0].ScriptPath;
                    _ScriptEditor.BlockB2SequencesList = ConvertOCBlockscript(dataset[0].TestSuiteB2[0].Command);

                    _ScriptEditor.IsEnableDetectDiag = dataset[0].DetectDiag[0].ISEnable;
                    _ScriptEditor.IsEnableDetectnormCurrent = dataset[0].NormCurrent[0].ISEnable;
                    _ScriptEditor.LowerLimitnormCurrentValue = dataset[0].NormCurrent[0].Lower;
                    _ScriptEditor.UpperLimitnormCurrentValue = dataset[0].NormCurrent[0].Upper;
                    _ScriptEditor.IsEnableDetectsleepCurrent = dataset[0].SleepCurrent[0].ISEnable;
                    _ScriptEditor.UpperLimitsleepCurrentValue = dataset[0].SleepCurrent[0].Upper;
                    _ScriptEditor.IsEnableDetectLightsensor = dataset[0].Lightsensor[0].ISEnable;
                    _ScriptEditor.LowerLimitLightsensorValue = dataset[0].Lightsensor[0].Lower;
                    _ScriptEditor.UpperLimitLightsensorValue = dataset[0].Lightsensor[0].Upper;
                    _ScriptEditor.IsEnableTouchfinger = dataset[0].Touchfinger[0].ISEnable;
                    _ScriptEditor.IsEnableTouchXY = dataset[0].TouchXY[0].ISEnable;
                    nlogMessageAggregator.Instance.SendMessage(new nlogtype { LogLevel = NLog.LogLevel.Debug, Msg = JSONfilePath });
                    nlogMessageAggregator.Instance.SendMessage(new nlogtype { LogLevel = NLog.LogLevel.Debug, Msg = "ReadPortCandidatelist" });

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
                throw;
            }
        }

        public List<SendorExecuteSendType> ConvertOCBlockscript(ObservableCollection<ScriptItemtype> OCbuffer)
        {
            List<SendorExecuteSendType> Temp = new List<SendorExecuteSendType>();
            try
            {
                foreach (var item in OCbuffer)
                {
                    if (item.Portnum == "all")
                    {
                        Temp.Add(new SendorExecuteSendType()
                        {
                            PortNum = 99,
                            CommandData = new byte[0],
                            intDataLen = 0,
                            strDataLen = "0",
                            Delaytime = Convert.ToInt32(item.Delaytime),
                            Loop = 1,
                            SendMsgdata = String.Format("延遲項目不應該出現")
                        });

                        for (int inum = 0; inum < 3; inum++)
                        {
                            Temp.Add(new SendorExecuteSendType()
                            {
                                PortNum = inum,
                                CommandData = SerialPortModel.Instance.HexStrToByte(item.Command),
                                strCommandData = item.Command,
                                intDataLen = SerialPortModel.Instance.HexStrToByte(item.Command).Length,
                                strDataLen = SerialPortModel.Instance.HexStrToByte(item.Command).Length.ToString(),
                                Delaytime = 0,
                                Loop = Convert.ToInt32(item.Loop),
                                SendMsgdata = String.Format("ID:{0}|Port:{1}|S| {2}", item.ID.ToString().PadLeft(3, ' '), inum, item.Command.Replace(" ", ""))
                            });
                        }

                    }
                    else
                    {
                        Temp.Add(new SendorExecuteSendType()
                        {
                            PortNum = Convert.ToInt32(item.Portnum),
                            CommandData = SerialPortModel.Instance.HexStrToByte(item.Command),
                            strCommandData = item.Command,
                            intDataLen = SerialPortModel.Instance.HexStrToByte(item.Command).Length,
                            strDataLen = SerialPortModel.Instance.HexStrToByte(item.Command).Length.ToString(),
                            Delaytime = Convert.ToInt32(item.Delaytime),
                            Loop = Convert.ToInt32(item.Loop),
                            SendMsgdata = String.Format("ID:{0}|Port:{1}|S| {2}", item.ID.ToString().PadLeft(3, ' '), item.Portnum, item.Command.Replace(" ", ""))
                        });

                    }

                }
                return Temp;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                System.Windows.MessageBox.Show(ex.StackTrace);
                return new List<SendorExecuteSendType> { };
            }
        }

        public void WriteMonitoringmodeScripttoJSON(string SavePath, ScheduledScriptEditor _ScriptEditor)
        {

            try
            {
                string JSONfilePath = SavePath;// FOLDER_config + "PortCandidatelist.json"; // 路徑為你的JSON文件的實際路徑
                if (true)//_SerialCandidator.Count > 0)
                {
                    if (System.IO.File.Exists(JSONfilePath))        // 查看檔案是否存在
                    {
                        System.IO.File.Delete(JSONfilePath);
                    }
                    using (System.IO.StreamWriter filestream = new System.IO.StreamWriter(JSONfilePath))
                    {

                        List<Monitoringmodescripttype> lstStuModel = new List<Monitoringmodescripttype>()
                        {
                            new Monitoringmodescripttype(){
                                    Loop = _ScriptEditor.Fullloop,
                            BlockALoop = _ScriptEditor.BlockALoop,
                            BlockBLoop = _ScriptEditor.BlockBLoop,
                            BlockA1Interval = _ScriptEditor.BlockA1Interval,
                            BlockA2Interval = _ScriptEditor.BlockA2Interval,
                            BlockB1Interval = _ScriptEditor.BlockB1Interval,
                            BlockB2Interval = _ScriptEditor.BlockB2Interval,
                            //
                            MonitoringIntervaltime = _ScriptEditor.MonitoringIntervaltime,
                            TestSuiteA1init = new List<subscript>() { new subscript { ScriptPath = _ScriptEditor.BlockA1initscriptPath, Command = _ScriptEditor.BlockA1initObsColSequences } },
                            TestSuiteA1 = new List<subscript>() { new subscript { ScriptPath = _ScriptEditor.BlockA1scriptPath, Command = _ScriptEditor.BlockA1ObsColSequences } },
                            TestSuiteA2init = new List<subscript>() { new subscript { ScriptPath = _ScriptEditor.BlockA2initscriptPath, Command = _ScriptEditor.BlockA2initObsColSequences } },
                            TestSuiteA2 = new List<subscript>() { new subscript { ScriptPath = _ScriptEditor.BlockA2scriptPath, Command = _ScriptEditor.BlockA2ObsColSequences } },
                            TestSuiteB1init = new List<subscript>() { new subscript { ScriptPath = _ScriptEditor.BlockB1initscriptPath, Command = _ScriptEditor.BlockB1initObsColSequences } },
                            TestSuiteB1 = new List<subscript>() { new subscript { ScriptPath = _ScriptEditor.BlockB1scriptPath, Command = _ScriptEditor.BlockB1ObsColSequences } },
                            TestSuiteB2init = new List<subscript>() { new subscript { ScriptPath = _ScriptEditor.BlockB2initscriptPath, Command = _ScriptEditor.BlockB2initObsColSequences } },
                            TestSuiteB2 = new List<subscript>() { new subscript { ScriptPath = _ScriptEditor.BlockB2scriptPath, Command = _ScriptEditor.BlockB2ObsColSequences } },


                            DetectDiag = new List<ThresholdSettingtype>() { new ThresholdSettingtype { ISEnable = _ScriptEditor.IsEnableDetectDiag, } },

                            NormCurrent = new List<ThresholdSettingtype>() {
                                new ThresholdSettingtype {
                                    ISEnable = _ScriptEditor.IsEnableDetectnormCurrent,
                                    Upper = _ScriptEditor.UpperLimitnormCurrentValue,
                                    Lower =   _ScriptEditor.LowerLimitnormCurrentValue } },
                            SleepCurrent = new List<ThresholdSettingtype>() {
                                new ThresholdSettingtype {
                                    ISEnable = _ScriptEditor.IsEnableDetectsleepCurrent,
                                    Upper = _ScriptEditor.UpperLimitsleepCurrentValue} },
                            Lightsensor = new List<ThresholdSettingtype>() {
                                new ThresholdSettingtype {
                                    ISEnable = _ScriptEditor.IsEnableDetectLightsensor,
                                    Upper = _ScriptEditor.UpperLimitLightsensorValue,
                                    Lower = _ScriptEditor.LowerLimitLightsensorValue} },
                            Touchfinger = new List<ThresholdSettingtype>() {
                                new ThresholdSettingtype {
                                    ISEnable = _ScriptEditor.IsEnableTouchfinger } },
                            TouchXY = new List<ThresholdSettingtype>() {
                                new ThresholdSettingtype {
                                    ISEnable = _ScriptEditor.IsEnableTouchXY } },
                            }
                    
                        };
                      
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
