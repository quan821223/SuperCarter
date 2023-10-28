using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

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
        public string ParityName { get; set; }
        public Parity ParityValue { get; set; }
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


}
