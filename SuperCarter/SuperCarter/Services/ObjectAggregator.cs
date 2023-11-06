using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperCarter.Services
{
    public class ObjectAggregator
    {
        private static readonly ObjectAggregator _instance = new ObjectAggregator();
        public static ObjectAggregator Instance => _instance;

        public delegate void ObjectReceivedHandler();
        public event ObjectReceivedHandler Propertychange;

        public void UpdateObject()
        {
            Propertychange?.Invoke();
        }

        public void Subscribe(ObjectReceivedHandler handler)
        {
            Propertychange += handler;
        }

        public void Unsubscribe(ObjectReceivedHandler handler)
        {
            Propertychange -= handler;
        }
    }
    public class WritedataToViewTextAggregator
    {
        private static readonly WritedataToViewTextAggregator _instance = new WritedataToViewTextAggregator();
        public static WritedataToViewTextAggregator Instance => _instance;

        public delegate void ObjectReceivedHandler(int Portid, string msg);
        public event ObjectReceivedHandler Propertychange;

        public void Updatemsg(int Portid, string msg)
        {
            Propertychange?.Invoke( Portid,  msg);
        }

        public void Subscribe(ObjectReceivedHandler handler)
        {
            Propertychange += handler;
        }

        public void Unsubscribe(ObjectReceivedHandler handler)
        {
            Propertychange -= handler;
        }
    }
}
