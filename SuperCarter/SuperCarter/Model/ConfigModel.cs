using Notification.Wpf;
using SuperCarter.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Management;
using System.IO;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Interop;
using SuperCarter.ViewModel;
using Notification.Wpf;
using SuperCarter.Services;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Controls;


namespace SuperCarter.Model
{
    public class ConfigModel : SingletonBase<ConfigModel>
    {

        private readonly string CONFIGNAME = "SMSconfig.xml";

        public void DetectConfigfile()
        {
            if (Directory.Exists(FOLDER_CONFIG + CONFIGNAME) == false) WriteDefaultxmlformate();

        }
        public int GetSystemIntrospectionTimerInterval()
        {
            int _interval = 0;
            XmlDocument SMS = new XmlDocument();
            SMS.Load(FOLDER_CONFIG + CONFIGNAME);
            XmlNode strinterval = SMS.SelectSingleNode("Systemroot/SMSSystemGroup/Params/SystemIntrospectionIntervalTimer");

            string str_interval = strinterval?.Attributes?["Interval"]?.Value;
            return _interval = string.IsNullOrEmpty(str_interval) ? 5000 : Convert.ToInt32(str_interval);
        }
        /// <summary>
        /// import COMPORT list from that PC detect
        /// </summary>
        /// <returns></returns>
        public List<Portdetectedtype> GetComPorts()
        {
            List<Portdetectedtype> RootNode = new List<Portdetectedtype>();
            RootNode.Clear();
            RootNode.Add(new Portdetectedtype() { FullPortName = "連接埠(COM 和 LPT)" });
            RootNode[0].ChildNode = new List<Portdetectedtype>();

            string CascadeCOMstr = null;
            using (ManagementClass i_Entity = new ManagementClass("Win32_PnPEntity"))
            {
                foreach (ManagementObject i_Inst in i_Entity.GetInstances())
                {
                    Object o_Guid = i_Inst.GetPropertyValue("ClassGuid");
                    if (o_Guid == null || o_Guid.ToString().ToUpper() != "{4D36E978-E325-11CE-BFC1-08002BE10318}")
                        continue; // Skip all devices except device class "PORTS"

                    String s_Caption = i_Inst.GetPropertyValue("Caption").ToString();
                    String s_subCaption = null;
                    int s32_Pos = s_Caption.IndexOf(" (COM");
                    if (s32_Pos > 0) // remove COM port from description
                    {
                        string reversName = s_Caption.Substring(0, s32_Pos);
                        s_subCaption = s_Caption.Replace(reversName, "");
                        s_subCaption = s_subCaption.Replace("(", " ");
                        s_subCaption = s_subCaption.Replace(")", " ").Trim();
                    }

                    CascadeCOMstr = string.Concat(CascadeCOMstr, s_subCaption);

                    RootNode[0].ChildNode.Add(new Portdetectedtype()
                    {

                        FullPortName = s_Caption,
                        PortName = s_subCaption
                    });
                }
                i_Entity.Dispose();
            }
            return RootNode;

        }
        /// <summary>
        /// this is a root of xmlnodelist 
        /// </summary>
        public XmlDocument SMS { get; set; }
        /// <summary>
        /// this is a root of xmlnodelist 
        /// </summary>
        public XmlDocument ScriptionXML { get; set; }
        /// <summary>
        /// import baudrate list from configfile
        /// </summary>
        /// <returns></returns>
        public List<SerialPortModel> GetBaudRates()
        {
            XmlDocument SMS = new XmlDocument();
            SMS.Load(FOLDER_CONFIG + CONFIGNAME);
            XmlNodeList NodeLists = SMS.SelectNodes("Systemroot/Serialport/Format/BaudRate/Item");

            List<SerialPortModel> _list = new List<SerialPortModel>();
            foreach (XmlNode OneNode in NodeLists)
            {
                foreach (XmlAttribute Attr in OneNode.Attributes)
                {
                    String StrAttr = Attr.Name.ToString();
                    String StrValue = OneNode.Attributes[Attr.Name.ToString()].Value;
                    String StrInnerText = OneNode.InnerText;

                    _list.Add(new SerialPortModel() { BaudRateName = StrValue + " bit/s", BaudRateValue = Convert.ToInt32(StrValue) });
                }
            }
            if (_list.Count == 0)
            {
                _list.Add(new SerialPortModel() { BaudRateName = "115200 baud", BaudRateValue = 115200 });
            }

            return _list;
        }

        /// <summary>
        /// import baudrate list from configfile
        /// </summary>
        /// <returns></returns>
        public List<SerialPortModel> GetDataReceivedCases()
        {
            XmlDocument SMS = new XmlDocument();
            SMS.Load(FOLDER_CONFIG + CONFIGNAME);
            XmlNodeList NodeLists = SMS.SelectNodes("Systemroot/Serialport/Format/DataReceivedCasenum/Item");

            List<SerialPortModel> _list = new List<SerialPortModel>();
            foreach (XmlNode OneNode in NodeLists)
            {
                foreach (XmlAttribute Attr in OneNode.Attributes)
                {
                    String StrAttr = Attr.Name.ToString();
                    String StrValue = OneNode.Attributes[Attr.Name.ToString()].Value;
                    String StrInnerText = OneNode.InnerText;

                    _list.Add(new SerialPortModel() { DataReceivedCasenum = Convert.ToInt32(StrValue) });
                }
            }
            if (_list.Count == 0)
            {
                _list.Add(new SerialPortModel() { DataReceivedCasenum = 0 });
            }

            return _list;
        }

