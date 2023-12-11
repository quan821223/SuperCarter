using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using Notification.Wpf;
using SuperCarter.Services;
using SuperCarter.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;
using Brush = System.Windows.Media.Brush;

namespace SuperCarter.Model
{
    public class CustomScriptEditor : ViewModelBase
    {
     
        public CustomScriptEditor()
        {
            Fullloop = 1;
            folderViewerlist = new ObservableCollection<Foldertype>();
            evt_Loadscriptroot(AppPath + @"scripts");
            Viewerpath = AppPath + @"scripts";
            OnPropertyChanged(nameof(folderViewerlist));
            OnPropertyChanged(nameof(Viewerpath));

            blockAfolderViewerlist = new ObservableCollection<IFiletype>();
            ctsScrollingcheck= new CancellationTokenSource();

            /// 燈號設定
            DynamicdisplaySwitch = Visibility.Visible;
            DynamicLabelText = "●";
            DynamicForeColor = new SolidColorBrush(Color.FromRgb(255, 97, 3));
            /// 燈號設定
            SDdisplaySwitch = Visibility.Visible;                                                                                                                         
            SDLabelText = "●";
            SDForeColor = new SolidColorBrush(Color.FromRgb(255, 97, 3));

            strLoopValue = "0";
        }

        ~CustomScriptEditor()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region property

        public bool IsEnableDetectnormCurrent { get; set; } = false;
        public int UpperLimitnormCurrentValue { get; set; } = 0;
        public int LowerLimitnormCurrentValue { get; set; } = 0;
        public bool IsEnableDetectsleepCurrent { get; set; } = false;
        public int UpperLimitsleepCurrentValue { get; set; } = 0;
        public bool IsEnableDetectDiag { get; set; } = false;
        public bool IsEnableDetectLightsensor { get; set; } = false;
        public int UpperLimitLightsensorValue { get; set; } = 0;
        public int LowerLimitLightsensorValue { get; set; } = 0;
        public bool IsEnableTouchfinger { get; set; } = false;
        public bool IsEnableTouchXY { get; set; } = false;
        public string Openblockfilepath { get; set; }
        public CSVfile cSVfile { get; set; } 
        public CSVfile Error_cSVfile { get; set; } 
        private static UnifiedHostCommandSettype UnifiedHostCommandSet { get; set; } =new UnifiedHostCommandSettype();
        public ObservableCollection<IFiletype> blockAfolderViewerlist { get; set; } = new ObservableCollection<IFiletype>();
        public ObservableCollection<Foldertype> folderViewerlist { get; set; } = new ObservableCollection<Foldertype>();
        public string Viewerpath { get; set; }
        public ObservableCollection<ScriptItemtype> ObsColBlockA1Sequences { get; set; } = new ObservableCollection<ScriptItemtype>();
        public ObservableCollection<ScriptItemtype> ObsColBlockA2Sequences { get; set; } = new ObservableCollection<ScriptItemtype>();
        public ObservableCollection<ScriptItemtype> ObsColBlockB1Sequences { get; set; } = new ObservableCollection<ScriptItemtype>();
        public ObservableCollection<ScriptItemtype> ObsColBlockB2Sequences { get; set; } = new ObservableCollection<ScriptItemtype>();
        public int PowerMode { get; set; }
        public int CommnadID { get; set; } = 0;
        private string sendmsg, recmsg;
        private readonly byte[] RESPONSE_TAIL = { 0x0D, 0x0A };
        public int blockAitemcount { get; set; } = 0;
        public int blockBitemcount { get; set; } = 0;
        public int blockCitemcount { get; set; } = 0;
        public int blockDitemcount { get; set; } = 0;
        private bool _IsEnableRollingmode;
        public bool IsEnableRollingmode
        {
            get => _IsEnableRollingmode;
            set {
                _IsEnableRollingmode = value;
                if(_IsEnableRollingmode)
                    DynamicForeColor = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                else
                    DynamicForeColor = new SolidColorBrush(Color.FromRgb(255, 97, 3));
                OnPropertyChanged(nameof(IsEnableRollingmode));
            }
        }
        private bool _IsEnableScheduledDetectmode;
        public bool IsEnableScheduledDetectmode
        {
            get => _IsEnableScheduledDetectmode;
            set
            {
                _IsEnableScheduledDetectmode = value;
                if (_IsEnableRollingmode)
                    SDForeColor = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                else
                    SDForeColor = new SolidColorBrush(Color.FromRgb(255, 97, 3));
                OnPropertyChanged(nameof(IsEnableScheduledDetectmode));
            }
        }
        public string Scriptpath { get; set; }
        public double Estimateruntimefullblock
        {
            get => _Estimateruntimefullblock;
            set
            {
                _Estimateruntimefullblock = Convert.ToDouble(value);
                _Estimateruntimefullblock = 0.0;
                _Estimateruntimefullblock += EstimateruntimeforblockA;
                _Estimateruntimefullblock += EstimateruntimeforblockB;
                _Estimateruntimefullblock *= Fullloop;
            }
        }

        public int Fullloop
        {
            get => _ExecuteFullloop;
            set
            {
                _ExecuteFullloop = value < 1 ? 1 : value;
                Estimateruntimefullblock = 0.0;
                Estimateruntimefullblock += EstimateruntimeforblockA;
                Estimateruntimefullblock += EstimateruntimeforblockB;
                Estimateruntimefullblock *= _ExecuteFullloop;
                OnPropertyChanged(nameof(Estimateruntimefullblock));
                OnPropertyChanged(nameof(Fullloop));
            }
        }
        public string blockA1scriptpath { get; set; }
        public string blockA2scriptpath { get; set; }
        public string blockB1scriptpath { get; set; }
        public string blockB2scriptpath { get; set; }
        public string blockA1initscriptpath { get; set; }
        public string blockA2initscriptpath { get; set; }
        public string blockB1initscriptpath { get; set; }
        public string blockB2initscriptpath { get; set; }

        private int _ExecuteFullloop, _BlockALoop, _BlockBLoop ;
        private double _Estimateruntimefullblock, _EstimateruntimeforblockA, _EstimateruntimeforblockB, _EstimateruntimeforblockC, _EstimateruntimeforblockD;
        public int BlockALoop
        {
            get => _BlockALoop;
            set
            {
                _BlockALoop =  value;
                EstimateruntimeforblockA = (BlockA1Interval + BlockA2Interval) * BlockALoop;
                OnPropertyChanged(nameof(EstimateruntimeforblockA));
                Fullloop = Fullloop;
            }
        }
        public int BlockBLoop
        {
            get => _BlockBLoop;
            set
            {
                _BlockBLoop = value;
                EstimateruntimeforblockB = (BlockB1Interval + BlockB2Interval) * BlockBLoop;
                OnPropertyChanged(nameof(EstimateruntimeforblockB));
                Fullloop = Fullloop;
            }
        }
        private int _EstimateBlockAtotaltime= 0, _EstimateBlockBtotaltime= 0;
        public int EstimateBlockAtotaltime
        {
            get
            {
                _EstimateBlockAtotaltime = (BlockA1Interval + BlockA2Interval) * BlockALoop;
                OnPropertyChanged(nameof(EstimateBlockAtotaltime));
                return _EstimateBlockAtotaltime;
            }
        }
        public int EstimateBlockBtotaltime
        {
            get
            {
                _EstimateBlockBtotaltime = (BlockB1Interval + BlockB2Interval) * BlockBLoop;
                OnPropertyChanged(nameof(EstimateBlockBtotaltime));
                return _EstimateBlockBtotaltime;
            }
        }
        public double EstimateruntimeforblockA
        {
            get => _EstimateruntimeforblockA;
            set
            {
                _EstimateruntimeforblockA = Convert.ToDouble(value);
                _EstimateruntimeforblockA /= 60000;
                _EstimateruntimeforblockA *= BlockALoop;
            }
        }
        public double EstimateruntimeforblockB
        {
            get => _EstimateruntimeforblockB;
            set
            {
                _EstimateruntimeforblockB = Convert.ToDouble(value);
                _EstimateruntimeforblockB /= 60000;
                _EstimateruntimeforblockB *= BlockBLoop;
            }
        }
        private int _MonitoringIntervaltime = 3000;
        public int MonitoringIntervaltime
        {
            get => _MonitoringIntervaltime;
            set
            {
                _MonitoringIntervaltime = value;
                OnPropertyChanged(nameof(MonitoringIntervaltime));
            }
        }

        private int _BlockA1Interval, _BlockA2Interval, _BlockB1Interval, _BlockB2Interval;
        public int BlockA1Interval
        {
            get => _BlockA1Interval;
            set
            {
                _BlockA1Interval = value;
                EstimateruntimeforblockA = _BlockA1Interval;
            }
        }
        public int BlockA2Interval
        {
            get => _BlockA2Interval;
            set
            {
                _BlockA2Interval = value;
                EstimateruntimeforblockB = _BlockA2Interval;
            }
        }
        public int BlockB1Interval
        {
            get => _BlockB1Interval;
            set
            {
                _BlockB1Interval = value;
                EstimateruntimeforblockA = _BlockB1Interval;
            }
        }
        public int BlockB2Interval
        {
            get => _BlockB2Interval;
            set
            {
                _BlockB2Interval = value;
                EstimateruntimeforblockB = _BlockB2Interval;
            }
        }
        #endregion

