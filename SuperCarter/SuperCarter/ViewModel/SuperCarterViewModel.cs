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

namespace SuperCarter.ViewModel
{
    public class SuperCarterViewModel : ViewModelBase
    {
    
        public ScriptEditor scriptEditor { get; set; }
        public SerialPortManager serialportmanager{get;set;}
        public InitialStateConfirm initialStateConfirm { get; set; }
        public CustomscriptExecution Customscript { get; set; }
        public int SelectSerialportIndex { get; set; }

        public SuperCarterViewModel() {

            // 允許定位該事件並且供給給其他視圖使用
            MessageAggregator.Instance.Subscribe(evt_promptlyMessageNotify);
            ConfigModel.OnRaisePopNotification += evt_promptlyMessageNotify;
            nlogMessageAggregator.Instance.Subscribe(evt_popNlogMessageNotify);

            scriptEditor = new ScriptEditor();
            serialportmanager = new SerialPortManager();
            initialStateConfirm = new InitialStateConfirm();

            Customscript = new CustomscriptExecution();
            //Customscript.serialPortViewModelBase = serialPortViewModelBase;
            Customscript.UpdateDashboardStartThread();
        }
        ~SuperCarterViewModel()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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

    }
}