        public SerialPortModel GetdefaultCOMPortinfo()
        {

            XmlDocument SMS = new XmlDocument();
            SMS.Load(FOLDER_CONFIG + CONFIGNAME);
            XmlNode child1 = SMS.SelectSingleNode("Systemroot/Serialport/Port1");

            XmlAttributeCollection child1Attrs = child1?.Attributes;
            string _portinfo = null;

            foreach (XmlAttribute attr in child1Attrs)
            {
                _portinfo += "   " + attr?.Name + " : " + attr?.Value;
            }
            return new SerialPortModel();//cominfo;
        }

        private void Paramwrite(XmlElement ParentNode, List<string> _list, string _name)
        {
            List<string> list = new List<string>();
            XmlElement baseParameter = SMS.CreateElement(_name);

            ParentNode.AppendChild(baseParameter);

            foreach (var item in _list)
            {
                XmlElement _item = SMS.CreateElement("Item");
                _item.SetAttribute("Value", item.ToString());
                baseParameter.AppendChild(_item);
            }
        }


        /// <summary>
        /// save the sequences list to xml file.
        /// </summary>
        /// <param name="SavePath"></param>
        /// <param name="_Scriptdatalist"></param>
        /// <param name="executeloop"></param>
        public void SaveScriptTestSequencefile(string SavePath, ObservableCollection<ScriptItemtype> _Scriptdatalist, int _iterationnumber = 1)
        {
            ScriptionXML = new XmlDocument();
            // 建立目錄的根
            XmlElement testgroup = ScriptionXML.CreateElement("TestSuite");
            ScriptionXML.AppendChild(testgroup); // Add Node.

            XmlElement preparation = ScriptionXML.CreateElement("Prerequisites");
            testgroup.AppendChild(preparation);

            XmlElement testcase = ScriptionXML.CreateElement("TestSequence");
            testgroup.AppendChild(testcase);

            int delaytime = 0;

            // 建立根目錄的子節點
            XmlElement testitem;
            try
            {
                for (int iD = 0; iD < _Scriptdatalist.Count; iD++)
                {
                    testitem = ScriptionXML.CreateElement("Sequence");
                    testitem.SetAttribute("ID", _Scriptdatalist[iD].ID.ToString() ?? "");
                    testitem.SetAttribute("PortNum", _Scriptdatalist[iD].SelectScriptPortnum.ToString() ?? "");
                    testitem.SetAttribute("Nodename", _Scriptdatalist[iD].Nodename.ToString() ?? "");
                    testitem.SetAttribute("MSGname", _Scriptdatalist[iD].MSGname.ToString() ?? "");
                    testitem.SetAttribute("Sequence", _Scriptdatalist[iD].Sequence.ToString() ?? "");
                    testitem.SetAttribute("Delaytime", _Scriptdatalist[iD].Delaytime.ToString() ?? "100");
                    testitem.SetAttribute("RecSequence", _Scriptdatalist[iD].RecSequence.ToString() ?? "");
                    testitem.SetAttribute("HashCodevalue", _Scriptdatalist[iD].HashCodevalue.ToString() ?? "");
                    testitem.SetAttribute("Loop", _Scriptdatalist[iD].Loop.ToString() ?? "");

                    // calculate the total time to estimate the duration required for entire scheduling. 
                    delaytime += _Scriptdatalist[iD].Delaytime;

                    // save the list to testitem
                    testcase.AppendChild(testitem);
                }
                //testcase.SetAttribute("TotalTime", delaytime.ToString() ?? "0");
                _iterationnumber = _iterationnumber > 0 ? _iterationnumber : 1;
                //testcase.SetAttribute("IterValue", _iterationnumber.ToString() ?? "1");
                ScriptionXML.Save(SavePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public delegate void PopNotificationDelegate(POPNotifyMsgType _popNotifyMsgType);
        public static event PopNotificationDelegate OnRaisePopNotification;
        public int GetScriptXMLSequence_itervalue(string SavePath)
        {
            try
            {

                // Loading from a file, you can also load from a stream
                ObservableCollection<ScriptItemtype> Temp = new ObservableCollection<ScriptItemtype>();
                XmlDocument ScriptionXML = new XmlDocument();

                ScriptionXML.Load(SavePath);
                XmlNode root = ScriptionXML.SelectSingleNode("TestSuite/TestSequence");

                string striteravalue;

                XmlElement element = (XmlElement)root;
                if (element is null)
                {
                    OnRaisePopNotification?.Invoke(new POPNotifyMsgType { Tital = "異常通知", Message = "開啟過程異常無法執行", NotifyType = NotificationType.Error });
                    return 0;
                }
                striteravalue = element.Attributes["IterValue"]?.Value;
                int itervalue = Convert.ToInt32(striteravalue);
                return itervalue > 0 ? itervalue : 1;
            }
            catch (Exception ex)
            {
                int iteravalue = 1;
                MessageBox.Show(ex.Message);
                return iteravalue;
            }
        }
        public int GetScriptXMLSequence_TotalTime(string SavePath)
        {
            try
            {
                // Loading from a file, you can also load from a stream
                ObservableCollection<ScriptItemtype> Temp = new ObservableCollection<ScriptItemtype>();
                XmlDocument ScriptionXML = new XmlDocument();

                ScriptionXML.Load(SavePath);
                XmlNode root = ScriptionXML.SelectSingleNode("TestSuite/TestSequence");

                string striteravalue;

                XmlElement element = (XmlElement)root;
                striteravalue = element.Attributes["TotalTime"]?.Value;
                int itervalue = Convert.ToInt32(striteravalue);
                return itervalue;
            }
            catch (Exception ex)
            {
                int iteravalue = 0;
                MessageBox.Show(ex.Message);
                return iteravalue;
            }
        }

        public void GetScriptXMLTestSuite(ScheduledScriptEditor _ScriptEditor)
        {
            try
            {
                // Loading from a file, you can also load from a stream
                XmlDocument ScriptionXML = new XmlDocument();
                ScriptionXML.Load(_ScriptEditor.OpenedBlockScriptPath);
                    List<string> list = new List<string>() {"TestSuiteA1init", "TestSuiteA1",
                                                             "TestSuiteA2init", "TestSuiteA2",
                                                             "TestSuiteB1init", "TestSuiteB1",
                                                             "TestSuiteB2init", "TestSuiteB2",
                                                            };

                XmlNode root = ScriptionXML.SelectSingleNode("TestSuites");
                _ScriptEditor.Fullloop = Convert.ToInt32(root.Attributes["Loop"]?.Value);
                _ScriptEditor.BlockALoop = Convert.ToInt32(root.Attributes["BlockALoop"]?.Value);
                _ScriptEditor.BlockBLoop = Convert.ToInt32(root.Attributes["BlockBLoop"]?.Value);
                _ScriptEditor.BlockA1Interval = Convert.ToInt32(root.Attributes["BlockA1Interval"]?.Value);
                _ScriptEditor.BlockA2Interval = Convert.ToInt32(root.Attributes["BlockA2Interval"]?.Value);
                _ScriptEditor.BlockB1Interval = Convert.ToInt32(root.Attributes["BlockB1Interval"]?.Value);
                _ScriptEditor.BlockB2Interval = Convert.ToInt32(root.Attributes["BlockB2Interval"]?.Value);
                foreach (var ite in list)
                {
                    string strblock = "TestSuites/" + ite + "/TestSequence";
                    XmlNode block = ScriptionXML.SelectSingleNode(strblock);
                    string strrequisites = "TestSuites/" + ite + "/Prerequisites";
                    XmlNode requisites = ScriptionXML.SelectSingleNode(strrequisites);

                    XmlElement element = (XmlElement)block;
                    ObservableCollection<ScriptItemtype> Temp = new ObservableCollection<ScriptItemtype>();
                    //取得節點內的欄位
                    if (element is null)
                        break;
                    foreach (XmlElement node in element)
                    {
                        String ID = node.Attributes["ID"].Value ?? "";
                        String PortNum = node.Attributes["PortNum"]?.Value ?? "all";
                        String Nodename = node.Attributes["Nodename"]?.Value ?? "";
                        String MSGname = node.Attributes["MSGname"]?.Value ?? "";
                        String Sequence = node.Attributes["Sequence"]?.Value ?? "";
                        String Delaytime = node.Attributes["Delaytime"]?.Value ?? "100";
                        String RecSequence = node.Attributes["RecSequence"]?.Value ?? "";
                        String HashCodevalue = node.Attributes["HashCodevalue"]?.Value ?? "";
                        String Loop = node.Attributes["Loop"]?.Value ?? "1";

                        Temp.Add(new ScriptItemtype()
                        {
                            ID = Convert.ToInt32(ID),
                            SelectScriptPortnum = PortNum,
                            Nodename = Nodename,
                            MSGname = MSGname,
                            Sequence = Sequence,
                            Delaytime = Convert.ToInt32(Delaytime),
                            RecSequence = RecSequence,
                            HashCodevalue = HashCodevalue,
                            Loop = Convert.ToInt32(Loop),
                        });

                    }
                    if (ite == "TestSuiteA1init")
                    {
                        _ScriptEditor.BlockA1initObsColSequences = Temp;
                        _ScriptEditor.BlockA1initscriptPath = requisites.Attributes["Path"]?.Value ?? "";
                    }
                    else if (ite == "TestSuiteA1")
                    {
                        _ScriptEditor.BlockA1ObsColSequences = Temp;
                        _ScriptEditor.BlockA1scriptPath = requisites.Attributes["Path"]?.Value ?? "";
                    }
                    else if (ite == "TestSuiteA2init")
                    {
                        _ScriptEditor.BlockA2initObsColSequences = Temp;
                        _ScriptEditor.BlockA2initscriptPath = requisites.Attributes["Path"]?.Value ?? "";
                    }
                    else if (ite == "TestSuiteA2")
                    {
                        _ScriptEditor.BlockA2ObsColSequences = Temp;
                        _ScriptEditor.BlockA2scriptPath = requisites.Attributes["Path"]?.Value ?? "";                    
                    }
                    else if (ite == "TestSuiteB1init")
                    {
                        _ScriptEditor.BlockB1initObsColSequences = Temp;
                        _ScriptEditor.BlockB1initscriptPath = requisites.Attributes["Path"]?.Value ?? "";
                    }
                    else if (ite == "TestSuiteB1")
                    {
                        _ScriptEditor.BlockB1ObsColSequences = Temp;
                        _ScriptEditor.BlockB1scriptPath = requisites.Attributes["Path"]?.Value ?? "";
                    }
                    else if (ite == "TestSuiteB2init")
                    {
                        _ScriptEditor.BlockB2initObsColSequences = Temp;
                        _ScriptEditor.BlockB2initscriptPath = requisites.Attributes["Path"]?.Value ?? "";
                    }
                    else if (ite == "TestSuiteB2")
                    {
                        _ScriptEditor.BlockB2ObsColSequences = Temp;
                        _ScriptEditor.BlockB2scriptPath = requisites.Attributes["Path"]?.Value ?? "";
                    }
                }
                string Thresetting = "TestSuites/ThresholdSetting";
                XmlNode Threblock = ScriptionXML.SelectSingleNode(Thresetting);
                XmlElement elements = (XmlElement)Threblock;
                if (elements is null)
                    return;
                foreach (XmlElement node in elements)
                {
                    if (node.Name == "DetectDiag")
                    {
                        _ScriptEditor.IsEnableDetectDiag = Convert.ToBoolean(node.Attributes["IsEnable"].Value ?? "false");
                    }
                    else if (node.Name == "NormCurrent")
                    {
                        _ScriptEditor.IsEnableDetectnormCurrent = Convert.ToBoolean(node.Attributes["IsEnable"].Value ?? "false");
                        _ScriptEditor.UpperLimitnormCurrentValue = Convert.ToInt32(node.Attributes["Upper"].Value);
                        _ScriptEditor.LowerLimitnormCurrentValue = Convert.ToInt32(node.Attributes["Lower"].Value);
                    }
                    else if (node.Name == "SleepCurrent")
                    {
                        _ScriptEditor.IsEnableDetectsleepCurrent = Convert.ToBoolean(node.Attributes["IsEnable"].Value ?? "false");
                        _ScriptEditor.UpperLimitsleepCurrentValue = Convert.ToInt32(node.Attributes["Upper"].Value);
                    }
                    else if (node.Name == "Lightsensor")
                    {
                        _ScriptEditor.IsEnableDetectLightsensor = Convert.ToBoolean(node.Attributes["IsEnable"].Value ?? "false");
                        _ScriptEditor.UpperLimitLightsensorValue = Convert.ToInt32(node.Attributes["Upper"].Value);
                        _ScriptEditor.LowerLimitLightsensorValue = Convert.ToInt32(node.Attributes["Lower"].Value);
                    }
                    else if (node.Name == "Touchfinger")
                    {
                        _ScriptEditor.IsEnableTouchfinger = Convert.ToBoolean(node.Attributes["IsEnable"].Value ?? "false");
                    }
                    else if (node.Name == "TouchXY")
                    {
                        _ScriptEditor.IsEnableTouchXY = Convert.ToBoolean(node.Attributes["IsEnable"].Value ?? "false");
                    }
                }
                  
                  
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }

        }
        public void evt_SaveScriptTestSuitefile(string SavePath, ScheduledScriptEditor _ScriptEditor)
        {
            try
            {
                ScriptionXML = new XmlDocument();             
                ScriptionXML.CreateXmlDeclaration("1.0", "utf-8", "yes");
                //建立根節點
                //ScriptionXML.LoadXml("<TestSuites></TestSuites>");
                XmlElement root = ScriptionXML.CreateElement("TestSuites");

                // 新增屬性
                root.SetAttribute("Loop", _ScriptEditor.Fullloop.ToString() ?? "");
                root.SetAttribute("BlockALoop", _ScriptEditor.BlockALoop.ToString() ?? "");
                root.SetAttribute("BlockBLoop", _ScriptEditor.BlockBLoop.ToString() ?? "");
                root.SetAttribute("BlockA1Interval", _ScriptEditor.BlockA1Interval.ToString() ?? "");
                root.SetAttribute("BlockA2Interval", _ScriptEditor.BlockA2Interval.ToString() ?? "");
                root.SetAttribute("BlockB1Interval", _ScriptEditor.BlockB1Interval.ToString() ?? "");
                root.SetAttribute("BlockB2Interval", _ScriptEditor.BlockB2Interval.ToString() ?? "");
           
                // Create the item element
                ScriptionXML.AppendChild(root);
               
                List<string> list = new List<string>() { "TestSuiteA1init", "TestSuiteA1", "TestSuiteA2init", "TestSuiteA2",
                    "TestSuiteB1init", "TestSuiteB1", "TestSuiteB2init", "TestSuiteB2" };

                ObservableCollection<ScriptItemtype> tempblockobsv = new ObservableCollection<ScriptItemtype>();

                foreach (var item in list)
                {
                    // 建立目錄的根
                    XmlElement testgroup = ScriptionXML.CreateElement(item);
                    root.AppendChild(testgroup); // Append to root


                    XmlElement preparation = ScriptionXML.CreateElement("Prerequisites");

                    // 建立根目錄的子節點
                    XmlElement testitem;

                    if (item == "TestSuiteA1init")
                    {
                        tempblockobsv = _ScriptEditor.BlockA1initObsColSequences;
                        preparation.SetAttribute("Path", _ScriptEditor.BlockA1initscriptPath ?? "");
                    }
                    else if(item == "TestSuiteA1")
                    {
                        tempblockobsv = _ScriptEditor.BlockA1ObsColSequences;                     
                        preparation.SetAttribute("Path", _ScriptEditor.BlockA1scriptPath ?? "");
                    }
                    else if (item == "TestSuiteA2init")
                    {
                        tempblockobsv = _ScriptEditor.BlockA2initObsColSequences;
                        preparation.SetAttribute("Path", _ScriptEditor.BlockA2initscriptPath ?? "");
                    }
                    else if (item == "TestSuiteA2")
                    {
                        tempblockobsv = _ScriptEditor.BlockA2ObsColSequences;
                        preparation.SetAttribute("Path", _ScriptEditor.BlockA2scriptPath ?? "");
                    }
                    else if (item == "TestSuiteB1init")
                    {
                        tempblockobsv = _ScriptEditor.BlockB1initObsColSequences;
                        preparation.SetAttribute("Path", _ScriptEditor.BlockB1initscriptPath ?? "");
                    }
                    else if (item == "TestSuiteB1")
                    {
                        tempblockobsv = _ScriptEditor.BlockB1ObsColSequences;
                        preparation.SetAttribute("Path", _ScriptEditor.BlockB1scriptPath ?? "");
                    }
                    else if (item == "TestSuiteB2init")
                    {
                        tempblockobsv = _ScriptEditor.BlockB2initObsColSequences;    
                        preparation.SetAttribute("Path", _ScriptEditor.BlockB2initscriptPath ?? "");
                    }
                    else if (item == "TestSuiteB2")
                    {
                        tempblockobsv = _ScriptEditor.BlockB2ObsColSequences;
                        preparation.SetAttribute("Path", _ScriptEditor.BlockB2scriptPath ?? "");
                    }

                    testgroup.AppendChild(preparation);
                    XmlElement testcase = ScriptionXML.CreateElement("TestSequence");
                    testgroup.AppendChild(testcase);

                    for (int iD = 0; iD < tempblockobsv.Count; iD++)
                    {
                        testitem = ScriptionXML.CreateElement("Sequence");
                        testitem.SetAttribute("ID", tempblockobsv[iD].ID.ToString() ?? "");
                        testitem.SetAttribute("PortNum", tempblockobsv[iD].SelectScriptPortnum.ToString() ?? "");
                        testitem.SetAttribute("Nodename", tempblockobsv[iD].Nodename.ToString() ?? "");
                        testitem.SetAttribute("MSGname", tempblockobsv[iD].MSGname.ToString() ?? "");
                        testitem.SetAttribute("Sequence", tempblockobsv[iD].Sequence.ToString() ?? "");
                        testitem.SetAttribute("Delaytime", tempblockobsv[iD].Delaytime.ToString() ?? "");
                        testitem.SetAttribute("RecSequence", tempblockobsv[iD].RecSequence.ToString() ?? "");
                        testitem.SetAttribute("HashCodevalue", tempblockobsv[iD].HashCodevalue.ToString() ?? "");
                        testitem.SetAttribute("Loop", tempblockobsv[iD].Loop.ToString() ?? "");

                        // calculate the total time to estimate the duration required for entire scheduling.   
                        // save the list to testitem
                        testcase.AppendChild(testitem);
                    }
          
           
                }
                // Create the threshold settings element
                XmlElement thresholdSetting = ScriptionXML.CreateElement("ThresholdSetting");
                                           
                // Create the elements for the threshold settings
                XmlElement detectDiag = ScriptionXML.CreateElement("DetectDiag");
                detectDiag.SetAttribute("IsEnable", (_ScriptEditor.IsEnableDetectDiag).ToString() ?? "N");
                thresholdSetting.AppendChild(detectDiag);

                XmlElement normCurrent = ScriptionXML.CreateElement("NormCurrent");
                normCurrent.SetAttribute("IsEnable", (_ScriptEditor.IsEnableDetectnormCurrent).ToString() ?? "N");
                normCurrent.SetAttribute("Upper", (_ScriptEditor.UpperLimitnormCurrentValue).ToString() ?? "0");
                normCurrent.SetAttribute("Lower", (_ScriptEditor.LowerLimitnormCurrentValue).ToString() ?? "0");
                thresholdSetting.AppendChild(normCurrent);

                XmlElement sleepCurrent = ScriptionXML.CreateElement("SleepCurrent");
                sleepCurrent.SetAttribute("IsEnable", (_ScriptEditor.IsEnableDetectsleepCurrent).ToString() ?? "N");
                sleepCurrent.SetAttribute("Upper", (_ScriptEditor.UpperLimitsleepCurrentValue).ToString() ?? "0");
                thresholdSetting.AppendChild(sleepCurrent);

                XmlElement Lightsensor = ScriptionXML.CreateElement("Lightsensor");
                Lightsensor.SetAttribute("IsEnable", (_ScriptEditor.IsEnableDetectLightsensor).ToString() ?? "N");
                Lightsensor.SetAttribute("Upper", (_ScriptEditor.UpperLimitLightsensorValue).ToString() ?? "0");
                Lightsensor.SetAttribute("Lower", (_ScriptEditor.LowerLimitLightsensorValue).ToString() ?? "0");
                thresholdSetting.AppendChild(Lightsensor);

                XmlElement Touchfinger = ScriptionXML.CreateElement("Touchfinger");
                Touchfinger.SetAttribute("IsEnable", (_ScriptEditor.IsEnableTouchfinger).ToString() ?? "N");
                thresholdSetting.AppendChild(Touchfinger);

                XmlElement TouchXY = ScriptionXML.CreateElement("TouchXY");
                TouchXY.SetAttribute("IsEnable", (_ScriptEditor.IsEnableTouchXY).ToString() ?? "N");
                thresholdSetting.AppendChild(TouchXY);

                root.AppendChild(thresholdSetting); // Append to root
                ScriptionXML.Save(SavePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message + "\n" + ex.StackTrace);

            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SavePath"></param>
        /// <returns></returns>
        public async Task<ObservableCollection<ScriptItemtype>> GetScriptXMLSequencesAsync(string SavePath)
        {
            try
            {
                ObservableCollection<ScriptItemtype> Temp = new ObservableCollection<ScriptItemtype>();

                // 使用异步方式加载 XML
                await Task.Run(async () =>
                {
                    try
                    {
                        XmlDocument ScriptionXML = new XmlDocument();
                        ScriptionXML.Load(SavePath);

                        XmlNode root = ScriptionXML.SelectSingleNode("TestSuite/TestSequence");

                        if (root == null)
                            return;

                        XmlElement element = (XmlElement)root;

                        // 设置批次大小
                        int batchSize = 30;

                        // 取得節點內的欄位
                        foreach (XmlElement node in element)
                        {
                            String ID = node.Attributes["ID"].Value ?? "";
                            String PortNum = node.Attributes["PortNum"]?.Value ?? "all";
                            String Nodename = node.Attributes["Nodename"]?.Value ?? "";
                            String MSGname = node.Attributes["MSGname"]?.Value ?? "";
                            String Sequence = node.Attributes["Sequence"]?.Value ?? "";
                            String Delaytime = node.Attributes["Delaytime"]?.Value ?? "100";
                            String RecSequence = node.Attributes["RecSequence"]?.Value ?? "";
                            String HashCodevalue = node.Attributes["HashCodevalue"]?.Value ?? "";
                            String Loop = node.Attributes["Loop"]?.Value ?? "1";

                            Temp.Add(new ScriptItemtype()
                            {
                                ID = Convert.ToInt32(ID),
                                SelectScriptPortnum = PortNum,
                                Nodename = Nodename,
                                MSGname = MSGname,
                                Sequence = Sequence,
                                Delaytime = Convert.ToInt32(Delaytime),
                                RecSequence = RecSequence,
                                HashCodevalue = HashCodevalue,
                                Loop = Convert.ToInt32(Loop),
                            });

                            // 如果达到批次大小，添加小延迟以允许 UI 响应
                            if (Temp.Count % batchSize == 0)
                            {
                                await Task.Delay(1);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                });

                return Temp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new ObservableCollection<ScriptItemtype>();
            }
        }

        public event EventHandler<SequencesUpdatedEventArgs> SequencesUpdated;

        protected virtual void OnSequencesUpdated(ObservableCollection<ScriptItemtype> sequences)
        {
            SequencesUpdated?.Invoke(this, new SequencesUpdatedEventArgs(sequences));
        }

        // 异步加载脚本的方法
        public async Task LoadScriptAsync(string scriptPath)
        {
            try
            {
                // 异步加载脚本
                ObservableCollection<ScriptItemtype> sequences = await GetScriptXMLSequencesAsync(scriptPath);

                // 触发更新事件
                OnSequencesUpdated(sequences);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        /// <summary>
        ///  Associate the schema with XML. Then, load the XML and validate it against the schema.
        /// </summary>
        /// <param name="SavePath"></param>
        /// <returns></returns>
        public ObservableCollection<ScriptItemtype> GetScriptXMLSequences(string SavePath)
        {
            //************************************************************************************
            //
            //  Associate the schema with XML. Then, load the XML and validate it against
            //  the schema.
            //
            //************************************************************************************

            try
            {
                // Loading from a file, you can also load from a stream
                ObservableCollection<ScriptItemtype> Temp = new ObservableCollection<ScriptItemtype>();
                XmlDocument ScriptionXML = new XmlDocument();

                ScriptionXML.Load(SavePath);
                XmlNode root = ScriptionXML.SelectSingleNode("TestSuite/TestSequence");

                if (root == null)
                    return new ObservableCollection<ScriptItemtype>();
                XmlElement element = (XmlElement)root;

                //取得節點內的欄位
                foreach (XmlElement node in element)
                {
                    String ID = node.Attributes["ID"].Value ?? "";
                    String PortNum = node.Attributes["PortNum"]?.Value ?? "all";
                    String Nodename = node.Attributes["Nodename"]?.Value ?? "";
                    String MSGname = node.Attributes["MSGname"]?.Value ?? "";
                    String Sequence = node.Attributes["Sequence"]?.Value ?? "";
                    String Delaytime = node.Attributes["Delaytime"]?.Value ?? "100";
                    String RecSequence = node.Attributes["RecSequence"]?.Value ?? "";
                    String HashCodevalue = node.Attributes["HashCodevalue"]?.Value ?? "";
                    String Loop = node.Attributes["Loop"]?.Value ?? "1";

                    Temp.Add(new ScriptItemtype()
                    {
                        ID = Convert.ToInt32(ID),
                        SelectScriptPortnum = PortNum,
                        Nodename = Nodename,
                        MSGname = MSGname,
                        Sequence = Sequence,
                        Delaytime = Convert.ToInt32(Delaytime),
                        RecSequence = RecSequence,
                        HashCodevalue = HashCodevalue,
                        Loop = Convert.ToInt32(Loop),
                    });

                }

                return Temp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return new ObservableCollection<ScriptItemtype>();
            }

        }

        /// <summary>
        /// return a string path of xml file.
        /// </summary>
        /// <returns>
        /// 返回路徑，此為欲打開的 xml 檔案之路徑
        /// </returns>
        public string GetStrScriptpath()
        {
            XmlDocument SMS = new XmlDocument();
            ObservableCollection<ScriptItemtype> Temp = new ObservableCollection<ScriptItemtype>();

            string myPath = AppPath + @"\script\";
            string PATHDDSSCRIPT = null;
            using (var openFileDialog1 = new System.Windows.Forms.OpenFileDialog())
            {

                // 設定OpenFileDialog屬性
                openFileDialog1.Title = "選擇要開啟的 XML 檔案";
                openFileDialog1.Filter = "xml Files (.xml)|*.xml|All Files (*.*)|*.*";
                openFileDialog1.FilterIndex = 1;
                openFileDialog1.Multiselect = true;
                openFileDialog1.InitialDirectory = myPath;

                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    PATHDDSSCRIPT = openFileDialog1.FileName; //取得檔名
                }
            }
            return PATHDDSSCRIPT == null ? "" : PATHDDSSCRIPT;

        }

        public List<SendorExecuteSendType> GetSendorExecuteSequencesList(string SavePath)
        {
            /// TODO: 1. 新增獨立的 DELAY ITEM, 該作用目的在於 PORTNUM 為ALL後面有需要做閒置的時間延遲效果在發送完所有 以開通的PORT NUM 
            ///  
            try
            {
                // Loading from a file, you can also load from a stream
                List<SendorExecuteSendType> Temp = new List<SendorExecuteSendType>();
                XmlDocument ScriptionXML = new XmlDocument();

                ScriptionXML.Load(SavePath);
                XmlNode root = ScriptionXML.SelectSingleNode("TestSuite/TestSequence");

                if (root == null)
                    return new List<SendorExecuteSendType>();
                XmlElement element = (XmlElement)root;

                //取得節點內的欄位
                foreach (XmlElement node in element)
                {
                    String ID = node.Attributes["ID"]?.Value ?? "";
                    String PortNum = node.Attributes["PortNum"]?.Value ?? "all";
                    String Nodename = node.Attributes["Nodename"]?.Value ?? "";
                    String MSGname = node.Attributes["MSGname"]?.Value ?? "";
                    String Sequence = node.Attributes["Sequence"]?.Value ?? "";
                    String Delaytime = node.Attributes["Delaytime"]?.Value ?? "100";
                    String RecSequence = node.Attributes["RecSequence"]?.Value ?? "";
                    String HashCodevalue = node.Attributes["HashCodevalue"]?.Value ?? "";
                    String Loop = node.Attributes["Loop"]?.Value ?? "1";

                    if (PortNum == "all")
                    {
                        Temp.Add(new SendorExecuteSendType()
                        {
                            PortNum = 9,
                            SequenceData = new byte[0],
                            intDataLen = 0,
                            strDataLen = "0",
                            Delaytime = Convert.ToInt32(Delaytime),
                            Loop = 1,
                            SendMsgdata = String.Format("ID:{0}|Port:{1}|S| {2}",
                      ID.PadLeft(3, ' '),
                         9,
                         "delay time")
                        });
                        Temp.Add(new SendorExecuteSendType()
                        {
                            PortNum = 0,
                            SequenceData = SerialPortModel.Instance.HexStrToByte(Sequence),
                            strSequenceData = Sequence,
                            intDataLen = SerialPortModel.Instance.HexStrToByte(Sequence).Length,
                            strDataLen = SerialPortModel.Instance.HexStrToByte(Sequence).Length.ToString(),
                            Delaytime = 0,
                            Loop = Convert.ToInt32(Loop),
                            SendMsgdata = String.Format("ID:{0}|Port:{1}|S| {2}",
                                                      ID.PadLeft(3, ' '),
                                                        0,
                                                        Sequence.Replace(" ", ""))
                        });
                        Temp.Add(new SendorExecuteSendType()
                        {
                            PortNum = 1,
                            SequenceData = SerialPortModel.Instance.HexStrToByte(Sequence),
                            strSequenceData = Sequence,
                            intDataLen = SerialPortModel.Instance.HexStrToByte(Sequence).Length,
                            strDataLen = SerialPortModel.Instance.HexStrToByte(Sequence).Length.ToString(),
                            Delaytime = 0,
                            Loop = Convert.ToInt32(Loop),
                            SendMsgdata = String.Format("ID:{0}|Port:{1}|S| {2}",
                                                    ID.PadLeft(3, ' '),
                                                        1,
                                                        Sequence.Replace(" ", ""))
                        });
                        Temp.Add(new SendorExecuteSendType()
                        {
                            PortNum = 2,
                            SequenceData = SerialPortModel.Instance.HexStrToByte(Sequence),
                            strSequenceData = Sequence,
                            intDataLen = SerialPortModel.Instance.HexStrToByte(Sequence).Length,
                            strDataLen = SerialPortModel.Instance.HexStrToByte(Sequence).Length.ToString(),
                            Delaytime = 0,
                            Loop = Convert.ToInt32(Loop),
                            SendMsgdata = String.Format("ID:{0}|Port:{1}|S| {2}",
                                                     ID.PadLeft(3, ' '),
                                                        2,
                                                        Sequence.Replace(" ", ""))
                        });
                 
                    }
                    else
                    {
                        Temp.Add(new SendorExecuteSendType()
                        {
                            PortNum = Convert.ToInt32(PortNum),
                            intDataLen = SerialPortModel.Instance.HexStrToByte(Sequence).Length,
                            strDataLen = SerialPortModel.Instance.HexStrToByte(Sequence).Length.ToString(),
                            Delaytime = Convert.ToInt32(Delaytime),
                            Loop = Convert.ToInt32(Loop),
                            SendMsgdata = String.Format("ID:{0}|Port:{1}|S| {2}",
                                                   ID.PadLeft(3, ' '),
                                                        PortNum,
                                                        Sequence.Replace(" ", ""))
                        });
                    }
                }

                return Temp;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
                return new List<SendorExecuteSendType>();
            }
        }


        public void WriteDefaultxmlformate()
        {
            #region config flow

            ///
            /// Design SupercarrierMonitoringSystem config file
            ///
            /// Part i System 
            /// 
            SMS = new XmlDocument();
            // 建立目錄的根
            XmlElement Systemroot = SMS.CreateElement("Systemroot");
            SMS.AppendChild(Systemroot); // Add Node.

            /// Part i System 
            /// 
            XmlElement SMSSystemGroup = SMS.CreateElement("SMSSystemGroup");
            Systemroot.AppendChild(SMSSystemGroup);
            XmlElement SystemGroupParams = SMS.CreateElement("Params");
            SMSSystemGroup.AppendChild(SystemGroupParams);
            XmlElement SystemGroupFormat = SMS.CreateElement("SystemIntrospectionIntervalTimer");
            SystemGroupFormat.SetAttribute("Interval", "5000");
            SystemGroupParams.AppendChild(SystemGroupFormat);

            /// Part ii Serialport 
            ///    
            // 建立根目錄的子節點
            XmlElement Serialport = SMS.CreateElement("Serialport");
            Systemroot.AppendChild(Serialport);

            XmlElement SerialporFormatt = SMS.CreateElement("Format");
            Serialport.AppendChild(SerialporFormatt);

            List<string> BaudRates = new List<string>() { "9600", "115200" };
            Paramwrite(SerialporFormatt, BaudRates, nameof(BaudRates));
            List<string> DataReceivedCasenum = new List<string>() { "0", "1" };
            Paramwrite(SerialporFormatt, DataReceivedCasenum, nameof(DataReceivedCasenum));

            // 建立根目錄的子節點
            XmlElement Port;
            XmlElement Paras;

            for (int i = 1; i <= 3; i++)
            {
                Port = SMS.CreateElement("Port" + i.ToString());
                Port.SetAttribute("BaudRate", "115200");
                Port.SetAttribute("DataBits", "8");
                Port.SetAttribute("Parity", "None");
                Port.SetAttribute("StopBits", "One");
                Port.SetAttribute("DtrEnable", "false");
                Port.SetAttribute("RtsEnable", "false");
                Port.SetAttribute("Handshake", "None");
                Port.SetAttribute("DataReceivedCase", "0");
                Serialport.AppendChild(Port);
            }

            SMS.Save(FOLDER_CONFIG + CONFIGNAME);
            SMS = null;
            #endregion
        }



        public void ReadDefaultxmlformate()
        {
            //取得根節點內的子節點
            XmlDocument SMS = new XmlDocument();
            SMS.Load(FOLDER_CONFIG + CONFIGNAME);

            // XmlNodeList lis = SMS.GetElementsByTagName("BaudRate");
            // string str = null;
            // XmlNodeList NodeLists = SMS.SelectNodes("Systemroot/Serialport/Format/BaudRate");
            // foreach (XmlNode node in NodeLists)
            // {
            //     str += node.Name + "," + node.Value + Environment.NewLine;
            // }
            //// string str = lis[0].InnerXml;
            // MessageBox.Show(str);

            string data = null;
            string menudata = null;
            List<string> Serialformat = new List<string>();

            XmlNodeList menuNodeLists = SMS.SelectNodes("Systemroot/Serialport/Format");

            foreach (XmlNode NodeL1 in menuNodeLists)
            {
                foreach (XmlNode NodeL2 in NodeL1.ChildNodes)
                {
                    Serialformat.Add(NodeL2.InnerText);

                }
            }

            XmlNodeList NodeLists = SMS.SelectNodes("Systemroot/Serialport/Format/BaudRate/Item");
            List<string> baudratelis = new List<string>();
            foreach (XmlNode OneNode in NodeLists)
            {
                String StrNodeName = OneNode.Name.ToString();
                foreach (XmlAttribute Attr in OneNode.Attributes)
                {
                    String StrAttr = Attr.Name.ToString();
                    String StrValue = OneNode.Attributes[Attr.Name.ToString()].Value;
                    String StrInnerText = OneNode.InnerText;

                    baudratelis.Add(StrValue);


                    data += StrAttr + "," + StrValue + "," + StrInnerText + Environment.NewLine;
                }
            }

        }

        public void ReadDefaultxmlformate2()
        {
            //取得根節點內的子節點
            XmlDocument doc = new XmlDocument();
            doc.Load(FOLDER_CONFIG + CONFIGNAME);
            //選擇節點
            XmlNode main = doc.SelectSingleNode("Systemroot/Serialport/Protocal/Parameter");
            //XmlNode main = doc.SelectSingleNode("Systemroot/Serialport/Port");

            if (main == null)
                return;

            //取得節點內的欄位
            XmlElement element = (XmlElement)main;

            //取得節點內的"部門名稱"內容
            string data = element.GetAttribute("item");

            //取得節點內的"部門名稱"的屬性
            XmlAttribute attribute = element.GetAttributeNode("item");

            //列舉節點內的屬性
            XmlAttributeCollection attributes = element.Attributes;
            string content = "";
            foreach (XmlAttribute item in attributes)
            {
                content += item.Name + "," + item.Value + Environment.NewLine;
                if (item.Name == "BaudRate")
                    item.Value = "BaudRate";
                if (item.Name == "部門負責人")
                    item.Value = "胎哥郎";
            }

        }

    }
}
