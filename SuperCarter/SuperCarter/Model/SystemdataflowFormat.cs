using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CsvHelper.Configuration.Attributes;
using System.Windows.Media.Imaging;
using CsvHelper.Configuration;
using System.Text.Json.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.Runtime.Serialization;

namespace SuperCarter.Model
{
    public class Comporttype : INode
    {
        public string FriendlyName { get; set; }

        public ObservableCollection<Portdetectedtype> Children { get; set; } = new ObservableCollection<Portdetectedtype>();

    }
    public class Foldertype : INode
    {
        public Foldertype()
        {
            this.Children = new ObservableCollection<INode>();
        }

        public string FriendlyName { get; set; }
        public BitmapSource MyIcon { get; set; }
        public string FullPathName { get; set; }
        public ObservableCollection<INode> Children { get; set; } = new ObservableCollection<INode>();
        public bool IsExpanded { get; set; }
        public bool IncludeFileChildren { get; set; }

    }

    public interface INode
    {
        string FriendlyName { get; }
    }
    public class IFiletype : INode
    {
        public string FriendlyName { get; set; }
        public BitmapSource MyIcon { get; set; }
        public string FullPathName { get; set; }
        public bool IsExpanded { get; set; }
        public bool IncludeFileChildren { get; set; }
    }

    public class SerialidList
    {
        public int idValue { get; set; }
        public string idName { get; set; }
    }
    public class PortIDItems : ObservableCollection<string>
    {
        public PortIDItems()
        {
            this.Add("0");
            this.Add("1");
            this.Add("2");
            this.Add("all");
        }
        public int idValue { get; set; }

    }
    public class CMDtypeItems : ObservableCollection<string>
    {   
        public CMDtypeItems()
        {
            this.Add("UART");
        }
    }

    public class CMDparm1Items : ObservableCollection<string>
    {
        public CMDparm1Items()
        {
            this.Add("HEX");
            this.Add("ASCII");
        }
    }

    public class CsvHelper
    {
        private static object _lock = new object();
        private string _path;

        public CsvHelper(string path)
        {
            _path = path;

            // Initialize CSV if it does not exist
            if (!File.Exists(_path))
            {
                using (StreamWriter sw = new StreamWriter(_path, true))
                {
                    sw.WriteLine("Time,Port,Type,Data"); // You can adjust these headers based on your requirements
                }
            }
        }

        public void AppendToCsv(string port, string type, string data)
        {
            lock (_lock) // Ensure thread safety when writing to the file
            {
                using (StreamWriter sw = new StreamWriter(_path, true))
                {
                    sw.WriteLine($"{DateTime.Now},{port},{type},{data}");
                }
            }
        }
    }

    public class QueueManager
    {
        private static readonly Lazy<QueueManager> instance = new Lazy<QueueManager>(() => new QueueManager());

        public static QueueManager Instance => instance.Value;

        public ConcurrentQueue<SendAndReceiveDatabatchcheck> SendAndReceiveDatabatchQ { get; } = new ConcurrentQueue<SendAndReceiveDatabatchcheck>();

        private QueueManager() { }

        // 取用的話
        //QueueManager.Instance.SendAndReceiveDatabatchQ.Enqueue(...);

    }
    public enum Msgtype
    {
        Message = 0,
        FromPort = 1
       
    }