        #region 參數更新的更新流程 
        public void UpdateDashboardStartThread()
        {
            Thread lowPriorityThread = new Thread(() =>
            {
                while (true)
                {
                    ProcessQueue();

                    // Sleep for a while to prevent busy waiting
                    Thread.Sleep(200);
                }
            });

            lowPriorityThread.Priority = ThreadPriority.Lowest;
            lowPriorityThread.Start();
        }
        private void ProcessQueue()
        {

            while (true)
            {
                while (SendAndReceiveDatabatchQ.TryDequeue(out SendAndReceiveDatabatchcheck bytes))
                {
                    if (bytes != null)
                    {
                        try
                        {
                            ProcessBytes(bytes);
                        }
                        catch (Exception ex)
                        {

                            System.Windows.MessageBox.Show(ex.StackTrace);
                            System.Windows.MessageBox.Show(ex.Message);
                        }
                    }
                }

                // 如果你不打算持續地檢查佇列，那麼這裡可以加上一個小的延遲以避免忙碌等待。
                Thread.Sleep(10);
            }
        }
        private void ProcessBytes(SendAndReceiveDatabatchcheck bytes)
        {
            if (bytes.byte_buffer_Receive.Length > 4 && bytes.byte_buffer_Receive[0] == 0xFA && bytes.byte_buffer_Send[1] == 0x52)
            {
                byte sendByte3 = bytes.byte_buffer_Send[3];
                byte sendByte4 = bytes.byte_buffer_Send[4];
                byte receiveByte2 = bytes.byte_buffer_Receive[2];

                // ... continue with your checks

                switch (sendByte3)
                {
                    case 0x00:
                        HandleBufferSend_00(sendByte4, bytes);
                        break;
                    case 0x01:
                        HandleBufferSend_01(sendByte4, bytes);
                        break;
                    case 0x04:
                        HandleBufferSend_04(sendByte4, bytes);
                        break;
                    case 0x05:
                        HandleBufferSend_05(sendByte4, bytes);
                        break;
                    case 0x06:
                        HandleBufferSend_06(sendByte4, bytes);
                        break;
                }

                App.Current.Dispatcher.Invoke(() =>
                {
                    updateUIobj();      // 刷新項目
                });
            }
            if(IsEnableScheduledDetectmode && bytes.byte_buffer_Send.Length > 0 && bytes.strSequenceData_Rec.Length < 1)
            {
                Error_cSVfile.ErrordataAppendTocsv(UnifiedHostCommandSet, bytes);
            }
            

        }
        #endregion

        #region icommand event
        private ICommand _LoadscripttoBlockA, _LoadscripttoBlockB, _LoadscripttoBlockC, _LoadscripttoBlockD, _ExecuteRollingmode, _ExecuteScheduledDetectgmode, _SaveDynamicMonitorresult;
        // IsEnableScheduledDetectmode
        public ICommand ExecuteRollingmode
        {
            get {
                _ExecuteRollingmode = new RelayCommand(
                    param => evt_ExecuteRollingmode());
                return _ExecuteRollingmode;
            }
        
        }
        public ICommand ExecuteScheduledDetectgmode
        {
            get
            {
                _ExecuteScheduledDetectgmode = new RelayCommand(
                    param => evt_ExecuteSheduleddetectmode());
                return _ExecuteScheduledDetectgmode;
            }

        }

        private ICommand _SettinViewPath, _ReleasetoRefresh;
        public ICommand SettinViewPath
        {
            get
            {
                _SettinViewPath = new RelayCommand(
                                param => evt_selectViewpath());
                return _SettinViewPath;
            }
        }
        private ICommand _SelectScriptpathCommand;
        public ICommand SelectScriptpathCommand
        {
            get
            {
                _SelectScriptpathCommand = new RelayCommand(
                    param => evt_SelectedScriptItem((IFiletype)param));
                return _SelectScriptpathCommand;
            }
        }
        public ICommand ReleasetoRefresh
        {
            get
            {
                _ReleasetoRefresh = new RelayCommand(
                    param => evt_ReleasetoRefresh());
                return _ReleasetoRefresh;
            }
        }

        #endregion

        #region Scheduled script runtime 
        #region properties
        public string Scheduledscriptpath { get; set; }
        public static List<SendorExecuteSendType> BlockA1SequencesList { get; set; } = new List<SendorExecuteSendType>();
        public static List<SendorExecuteSendType> BlockA2SequencesList { get; set; } = new List<SendorExecuteSendType>();
        public static List<SendorExecuteSendType> BlockB1SequencesList { get; set; } = new List<SendorExecuteSendType>();
        public static List<SendorExecuteSendType> BlockB2SequencesList { get; set; } = new List<SendorExecuteSendType>();
        public static List<SendorExecuteSendType> BlockA1initSequencesList { get; set; } = new List<SendorExecuteSendType>();
        public static List<SendorExecuteSendType> BlockA2initSequencesList { get; set; } = new List<SendorExecuteSendType>();
        public static List<SendorExecuteSendType> BlockB1initSequencesList { get; set; } = new List<SendorExecuteSendType>();
        public static List<SendorExecuteSendType> BlockB2initSequencesList { get; set; } = new List<SendorExecuteSendType>();

        private int _CurLoopValue;
        public int CurLoopValue
        {
            get => _CurLoopValue;
            set
            {
                _CurLoopValue = value;
                float temp = (CurLoopValue * 1.0f / Fullloop) * 100;
                PercentLoopValue = Convert.ToInt32(temp);
                strLoopValue = PercentLoopValue.ToString();
                OnPropertyChanged(nameof(strLoopValue));
            }
        }
        private string _strLoopValue;
        public string strLoopValue
        {
            get { return _strLoopValue; }
            set
            {
                _strLoopValue = string.Format("{0}%", PercentLoopValue);
                OnPropertyChanged(nameof(strLoopValue));
            }
        }
        private string _Curphase;
        public string Curphase
        {
            get => _Curphase;
            set
            {
                _Curphase = value;
                OnPropertyChanged(nameof(Curphase));
            }
        }
        private int _subloop;
        public int subloop
        {
            get => _subloop;
            set
            {
                _subloop = value;
                OnPropertyChanged(nameof(subloop));
            }
        }
        public int PercentLoopValue { get; set; }
        #endregion

        #region icommand
        private ICommand _LoadScheduledscript;
        public ICommand LoadScheduledscript {
            get
            {
                _LoadScheduledscript = new RelayCommand(
                    param => evt_LoadScheduledscript());
                return _LoadScheduledscript;
            }
        }
        #endregion

