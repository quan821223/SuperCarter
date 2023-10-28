using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperCarter.Services
{
    public class nlogMessageAggregator
    {
        private static readonly nlogMessageAggregator _instance = new nlogMessageAggregator();
        public static nlogMessageAggregator Instance => _instance;

        public delegate void MessageReceivedHandler(nlogtype _popmsg);
        public event MessageReceivedHandler MessageReceived;

        public void SendMessage(nlogtype _popmsg)
        {
            MessageReceived?.Invoke(_popmsg);
        }

        public void Subscribe(MessageReceivedHandler handler)
        {
            MessageReceived += handler;
        }

        public void Unsubscribe(MessageReceivedHandler handler)
        {
            MessageReceived -= handler;
        }

    }
    public class nlogtype
    { 
        public NLog.LogLevel LogLevel { get; set; }
        public string Msg { get; set; }
    
    }
    public class MessageAggregator
    {
        private static readonly MessageAggregator _instance = new MessageAggregator();
        public static MessageAggregator Instance => _instance;

        public delegate void MessageReceivedHandler(POPNotifyMsgType _popmsg);
        public event MessageReceivedHandler MessageReceived;

        public void SendMessage(POPNotifyMsgType _popmsg)
        {
            MessageReceived?.Invoke(_popmsg);
        }

        public void Subscribe(MessageReceivedHandler handler)
        {
            MessageReceived += handler;
        }

        public void Unsubscribe(MessageReceivedHandler handler)
        {
            MessageReceived -= handler;
        }
    }
    public class RealtimeMessageAggregator
    {
        private static readonly RealtimeMessageAggregator _instance = new RealtimeMessageAggregator();
        public static RealtimeMessageAggregator Instance => _instance;

        public delegate void MessageReceivedHandler(int Portid, string msg);
        public event MessageReceivedHandler MessageReceived;

        public void SendMessage(int Portid, string msg)
        {
            MessageReceived?.Invoke(Portid, msg);
        }

        public void Subscribe(MessageReceivedHandler handler)
        {
            MessageReceived += handler;
        }

        public void Unsubscribe(MessageReceivedHandler handler)
        {
            MessageReceived -= handler;
        }
    }
    public class POPNotifyMsgType
    {
        public NotificationType NotifyType { get; set; } = NotificationType.Notification;
        public string Tital { get; set; } = "";
        public string Message { get; set; } = "";

    }
}
