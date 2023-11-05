using Notification.Wpf;
using SuperCarter.Model;
using SuperCarter.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SuperCarter.ViewModel
{
    public class SerialPortManager : ViewModelBase
    {
        public Thread ThreadReceiver1;
        //public  ObservableCollection<Comporttype> PortTree { get; set; } = new ObservableCollection<Comporttype>();
        public ObservableCollection<Portdetectedtype> PortTree { get; set; } = new ObservableCollection<Portdetectedtype>();
        public ObservableCollection<Portdetectedtype> SerialCandidator { get; set; } = new ObservableCollection<Portdetectedtype>();
        private System.Timers.Timer AutoDetectedPortCount { get; set; }
        public int SelectSerialportIndex { get; set; }
        public int IntrospectIntervalTimer { get; set; } = 3000;
        public SerialPortManager()
        {
            try
            {

                serialPorts = new Dictionary<string, SerialPort>();

                PortTree = new ObservableCollection<Portdetectedtype>(ConfigModel.Instance.GetComPorts());

                SerialCandidator = ConfigbyJSON.Instance.ReadPortCandidatelist();
                OnPropertyChanged(nameof(PortTree));

                AutoDetectedPortCount = new System.Timers.Timer(IntrospectIntervalTimer);

                //設定呼叫間隔時間為30ms
                AutoDetectedPortCount.Elapsed += new System.Timers.ElapsedEventHandler(UpdateSystemIntrospectionEvent);
                AutoDetectedPortCount.AutoReset = true;
                //設置 執行一次（false）;一直執行(true)
                AutoDetectedPortCount.Enabled = true;
                AutoDetectedPortCount.Start();
            }
            catch(Exception ex) {
                MessageBox.Show(ex.StackTrace);
            }
     
        }
        public void OpenPort(string portName)
        {
            try
            {
                serialPorts[portName].Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }
        }
        public void ClosePort(string portName)
        {
            try
            {
                SerialPort serialPort = serialPorts[portName];
                if (serialPort != null)
                {
                    if (serialPort.IsOpen)
                    {
                        serialPort.Close();
                        // 可以進行其他關閉串列埠後的邏輯
                    }
                    serialPorts.Remove(portName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                MessageBox.Show(ex.StackTrace);
            }

        }
        public void SendData(string portName, string dataToSend)
        {
            SerialPort serialPort = serialPorts[portName];
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Write(dataToSend);
            }
        }

        #region Serial port setting 
        private ICommand _DelectedSelectPortitem, _SelectItemChangeCommand, _ClearPortitems, _SortingPortitem;
        public ICommand DelectedSelectPortitem
        {
            get
            {
                _DelectedSelectPortitem = new RelayCommand(
                    param => envt_RemoveSelectSerialport());
                return _DelectedSelectPortitem;
            }
        }     
        public ICommand SelectItemChangeCommand
        {
            get
            {
                _SelectItemChangeCommand = new RelayCommand(
                    param => evt_SelectedSerialport((Portdetectedtype)param));
                return _SelectItemChangeCommand;
            }
        }
        public ICommand ClearPortitems
        {
            get
            {
                _ClearPortitems = new RelayCommand(
                    param => SerialCandidator.Clear());
                return _ClearPortitems;
            }
        }
        public ICommand SortingPortitem
        {
            get
            {
                _SortingPortitem = new RelayCommand(
                    param => envt_SortingSerialport());
                return _SortingPortitem;
            }
        }

        private bool _tbt__Port0, _tbt__Port1, _tbt__Port2;
        public bool tbt__Port0
        {
            get => _tbt__Port0;
            set
            {
                if (_tbt__Port0 != value)
                {
                    _tbt__Port0 = value;
                    OnPropertyChanged(nameof(tbt__Port0));

                    if (_tbt__Port0)
                    {
                        StartListeningPort(0);

                    }
                    else
                    {
                        StopListeningPort(0);

                        ThreadReceiver1?.Interrupt();
                    }
                }
            }
        }
        public bool tbt__Port1
        {
            get => _tbt__Port1;

            set
            {
                if (_tbt__Port1 != value)
                {
                    _tbt__Port1 = value;
                    OnPropertyChanged(nameof(tbt__Port1));

                    if (_tbt__Port1)
                    {
                        StartListeningPort(1);
                    }
                    else
                    {
                        StopListeningPort(1);
                    }
                }
            }
        }
        public bool tbt__Port2
        {
            get => _tbt__Port2;
            set
            {
                if (_tbt__Port2 != value)
                {
                    _tbt__Port2 = value;
                    OnPropertyChanged(nameof(tbt__Port2));

                    if (_tbt__Port2)
                    {
                        StartListeningPort(2);
                    }
                    else
                    {
                        StopListeningPort(2);
                    }
                }
            }
        }
        // This is public get-only here but you could implement a public setter which
        // also selects the item.
        // Also this should be moved to an instance property on a VM for the whole tree, 
        // otherwise there will be conflicts for more than one tree.
        private static object _selectedItem = null;
        public static object SelectedItem
        {
            get { return _selectedItem; }
            private set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                }
            }
        }
        private void evt_SelectedSerialport(Portdetectedtype _va)
        {
            SelectedItem = this;
            bool IsAllowAddingitem = false;

            if (SerialCandidator.Count < 3)
            {
                if (SerialCandidator.Count < 1)
                {
                    IsAllowAddingitem = true;
                }
                else
                {
                    foreach (var _item in SerialCandidator)
                    {
                        if (!_item.PortName.Contains(_va.PortName))
                        {
                            IsAllowAddingitem = true;
                            logger.Log(NLog.LogLevel.Debug, "catch_" + _va.PortName);
                        }
                        else
                        {
                            IsAllowAddingitem = false;
                            logger.Log(NLog.LogLevel.Debug, "catch_" + _va.PortName + " be denied");
                            MessageBox.Show("無法新增此物件，comport 已被取用", "Information !", MessageBoxButton.OK, MessageBoxImage.Information);
                            break;
                        }
                    }
                }
                if (IsAllowAddingitem)
                {
                    try
                    {
                        SerialCandidator.Add(new Portdetectedtype()
                        {
                            IsUse = false,
                            PortID = 0,
                            FullPortName = _va.FullPortName,
                            PortName = _va.PortName,
                            BaudRateValue = 115200,
                            DataReceivedCasenum = 0
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("無法新增此物件，目前數量 已達上限", "Warning !", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            OnPropertyChangedForStatic(nameof(SerialCandidator));

        }
        public void envt_RemoveSelectSerialport()
        {
            if (SelectSerialportIndex == null)
                return;
            if (SerialCandidator is not null)
            {
                if (SelectSerialportIndex > -1 && SelectSerialportIndex < SerialCandidator.Count)
                {
                    SerialCandidator.Remove(SerialCandidator[SelectSerialportIndex]);
                }
            }

        }
        public void envt_SortingSerialport()
        {
            if (SerialCandidator.Count > 1)
            {
                SerialCandidator = new ObservableCollection<Portdetectedtype>(SerialCandidator.OrderBy(item => item.PortID));

                OnPropertyChanged(nameof(SerialCandidator));
            }
        }
        public void AutoSaveStatus()
        {
            ConfigbyJSON.Instance.WritePortCandidatelist(SerialCandidator);
        }
        public void UpdateSystemIntrospectionEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            PortTree = new ObservableCollection<Portdetectedtype>(ConfigModel.Instance.GetComPorts());
           
            if (tbt__Port0)
            {
                if (!DicSerialPort[0].IsOpen)
                {
                    tbt__Port0 = false;
                }
                else
                {
                    var portName = DicSerialPort[0].PortName;
                    // 使用 LINQ 查找具有指定 PortName 的項目
                    var portdetectedtype = SerialCandidator.FirstOrDefault(item => item.PortName == portName);
                    if (portdetectedtype != null)
                        if (portdetectedtype.PortID != 0) tbt__Port0 = false;
                }
                   
            }
            if (tbt__Port1)
            {
                if (!DicSerialPort[1].IsOpen)
                    tbt__Port1 = false;
                else
                {
                    var portName = DicSerialPort[1].PortName;
                    // 使用 LINQ 查找具有指定 PortName 的項目
                    var portdetectedtype = SerialCandidator.FirstOrDefault(item => item.PortName == portName);
                    if (portdetectedtype != null)
                        if (portdetectedtype.PortID != 1) tbt__Port1 = false;
                }
            }
            if (tbt__Port2)
            {
                if (!DicSerialPort[2].IsOpen)
                    tbt__Port2 = false;
                else
                {
                    var portName = DicSerialPort[2].PortName;
                    // 使用 LINQ 查找具有指定 PortName 的項目
                    var portdetectedtype = SerialCandidator.FirstOrDefault(item => item.PortName == portName);
                    if (portdetectedtype != null)
                        if (portdetectedtype.PortID != 2) tbt__Port2 = false;
                }
            }
            OnPropertyChanged(nameof(PortTree));
        }

        /// <summary>
        /// 開始與 port 進行連線通訊
        /// </summary>
        /// <param name="_id"></param>
        private void StartListeningPort(int _id)
        {

            if (CheckPortlistinfo(_id))
            {
                try
                {

                    var _item = SerialCandidator.Where(i => i.PortID == _id).ToList();

                    if (!DicSerialPort[_id].IsOpen)
                    {
                        DicSerialPort[_id].PortName = _item[0].PortName;
                        DicSerialPort[_id].BaudRate = _item[0].BaudRateValue;
                        DicSerialPort[_id].Parity = Parity.None;
                        DicSerialPort[_id].DataBits = 8;
                        DicSerialPort[_id].StopBits = System.IO.Ports.StopBits.One;
                        DicSerialPort[_id].DtrEnable = false;
                        DicSerialPort[_id].RtsEnable = false;
                        DicSerialPort[_id].Handshake = System.IO.Ports.Handshake.None;
                        DicSerialPort[_id].Open();
                        logger.Log(NLog.LogLevel.Debug, "linked with " + _item[0].PortName);

                        // DicSerialPort[_id].DataReceived += new SerialDataReceivedEventHandler(DataReceivedCom0);

                        if (_id == 0)
                        {
                            tbt__Port0 = true;

                            MessageAggregator.Instance.SendMessage(new POPNotifyMsgType
                            {
                                Tital = "通知",
                                Message = string.Format("Open Port : {0} - Name : {1} Baudrate :{2}", _id, DicSerialPort[0].PortName, DicSerialPort[0].BaudRate),
                                NotifyType = NotificationType.Notification,

                            });
                        }


                        else if (_id == 1)
                        {
                            tbt__Port1 = true;

                            MessageAggregator.Instance.SendMessage(new POPNotifyMsgType
                            {
                                Tital = "通知",
                                Message = string.Format("Open Port {0} - Name : {1} Baudrate :{2}", _id, DicSerialPort[1].PortName, DicSerialPort[1].BaudRate),
                                NotifyType = NotificationType.Notification,

                            });

                        }

                        else if (_id == 2)
                        {
                            tbt__Port2 = true;
                            MessageAggregator.Instance.SendMessage(new POPNotifyMsgType
                            {
                                Tital = "通知",
                                Message = string.Format("Open Port {0} - Name : {1} Baudrate :{2}", _id, DicSerialPort[2].PortName, DicSerialPort[2].BaudRate),
                                NotifyType = NotificationType.Notification,

                            });
                        }
                    }

                    //
                    SerialPortModel.Instance.PortNameBinding.Add(_item[0].PortName, _id);
                }
                catch (Exception ex)
                {
                    if (_id == 0)
                        tbt__Port0 = false;
                    else if (_id == 1)
                        tbt__Port1 = false;
                    else if (_id == 2)
                        tbt__Port2 = false;

                    MessageAggregator.Instance.SendMessage(new POPNotifyMsgType
                    {
                        Tital = "通知",
                        Message = "設置異常，請至 '設定' >> '埠設定' 設定與查看。",
                        NotifyType = NotificationType.Warning,

                    });
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                if (_id == 0)
                    tbt__Port0 = false;
                else if (_id == 1)
                    tbt__Port1 = false;
                else if (_id == 2)
                    tbt__Port2 = false;

            }
            if (_id == 0)
                OnPropertyChanged(nameof(tbt__Port0));
            else if (_id == 1)
                OnPropertyChanged(nameof(tbt__Port1));
            else if (_id == 2)
                OnPropertyChanged(nameof(tbt__Port2));
            OnPropertyChangedForStatic(nameof(SerialCandidator));
        }

        /// <summary>
        /// 關閉串列埠
        /// </summary>
        /// <param name="_id"></param>
        private void StopListeningPort(int _id)
        {

            if (DicSerialPort[_id].IsOpen)
            {
                string tempname = null;
                try
                {
                    tempname = DicSerialPort[_id].PortName;
                    SerialPortModel.Instance.PortNameBinding.Remove(DicSerialPort[_id].PortName);
                    DicSerialPort[_id].Close();
                    DicSerialPort[_id].Dispose();
                    DicSerialPort[_id] = new SerialPort();
                    logger.Log(NLog.LogLevel.Debug, "Stop listening" + tempname);
                    MessageAggregator.Instance.SendMessage(new POPNotifyMsgType
                    {
                        Tital = "通知",
                        Message = string.Format("Closed Port : {0}, Portname : {1}", _id, tempname),
                        NotifyType = NotificationType.Notification,

                    });
                }
                catch (Exception ex)
                {
                    logger.Log(NLog.LogLevel.Debug, "linked with " + ex.StackTrace);
                    throw;
                }

            }
        }

        /// <summary>
        /// check this SerialCandidator's Port ID, It is correctly written
        /// </summary>
        /// <returns></returns>
        private bool CheckPortlistinfo(int _id)
        {
            if (SerialCandidator.Count < 1)
            {
                MessageAggregator.Instance.SendMessage(new POPNotifyMsgType
                {
                    Tital = "通知",
                    Message = string.Format("{0}", "候選序列埠無資料，設置埠偵測到異常，請至 '設定' >> '埠設定' 設定與查看。"),
                    NotifyType = NotificationType.None,

                });
                tbt__Port1 = false;
                return false;
            }
            List<bool> portchecklist = new List<bool>() { false, false, false };

            foreach (var item in SerialCandidator)
            {
                if (portchecklist[Convert.ToInt32(item.PortID)] == false)
                {
                    portchecklist[Convert.ToInt32(item.PortID)] = true;
                }
                else
                {
                    tbt__Port1 = false;
                    MessageAggregator.Instance.SendMessage(new POPNotifyMsgType
                    {
                        Tital = "通知",
                        Message = string.Format("{0}", "Port ID 設定異常，設置埠偵測到異常，請至 '設定' >> '埠設定' 設定與查看。"),
                        NotifyType = NotificationType.None,

                    });

                    return false;

                }
            }
            if (_id == 0)
                OnPropertyChanged(nameof(tbt__Port0));
            else if (_id == 1)
                OnPropertyChanged(nameof(tbt__Port1));
            else if (_id == 2)
                OnPropertyChanged(nameof(tbt__Port2));
            OnPropertyChangedForStatic(nameof(SerialCandidator));
            return (portchecklist[_id]) ? true : false;

        }
        #endregion

        #region StartListening


        #endregion

    }
}
