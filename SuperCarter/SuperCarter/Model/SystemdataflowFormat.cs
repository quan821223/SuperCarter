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

namespace SuperCarter.Model
{
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

    }

}