    public class RealtimeMsgQueuetype
    {
        public Msgtype msgtype;
        public int PortNum { get; set; }
        public string msg { get; set; }
    }
    public class SendorExecuteSendType
    {
        /// <summary>
        /// 
        /// 此規格主要為
        /// 
        /// 必須要注意 Portnum 9 為 delay port
        /// 
        /// 
        /// </summary>
        public int PortNum { get; set; }
        public int intDataLen { get; set; }
        public string strDataLen { get; set; }
        public byte[] CommandData { get; set; }
        public string strCommandData { get; set; }
        public int Delaytime { get; set; }
        public string SendMsgdata { get; set; }
        public int Loop { get; set; }
    }
    public class Portdetectedtype
    {
        public bool IsUse { get; set; }
        public int _PortID;
        public int PortID
        {
            get { return _PortID; }
            set
            {
                _PortID = value > 2 ? 0 : value;
            }
        }
        public string FullPortName { get; set; }
        public string PortName { get; set; }
        public List<Portdetectedtype> ChildNode { get; set; }
        public int BaudRateValue { get; set; }
        public int DataReceivedCasenum { get; set; }

    }
    [DataContract]
    public class ScriptItemtype
    {
        public ScriptItemtype()
        {
            this.ChildNode = new List<ScriptItemtype>();
        }
        [DataMember]
        public int ID { get; set; } = 0;
        //public PortIDItems lists { get; set; }
        [DataMember]
        public string Portnum { get; set; } = "0";
        [DataMember]
        public string CMDtype { get; set; } = "UART";
        [DataMember]
        public string CMDparm1 { get; set; } = "HEX";      
        public string Nodename { get; set; } = "";
        [DataMember]
        public string MSGname { get; set; } = "";
        [DataMember]
        public string Command { get; set; } = "";
        [DataMember]
        public int Delaytime { get; set; } = 200;
        [DataMember]
        public string RecCommand { get; set; } = "";
        [DataMember]
        public string HashValue { get; set; } = "";
        [DataMember]
        public string HashCode { get; set; } = "";
        public List<ScriptItemtype> ChildNode { get; set; }
        [DataMember]
        public int Loop { get; set; } = 1;
    }
    public class SendAndReceiveDatabatchcheck
    {
        public int CommnadID { get; set; }
        public int Portnum { get; set; }
        public string strCommandData_send { get; set; }
        public string strCommandData_Rec { get; set; }
        public byte[] byte_buffer_Send { get; set; }
        public byte[] byte_buffer_Receive { get; set; }
    }
    public class MonitoringErrordataOutputFormat
    {
        [Name("*Time")]
        public string Time { get; set; }
        [Name("*Loop")]
        public string Loop { get; set; }
        [Name("*Phase")]
        public string Blockphase { get; set; }

        [Name("*Blockloop")]
        public string Blockloop { get; set; }
        [Name("*PowerMode")]
        public string PowerMode { get; set; }
        [Name("*Send_CMD")]
        public string Send_command { get; set; }
        [Name("*Receive_CMD")]
        public string Receive_command { get; set; }
    }
    public class UnifiedHostCommandSettype
    {
        [Ignore]
        public  bool IsEnableExecuteSDMcheck { get; set; }
        [Ignore]
        public string _DUT1Lightsensor;
        [Ignore]
        public string _DUT2Lightsensor;
        [Ignore]
        public string _DUT3Lightsensor;
        [Ignore]
        public string _DUT1NormalCurrent;
        [Ignore]
        public string _DUT2NormalCurrent;
        [Ignore]
        public string _DUT3NormalCurrent;
        [Ignore]
        public string _DUT1SleepCurrent;
        [Ignore]
        public string _DUT2SleepCurrent;
        [Ignore]
        public string _DUT3SleepCurrent;
        // working information
        [Name("Time")]
        public string Time { get; set; }
        [Name("Loop")]
        public string Loop { get; set; }
        [Name("Phase")]
        public string Blockphase { get; set; }

        [Name("Blockloop")]
        public string Blockloop { get; set; }
        [Name("#1 PowerMode")]
        public string DUT1PowerMode { get; set; }
        [Ignore]
        public ConcurrentQueue<string> DUT1LightsensorList { get; set; } = new ConcurrentQueue<string>();
        [Ignore]
        public ConcurrentQueue<string> DUT2LightsensorList { get; set; } = new ConcurrentQueue<string>();
        [Ignore]
        public ConcurrentQueue<string> DUT3LightsensorList { get; set; } = new ConcurrentQueue<string>();
        [Ignore]
        public ConcurrentQueue<string> DUT1CurrentList { get; set; } = new ConcurrentQueue<string> ();
        [Ignore]
        public ConcurrentQueue<string> DUT2CurrentList { get; set; } = new ConcurrentQueue<string>();
        [Ignore]
        public ConcurrentQueue<string> DUT3CurrentList { get; set; } = new ConcurrentQueue<string>();