        #region event
        private void evt_LoadScheduledscript()
        {
            Scheduledscriptpath = evt_Openfile();
            ImportBlockScript(Scheduledscriptpath);
            OnPropertyChanged(nameof(Scheduledscriptpath));
        }
        public void ImportBlockScript(string ScriptPath)
        {
            try
            {
                // Loading from a file, you can also load from a stream

                XmlDocument ScriptionXML = new XmlDocument();

                ScriptionXML.Load(ScriptPath);
                XmlNode root = ScriptionXML.SelectSingleNode("TestSuites");

                if (root is not null)
                {
                    Fullloop = Convert.ToInt32(root.Attributes["Loop"]?.Value ?? "0");
                    BlockALoop = Convert.ToInt32(root.Attributes["BlockALoop"]?.Value);
                    BlockBLoop = Convert.ToInt32(root.Attributes["BlockBLoop"]?.Value);
                    BlockA1Interval = Convert.ToInt32(root.Attributes["BlockA1Interval"]?.Value);
                    BlockA2Interval = Convert.ToInt32(root.Attributes["BlockA2Interval"]?.Value);
                    BlockB1Interval = Convert.ToInt32(root.Attributes["BlockB1Interval"]?.Value);
                    BlockB2Interval = Convert.ToInt32(root.Attributes["BlockB2Interval"]?.Value);
                    MonitoringIntervaltime = Convert.ToInt32(root.Attributes["MonitoringIntervaltime"]?.Value?? "3000");
                    List<string> list = new List<string>() {"TestSuiteA1init", "TestSuiteA1", "TestSuiteA2init", "TestSuiteA2",
                                                        "TestSuiteB1init", "TestSuiteB1", "TestSuiteB2init", "TestSuiteB2",
                                                         };
                    string Threblock = "TestSuites/ThresholdSetting";
                    XmlNode blocknode = ScriptionXML.SelectSingleNode(Threblock);

                    foreach (XmlElement node in blocknode)
                    {
                        if (node.Name == "DetectDiag")
                        {
                            IsEnableDetectDiag = Convert.ToBoolean(node.Attributes["IsEnable"].Value);
                        }
                        else if (node.Name == "NormCurrent")
                        {
                            IsEnableDetectnormCurrent = Convert.ToBoolean(node.Attributes["IsEnable"].Value ?? "false");
                            UpperLimitnormCurrentValue = Convert.ToInt32(node.Attributes["Upper"].Value);
                            LowerLimitnormCurrentValue = Convert.ToInt32(node.Attributes["Lower"].Value);
                        }
                        else if (node.Name == "SleepCurrent")
                        {
                            IsEnableDetectsleepCurrent = Convert.ToBoolean(node.Attributes["IsEnable"].Value ?? "false");
                            UpperLimitsleepCurrentValue = Convert.ToInt32(node.Attributes["Upper"].Value);
                        }
                        else if (node.Name == "Lightsensor")
                        {
                            IsEnableDetectLightsensor = Convert.ToBoolean(node.Attributes["IsEnable"].Value ?? "false");
                            UpperLimitLightsensorValue = Convert.ToInt32(node.Attributes["Upper"].Value);
                            LowerLimitLightsensorValue = Convert.ToInt32(node.Attributes["Lower"].Value);
                        }
                        else if (node.Name == "Touchfinger")
                        {
                            IsEnableTouchfinger = Convert.ToBoolean(node.Attributes["IsEnable"].Value ?? "false");
                        }
                        else if (node.Name == "TouchXY")
                        {
                            IsEnableTouchXY = Convert.ToBoolean(node.Attributes["IsEnable"].Value ?? "false");
                        }
                    }


                    foreach (var ite in list)
                    {


                        string strblock = "TestSuites/" + ite + "/TestSequence";
                        XmlNode block = ScriptionXML.SelectSingleNode(strblock);
                        string strrequisites = "TestSuites/" + ite + "/Prerequisites";
                        XmlNode requisites = ScriptionXML.SelectSingleNode(strrequisites);

                        XmlElement element = (XmlElement)block;
                        ObservableCollection<ScriptItemtype> obsTemp = new ObservableCollection<ScriptItemtype>();
                        List<SendorExecuteSendType> Temp = new List<SendorExecuteSendType>();
                        //取得節點內的欄位
                        foreach (XmlElement node in element)
                        {
                            String ID = node.Attributes["ID"].Value ?? "";
                            String PortNum = node.Attributes["PortNum"]?.Value ?? "all";
                            String Nodename = node.Attributes["Nodename"]?.Value ?? "";
                            String MSGname = node.Attributes["MSGname"]?.Value ?? "";
                            String Sequence = node.Attributes["Sequence"]?.Value ?? "";
                            String Delaytime = node.Attributes["Delaytime"]?.Value ?? "200";
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
                                    strSequenceData = Sequence,
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

                        if (ite == "TestSuiteA1init")
                        {
                            blockA1initscriptpath = requisites.Attributes["Path"]?.Value ?? "";
                            BlockA1initSequencesList = Temp;
                        }
                        else if (ite == "TestSuiteA1")
                        {
                            blockA1scriptpath = requisites.Attributes["Path"]?.Value ?? "";
                            BlockA1SequencesList = Temp;
                        }
                        else if (ite == "TestSuiteA2init")
                        {
                            blockA2initscriptpath = requisites.Attributes["Path"]?.Value ?? "";
                            BlockA2initSequencesList = Temp;
                        }
                        else if (ite == "TestSuiteA2")
                        {
                            blockA2scriptpath = requisites.Attributes["Path"]?.Value ?? "";
                            BlockA2SequencesList = Temp;
                        }
                        else if (ite == "TestSuiteB1init")
                        {
                            blockB1initscriptpath = requisites.Attributes["Path"]?.Value ?? "";
                            BlockB1initSequencesList = Temp;
                        }
                        else if (ite == "TestSuiteB1")
                        {
                            blockB1scriptpath = requisites.Attributes["Path"]?.Value ?? "";
                            BlockB1SequencesList = Temp;
                        }
                        else if (ite == "TestSuiteB2init")
                        {
                            blockB2initscriptpath = requisites.Attributes["Path"]?.Value ?? "";
                            BlockB2initSequencesList = Temp;
                        }
                        else if (ite == "TestSuiteB2")
                        {
                            blockB2scriptpath = requisites.Attributes["Path"]?.Value ?? "";
                            BlockB2SequencesList = Temp;
                        }
                    }

                }



                //return _ScriptEditor;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                //return new ScriptEditor();
            }
        }

        #endregion

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try 
            {
                string path = string.Format("{0}\\{1}_{2}", FOLDER_RESULT, DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"), "-outputtestdata.csv");           
                cSVfile = new CSVfile( path); // 請替換為您希望保存文件的路徑
                cSVfile.SetCSVfileStoragepath(path);

                string error_path = string.Format("{0}\\{1}_{2}", FOLDER_RESULT, DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"), "-Errordata.csv");            
                Error_cSVfile = new CSVfile( error_path);
                Error_cSVfile.SetCSVfileStoragepath(error_path);

                CommnadID = 0;

                logger.Log(NLog.LogLevel.Trace, "– Start Custom script schedule.");
                var Startmsg = "– Start Custom script schedule.";
                WritedataToViewTextAggregator.Instance.Updatemsg(new RealtimeMsgQueuetype { msgtype = Msgtype.Message, msg = Startmsg });

                for (CurLoopValue = 0; CurLoopValue < Fullloop; CurLoopValue++) // 總迴圈
                {
                    var msg = string.Format("- Currently on iteration : {0}", CurLoopValue);
                    logger.Log(NLog.LogLevel.Trace, msg);
                    WritedataToViewTextAggregator.Instance.Updatemsg(new RealtimeMsgQueuetype { msgtype = Msgtype.Message, msg = msg });

                    // 如果token已被取消，跳出迴圈
                    if (cancellationToken.IsCancellationRequested)
                        break;
                    OnPropertyChanged(nameof(Fullloop));
                    OnPropertyChanged(nameof(CurLoopValue));
                    OnPropertyChanged(nameof(PercentLoopValue));
                    await BlockWorkstation("A", BlockA1initSequencesList, BlockA2initSequencesList, BlockA1SequencesList, BlockA2SequencesList, BlockALoop, BlockA1Interval, BlockA2Interval, cancellationToken);
                    await BlockWorkstation("B", BlockB1initSequencesList, BlockB2initSequencesList, BlockB1SequencesList, BlockB2SequencesList, BlockBLoop, BlockB1Interval, BlockB2Interval, cancellationToken);

                }
                ObjectAggregator.Instance.UpdateObject();

                SDForeColor = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                IsEnableScheduledDetectmode = false;

                OnPropertyChanged(nameof(Fullloop));
                OnPropertyChanged(nameof(CurLoopValue));
                OnPropertyChanged(nameof(PercentLoopValue));
                OnPropertyChanged(nameof(IsEnableScheduledDetectmode));

                logger.Log(NLog.LogLevel.Trace, "– End Custom script schedule.");

                var Endmmsg = "– End Custom script schedule.";
                WritedataToViewTextAggregator.Instance.Updatemsg(new RealtimeMsgQueuetype { msgtype = Msgtype.Message, msg = Endmmsg });
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error");
                System.Windows.MessageBox.Show(ex.StackTrace, "Error");
            }
            finally
            {
     
            }

        }

        private async Task BlockWorkstation(string blockName, List<SendorExecuteSendType> initsequences1, List<SendorExecuteSendType> initsequences2,
            List<SendorExecuteSendType> sequences1, List<SendorExecuteSendType> sequences2,
                                             int maxLoop, int Block1Interval, int Block2Interval, CancellationToken cancellationToken)
        {
            try 
            {
                //cancellationToken.ThrowIfCancellationRequested();
                int blockcycletime = Block1Interval + Block2Interval;
          

                for (int curLoop = 0; curLoop < maxLoop; curLoop++)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;
                    //cancellationToken.ThrowIfCancellationRequested();
                    subloop = curLoop;

                    logger.Log(NLog.LogLevel.Trace, $"– Enter {blockName}-{curLoop} step.");
                    WritedataToViewTextAggregator.Instance.Updatemsg(new RealtimeMsgQueuetype { msgtype = Msgtype.Message, msg = $"– Enter {blockName}-{curLoop} step." });
                    // 
                    var blockStartTime = DateTime.Now;

                    if (Block1Interval > 0)
                    {
                        var block1StartTime = DateTime.Now;
                        await ExecuteBlockinitSequences($"{blockName}-{1} initial", initsequences1, cancellationToken);
                        var remainingblock1SpentTime = Block1Interval - (DateTime.Now - block1StartTime).TotalMilliseconds;
                        // Execute sequences1 within cycletime
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            var seq1Task = ExecuteBlockSequences($"{blockName}-{1}", sequences1, (int)remainingblock1SpentTime, cancellationToken, curLoop, MonitoringIntervaltime);
                            await seq1Task; // Wait for seq1Task to complete
                        }                    

                    }

                    if (Block2Interval > 0)
                    {
                        var block2StartTime = DateTime.Now;
                        await ExecuteBlockinitSequences($"{blockName}-{2} initial", initsequences2, cancellationToken);
                        var remainingblock2SpentTime = Block2Interval - (DateTime.Now - block2StartTime).TotalMilliseconds;
                        // Execute sequences2 within cycletime
                        if (!cancellationToken.IsCancellationRequested)
                        {
                            var seq2Task = ExecuteBlockSequences($"{blockName}-{2}", sequences2, (int)remainingblock2SpentTime, cancellationToken, curLoop, MonitoringIntervaltime);
                            await seq2Task; // Wait for seq2Task to complete

                        }

                    }
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    // Check if block working time is reached
                    if ((DateTime.Now - blockStartTime).TotalMilliseconds >= blockcycletime * maxLoop)
                    {
                        break; // Exit the loop
                    }
                }
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error");
                System.Windows.MessageBox.Show(ex.StackTrace, "Error");
            }
            finally
            {

            }

        }
        private async Task ExecuteBlockinitSequences(string blockName, List<SendorExecuteSendType> sequences,  CancellationToken cancellationToken)
        {
            try
            {
                //cancellationToken.ThrowIfCancellationRequested();
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var Sendorwatch = new Stopwatch();
                Curphase = blockName;
                var msg1 = $"- {blockName}";
                logger.Log(NLog.LogLevel.Trace, msg1);
                WritedataToViewTextAggregator.Instance.Updatemsg(new RealtimeMsgQueuetype { msgtype = Msgtype.Message, msg = msg1 });
                // 

                for (int i = 0; i < sequences.Count; i++)
                {
                    //cancellationToken.ThrowIfCancellationRequested();
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    if (sequences[i].PortNum == 9)
                    {
                        var roundtimedelay = sequences[i].Delaytime;

                        Sendorwatch.Restart();
                        for (int j = 0; j < 3; j++)
                        {
                            i++;
                            if (i >= sequences.Count || cancellationToken.IsCancellationRequested)
                            {
                                break; // Exit the loop
                            }
                            await SendAndReceivesAsync(sequences[i], cancellationToken);
                        }

                        Sendorwatch.Stop();
                        var remainingSpentTime = roundtimedelay - (int)Sendorwatch.ElapsedMilliseconds;

                        if (remainingSpentTime > 0)
                        {
                            await Task.Delay(remainingSpentTime, cancellationToken);
                        }
                    }
                    else
                    {
                        var roundtimedelay = sequences[i].Delaytime;
                        Sendorwatch.Restart();
                        await SendAndReceivesAsync(sequences[i], cancellationToken);
                        Sendorwatch.Stop();
                        var remainingSpentTime = roundtimedelay - (int)Sendorwatch.ElapsedMilliseconds;

                        if (remainingSpentTime > 0)
                        {
                            await Task.Delay(remainingSpentTime, cancellationToken);
                        }
                    }
                }
      
            }
            catch (TaskCanceledException)
            {
             
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            finally
            {

            }
        }
        private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private async Task ExecuteBlockSequences(string blockName, List<SendorExecuteSendType> sequences, int scriptDelayTime, CancellationToken cancellationToken, int curLoop, int cycletime )
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();          

            try
            {
                //await semaphoreSlim.WaitAsync(); // 等待許可
                cancellationToken.Register(() => cancellationTokenSource.Cancel());
             
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var msg1 = $"- {blockName} 進入偵測階段";
                logger.Log(NLog.LogLevel.Trace, msg1);
                WritedataToViewTextAggregator.Instance.Updatemsg(new RealtimeMsgQueuetype { msgtype = Msgtype.Message, msg = msg1 });
                var blockStartTime = DateTime.Now;
                Curphase = blockName;
                while ((DateTime.Now - blockStartTime).TotalMilliseconds < scriptDelayTime)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    var cycleStartTime = DateTime.Now;
                    int sequenceIndex = 0;

                    while ((DateTime.Now - cycleStartTime).TotalMilliseconds < cycletime)
                    {
                        //cancellationToken.ThrowIfCancellationRequested();
                        if (cancellationToken.IsCancellationRequested )
                        {
                            break;
                        }
                         

                        var elapsedTime = (DateTime.Now - cycleStartTime).TotalMilliseconds;
                        var remainingTime = cycletime - (int)elapsedTime;

                        if (sequenceIndex < sequences.Count)
                        {
                            if (sequences[sequenceIndex].PortNum == 9)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    sequenceIndex++;
                                    if (sequenceIndex >= sequences.Count || cancellationToken.IsCancellationRequested)
                                    {
                                        break; // Exit the loop
                                    }
                                    await SendAndReceivesAsync(sequences[sequenceIndex], cancellationToken);
                                }
                            }
                            else
                            {
                                await SendAndReceivesAsync(sequences[sequenceIndex], cancellationToken);
                                sequenceIndex++;
                            }
                        }

                        if (sequenceIndex >= sequences.Count -1)
                        {
                            UnifiedHostCommandSet.Time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ffff");
                            UnifiedHostCommandSet.Loop = CurLoopValue.ToString();
                            UnifiedHostCommandSet.Blockphase = Curphase.ToString();
                            UnifiedHostCommandSet.Blockloop = curLoop.ToString();
                            cSVfile.AppendToCsv(UnifiedHostCommandSet);
                            UnifiedHostCommandSet = new UnifiedHostCommandSettype();

                            // Check if scriptDelayTime has already expired
                            var remainingScriptTime = scriptDelayTime - (int)(DateTime.Now - blockStartTime).TotalMilliseconds;
                            if (remainingScriptTime <= 0)
                            {
                                return;
                            }
                            await Task.Delay(Math.Min(remainingTime, remainingScriptTime), cancellationToken);
                        }
                    }

