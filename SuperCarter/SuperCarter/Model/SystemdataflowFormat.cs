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
    public class SystemdataflowFormat
    {
    }

    public class RealtimeMsgQueuetype
    {
        public enum PROPERTY
        {
            COM,
            MSG
        }
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
        public byte[] SequenceData { get; set; }
        public string strSequenceData { get; set; }
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

    public class ScriptItemtype
    {
        public int ID { get; set; } = 0;
        //public PortIDItems lists { get; set; }
        public string SelectScriptPortnum { get; set; } = "all";
        public string Porttype { get; set; }
        public string Nodename { get; set; } = "";
        public string MSGname { get; set; } = "";
        public string Sequence { get; set; } = "";
        public int Delaytime { get; set; } = 200;
        public string RecSequence { get; set; } = "";
        public string HashCodevalue { get; set; } = "";
        public List<ScriptItemtype> ChildNode { get; set; }
        public int Loop { get; set; } = 1;
    }
    public class SendAndReceiveDatabatchcheck
    {
        public int CommnadID { get; set; }
        public int Portnum { get; set; }
        public string strSequenceData_send { get; set; }
        public string strSequenceData_Rec { get; set; }
        public byte[] byte_buffer_Send { get; set; }
        public byte[] byte_buffer_Receive { get; set; }
    }

    public class UnifiedHostCommandSettype
    {
        [Ignore]
        private ConcurrentQueue<double>  _DUT1Currentlist;
        [Ignore]
        private ConcurrentQueue<double> _DUT2Currentlist;
        [Ignore]
        private ConcurrentQueue<double> _DUT3Currentlist;
        // working information
        [Name("Time")]
        public string Time { get; set; }
        [Name("Loop")]
        public string Loop { get; set; }

        [Name("Blockloop")]
        public string Blockloop { get; set; }
        [Name("Phase")]
        public string Blockphase { get; set; }
        [Name("#1 PowerMode")]
        public string DUT1PowerMode { get; set; }
        [Ignore]
        public ConcurrentQueue<double> DUT1Currentlist
        {
            get => _DUT1Currentlist;
            set { 
                _DUT1Currentlist= value;
                CheckoutCurlist(DUT1Currentlist);
            }
        }
        [Ignore]
        public ConcurrentQueue<double> DUT2Currentlist
        {
            get => _DUT2Currentlist;
            set
            {
                _DUT2Currentlist = value;
                CheckoutCurlist(DUT2Currentlist);
            }
        }
        [Ignore]
        public ConcurrentQueue<double> DUT3Currentlist
        {
            get => _DUT3Currentlist;
            set
            {
                _DUT3Currentlist = value;
                CheckoutCurlist(DUT3Currentlist);
            }
        }
        [Ignore]
        public string DUT1SWversion { get; set; }
        [Ignore]
        public string DUT2SWversion { get; set; }
        [Ignore]
        public string DUT3SWversion { get; set; }
        [Ignore]
        public string DUT1HWversion { get; set; }
        [Ignore]
        public string DUT2HWversion { get; set; }
        [Ignore]
        public string DUT3HWversion { get; set; }
        // for DUT1
        [Name("#1 InputVoltage")]
        public string DUT1Voltage { get; set; }
        [Name("#1 Current(A/uA)")]
        public string DUT1NormalCurrent { get; set; }
        [Name("#1 Current(uA)")]
        public string DUT1SleepCurrent { get; set; }
        [Name("#1 Diagnostic")]
        public string DUT1Diagnostic { get; set; }
        [Name("#1 Lightsensor")]
        public string DUT1Lightsensor { get; set; }
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
        [Name("#2 PowerMode")]
        public string DUT2PowerMode { get; set; }
        [Name("#2 InputVoltage")]
        public string DUT2Voltage { get; set; }
        [Name("#2 Current(A/uA)")]
        public string DUT2NormalCurrent { get; set; }
        [Name("#2 Current(uA)")]
        public string DUT2SleepCurrent { get; set; }
        [Name("#2 Diagnostic")]
        public string DUT2Diagnostic { get; set; }
        [Name("#2 Lightsensor")]
        public string DUT2Lightsensor { get; set; }
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
        [Name("#3 PowerMode")]
        public string DUT3PowerMode { get; set; }
        [Name("#3 InputVoltage")]
        public string DUT3Voltage { get; set; }
        [Name("#3 Current(A/uA)")]
        public string DUT3NormalCurrent { get; set; }
        [Name("#3 Current(uA)")]
        public string DUT3SleepCurrent { get; set; }
        [Name("#3 Diagnostic")]
        public string DUT3Diagnostic { get; set; }
        [Name("#3 Lightsensor")]
        public string DUT3Lightsensor { get; set; }
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
        public void CheckoutCurlist(ConcurrentQueue<double> DUTCurrentlist)
        {
            if (DUTCurrentlist.Count > 20)
                DUTCurrentlist.TryDequeue(out double bytes);
                    
        }

        public string[] ToCsvRecord(string functionName)
        {
            switch (functionName)
            {
                case "Function1":
                    return new[]
                    {
                    Time,
                    Loop,
                    Blockloop,
                    Blockphase,
                    DUT1PowerMode,
                    DUT1Voltage ,
                    DUT1NormalCurrent ,
                    DUT1SleepCurrent ,
                    DUT1Diagnostic ,
                    DUT1Lightsensor ,
                    DUT1Touchfinger ,
                    DUT1Touch_XY ,
                    DUT1Brightness ,
                    DUT1T_chamber ,
                    DUT1T_LED1_2PCB ,
                    DUT1Diagnostic_raw ,
                    DUT2PowerMode ,
                    DUT2Voltage ,
                    DUT2NormalCurrent ,
                    DUT2SleepCurrent ,
                    DUT2Diagnostic ,
                    DUT2Lightsensor ,
                    DUT2Touchfinger ,
                    DUT2Touch_XY ,
                    DUT2Brightness ,
                    DUT2T_chamber ,
                    DUT2T_LED1_2PCB ,
                    DUT2Diagnostic_raw ,
                    DUT3PowerMode ,
                    DUT3Voltage ,
                    DUT3NormalCurrent ,
                    DUT3SleepCurrent ,
                    DUT3Diagnostic ,
                    DUT3Lightsensor ,
                    DUT3Touchfinger ,
                    DUT3Touch_XY ,
                    DUT3Brightness ,
                    DUT3T_chamber ,
                    DUT3T_LED1_2PCB ,
                    DUT3Diagnostic_raw ,
                };
                case "Function2":
                    return new[]
                    {
                        $" #Sample 1 SWversion=({    DUT1SWversion})",
                        $" #Sample 1 HWversion=({    DUT1HWversion})",
                        $" #Sample 1 PowerMode=({    DUT1PowerMode})",
                        $" #Sample 1 Voltage =({    DUT1Voltage })",
                        $" #Sample 1 NormalCurrent =({    DUT1NormalCurrent })",
                        $" #Sample 1 SleepCurrent =({    DUT1SleepCurrent })",
                        $" #Sample 1 Diagnostic =({    DUT1Diagnostic })",
                        $" #Sample 1 Lightsensor =({    DUT1Lightsensor })",
                        $" #Sample 1 Touchfinger =({    DUT1Touchfinger })",
                        $" #Sample 1 Touch_XY =({    DUT1Touch_XY })",
                        $" #Sample 1 Brightness =({    DUT1Brightness })",
                        $" #Sample 1 T_chamber =({    DUT1T_chamber })",
                        $" #Sample 1 T_LED1_2PCB =({    DUT1T_LED1_2PCB })",
                        $" #Sample 1 Diagnostic_raw =({    DUT1Diagnostic_raw })",
                        $" #Sample 2 SWversion=({    DUT2SWversion})",
                        $" #Sample 2 HWversion=({    DUT2HWversion})",
                        $" #Sample 2 PowerMode =({    DUT2PowerMode })",
                        $" #Sample 2 Voltage =({    DUT2Voltage })",
                        $" #Sample 2 NormalCurrent =({    DUT2NormalCurrent })",
                        $" #Sample 2 SleepCurrent =({    DUT2SleepCurrent })",
                        $" #Sample 2 Diagnostic =({    DUT2Diagnostic })",
                        $" #Sample 2 Lightsensor =({    DUT2Lightsensor })",
                        $" #Sample 2 Touchfinger =({    DUT2Touchfinger })",
                        $" #Sample 2 Touch_XY =({    DUT2Touch_XY })",
                        $" #Sample 2 Brightness =({    DUT2Brightness })",
                        $" #Sample 2 T_chamber =({    DUT2T_chamber })",
                        $" #Sample 2 T_LED1_2PCB =({    DUT2T_LED1_2PCB })",
                        $" #Sample 2 Diagnostic_raw =({    DUT2Diagnostic_raw })",
                        $" #Sample 3 SWversion=({    DUT3SWversion})",
                        $" #Sample 3 HWversion=({    DUT3HWversion})",
                        $" #Sample 3 PowerMode =({    DUT3PowerMode })",
                        $" #Sample 3 Voltage =({    DUT3Voltage })",
                        $" #Sample 3 NormalCurrent =({    DUT3NormalCurrent })",
                        $" #Sample 3 SleepCurrent =({    DUT3SleepCurrent })",
                        $" #Sample 3 Diagnostic =({    DUT3Diagnostic })",
                        $" #Sample 3 Lightsensor =({    DUT3Lightsensor })",
                        $" #Sample 3 Touchfinger =({    DUT3Touchfinger })",
                        $" #Sample 3 Touch_XY =({    DUT3Touch_XY })",
                        $" #Sample 3 Brightness =({    DUT3Brightness })",
                        $" #Sample 3 T_chamber =({    DUT3T_chamber })",
                        $" #Sample 3 T_LED1_2PCB =({    DUT3T_LED1_2PCB })",
                        $" #Sample 3 Diagnostic_raw =({    DUT3Diagnostic_raw })",

                    };
                
                default:
                    // Default to return all properties
                    return GetType().GetProperties().Where(p => Attribute.IsDefined(p, typeof(NameAttribute))).Select(p => p.GetValue(this)?.ToString()).ToArray();
            }
        }

    }

}


//        case "Function2":
//            var csvRecord = new StringBuilder();
//csvRecord.AppendLine($"swversion = ({DUT1SWversion})");
//csvRecord.AppendLine($"hwversion = ({DUT2SWversion})");
//csvRecord.AppendLine(DUT1NormalCurrent);
//csvRecord.AppendLine(DUT1SleepCurrent);
//csvRecord.AppendLine(DUT2NormalCurrent);
//csvRecord.AppendLine(DUT2SleepCurrent);
//csvRecord.AppendLine(DUT3NormalCurrent);
//csvRecord.AppendLine(DUT3SleepCurrent);
//// Add more properties vertically as desired

//return new[] { csvRecord.ToString() };