        // for DUT1
        //[Name("#1 SWversion")]
        [Ignore]
        public string DUT1SWversion { get; set; }
        //[Name("#1 HWversion")]
        [Ignore]
        public string DUT1HWversion { get; set; }
        [Name("#1 InputVoltage")]
        public string DUT1Voltage { get; set; }
        [Name("#1 Current(A/uA)")]
        public string DUT1NormalCurrent 
        {
            get => _DUT1NormalCurrent;
            set {
                _DUT1NormalCurrent = value;
                if (IsEnableExecuteSDMcheck)
                {
                    if (!string.IsNullOrWhiteSpace(DUT1NormalCurrent))
                        DUT1CurrentList.Enqueue(DUT1NormalCurrent);
                }
            }
        }
        [Name("#1 Current(uA)")]
        public string DUT1SleepCurrent
        {
            get => _DUT1SleepCurrent;
            set
            {
                _DUT1SleepCurrent = value;
                if (IsEnableExecuteSDMcheck)
                {
                    if (!string.IsNullOrWhiteSpace(DUT1SleepCurrent))
                        DUT1CurrentList.Enqueue(DUT1SleepCurrent);
                }
            }
        }
        [Name("#1 Diagnostic")]
        public string DUT1Diagnostic { get; set; }
        [Name("#1 Lightsensor")]
        public string DUT1Lightsensor
        {
            get => _DUT1Lightsensor;
            set
            {
                _DUT1Lightsensor = value;
                if (IsEnableExecuteSDMcheck)
                {
                    DUT1LightsensorList.Enqueue(DUT1Lightsensor);
                }
            }
        }
        [Name("#1 Touch_finger")]
        public string DUT1Touchfinger { get; set; }
        [Name("#1 Touch_XY")]
        public string DUT1Touch_XY { get; set; }
        [Name("#1 Brightness(%)")]
        public string DUT1Brightness { get; set; }
        [Name("#1 T_chamber")]
        public string DUT1T_chamber { get; set; }
        [Name("#1 T_LED1, 2PCB")]
        public string DUT1T_LED1_2PCB { get; set; }
        [Name("#1 Diagnostic_raw")]
        public string DUT1Diagnostic_raw { get; set; }
        // for DUT2
        //[Name("#2 SWversion")]
        [Ignore]
        public string DUT2SWversion { get; set; }
        //[Name("#2 HWversion")]
        [Ignore]
        public string DUT2HWversion { get; set; }
        [Name("#2 PowerMode")]
        public string DUT2PowerMode { get; set; }
        [Name("#2 InputVoltage")]
        public string DUT2Voltage { get; set; }
        [Name("#2 Current(A/uA)")]
        public string DUT2NormalCurrent
        {
            get => _DUT2NormalCurrent;
            set
            {
                _DUT2NormalCurrent = value;
                if (IsEnableExecuteSDMcheck)
                {
                    if (!string.IsNullOrWhiteSpace(DUT2NormalCurrent))
                        DUT2CurrentList.Enqueue(DUT2NormalCurrent);
                }
            }
        }
        [Name("#2 Current(uA)")]
        public string DUT2SleepCurrent
        {
            get => _DUT2SleepCurrent;
            set
            {
                _DUT2SleepCurrent = value;
                if (IsEnableExecuteSDMcheck)
                {
                    if (!string.IsNullOrWhiteSpace(DUT2SleepCurrent))
                        DUT2CurrentList.Enqueue(DUT2SleepCurrent);
                }
            }
        }
        [Name("#2 Diagnostic")]
        public string DUT2Diagnostic { get; set; }
        [Name("#2 Lightsensor")]
        public string DUT2Lightsensor
        {
            get => _DUT2Lightsensor;
            set
            {
                _DUT2Lightsensor = value;
                if (IsEnableExecuteSDMcheck)
                {
                    DUT2LightsensorList.Enqueue(DUT2Lightsensor);
                }
            }
        }