                    UnifiedHostCommandSet.Time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ffff");
                    UnifiedHostCommandSet.Loop = CurLoopValue.ToString();
                    UnifiedHostCommandSet.Blockphase = Curphase.ToString();
                    UnifiedHostCommandSet.Blockloop = curLoop.ToString();
                    cSVfile.AppendToCsv(UnifiedHostCommandSet);
                    UnifiedHostCommandSet = new UnifiedHostCommandSettype();

                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            finally
            {
             
            }

        }
        #endregion

        #region DynamicMonitorCheck
        public string SDMChecklistscriptXMLPath { get; set; } = "";
        public ICommand SaveDynamicMonitorresult
        {
            get
            {
                _SaveDynamicMonitorresult = new RelayCommand(
                    param => SaveDynamicMonitorResult());
                return _SaveDynamicMonitorresult;
            }
        }
        private ICommand _LoadSDMchecklistScript;
        public ICommand LoadSDMchecklistScript
        {
            get
            {
                _LoadSDMchecklistScript = new RelayCommand(
                      param => LoadSDMchecklistScriptXMLfile());
                return _LoadSDMchecklistScript;
            }
        }

        public  CancellationTokenSource ctsScrollingcheck;
        public  CancellationTokenSource ctsExecutecheck;
        public CancellationToken ctsExecutecheckToken;

        #region 燈號設置
        /// <summary>
        /// 燈號控制
        /// </summary>
        public Visibility DynamicdisplaySwitch { get; set; } = Visibility.Hidden;
        public Visibility SDdisplaySwitch { get; set; } = Visibility.Hidden;
        public string SDLabelText { get; set; }
        public string DynamicLabelText { get; set; }
        private Brush _DynamicForeColor, _SDForeColor;

        public Brush DynamicForeColor
        {
            get => _DynamicForeColor;
            set
            {
                _DynamicForeColor = value;
                OnPropertyChanged(nameof(DynamicForeColor));   
            } 
        }
        public Brush SDForeColor
        {
            get => _SDForeColor;
            set
            {
                _SDForeColor = value;
                OnPropertyChanged(nameof(SDForeColor));
            }
        }
        #endregion

