using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperCarter.Services;
using SuperCarter.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop;
using System.Windows;
using System.Globalization;
using System.IO.Ports;
using System.IO;
using System.Windows.Interop;
using SuperCarter.View.Dashboard;
using System.ComponentModel;
using System.Reflection;

namespace SuperCarter.ViewModel
{
    public class SuperCarterViewModel : ViewModelBase
    {

        public ScheduledScriptEditor scheduledscriptEditor { get; set; }
        public ScriptEditor scriptEditor { get; set; }
        public SerialPortManager serialportmanager { get; set; }
        public InitialStateConfirm initialStateConfirm { get; set; }
        public int SelectSerialportIndex { get; set; }
        public CustomScriptEditor CustomScriptEditor { get; set; } = new CustomScriptEditor();
        public string SWversion { get; set; } 
        public SuperCarterViewModel() {
            SWversion = "v0.0.4";

            // 允許定位該事件並且供給給其他視圖使用
            MessageAggregator.Instance.Subscribe(evt_promptlyMessageNotify);
            ConfigModel.OnRaisePopNotification += evt_promptlyMessageNotify;
            nlogMessageAggregator.Instance.Subscribe(evt_popNlogMessageNotify);
            WritedataToViewTextAggregator.Instance.Subscribe(DisplayInfoToViewText);

            CustomScriptEditor = new CustomScriptEditor();
            CustomScriptEditor.UpdateDashboardStartThread();

            scheduledscriptEditor = new ScheduledScriptEditor();
            scriptEditor = new ScriptEditor();
            serialportmanager = new SerialPortManager();
            initialStateConfirm = new InitialStateConfirm();

            // 更新 View Text UI 的程序
            var systemMissonExecute = new SystemMissonExecute();
            systemMissonExecute.StartThread();
        }
        ~SuperCarterViewModel()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 分類出該訊息於系統與各 port 的通訊內容 
        /// </summary>
        /// <param name="Portid"></param>
        /// <param name="msg"></param>
        public void DisplayInfoToViewText(RealtimeMsgQueuetype data)
        {
            if (data.msgtype == 0)
            {
                AllViewText += data.msg + "\r\n";
            }
            else
            {
                AllViewText += data.msg + "\r\n";
                if (data.PortNum == 0)
                    TextViewPortI += data.msg + "\r\n";
                else if (data.PortNum == 1)
                    TextViewPortII += data.msg + "\r\n";
                else if (data.PortNum == 2)
                    TextViewPortIII += data.msg + "\r\n";
            }


            OnPropertyChanged(nameof(AllViewText));
        }
        /// <summary>
        /// that is a pop notify message  
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        /// <param name="_NotificationType"></param>      
        /// 
        public void evt_promptlyMessageNotify(POPNotifyMsgType _popmsg)
        {
            var content = new NotificationContent { Title = _popmsg.Tital, Message = _popmsg.Message, Type = _popmsg.NotifyType };

            var clickContent = new NotificationContent
            {
                Title = "DQA notification msg！💛",
                Message = _popmsg.Tital + _popmsg.Message,
                Type = _popmsg.NotifyType
            };
            _notificationManager.Show(content, "WindowArea", onClick: () => _notificationManager.Show(clickContent));

        }
        private readonly NotificationManager _notificationManager = new NotificationManager();

        public void evt_popNlogMessageNotify(nlogtype _popmsg)
        {
            logger.Log(_popmsg.LogLevel, _popmsg.Msg);
        }

        private ICommand _ViewNaviIMG;
        public ICommand ViewNaviIMG
        {
            get
            {
                _ViewNaviIMG = new RelayCommand(
                      param => ViewIMG(param));
                return _ViewNaviIMG;
            }
        }

        private string _PageName = "navi_img1.xaml";
        public string PageName 
        {
            get { return _PageName; }
            set { _PageName = value; OnPropertyChanged(nameof(PageName)); }
        }
        private void ViewIMG(object obj)
        {
            PageName = obj.ToString();
        }
        #region Single Sendor Execute function.  
        /// <summary>
        /// 功能:觸發介面 
        /// 發送在腳本編譯器的指令內容
        /// </summary>
        private ICommand _ScriptSenderExecute;
        public ICommand ScriptSenderExecute
        {
            get
            {
                if (_ScriptSenderExecute == null)
                {
                    _ScriptSenderExecute = new DelegateCommand(ScriptitemCanExecute, ScriptitemExecute);
                    OnPropertyChanged(nameof(ScriptSenderExecute));
                }
                return _ScriptSenderExecute;
            }
        }
        private bool ScriptitemCanExecute(object parameter)
        {
            return true;
        }
        /// <summary>
        /// 功能:觸發方法
        /// 發送在腳本編譯器的指令內容
        /// </summary>
        /// <param name="parameter"></param>
        private async void ScriptitemExecute(object parameter)
        {
            int index = Scriptdatalist.IndexOf(parameter as ScriptItemtype);
            if (Scriptdatalist[index].Portnum == "all")
            {
                for (int inum = 0; inum < DicSerialPort.Count; inum++)
                {
                    if (DicSerialPort[inum].IsOpen)
                        serialportmanager.evnt_sendAsync(inum, Scriptdatalist[index]);
              
                }    
            }
            else
            {
                if (DicSerialPort[Convert.ToInt32(Scriptdatalist[index].Portnum)].IsOpen)
                    serialportmanager.evnt_sendAsync(Convert.ToInt32(Scriptdatalist[index].Portnum), Scriptdatalist[index]);
          
            }  
        }
        #endregion Single Sendor Execute function.  



    }
}