        [Name("#2 Touch_finger")]
        public string DUT2Touchfinger { get; set; }
        [Name("#2 Touch_XY")]
        public string DUT2Touch_XY { get; set; }
        [Name("#2 Brightness(%)")]
        public string DUT2Brightness { get; set; }
        [Name("#2 T_chamber")]
        public string DUT2T_chamber { get; set; }
        [Name("#2 T_LED1, 2PCB")]
        public string DUT2T_LED1_2PCB { get; set; }
        [Name("#2 Diagnostic_raw")]
        public string DUT2Diagnostic_raw { get; set; }
        // DUT3
        //[Name("#3 SWversion")]
        [Ignore]
        public string DUT3SWversion { get; set; }
        //[Name("#3 HWversion")]
        [Ignore]
        public string DUT3HWversion { get; set; }
        [Name("#3 PowerMode")]
        public string DUT3PowerMode { get; set; }
        [Name("#3 InputVoltage")]
        public string DUT3Voltage { get; set; }
        [Name("#3 Current(A/uA)")]
        public string DUT3NormalCurrent
        {
            get => _DUT3NormalCurrent;
            set
            {
                _DUT3NormalCurrent = value;
                if (IsEnableExecuteSDMcheck)
                {
                    if (!string.IsNullOrWhiteSpace(DUT3NormalCurrent))
                        DUT3CurrentList.Enqueue(DUT3NormalCurrent);
                }
            }
        }
        [Name("#3 Current(uA)")]
        public string DUT3SleepCurrent
        {
            get => _DUT3SleepCurrent;
            set
            {
                _DUT3SleepCurrent = value;
                if (IsEnableExecuteSDMcheck)
                {  
                    if(!string.IsNullOrWhiteSpace(DUT3SleepCurrent))
                        DUT3CurrentList.Enqueue(DUT3SleepCurrent);
                }
            }
        }
        [Name("#3 Diagnostic")]
        public string DUT3Diagnostic { get; set; }
        [Name("#3 Lightsensor")]
        public string DUT3Lightsensor
        {
            get => _DUT3Lightsensor;
            set
            {
                _DUT3Lightsensor = value;
                if (IsEnableExecuteSDMcheck)
                {
                    DUT3LightsensorList.Enqueue(DUT3Lightsensor);
                }
            }
        }
        [Name("#3 Touch_finger")]
        public string DUT3Touchfinger { get; set; }
        [Name("#3 Touch_XY")]
        public string DUT3Touch_XY { get; set; }
        [Name("#3 Brightness(%)")]
        public string DUT3Brightness { get; set; }
        [Name("#3 T_chamber")]
        public string DUT3T_chamber { get; set; }
        [Name("#3 T_LED1, 2PCB")]
        public string DUT3T_LED1_2PCB { get; set; }
        [Name("#3 Diagnostic_raw")]      
        public string DUT3Diagnostic_raw { get; set; }
        // Rest of the properties...
       
        public void CheckoutCurlist(ConcurrentQueue<string> DUTCurrentlist)
        {
            if (DUTCurrentlist.Count > 20)
                DUTCurrentlist.TryDequeue(out string bytes);
                    
        }

        public string ConcurrentQueueTostring(ConcurrentQueue<string> DUTCurrentlist)
        {
            string[] currentListArray = DUTCurrentlist.Select(d => d.ToString()).ToArray();
            string output = string.Join(", ", currentListArray);
            return output;
        }

    }
    public class FunctionSelector
    {
        public bool IncludeTime { get; set; } = true;
        public bool IncludeLoop { get; set; } = true;
        public bool IncludeBlockloop { get; set; } = true;
        public bool IncludeBlockphase { get; set; } = true;
        