        #region 模式運行端口
        /// <summary>
        /// 運行動態顯示
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartScrollingCheck(CancellationToken cancellationToken)
        {
            bool workingBreak = false;
        
            try
            {
                CommnadID = 0;
    
                List<SendorExecuteSendType> sequences = SendorExecuteSequencesList;
                int sequenceIndex = 0;
                var Sendorwatch = new Stopwatch();

                // 將燈號設定成未完成狀態
                DynamicForeColor = new SolidColorBrush(Color.FromRgb(255, 97, 3));

                while (!cancellationToken.IsCancellationRequested)
                {
                    if (sequences[sequenceIndex].PortNum == 9)
                    {
                        var roundTimeDelay = sequences[sequenceIndex].Delaytime;
                        Sendorwatch.Restart();

                        for (int j = 0; j < 3; j++)
                        {
                            sequenceIndex++;

                            if (sequenceIndex >= sequences.Count || cancellationToken.IsCancellationRequested)
                            {
                                workingBreak = true;
                                break; // Exit the loop
                            }

                            await SendAndReceivesAsync(sequences[sequenceIndex], cancellationToken);
                        }

                        Sendorwatch.Stop();
                        var remainingSpentTime = roundTimeDelay - (int)Sendorwatch.ElapsedMilliseconds;

                        if (remainingSpentTime > 0)
                        {
                            await Task.Delay(remainingSpentTime, cancellationToken);
                        }
                    }
                    else
                    {
                        await SendAndReceivesAsync(sequences[sequenceIndex], cancellationToken);
                        sequenceIndex++;
                    }

                    if (sequenceIndex >= sequences.Count)
                    {
                   
                        sequenceIndex = 0;
                        IsEnableRollingmode = false;                
                        DynamicForeColor = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                        break;
                    }
                }
            }
            catch (TaskCanceledException)
            {
                workingBreak = true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            finally
            {
                // 檢查 CancellationToken 是否已觸發取消操作
                if (workingBreak || cancellationToken.IsCancellationRequested)
                {
                    System.Windows.MessageBox.Show("運行任務已被取消");
                   
                }
            }
        }
        #endregion

        #region Event
        public void LoadSDMchecklistScriptXMLfile()
        {
            SDMChecklistscriptXMLPath = ConfigModel.Instance.GetStrScriptpath();

            if (!string.IsNullOrWhiteSpace(SDMChecklistscriptXMLPath))
            {
                OnPropertyChanged(nameof(SDMChecklistscriptXMLPath));
                SendorExecuteSequencesList = new List<SendorExecuteSendType>();
                SendorExecuteSequencesList = ConfigModel.Instance.GetSendorExecuteSequencesList(SDMChecklistscriptXMLPath);

            }
            else
            {
                System.Windows.MessageBox.Show("未偵測到腳本路徑", "提醒 !", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        public void SaveDynamicMonitorResult()
        {
            try
            {
                var FOLDER_RESULT = System.Windows.Forms.Application.StartupPath + @"\result\Dynamic";
                string defaultpath = string.Format("{0}\\{1}_{2}", FOLDER_RESULT, DateTime.Now.ToString("yyyyMMddHHmmss"), "_dataOutput.csv");

                cSVfile = new CSVfile(defaultpath);
                cSVfile.SetCSVfileStoragepath(defaultpath);
                cSVfile.WriteToCsv(UnifiedHostCommandSet);
                IsEnableRollingmode = false;
                OnPropertyChanged(nameof(IsEnableRollingmode));
                // 開啟檔案位置
                string dataPath = AppPath + @"\result\Dynamic";
                Process.Start(new ProcessStartInfo { FileName = dataPath, UseShellExecute = true });
            }
            catch (Exception ex)
            {
      
                System.Windows.MessageBox.Show(ex.StackTrace);
                System.Windows.MessageBox.Show(ex.Message);
            }

        }
        #endregion


        #endregion

        #region Scheduled detection cmd

        #endregion

        #region event
        private async Task SendAndReceivesAsync(object Sequence, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var command = (SendorExecuteSendType)Sequence;

            if (!DicSerialPort[command.PortNum].IsOpen)
                return;

            byte[] receivedData = new byte[0];

            if (command.SequenceData[0] == 0xFA && command.SequenceData[3] == 0x02 && command.SequenceData[4] == 0x01)
            {
                PowerMode = 1;
                // UnifiedHostCommandSet = new UnifiedHostCommandSettype();
                UnifiedHostCommandSet.DUT1PowerMode = "Normal";
                UnifiedHostCommandSet.DUT2PowerMode = "Normal";
                UnifiedHostCommandSet.DUT3PowerMode = "Normal";
             
            }

            else if (command.SequenceData[0] == 0xFA && command.SequenceData[3] == 0x02 && command.SequenceData[4] == 0x00)
            {
                PowerMode = 0;
                // UnifiedHostCommandSet = new UnifiedHostCommandSettype();
                UnifiedHostCommandSet.DUT1PowerMode = "Sleep";
                UnifiedHostCommandSet.DUT2PowerMode = "Sleep";
                UnifiedHostCommandSet.DUT3PowerMode = "Sleep";
          
            }

            String OutputMsg = String.Format("{0}|{1}|{2}| S | CommandID {4}| {3}",
                DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ffff"),
                SerialPortModel.Instance.PortNameBinding[DicSerialPort[command.PortNum].PortName].ToString().PadLeft(2, ' '),
                DicSerialPort[command.PortNum].PortName.PadLeft(6, ' '),
                command.strSequenceData.Replace(" ", ""),
                CommnadID.ToString().PadLeft(6, ' ')
                );
            logger.Log(NLog.LogLevel.Trace, OutputMsg);
            WritedataToViewTextAggregator.Instance.Updatemsg(new RealtimeMsgQueuetype { msgtype = Msgtype.FromPort, PortNum = command.PortNum, msg = OutputMsg });

          

            DicSerialPort[command.PortNum].Write(command.SequenceData, 0, command.SequenceData.Length);

            // 紀錄讀取 sdm 回傳的資料
            receivedData = await ReadFromPortAsync(Sequence, cancellationToken);

            // update to Dashboard
            if (receivedData.Length > 0)
                RealtimeSDMDataQueue.Enqueue(receivedData);

            
            recmsg = SerialPortModel.Instance.byteToHexStr(receivedData);

            OutputMsg = String.Format("{0}|{1}|{2}| R | CommandID {4}| {3}",
                DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ffff"),
                SerialPortModel.Instance.PortNameBinding[DicSerialPort[command.PortNum].PortName].ToString().PadLeft(2, ' '),
                DicSerialPort[command.PortNum].PortName.PadLeft(6, ' '),
                recmsg.Replace(" ", ""),
              CommnadID.ToString().PadLeft(6, ' ')
                );
            // update to nlog file
            logger.Log(NLog.LogLevel.Trace, OutputMsg);

            WritedataToViewTextAggregator.Instance.Updatemsg(new RealtimeMsgQueuetype { msgtype = Msgtype.FromPort, PortNum = command.PortNum, msg = OutputMsg });
            SendAndReceiveDatabatchQ.Enqueue(new SendAndReceiveDatabatchcheck()
            {
                CommnadID = CommnadID,
                Portnum = command.PortNum,
                strSequenceData_send = command.strSequenceData,
                strSequenceData_Rec = recmsg,
                byte_buffer_Send = command.SequenceData,
                byte_buffer_Receive = receivedData,
            });

            CommnadID += 1;
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
        }

        private async Task<byte[]> ReadFromPortAsync(object Sequence, CancellationToken cancellationToken)
        {
            var command = (SendorExecuteSendType)Sequence;
            const int bufferSize = 1024;
            byte[] buffer = new byte[bufferSize];
            int bufferIndex = 0;
            string data;
            using (var cts = new CancellationTokenSource(300))
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    // 檢查是否有數據可讀
                    if (DicSerialPort[command.PortNum].BytesToRead > 0)
                    {
                        var readByte = (byte)DicSerialPort[command.PortNum].ReadByte();

                        // 檢查是否超過緩存大小
                        if (bufferIndex >= bufferSize)
                        {
                            logger.Log(NLog.LogLevel.Warn, "Buffer overflow detected. Exiting.");
                            break;
                        }

                        buffer[bufferIndex++] = readByte;

                        if (buffer[0] == 0xFA && bufferIndex > 2)
                        {
                            if (buffer[2] - 2 <= bufferIndex)
                            {
                                // 檢查是否收到期望的字元
                                if (bufferIndex > 1 && buffer[bufferIndex - 2] == RESPONSE_TAIL[0] && buffer[bufferIndex - 1] == RESPONSE_TAIL[1])
                                {
                                    byte[] resultBuffer = new byte[bufferIndex];
                                    Array.Copy(buffer, resultBuffer, bufferIndex);

                                    return resultBuffer;
                                }
                            }
                        }
                        else
                        {
                            // 檢查是否收到期望的字元
                            if (bufferIndex > 1 && buffer[bufferIndex - 2] == RESPONSE_TAIL[0] && buffer[bufferIndex - 1] == RESPONSE_TAIL[1])
                            {
                                byte[] resultBuffer = new byte[bufferIndex];
                                Array.Copy(buffer, resultBuffer, bufferIndex);

                                return resultBuffer;
                            }
                        }
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                    else
                    {
                        await Task.Delay(10, cancellationToken);
                    }
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                }

                // 如果超時且沒有得到預期響應
                if (bufferIndex > 0)
                {
                    logger.Log(NLog.LogLevel.Trace, "Timeout without receiving expected response. Partial data returned.");
                    Array.Resize(ref buffer, bufferIndex);
                    return buffer;
                }
                else
                {
                    logger.Log(NLog.LogLevel.Trace, "Timeout without any data received.");
                    return Array.Empty<byte>();
                }
            }
        }

        /// <summary>
        /// Rolling mode 
        /// </summary>
        private void evt_ExecuteRollingmode()
        {
            if (IsEnableRollingmode)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (DicSerialPort[i].IsOpen)
                    {
                        DicSerialPort[i].DataReceived -= new SerialDataReceivedEventHandler(SerialPortModel.Instance.DataReceivedCom);
                        DicSerialPort[i].DiscardInBuffer();
                        DicSerialPort[i].DiscardOutBuffer();
                    }
                }

                if (SendorExecuteSequencesList.Count <1 || string.IsNullOrEmpty(SDMChecklistscriptXMLPath) || ! (DicSerialPort[0].IsOpen || DicSerialPort[1].IsOpen || DicSerialPort[2].IsOpen ))
                {
                    IsEnableRollingmode = false;
                    OnPropertyChanged(nameof(IsEnableRollingmode));
                }                      
                else
                {
                    UnifiedHostCommandSet = new UnifiedHostCommandSettype();
                    UnifiedHostCommandSet.IsEnableExecuteSDMcheck = true;
                    ctsScrollingcheck = new CancellationTokenSource();
                    StartScrollingCheck(ctsScrollingcheck.Token);
                }         
            }
            else
            {
                // 取消 Rolling Mode 時重置 UnifiedHostCommandSet
                UnifiedHostCommandSet = new UnifiedHostCommandSettype();
                UnifiedHostCommandSet.IsEnableExecuteSDMcheck = false;
                IsEnableRollingmode = false;
                ctsScrollingcheck.Cancel();
            }
        }

        private void evt_ExecuteSheduleddetectmode()
        {
            if (IsEnableScheduledDetectmode)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (DicSerialPort[i].IsOpen)
                    {
                        DicSerialPort[i].DataReceived -= new SerialDataReceivedEventHandler(SerialPortModel.Instance.DataReceivedCom);
                        DicSerialPort[i].DiscardInBuffer();
                        DicSerialPort[i].DiscardOutBuffer();
                    }
                }

                if (string.IsNullOrEmpty(Scheduledscriptpath) || !(DicSerialPort[0].IsOpen || DicSerialPort[1].IsOpen || DicSerialPort[2].IsOpen))
                {
                    IsEnableScheduledDetectmode = false;
                    OnPropertyChanged(nameof(IsEnableRollingmode));
                }
                else
                {
                    UnifiedHostCommandSet = new UnifiedHostCommandSettype();
                    UnifiedHostCommandSet.IsEnableExecuteSDMcheck = false;
                 
                    if (ctsExecutecheck == null || ctsExecutecheck.Token.IsCancellationRequested)
                    {
                        ctsExecutecheck = new CancellationTokenSource();
                        ctsExecutecheckToken = ctsExecutecheck.Token;
                    }
                    StartAsync(ctsExecutecheckToken);
                }
            }
            else
            {
                if (ctsExecutecheck != null)
                {
                    ctsExecutecheck.Cancel();
                }
            }
        }
        private void evt_SelectedScriptItem(IFiletype _va)
        {
            blockA1scriptpath = _va.FullPathName;
            blockAfolderViewerlist.Add(new IFiletype
            {
                FriendlyName = _va.FriendlyName,
                FullPathName = _va.FullPathName,
                IncludeFileChildren = false,
                IsExpanded = false,

            });
        }

        public void evt_selectViewpath()
        {
            string myPath = AppPath;
            string PATHDDSSCRIPT = null;
            string foldername = null;
            folderViewerlist = new ObservableCollection<Foldertype>();
            using (var openFileDialog1 = new System.Windows.Forms.FolderBrowserDialog())
            {
                openFileDialog1.Description = "請選擇欲顯示的資料夾路徑";
                // 設定OpenFileDialog屬性
                openFileDialog1.SelectedPath = myPath;

                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    PATHDDSSCRIPT = openFileDialog1.SelectedPath; //取得檔名
                    var strarr = PATHDDSSCRIPT.Split('\\').ToList();
                    foldername = strarr[strarr.Count - 1];
                }
                evt_Loadscriptroot(PATHDDSSCRIPT);
                Viewerpath = PATHDDSSCRIPT;
            }
            OnPropertyChanged(nameof(folderViewerlist));
            OnPropertyChanged(nameof(Viewerpath));
        }

        public void evt_Loadscriptroot(string _path)
        {
            string foldername;
            if (_path == null) return;
            else
            {

                var strarr = _path.Split('\\').ToList();
                foldername = strarr[strarr.Count - 1];

                folderViewerlist.Add(new Foldertype
                {
                    FriendlyName = foldername,
                    FullPathName = Viewerpath + _path,
                    IncludeFileChildren = false,
                    IsExpanded = false,
                    Children = evt_addfolderNodes(_path)
                });
            }
        }

        public ObservableCollection<INode> evt_addfolderNodes(string _path)
        {
            try
            {
                ObservableCollection<INode> _obj = new ObservableCollection<INode>();

                foreach (string fname in System.IO.Directory.GetFileSystemEntries(_path))
                {
                    // get the file attributes for file or directory
                    // FileAttributes attr = File.GetAttributes(Viewerpath + fname);
                    var strarr = fname.Split('\\').ToList();
                    var foldername = strarr[strarr.Count - 1];
                    //detect whether its a directory or file

                    if (Directory.Exists(fname))
                    {
                        _obj.Add(new Foldertype
                        {
                            FriendlyName = foldername,
                            FullPathName = fname,
                            IncludeFileChildren = false,
                            IsExpanded = false,
                            Children = evt_addfolderNodes(fname)

                        });
                    }
                    else
                    {
                        _obj.Add(new IFiletype
                        {
                            FriendlyName = foldername,
                            FullPathName = fname,
                            IncludeFileChildren = false,
                            IsExpanded = false,

                        });
                    }
                }
                return _obj;
            }
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show(ex.Message);
                return new ObservableCollection<INode>();
            }
        }
     
        public void evt_ReleasetoRefresh()
        {
            OnPropertyChanged(nameof(Scriptpath));
            OnPropertyChanged(nameof(Fullloop));
            OnPropertyChanged(nameof(EstimateruntimeforblockA));
            OnPropertyChanged(nameof(EstimateruntimeforblockB));
            OnPropertyChanged(nameof(BlockA1Interval));
            OnPropertyChanged(nameof(BlockA2Interval));
            OnPropertyChanged(nameof(BlockB1Interval));
            OnPropertyChanged(nameof(BlockB2Interval));
            OnPropertyChanged(nameof(BlockALoop));
            OnPropertyChanged(nameof(BlockBLoop));
            OnPropertyChanged(nameof(blockA1scriptpath));
            OnPropertyChanged(nameof(blockA2scriptpath));
            OnPropertyChanged(nameof(blockB1scriptpath));
            OnPropertyChanged(nameof(blockB2scriptpath));
            OnPropertyChanged(nameof(blockAitemcount));
            OnPropertyChanged(nameof(blockBitemcount));
            OnPropertyChanged(nameof(blockCitemcount));
            OnPropertyChanged(nameof(blockDitemcount));

            Estimateruntimefullblock = 0;
            OnPropertyChanged(nameof(Estimateruntimefullblock));
        }

        public string evt_Openfile()
        {
            using (var openFileDialog1 = new System.Windows.Forms.OpenFileDialog())
            {
                string myPath = AppPath + @"\script\";
                string strpath = null;
                // 設定OpenFileDialog屬性
                openFileDialog1.Title = "選擇要開啟的 XML 檔案";
                openFileDialog1.Filter = "xml Files (.xml)|*.xml|All Files (*.*)|*.*";
                openFileDialog1.FilterIndex = 1;
                openFileDialog1.Multiselect = true;
                openFileDialog1.InitialDirectory = myPath;

                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    strpath = openFileDialog1.FileName; //取得檔名
                    //Openblockfilepath = strpath;
                    //ConfigModel.Instance.GetScriptXMLTestSuite(this);
                    evt_ReleasetoRefresh();
                }
                return strpath;
            }
        }

        #endregion

        #region Monitor dashboard
        private void HandleBufferSend_00(byte sendByte4, SendAndReceiveDatabatchcheck bytes)
        {
            if (sendByte4 == 0x01)
            {
                string msg = string.Format("{0}.{1}", bytes.byte_buffer_Receive[3].ToString(), bytes.byte_buffer_Receive[4].ToString());
          
                if (bytes.byte_buffer_Receive[1] == 1)
                {
                    UnifiedHostCommandSet.DUT1SWversion = msg;
                }
                else if (bytes.byte_buffer_Receive[1] == 2)
                {
                    UnifiedHostCommandSet.DUT2SWversion = msg;
                }
                else if (bytes.byte_buffer_Receive[1] == 3)
                {
                    UnifiedHostCommandSet.DUT3SWversion = msg;
                }
                evt_func_SWversion(bytes.byte_buffer_Receive[1], msg);
            }
            else if (sendByte4 == 0x02)
            {
                string msg = string.Format("{0}.{1}", bytes.byte_buffer_Receive[3].ToString(), bytes.byte_buffer_Receive[4].ToString());
                if (bytes.byte_buffer_Receive[1] == 1)
                {
                    UnifiedHostCommandSet.DUT1HWversion = msg;
                }
                else if (bytes.byte_buffer_Receive[1] == 2)
                {
                    UnifiedHostCommandSet.DUT2HWversion = msg;
                }
                else if (bytes.byte_buffer_Receive[1] == 3)
                {
                    UnifiedHostCommandSet.DUT3HWversion = msg;
                }
                evt_func_HWversion(bytes.byte_buffer_Receive[1], msg);
            }
        }
        private void HandleBufferSend_01(byte sendByte4, SendAndReceiveDatabatchcheck bytes)
        {
            if (sendByte4 == 0x01)
            {
                int bbvalue = Convert.ToInt32(bytes.byte_buffer_Receive[3]);

                evt_func_Backlighbrightness(bytes.byte_buffer_Receive[1], bbvalue);
            }
            // ... other conditions
        }
        private void HandleBufferSend_04(byte sendByte4, SendAndReceiveDatabatchcheck bytes)
        {
            if (sendByte4 == 0x01)
            {
                int DTC = Convert.ToInt32(bytes.byte_buffer_Receive[3]);

                if (bytes.byte_buffer_Receive[1] == 1)
                {
                    UnifiedHostCommandSet.DUT1Diagnostic_raw = bytes.strSequenceData_Rec;
                }
                else if (bytes.byte_buffer_Receive[1] == 2)
                {
                    UnifiedHostCommandSet.DUT2Diagnostic_raw = bytes.strSequenceData_Rec;
                }
                else if (bytes.byte_buffer_Receive[1] == 3)
                {
                    UnifiedHostCommandSet.DUT3Diagnostic_raw = bytes.strSequenceData_Rec;
                }

                // 監控條件是否成立
                if (IsEnableDetectDiag)
                {
                    if (bytes.byte_buffer_Receive[3] != 0x01)
                    {
                        // 將錯誤的項目輸出csv
                        Error_cSVfile.ErrordataAppendTocsv(UnifiedHostCommandSet, bytes);
                    }                                 
                }

                evt_func_DTC(bytes.byte_buffer_Receive[1], DTC);
            }

            else if (sendByte4 == 0x02)
            {
                bytes.strSequenceData_Rec
                double volvalue1 = (double)(bytes.byte_buffer_Receive[3] << 8 | bytes.byte_buffer_Receive[4]) / 10;
                evt_func_Voltage(bytes.byte_buffer_Receive[1], volvalue1);

            }
            else if (sendByte4 == 0x03)
            {

                int led1temp = Convert.ToInt32(bytes.byte_buffer_Receive[3]);
                int led2temp = Convert.ToInt32(bytes.byte_buffer_Receive[4]);
                int pcbtemp = Convert.ToInt32(bytes.byte_buffer_Receive[5]);
                evt_func_Temp(bytes.byte_buffer_Receive[1], led1temp, led2temp, pcbtemp);

            }

            else if (sendByte4 == 0x04)
            {
                int Ambienttemp = Convert.ToInt32(bytes.byte_buffer_Receive[3]);
                evt_func_T_chamber(bytes.byte_buffer_Receive[1], Ambienttemp);

            }
            else if (sendByte4 == 0x05)
            {
                //int adcvalue = bytes.byte_buffer_Receive[3] << 8 + bytes.byte_buffer_Receive[4];
                double adcvalue = (double)(bytes.byte_buffer_Receive[3] << 8 | bytes.byte_buffer_Receive[4]);
                evt_func_ADC(bytes.byte_buffer_Receive[1], adcvalue);

                // 監控條件是否成立
                if (IsEnableDetectLightsensor)
                {
                    if (adcvalue < LowerLimitLightsensorValue)
                    {
                        // 將錯誤的項目輸出csv
                        Error_cSVfile.ErrordataAppendTocsv(UnifiedHostCommandSet, bytes);
                    }
                    else if (adcvalue > UpperLimitLightsensorValue)
                    {
                        // 將錯誤的項目輸出csv
                        Error_cSVfile.ErrordataAppendTocsv(UnifiedHostCommandSet, bytes);
                    }
                }
            }

        }
        private void HandleBufferSend_05(byte sendByte4, SendAndReceiveDatabatchcheck bytes)
        {
            if (PowerMode == 0x01)
            {
                double current1 = Convert.ToDouble(Convert.ToInt32(bytes.byte_buffer_Receive[3].ToString("X2"), 16));
                double current2 = Convert.ToDouble(Convert.ToInt32(bytes.byte_buffer_Receive[4].ToString("X2"), 16)) / 100.0;
                evt_func_normalCurrent(bytes.byte_buffer_Receive[1], current1 + current2);

                // 監控條件是否成立
                if (IsEnableDetectnormCurrent)
                {
                    if (current1 + current2 > LowerLimitnormCurrentValue)
                    {
                        // 將錯誤的項目輸出csv
                        Error_cSVfile.ErrordataAppendTocsv(UnifiedHostCommandSet, bytes);
                    }
                    else if (current1 + current2 > UpperLimitnormCurrentValue)
                    {
                        // 將錯誤的項目輸出csv
                        Error_cSVfile.ErrordataAppendTocsv(UnifiedHostCommandSet, bytes);
                    }
                }
            }
            else if (PowerMode == 0x00)
            {

                double current1 = Convert.ToDouble(Convert.ToInt32(bytes.byte_buffer_Receive[3].ToString("X2"), 16));
                double current2 = Convert.ToDouble(Convert.ToInt32(bytes.byte_buffer_Receive[4].ToString("X2"), 16)) / 100.0;
                evt_func_sleepCurrent(bytes.byte_buffer_Receive[1], current1 + current2);

                // 監控條件是否成立
                if (IsEnableDetectsleepCurrent)
                {
                    if (current1 + current2 > UpperLimitsleepCurrentValue)
                    {
                        // 將錯誤的項目輸出csv
                        Error_cSVfile.ErrordataAppendTocsv(UnifiedHostCommandSet, bytes);
                    }
                }
            }

        }
        private void HandleBufferSend_06(byte sendByte4, SendAndReceiveDatabatchcheck bytes)
        {
            if (sendByte4 == 0x01)
            {
                //UnifiedHostCommandSet.TouchPoint = bytes.strSequenceData_Rec;
            }


        }
        public void evt_func_SWversion(byte _txb, string _value)
        {

            switch (_txb)
            {
                case 0x01:
                    Port0SDM1SWversion = _value;
                    break;
                case 0x02:
                    Port0SDM2SWversion = _value;
                    break;
                case 0x03:
                    Port0SDM3SWversion = _value;
                    break;
            }
            OnPropertyChanged(nameof(Port0SDM1SWversion));
            OnPropertyChanged(nameof(Port0SDM2SWversion));
            OnPropertyChanged(nameof(Port0SDM3SWversion));


        }
        public void evt_func_HWversion(byte _txb, string _value)
        {
            switch (_txb)
            {
                case 0x01:
                    Port0SDM1HWversion = _value;
                    break;
                case 0x02:
                    Port0SDM2HWversion = _value;
                    break;
                case 0x03:
                    Port0SDM3HWversion = _value;
                    break;
            }
            OnPropertyChanged(nameof(Port0SDM1HWversion));
            OnPropertyChanged(nameof(Port0SDM2HWversion));
            OnPropertyChanged(nameof(Port0SDM3HWversion));
        }

        public void evt_func_DTC(byte _txb, int _value)
        {
            // 0x04 0x01
            switch (_txb)
            {
                case 0x01:
                    UnifiedHostCommandSet.DUT1Diagnostic = _value.ToString();
                    Port0SDM1DTC = _value;
                    break;
                case 0x02:
                    UnifiedHostCommandSet.DUT2Diagnostic = _value.ToString();
                    Port0SDM2DTC = _value;
                    break;
                case 0x03:
                    UnifiedHostCommandSet.DUT3Diagnostic = _value.ToString();
                    Port0SDM3DTC = _value;
                    break;
            }
            OnPropertyChanged(nameof(Port0SDM1DTC));
            OnPropertyChanged(nameof(Port0SDM2DTC));
            OnPropertyChanged(nameof(Port0SDM3DTC));
        }

        public void evt_func_Voltage(byte _txb, double _value)
        {
            // 0x04 0x02
            switch (_txb)
            {
                case 0x01:
                    UnifiedHostCommandSet.DUT1Voltage = _value.ToString();
                    Port0SDM1Voltage = _value;
                    break;
                case 0x02:
                    UnifiedHostCommandSet.DUT2Voltage = _value.ToString();
                    Port0SDM2Voltage = _value;
                    break;
                case 0x03:
                    UnifiedHostCommandSet.DUT3Voltage = _value.ToString();
                    Port0SDM3Voltage = _value;
                    break;
            }
            OnPropertyChanged(nameof(Port0SDM1Voltage));
            OnPropertyChanged(nameof(Port0SDM2Voltage));
            OnPropertyChanged(nameof(Port0SDM3Voltage));
        }

        public void evt_func_Temp(byte _txb, int _LED1, int _LED2, int _PCBA)
        {
            // 0x04 0x03
            string ledAndPCBAData = $"{_LED1}, {_LED2}, {_PCBA}";
            switch (_txb)
            {
                case 0x01:
                    Port0SDM1PCBTemp = _PCBA;
                    Port0SDM1LED1Temp = _LED1;
                    Port0SDM1LED2Temp = _LED2;
                    UnifiedHostCommandSet.DUT1T_LED1_2PCB = ledAndPCBAData;
                    OnPropertyChanged(nameof(Port0SDM1PCBTemp));
                    OnPropertyChanged(nameof(Port0SDM1LED1Temp));
                    OnPropertyChanged(nameof(Port0SDM1LED2Temp));
                    break;
                case 0x02:
                    Port0SDM2PCBTemp = _PCBA;
                    Port0SDM2LED1Temp = _LED1;
                    Port0SDM2LED2Temp = _LED2;
                    UnifiedHostCommandSet.DUT2T_LED1_2PCB = ledAndPCBAData;
                    OnPropertyChanged(nameof(Port0SDM2PCBTemp));
                    OnPropertyChanged(nameof(Port0SDM2LED1Temp));
                    OnPropertyChanged(nameof(Port0SDM2LED2Temp));
                    break;
                case 0x03:
                    Port0SDM3PCBTemp = _PCBA;
                    Port0SDM3LED1Temp = _LED1;
                    Port0SDM3LED2Temp = _LED2;
                    UnifiedHostCommandSet.DUT3T_LED1_2PCB = ledAndPCBAData;
                    OnPropertyChanged(nameof(Port0SDM3PCBTemp));
                    OnPropertyChanged(nameof(Port0SDM3LED1Temp));
                    OnPropertyChanged(nameof(Port0SDM3LED2Temp));
                    break;
            }
        }

        public void evt_func_T_chamber(byte _txb, int _value)
        {
            // 0x04 0x04
            switch (_txb)
            {
                case 0x01:
                    UnifiedHostCommandSet.DUT1T_chamber = _value.ToString();
                    Port0SDM1T_chamber = _value;
                    break;
                case 0x02:
                    UnifiedHostCommandSet.DUT2T_chamber = _value.ToString();
                    Port0SDM2T_chamber = _value;
                    break;
                case 0x03:
                    UnifiedHostCommandSet.DUT3T_chamber = _value.ToString();
                    Port0SDM3T_chamber = _value;
                    break;
            }
            OnPropertyChanged(nameof(Port0SDM1T_chamber));
            OnPropertyChanged(nameof(Port0SDM2T_chamber));
            OnPropertyChanged(nameof(Port0SDM3T_chamber));
        }
       
        public void evt_func_ADC(byte _txb, double _value)
        {
            // 0x04 0x05
            switch (_txb)
            {
                case 0x01:
                    UnifiedHostCommandSet.DUT1Lightsensor = _value.ToString();
                    Port0SDM1ADC = _value;
                    break;
                case 0x02:
                    UnifiedHostCommandSet.DUT2Lightsensor = _value.ToString();
                    Port0SDM2ADC = _value;
                    break;
                case 0x03:
                    UnifiedHostCommandSet.DUT3Lightsensor = _value.ToString();
                    Port0SDM3ADC = _value;
                    break;
            }
            OnPropertyChanged(nameof(Port0SDM1ADC));
            OnPropertyChanged(nameof(Port0SDM2ADC));
            OnPropertyChanged(nameof(Port0SDM3ADC));
        }

        public void evt_func_normalCurrent(byte _txb, double _value)
        {
            var curvalue = Math.Round(_value, 2, MidpointRounding.AwayFromZero);
            switch (_txb)
            {
                case 0x01:
                    UnifiedHostCommandSet.DUT1NormalCurrent = curvalue.ToString();
                    UnifiedHostCommandSet.DUT1SleepCurrent = "";
                    Port0SDM1normalCurrent = curvalue;
                    Port0SDM1sleepCurrent = 0;
                    OnPropertyChanged(nameof(Port0SDM1normalCurrent));
                    OnPropertyChanged(nameof(Port0SDM1sleepCurrent));
                    // 清空較早的Queue
                    if(IsEnableRollingmode)
                    {
                        UnifiedHostCommandSet.CheckoutCurlist(UnifiedHostCommandSet.DUT1CurrentList);
                    }                    
                    break;
                case 0x02:
                    UnifiedHostCommandSet.DUT2NormalCurrent = curvalue.ToString();
                    UnifiedHostCommandSet.DUT2SleepCurrent = "";
                    Port0SDM2normalCurrent = curvalue;
                    Port0SDM2sleepCurrent = 0;
                    OnPropertyChanged(nameof(Port0SDM2normalCurrent));
                    OnPropertyChanged(nameof(Port0SDM2sleepCurrent));
                    // 清空較早的Queue
                    if (IsEnableRollingmode)
                    {
                        UnifiedHostCommandSet.CheckoutCurlist(UnifiedHostCommandSet.DUT2CurrentList);
                    }
                    break;
                case 0x03:
                    UnifiedHostCommandSet.DUT3NormalCurrent = curvalue.ToString();
                    UnifiedHostCommandSet.DUT3SleepCurrent = "";
                    Port0SDM3normalCurrent = curvalue;
                    Port0SDM3sleepCurrent = 0;
                    OnPropertyChanged(nameof(Port0SDM3normalCurrent));
                    OnPropertyChanged(nameof(Port0SDM3sleepCurrent));
                    // 清空較早的Queue
                    if (IsEnableRollingmode)
                    {
                        UnifiedHostCommandSet.CheckoutCurlist(UnifiedHostCommandSet.DUT3CurrentList);
                    }
                    break;
            }

        }

        public void evt_func_sleepCurrent(byte _txb, double _value)
        {
            var curvalue = Math.Round(_value, 2, MidpointRounding.AwayFromZero);
            switch (_txb)
            {
                case 0x01:
                    UnifiedHostCommandSet.DUT1NormalCurrent = "";
                    UnifiedHostCommandSet.DUT1SleepCurrent = curvalue.ToString();
                    Port0SDM1normalCurrent = 0;
                    Port0SDM1sleepCurrent = curvalue;
                    OnPropertyChanged(nameof(Port0SDM1normalCurrent));
                    OnPropertyChanged(nameof(Port0SDM1sleepCurrent));
                    // 清空較早的Queue
                    if (IsEnableRollingmode)
                    {
                        UnifiedHostCommandSet.CheckoutCurlist(UnifiedHostCommandSet.DUT1CurrentList);
                    }
                    break;
                case 0x02:
                    UnifiedHostCommandSet.DUT2NormalCurrent = "";
                    UnifiedHostCommandSet.DUT2SleepCurrent = curvalue.ToString();
                    Port0SDM2normalCurrent = 0;
                    Port0SDM2sleepCurrent = curvalue;
                    OnPropertyChanged(nameof(Port0SDM2normalCurrent));
                    OnPropertyChanged(nameof(Port0SDM2sleepCurrent));
                    // 清空較早的Queue
                    if (IsEnableRollingmode)
                    {
                        UnifiedHostCommandSet.CheckoutCurlist(UnifiedHostCommandSet.DUT2CurrentList);
                    }
                    break;
                case 0x03:
                    UnifiedHostCommandSet.DUT3NormalCurrent = curvalue.ToString();
                    UnifiedHostCommandSet.DUT3SleepCurrent = "";
                    Port0SDM3normalCurrent = 0;
                    Port0SDM3sleepCurrent = curvalue;
                    OnPropertyChanged(nameof(Port0SDM3normalCurrent));
                    OnPropertyChanged(nameof(Port0SDM3sleepCurrent));
                    // 清空較早的Queue
                    if (IsEnableRollingmode)
                    {
                        UnifiedHostCommandSet.CheckoutCurlist(UnifiedHostCommandSet.DUT3CurrentList);
                    }
                    break;
            }
        }

        public void evt_func_Backlighbrightness(byte _txb, int _value)
        {
            switch (_txb)
            {
                case 0x01:
                    UnifiedHostCommandSet.DUT1Brightness = _value.ToString();
                    Port0SDM1Backlighbrightness = _value;
                    break;
                case 0x02:
                    UnifiedHostCommandSet.DUT2Brightness = _value.ToString();
                    Port0SDM2Backlighbrightness = _value;
                    break;
                case 0x03:
                    UnifiedHostCommandSet.DUT3Brightness = _value.ToString();
                    Port0SDM3Backlighbrightness = _value;
                    break;
            }
            OnPropertyChanged(nameof(Port0SDM1Backlighbrightness));
            OnPropertyChanged(nameof(Port0SDM2Backlighbrightness));
            OnPropertyChanged(nameof(Port0SDM3Backlighbrightness));
        }

        public void updateUIobj()
        {
            OnPropertyChanged(nameof(Port0SDM1HWversion));
            OnPropertyChanged(nameof(Port0SDM2HWversion));
            OnPropertyChanged(nameof(Port0SDM3HWversion));
            OnPropertyChanged(nameof(Port0SDM1SWversion));
            OnPropertyChanged(nameof(Port0SDM2SWversion));
            OnPropertyChanged(nameof(Port0SDM3SWversion));
            OnPropertyChanged(nameof(Port0SDM1DTC));
            OnPropertyChanged(nameof(Port0SDM2DTC));
            OnPropertyChanged(nameof(Port0SDM3DTC));
            OnPropertyChanged(nameof(Port0SDM1Voltage));
            OnPropertyChanged(nameof(Port0SDM2Voltage));
            OnPropertyChanged(nameof(Port0SDM3Voltage));
            OnPropertyChanged(nameof(Port0SDM1PCBTemp));
            OnPropertyChanged(nameof(Port0SDM2PCBTemp));
            OnPropertyChanged(nameof(Port0SDM3PCBTemp));
            OnPropertyChanged(nameof(Port0SDM1LED1Temp));
            OnPropertyChanged(nameof(Port0SDM2LED1Temp));
            OnPropertyChanged(nameof(Port0SDM3LED1Temp));
            OnPropertyChanged(nameof(Port0SDM1LED2Temp));
            OnPropertyChanged(nameof(Port0SDM2LED2Temp));
            OnPropertyChanged(nameof(Port0SDM3LED2Temp));
            OnPropertyChanged(nameof(Port0SDM1T_chamber));
            OnPropertyChanged(nameof(Port0SDM2T_chamber));
            OnPropertyChanged(nameof(Port0SDM3T_chamber));
            OnPropertyChanged(nameof(Port0SDM1ADC));
            OnPropertyChanged(nameof(Port0SDM2ADC));
            OnPropertyChanged(nameof(Port0SDM3ADC));
            OnPropertyChanged(nameof(Port0SDM1normalCurrent));
            OnPropertyChanged(nameof(Port0SDM2normalCurrent));
            OnPropertyChanged(nameof(Port0SDM3normalCurrent));
            OnPropertyChanged(nameof(Port0SDM1sleepCurrent));
            OnPropertyChanged(nameof(Port0SDM2sleepCurrent));
            OnPropertyChanged(nameof(Port0SDM3sleepCurrent));
            OnPropertyChanged(nameof(Port0SDM1Backlighbrightness));
            OnPropertyChanged(nameof(Port0SDM2Backlighbrightness));
            OnPropertyChanged(nameof(Port0SDM3Backlighbrightness));

        }
        #endregion

        #region SDM1
        public int Port0SDM1DTC { get; set; }
        public string Port0SDM1SWversion { get; set; } = "___";
        public string Port0SDM1HWversion { get; set; } = "___";
        public string Port0SDM1Status { get; set; } = "___";
        public double Port0SDM1Voltage { get; set; } = 0.0;
        public double Port0SDM1normalCurrent { get; set; } = 0.0;
        public double Port0SDM1sleepCurrent { get; set; } = 0.0;
        public double Port0SDM1Backlighbrightness { get; set; } = 0;
        public int Port0SDM1PCBTemp { get; set; } = 0;
        public int Port0SDM1LED1Temp { get; set; } = 0;
        public int Port0SDM1LED2Temp { get; set; } = 0;
        public int Port0SDM1T_chamber { get; set; } = 0;
        public double Port0SDM1ADC { get; set; } = 0;
        public string Port0SDM1Touchresponse { get; set; } = "___";
        #endregion

        #region SDM2

        public int Port0SDM2DTC { get; set; }
        public string Port0SDM2SWversion { get; set; } = "___";
        public string Port0SDM2HWversion { get; set; } = "___";
        public string Port0SDM2Status { get; set; } = "___";
        public double Port0SDM2Voltage { get; set; } = 0.0;
        public double Port0SDM2normalCurrent { get; set; } = 0.0;
        public double Port0SDM2sleepCurrent { get; set; } = 0.0;
        public int Port0SDM2Backlighbrightness { get; set; } = 0;
        public int Port0SDM2PCBTemp { get; set; } = 0;
        public int Port0SDM2LED1Temp { get; set; } = 0;
        public int Port0SDM2LED2Temp { get; set; } = 0;
        public int Port0SDM2T_chamber { get; set; } = 0;
        public double Port0SDM2ADC { get; set; } = 0;
        public string Port0SDM2Touchresponse { get; set; } = "___";
        #endregion

        #region SDM3
        public int Port0SDM3DTC { get; set; }
        public string Port0SDM3SWversion { get; set; } = "___";
        public string Port0SDM3HWversion { get; set; } = "___";
        public string Port0SDM3Status { get; set; } = "___";
        public double Port0SDM3Voltage { get; set; } = 0.0;
        public double Port0SDM3normalCurrent { get; set; } = 0.0;
        public double Port0SDM3sleepCurrent { get; set; } = 0.0;
        public int Port0SDM3Backlighbrightness { get; set; } = 0;
        public int Port0SDM3PCBTemp { get; set; } = 0;
        public int Port0SDM3LED1Temp { get; set; } = 0;
        public int Port0SDM3LED2Temp { get; set; } = 0;
        public int Port0SDM3T_chamber { get; set; } = 0;
        public double Port0SDM3ADC { get; set; } = 0;
        public string Port0SDM3Touchresponse { get; set; } = "___";
        #endregion
    }
}
