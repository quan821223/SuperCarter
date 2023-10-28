using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using NLog;
namespace SuperCarter.Model
{
    public class SerialPortModel : SingletonBase<SerialPortModel>
    {
        public static Logger logger { get; set; }
        public SerialPortModel()
        {
            PortNameBinding = new Dictionary<string, int>();
        }
        public string DeviceID { get; set; }
        public string DeviceInfo { get; set; }
        public string BaudRateName { get; set; }
        public int BaudRateValue { get; set; }
        public string ParityName { get; set; }
        public Parity ParityValue { get; set; }
        public int DataReceivedCasenum { get; set; }

        /// <summary>
        /// the bytes data convert to string.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        /// 
        public string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {                 
                for (int i = 0; i < bytes.Length; i++)
                {
                    //returnStr = Convert.ToString(bytes[i], 16);
                    returnStr += bytes[i].ToString("X2");
                    returnStr += bytes[i].ToString(" ");
                }
            }
            return returnStr.Trim();
        }


        public string RealtimeTextview(string type, string portname, string msg)
        {

            return String.Format("{0} [{1}] [{2}] [S] {3}",
                DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ffff"),
                SerialPortModel.Instance.PortNameBinding[portname],
                portname,
                msg.Replace(" ", ""));
        }

        public byte[] HexStrToByte(string strhex)
        {
            int SendCount = 0;
            string[] _sendData = strhex.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            byte[] BytedData = new byte[_sendData.Length];
            foreach (var tmp in _sendData)
            {

                BytedData[SendCount++] = byte.Parse(tmp, NumberStyles.AllowHexSpecifier, new CultureInfo(CultureInfo.CurrentUICulture.Name));
            }
            return BytedData;
        }

        public Dictionary<string, int> PortNameBinding { get; set; }


        public ObservableCollection<Portdetectedtype> GetComPorts()
        {
            ObservableCollection<Portdetectedtype> RootNode = new ObservableCollection<Portdetectedtype>();
            RootNode.Clear();

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

                    RootNode.Add(new Portdetectedtype()
                    {

                        FullPortName = s_Caption,
                        PortName = s_subCaption,
                        BaudRateValue = 115200,
                       
                    });
                }
                i_Entity.Dispose();
            }
            return RootNode;

        }
    }
}