        public bool IncludeDUT1PowerMode { get; set; } = true;
        public bool IncludeDUT1SWversion { get; set; } = true;
        public bool IncludeDUT1HWversion { get; set; } = true;
        public bool IncludeDUT1Voltage { get; set; } = true;
        public bool IncludeDUT1NormalCurrent { get; set; } = true;
        public bool IncludeDUT1SleepCurrent { get; set; } = true;
        public bool IncludeDUT1Diagnostic { get; set; } = true;
        public bool IncludeDUT1Lightsensor { get; set; } = true;
        public bool IncludeDUT1Touchfinger { get; set; } = true;
        public bool IncludeDUT1Touch_XY { get; set; } = true;
        public bool IncludeDUT1Brightness { get; set; } = true;
        public bool IncludeDUT1T_chamber { get; set; } = true;
        public bool IncludeDUT1T_LED1_2PCB { get; set; } = true;
        public bool IncludeDUT1Diagnostic_raw { get; set; } = true;
        public bool IncludeDUT2SWversion { get; set; } = true;
        public bool IncludeDUT2HWversion { get; set; } = true;
        public bool IncludeDUT2PowerMode { get; set; } = true;
        public bool IncludeDUT2Voltage { get; set; } = true;
        public bool IncludeDUT2NormalCurrent { get; set; } = true;
        public bool IncludeDUT2SleepCurrent { get; set; } = true;
        public bool IncludeDUT2Diagnostic { get; set; } = true;
        public bool IncludeDUT2Lightsensor { get; set; } = true;
        public bool IncludeDUT2Touchfinger { get; set; } = true;
        public bool IncludeDUT2Touch_XY { get; set; } = true;
        public bool IncludeDUT2Brightness { get; set; } = true;
        public bool IncludeDUT2T_chamber { get; set; } = true;
        public bool IncludeDUT2T_LED1_2PCB { get; set; } = true;
        public bool IncludeDUT2Diagnostic_raw { get; set; } = true;
        public bool IncludeDUT3SWversion { get; set; } = true;
        public bool IncludeDUT3HWversion { get; set; } = true;
        public bool IncludeDUT3PowerMode { get; set; } = true;
        public bool IncludeDUT3Voltage { get; set; } = true;
        public bool IncludeDUT3NormalCurrent { get; set; } = true;
        public bool IncludeDUT3SleepCurrent { get; set; } = true;
        public bool IncludeDUT3Diagnostic { get; set; } = true;
        public bool IncludeDUT3Lightsensor { get; set; } = true;
        public bool IncludeDUT3Touchfinger { get; set; } = true;
        public bool IncludeDUT3Touch_XY { get; set; } = true;
        public bool IncludeDUT3Brightness { get; set; } = true;
        public bool IncludeDUT3T_chamber { get; set; } = true;
        public bool IncludeDUT3T_LED1_2PCB { get; set; } = true;
        public bool IncludeDUT3Diagnostic_raw { get; set; } = true;

    }




    public class UnifiedHostCommandSettypeBeta
    {
        [Ignore]
        public FunctionSelector Selector { get; set; }

        [Name("Time")]
        public string BlockTime
        {
            get
            {
                if (Selector != null && !Selector.IncludeTime)
                {
                    return null;
                }
                return Time;
            }
            set { Time = value; }
        }

        [Ignore]
        public string Time { get; set; }



        [Name("Loop")]
        public string BlockLoop
        {
            get
            {
                if (Selector != null && !Selector.IncludeLoop)
                {
                    return null;
                }
                return Loop;
            }
            set { Loop = value; }
        }

        [Ignore]
        public string Loop { get; set; }



        [Name("Blockloop")]
        public string BlockBlockloop
        {
            get
            {
                if (Selector != null && !Selector.IncludeBlockloop)
                {
                    return null;
                }
                return Blockloop;
            }
            set { Blockloop = value; }
        }

        [Ignore]
        public string Blockloop { get; set; }



        [Name("Blockphase")]
        public string BlockBlockphase
        {
            get
            {
                if (Selector != null && !Selector.IncludeBlockphase)
                {
                    return null;
                }
                return Blockphase;
            }
            set { Blockphase = value; }
        }

        [Ignore]
        public string Blockphase { get; set; }



