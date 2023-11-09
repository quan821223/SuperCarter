using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using SuperCarter.Model;
using System.Windows;

namespace SuperCarter.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        public static Dictionary<string, SerialPort> serialPorts;
        public static int selectedserialtest { get; set; }
        public static List<SendorExecuteSendType> SendorExecuteSequencesList { get; set; } = new List<SendorExecuteSendType>();
       
        public static ObservableCollection<ScriptItemtype> Scriptdatalist { get; set; } = new ObservableCollection<ScriptItemtype>();

        public static Logger logger = LogManager.GetCurrentClassLogger();
        internal CultureInfo CultureInfos = new CultureInfo(CultureInfo.CurrentUICulture.Name);
        public readonly string AppPath = System.Windows.Forms.Application.StartupPath;
        public readonly string FOLDER_config = System.Windows.Forms.Application.StartupPath + @"config\";
        public readonly string FOLDER_SCRIPT = System.Windows.Forms.Application.StartupPath + @"scripts\";
        public readonly string FOLDER_IMG = System.Windows.Forms.Application.StartupPath + @"img\";
        public readonly string FOLDER_UTIL = System.Windows.Forms.Application.StartupPath + @"util\";
        public readonly string FOLDER_ACCESS = System.Windows.Forms.Application.StartupPath + @"access\";
        public readonly string FOLDER_RESULT = System.Windows.Forms.Application.StartupPath + @"result\";
        public readonly string FOLDER_DYNAMICMONITOR = System.Windows.Forms.Application.StartupPath + @"result\Dynamic\";
        public static Dictionary<int, SerialPort> DicSerialPort { get;  set; } = new Dictionary<int, SerialPort>();
        //public static ObservableCollection<Portdetectedtype> SerialCandidator { get; set; } = new ObservableCollection<Portdetectedtype>();
        //public static string AllViewText = null;
        private string _TextViewPortI, _TextViewPortII, _TextViewPortIII, _TextViewTestJSON, _PreviewScriptiontext, _TestTextView;

        public static Queue<byte[]> RealtimeSDMDataQueue = new Queue<byte[]>();
        public static ConcurrentQueue<RealtimeMsgQueuetype> RealtimeMsgQueue = new ConcurrentQueue<RealtimeMsgQueuetype>();
        //public ConcurrentQueue<SendAndReceiveDatabatchcheck> SendAndReceiveDatabatchQ = new ConcurrentQueue<SendAndReceiveDatabatchcheck>();
        public ConcurrentQueue<SendAndReceiveDatabatchcheck> SendAndReceiveDatabatchQ { get; } = new ConcurrentQueue<SendAndReceiveDatabatchcheck>();


        public string TestTextView
        {
            get { return _TestTextView; }
            set
            {
                _TestTextView = value;
                OnPropertyChanged(nameof(TestTextView));
            }
        }
        public string PreviewScriptiontext
        {
            get { return _PreviewScriptiontext; }
            set
            {
                _PreviewScriptiontext = value;
                OnPropertyChanged(nameof(PreviewScriptiontext));
            }
        }
        private string _AllViewText;
        public string AllViewText
        {
            get { return _AllViewText; }
            set
            {
                _AllViewText = value;
                OnPropertyChanged(nameof(AllViewText));
            }
        }
        public string TextViewPortI
        {
            get { return _TextViewPortI; }
            set
            {
                _TextViewPortI = value;
                OnPropertyChanged(nameof(TextViewPortI));
            }
        }
        public string TextViewPortII
        {
            get { return _TextViewPortII; }
            set
            {
                _TextViewPortII = value;
                OnPropertyChanged(nameof(TextViewPortII));
            }
        }
        public string TextViewPortIII
        {
            get { return _TextViewPortIII; }
            set
            {
                _TextViewPortIII = value;
                OnPropertyChanged(nameof(TextViewPortIII));
            }
        }
        public string TextViewTestJSON
        {
            get { return _TextViewTestJSON; }
            set
            {
                _TextViewTestJSON = value;
                OnPropertyChanged(nameof(TextViewTestJSON));
            }
        }

        /// <summary>
        /// 分類出該訊息於系統與各 port 的通訊內容 
        /// </summary>
        /// <param name="Portid"></param>
        /// <param name="msg"></param>
        public void DisplayInfoToViewText(Msgtype datatype, int Portid, string msg)
        {
            if (datatype == 0)
            {
                AllViewText += msg + "\r\n";
            }
            else
            {
                if (Portid == 0)
                    TextViewPortI += msg + "\r\n";
                else if (Portid == 1)
                    TextViewPortII += msg + "\r\n";
                else if (Portid == 2)
                    TextViewPortIII += msg + "\r\n";
            }
     

            OnPropertyChanged(nameof(AllViewText));
        }


        protected ViewModelBase()
        {
            // 預設生成的檔案位置
            if (Directory.Exists(AppPath + @"\access") == false) Directory.CreateDirectory(AppPath + @"\access");
            if (Directory.Exists(AppPath + @"\util") == false) Directory.CreateDirectory(AppPath + @"\util");
            if (Directory.Exists(AppPath + @"\config") == false) Directory.CreateDirectory(AppPath + @"\config");
            if (Directory.Exists(AppPath + @"\img") == false) Directory.CreateDirectory(AppPath + @"\img");
            if (Directory.Exists(AppPath + @"\src") == false) Directory.CreateDirectory(AppPath + @"\src");
            if (Directory.Exists(AppPath + @"\scripts") == false) Directory.CreateDirectory(AppPath + @"\scripts");
            if (Directory.Exists(AppPath + @"\scripts\macro") == false) Directory.CreateDirectory(AppPath + @"\scripts\macro");
            if (Directory.Exists(AppPath + @"\result") == false) Directory.CreateDirectory(AppPath + @"\result");
            if (Directory.Exists(AppPath + @"\result\Dynamic") == false) Directory.CreateDirectory(AppPath + @"\result\Dynamic");
            if (!DicSerialPort.ContainsKey(0))
                DicSerialPort.Add(0, new SerialPort());
            if (!DicSerialPort.ContainsKey(1))
                DicSerialPort.Add(1, new SerialPort());
            if (!DicSerialPort.ContainsKey(2))
                DicSerialPort.Add(2, new SerialPort());
            AllViewText = "";
        }

        /// <summary>
        /// Raised when a property on this object (focusing static variables) has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChangedForStaticVariable;

        /// <summary>
        /// Raises this object's PropertyChanged (focusing static variables) event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void OnPropertyChangedForStatic(string propertyName)
        {
            var handler = PropertyChangedForStaticVariable;
            if (handler != null )
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        #region Debugging Aides

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This 
        /// method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public virtual void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might 
        /// override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        #endregion // Debugging Aides

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raises the PropertyChange event for the property specified
        /// </summary>
        /// <param name="propertyName">Property name to update. Is case-sensitive.</param>
        public virtual void RaisePropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);
            OnPropertyChanged(propertyName);
        }


        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
            //MessageBox.Show("Property changed: " + propertyName);
        }

        #endregion // INotifyPropertyChanged Members

        #region IDisposable Members

        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ViewModelBase()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // free other managed objects that implement
                // IDisposable only
            }

            // release any unmanaged objects
            // set the object references to null

            _disposed = true;
        }

        #endregion // IDisposable Members

    }
}