        [Name("DUT1PowerMode")]
        public string BlockDUT1PowerMode
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT1PowerMode)
                {
                    return null;
                }
                return DUT1PowerMode;
            }
            set { DUT1PowerMode = value; }
        }

        [Ignore]
        public string DUT1PowerMode { get; set; }



        [Name("DUT1SWversion")]
        public string BlockDUT1SWversion
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT1SWversion)
                {
                    return null;
                }
                return DUT1SWversion;
            }
            set { DUT1SWversion = value; }
        }

        [Ignore]
        public string DUT1SWversion { get; set; }



        [Name("DUT1HWversion")]
        public string BlockDUT1HWversion
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT1HWversion)
                {
                    return null;
                }
                return DUT1HWversion;
            }
            set { DUT1HWversion = value; }
        }

        [Ignore]
        public string DUT1HWversion { get; set; }



        [Name("DUT1Voltage")]
        public string BlockDUT1Voltage
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT1Voltage)
                {
                    return null;
                }
                return DUT1Voltage;
            }
            set { DUT1Voltage = value; }
        }

        [Ignore]
        public string DUT1Voltage { get; set; }



        [Name("DUT1NormalCurrent")]
        public string BlockDUT1NormalCurrent
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT1NormalCurrent)
                {
                    return null;
                }
                return DUT1NormalCurrent;
            }
            set { DUT1NormalCurrent = value; }
        }

        [Ignore]
        public string DUT1NormalCurrent { get; set; }



        [Name("DUT1SleepCurrent")]
        public string BlockDUT1SleepCurrent
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT1SleepCurrent)
                {
                    return null;
                }
                return DUT1SleepCurrent;
            }
            set { DUT1SleepCurrent = value; }
        }

        [Ignore]
        public string DUT1SleepCurrent { get; set; }



        [Name("DUT1Diagnostic")]
        public string BlockDUT1Diagnostic
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT1Diagnostic)
                {
                    return null;
                }
                return DUT1Diagnostic;
            }
            set { DUT1Diagnostic = value; }
        }

        [Ignore]
        public string DUT1Diagnostic { get; set; }



        [Name("DUT1Lightsensor")]
        public string BlockDUT1Lightsensor
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT1Lightsensor)
                {
                    return null;
                }
                return DUT1Lightsensor;
            }
            set { DUT1Lightsensor = value; }
        }

        [Ignore]
        public string DUT1Lightsensor { get; set; }



        [Name("DUT1Touchfinger")]
        public string BlockDUT1Touchfinger
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT1Touchfinger)
                {
                    return null;
                }
                return DUT1Touchfinger;
            }
            set { DUT1Touchfinger = value; }
        }

        [Ignore]
        public string DUT1Touchfinger { get; set; }



        [Name("DUT1Touch_XY")]
        public string BlockDUT1Touch_XY
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT1Touch_XY)
                {
                    return null;
                }
                return DUT1Touch_XY;
            }
            set { DUT1Touch_XY = value; }
        }

        [Ignore]
        public string DUT1Touch_XY { get; set; }



        [Name("DUT1Brightness")]
        public string BlockDUT1Brightness
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT1Brightness)
                {
                    return null;
                }
                return DUT1Brightness;
            }
            set { DUT1Brightness = value; }
        }

        [Ignore]
        public string DUT1Brightness { get; set; }



        [Name("DUT1T_chamber")]
        public string BlockDUT1T_chamber
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT1T_chamber)
                {
                    return null;
                }
                return DUT1T_chamber;
            }
            set { DUT1T_chamber = value; }
        }

        [Ignore]
        public string DUT1T_chamber { get; set; }



        [Name("DUT1T_LED1_2PCB")]
        public string BlockDUT1T_LED1_2PCB
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT1T_LED1_2PCB)
                {
                    return null;
                }
                return DUT1T_LED1_2PCB;
            }
            set { DUT1T_LED1_2PCB = value; }
        }

        [Ignore]
        public string DUT1T_LED1_2PCB { get; set; }



        [Name("DUT1Diagnostic_raw")]
        public string BlockDUT1Diagnostic_raw
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT1Diagnostic_raw)
                {
                    return null;
                }
                return DUT1Diagnostic_raw;
            }
            set { DUT1Diagnostic_raw = value; }
        }

        [Ignore]
        public string DUT1Diagnostic_raw { get; set; }



        [Name("DUT2SWversion")]
        public string BlockDUT2SWversion
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT2SWversion)
                {
                    return null;
                }
                return DUT2SWversion;
            }
            set { DUT2SWversion = value; }
        }

        [Ignore]
        public string DUT2SWversion { get; set; }



        [Name("DUT2HWversion")]
        public string BlockDUT2HWversion
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT2HWversion)
                {
                    return null;
                }
                return DUT2HWversion;
            }
            set { DUT2HWversion = value; }
        }

        [Ignore]
        public string DUT2HWversion { get; set; }



        [Name("DUT2PowerMode")]
        public string BlockDUT2PowerMode
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT2PowerMode)
                {
                    return null;
                }
                return DUT2PowerMode;
            }
            set { DUT2PowerMode = value; }
        }

        [Ignore]
        public string DUT2PowerMode { get; set; }



        [Name("DUT2Voltage")]
        public string BlockDUT2Voltage
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT2Voltage)
                {
                    return null;
                }
                return DUT2Voltage;
            }
            set { DUT2Voltage = value; }
        }

        [Ignore]
        public string DUT2Voltage { get; set; }



        [Name("DUT2NormalCurrent")]
        public string BlockDUT2NormalCurrent
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT2NormalCurrent)
                {
                    return null;
                }
                return DUT2NormalCurrent;
            }
            set { DUT2NormalCurrent = value; }
        }

        [Ignore]
        public string DUT2NormalCurrent { get; set; }



        [Name("DUT2SleepCurrent")]
        public string BlockDUT2SleepCurrent
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT2SleepCurrent)
                {
                    return null;
                }
                return DUT2SleepCurrent;
            }
            set { DUT2SleepCurrent = value; }
        }

        [Ignore]
        public string DUT2SleepCurrent { get; set; }



        [Name("DUT2Diagnostic")]
        public string BlockDUT2Diagnostic
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT2Diagnostic)
                {
                    return null;
                }
                return DUT2Diagnostic;
            }
            set { DUT2Diagnostic = value; }
        }

        [Ignore]
        public string DUT2Diagnostic { get; set; }



        [Name("DUT2Lightsensor")]
        public string BlockDUT2Lightsensor
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT2Lightsensor)
                {
                    return null;
                }
                return DUT2Lightsensor;
            }
            set { DUT2Lightsensor = value; }
        }

        [Ignore]
        public string DUT2Lightsensor { get; set; }



        [Name("DUT2Touchfinger")]
        public string BlockDUT2Touchfinger
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT2Touchfinger)
                {
                    return null;
                }
                return DUT2Touchfinger;
            }
            set { DUT2Touchfinger = value; }
        }

        [Ignore]
        public string DUT2Touchfinger { get; set; }



        [Name("DUT2Touch_XY")]
        public string BlockDUT2Touch_XY
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT2Touch_XY)
                {
                    return null;
                }
                return DUT2Touch_XY;
            }
            set { DUT2Touch_XY = value; }
        }

        [Ignore]
        public string DUT2Touch_XY { get; set; }



        [Name("DUT2Brightness")]
        public string BlockDUT2Brightness
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT2Brightness)
                {
                    return null;
                }
                return DUT2Brightness;
            }
            set { DUT2Brightness = value; }
        }

        [Ignore]
        public string DUT2Brightness { get; set; }



        [Name("DUT2T_chamber")]
        public string BlockDUT2T_chamber
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT2T_chamber)
                {
                    return null;
                }
                return DUT2T_chamber;
            }
            set { DUT2T_chamber = value; }
        }

        [Ignore]
        public string DUT2T_chamber { get; set; }



        [Name("DUT2T_LED1_2PCB")]
        public string BlockDUT2T_LED1_2PCB
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT2T_LED1_2PCB)
                {
                    return null;
                }
                return DUT2T_LED1_2PCB;
            }
            set { DUT2T_LED1_2PCB = value; }
        }

        [Ignore]
        public string DUT2T_LED1_2PCB { get; set; }



        [Name("DUT2Diagnostic_raw")]
        public string BlockDUT2Diagnostic_raw
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT2Diagnostic_raw)
                {
                    return null;
                }
                return DUT2Diagnostic_raw;
            }
            set { DUT2Diagnostic_raw = value; }
        }

        [Ignore]
        public string DUT2Diagnostic_raw { get; set; }



        [Name("DUT3SWversion")]
        public string BlockDUT3SWversion
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT3SWversion)
                {
                    return null;
                }
                return DUT3SWversion;
            }
            set { DUT3SWversion = value; }
        }

        [Ignore]
        public string DUT3SWversion { get; set; }



        [Name("DUT3HWversion")]
        public string BlockDUT3HWversion
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT3HWversion)
                {
                    return null;
                }
                return DUT3HWversion;
            }
            set { DUT3HWversion = value; }
        }

        [Ignore]
        public string DUT3HWversion { get; set; }



        [Name("DUT3PowerMode")]
        public string BlockDUT3PowerMode
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT3PowerMode)
                {
                    return null;
                }
                return DUT3PowerMode;
            }
            set { DUT3PowerMode = value; }
        }

        [Ignore]
        public string DUT3PowerMode { get; set; }



        [Name("DUT3Voltage")]
        public string BlockDUT3Voltage
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT3Voltage)
                {
                    return null;
                }
                return DUT3Voltage;
            }
            set { DUT3Voltage = value; }
        }

        [Ignore]
        public string DUT3Voltage { get; set; }



        [Name("DUT3NormalCurrent")]
        public string BlockDUT3NormalCurrent
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT3NormalCurrent)
                {
                    return null;
                }
                return DUT3NormalCurrent;
            }
            set { DUT3NormalCurrent = value; }
        }

        [Ignore]
        public string DUT3NormalCurrent { get; set; }



        [Name("DUT3SleepCurrent")]
        public string BlockDUT3SleepCurrent
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT3SleepCurrent)
                {
                    return null;
                }
                return DUT3SleepCurrent;
            }
            set { DUT3SleepCurrent = value; }
        }

        [Ignore]
        public string DUT3SleepCurrent { get; set; }



        [Name("DUT3Diagnostic")]
        public string BlockDUT3Diagnostic
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT3Diagnostic)
                {
                    return null;
                }
                return DUT3Diagnostic;
            }
            set { DUT3Diagnostic = value; }
        }

        [Ignore]
        public string DUT3Diagnostic { get; set; }



        [Name("DUT3Lightsensor")]
        public string BlockDUT3Lightsensor
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT3Lightsensor)
                {
                    return null;
                }
                return DUT3Lightsensor;
            }
            set { DUT3Lightsensor = value; }
        }

        [Ignore]
        public string DUT3Lightsensor { get; set; }



        [Name("DUT3Touchfinger")]
        public string BlockDUT3Touchfinger
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT3Touchfinger)
                {
                    return null;
                }
                return DUT3Touchfinger;
            }
            set { DUT3Touchfinger = value; }
        }

        [Ignore]
        public string DUT3Touchfinger { get; set; }



        [Name("DUT3Touch_XY")]
        public string BlockDUT3Touch_XY
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT3Touch_XY)
                {
                    return null;
                }
                return DUT3Touch_XY;
            }
            set { DUT3Touch_XY = value; }
        }

        [Ignore]
        public string DUT3Touch_XY { get; set; }



        [Name("DUT3Brightness")]
        public string BlockDUT3Brightness
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT3Brightness)
                {
                    return null;
                }
                return DUT3Brightness;
            }
            set { DUT3Brightness = value; }
        }

        [Ignore]
        public string DUT3Brightness { get; set; }



        [Name("DUT3T_chamber")]
        public string BlockDUT3T_chamber
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT3T_chamber)
                {
                    return null;
                }
                return DUT3T_chamber;
            }
            set { DUT3T_chamber = value; }
        }

        [Ignore]
        public string DUT3T_chamber { get; set; }



        [Name("DUT3T_LED1_2PCB")]
        public string BlockDUT3T_LED1_2PCB
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT3T_LED1_2PCB)
                {
                    return null;
                }
                return DUT3T_LED1_2PCB;
            }
            set { DUT3T_LED1_2PCB = value; }
        }

        [Ignore]
        public string DUT3T_LED1_2PCB { get; set; }



        [Name("DUT3Diagnostic_raw")]
        public string BlockDUT3Diagnostic_raw
        {
            get
            {
                if (Selector != null && !Selector.IncludeDUT3Diagnostic_raw)
                {
                    return null;
                }
                return DUT3Diagnostic_raw;
            }
            set { DUT3Diagnostic_raw = value; }
        }

        [Ignore]
        public string DUT3Diagnostic_raw { get; set; }



    }

}


//Time
//Loop
//Blockloop
//Blockphase
//DUT1PowerMode
//DUT1SWversion
//DUT1HWversion
//DUT1Voltage
//DUT1NormalCurrent
//DUT1SleepCurrent
//DUT1Diagnostic
//DUT1Lightsensor
//DUT1Touchfinger
//DUT1Touch_XY
//DUT1Brightness
//DUT1T_chamber
//DUT1T_LED1_2PCB
//DUT1Diagnostic_raw
//DUT2SWversion
//DUT2HWversion
//DUT2PowerMode
//DUT2Voltage
//DUT2NormalCurrent
//DUT2SleepCurrent
//DUT2Diagnostic
//DUT2Lightsensor
//DUT2Touchfinger
//DUT2Touch_XY
//DUT2Brightness
//DUT2T_chamber
//DUT2T_LED1_2PCB
//DUT2Diagnostic_raw
//DUT3SWversion
//DUT3HWversion
//DUT3PowerMode
//DUT3Voltage
//DUT3NormalCurrent
//DUT3SleepCurrent
//DUT3Diagnostic
//DUT3Lightsensor
//DUT3Touchfinger
//DUT3Touch_XY
//DUT3Brightness
//DUT3T_chamber
//DUT3T_LED1_2PCB
//DUT3Diagnostic_raw