using Notification.Wpf;
using SuperCarter.Model;
using SuperCarter.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace SuperCarter.ViewModel
{
    public  class ScriptEditor :ViewModelBase
    {
        public Dictionary<string, List<ScriptItemtype>> ScriptToolboxTree { get; set; } = new Dictionary<string, List<ScriptItemtype>>();
        public int script_itervalue { get; set; } = 1;
        public int SelectedScriptitem { get; set; }
        public ScriptEditor() {
            ScriptToolboxTree = new Dictionary<string, List<ScriptItemtype>>(Toolscript.Instance.GetdefaultToolScriptItem());
            OnPropertyChanged(nameof(ScriptToolboxTree));
        }
        ~ScriptEditor() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region Scription TreeView funcions
        private ICommand _ScriptToolBar_Clear,
            _ScriptToolBar_AddScriptitem,
            _ScriptToolBar_RemoveScriptitem,
            _ScriptToolBar_SortScriptitem,
            _SciptToolBar_Openfolder,
            _SciptToolBar_Saveas;
        public ICommand ScriptToolBar_Clear
        {
            get
            {
                _ScriptToolBar_Clear = new RelayCommand(
                      param => evt_ScriptToolBar_Clear());
                return _ScriptToolBar_Clear;
            }
        }
        public ICommand ScriptToolBar_AddScriptitem
        {
            get
            {
                _ScriptToolBar_AddScriptitem = new RelayCommand(
                      param => evt_ScriptToolBar_Additem());
                return _ScriptToolBar_AddScriptitem;
            }
        }
        public ICommand ScriptToolBar_RemoveScriptitem
        {
            get
            {
                _ScriptToolBar_RemoveScriptitem = new RelayCommand(
                      param => evt_ScriptToolBar_Removeitem());
                return _ScriptToolBar_RemoveScriptitem;
            }
        }
        public ICommand ScriptToolBar_SortScriptitem
        {
            get
            {
                _ScriptToolBar_SortScriptitem = new RelayCommand(
                      param => evt_ScriptToolBar_Sortintitem());
                return _ScriptToolBar_SortScriptitem;
            }
        }

        public ICommand SciptToolBar_Saveas
        {
            get
            {
                _SciptToolBar_Saveas = new RelayCommand(
                      param => evt_evt_ScriptToolBar_Saveas(script_itervalue));
                return _SciptToolBar_Saveas;
            }
        }
        public ICommand SciptToolBar_Openfolder
        {
            get
            {
                _SciptToolBar_Openfolder = new RelayCommand(
                      param => evt_Openfile());
                return _SciptToolBar_Openfolder;
            }
        }



        #region Scription TreeView funcions events



        public void evt_ScriptToolBar_Clear()
        {
            Scriptdatalist.Clear();
        }
        public void evt_ScriptToolBar_Additem()
        {
            Scriptdatalist.Add(new ScriptItemtype()
            {
                ID = 0,
                Nodename = "",
                MSGname = "",
                Sequence = "",
                Delaytime = 200,
                Loop = 1

            });
        }
        public void evt_ScriptToolBar_Removeitem()
        {
            if (SelectedScriptitem == null)
                return;
            if (Scriptdatalist is not null)
            {
                if (SelectedScriptitem > -1 && SelectedScriptitem < Scriptdatalist.Count)
                {
                    Scriptdatalist.Remove(Scriptdatalist[SelectedScriptitem]);
                }
            }
        }
        public void evt_ScriptToolBar_Sortintitem()
        {
            ObservableCollection<ScriptItemtype> _Scriptdatalist = new ObservableCollection<ScriptItemtype>();
            _Scriptdatalist = Scriptdatalist;
            if (Scriptdatalist == null)
                return;
            if (Scriptdatalist.Count > 0)
            {
                for (int id = 0; id < Scriptdatalist.Count; id++)
                {
                    Scriptdatalist[id].ID = id + 1;
                }
                Scriptdatalist = null;
                Scriptdatalist = _Scriptdatalist;
                //OnPropertyChangedForStatic(nameof(Scriptdatalist));
            }

        }
        private string _Textscriptpath;
        public string Textscriptpath
        {
            get { return _Textscriptpath; }
            set
            {
                _Textscriptpath = value;
                OnPropertyChanged(nameof(TestTextView));
            }
        }
        public void evt_Openfile()
        {
            using (var openFileDialog1 = new System.Windows.Forms.OpenFileDialog())
            {
                string myPath = AppPath + @"\script\";
                string PATHDDSSCRIPT = null;
                // 設定OpenFileDialog屬性
                openFileDialog1.Title = "選擇要開啟的 XML 檔案";
                openFileDialog1.Filter = "xml Files (.xml)|*.xml|All Files (*.*)|*.*";
                openFileDialog1.FilterIndex = 1;
                openFileDialog1.Multiselect = true;
                openFileDialog1.InitialDirectory = myPath;

                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    PATHDDSSCRIPT = openFileDialog1.FileName; //取得檔名
                    evt_ScriptToolBar_Openfile(PATHDDSSCRIPT);
                }
                else
                { }
            }
        }
        public void evt_ScriptToolBar_Openfile(string _pathscript)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_pathscript))
                    return;

                Scriptdatalist.Clear();

                var tempItems = ConfigModel.Instance.GetScriptXMLSequences(_pathscript);
                script_itervalue = ConfigModel.Instance.GetScriptXMLSequence_itervalue(_pathscript);
                Textscriptpath = _pathscript;

                if (tempItems != null && tempItems.Any())
                {
                    // 如果ObservableCollection支持AddRange则使用，否则使用foreach
                    foreach (var item in tempItems)
                        Scriptdatalist.Add(item);

                    var testmsg = new POPNotifyMsgType
                    {
                        Tital = "通知",
                        Message = "已開啟" + _pathscript,
                        NotifyType = NotificationType.Notification,
                    };



                    MessageAggregator.Instance.SendMessage(testmsg);
                }
                OnPropertyChanged(nameof(Textscriptpath));
                OnPropertyChanged(nameof(script_itervalue));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }


        public void evt_evt_ScriptToolBar_Saveas(int _iterationnumber)
        {
            if (Scriptdatalist == null)
                return;
            if (Scriptdatalist.Count > 0)
            {
                using (var saveFileDialog1 = new System.Windows.Forms.SaveFileDialog())
                {
                    string filter = "xml file (*.xml)|*.xml| All Files (*.*)|*.*";
                    const string header = "Command ,Times/ Keyword#, Interval, COM PORT/Pin, Function,Sub -func., SerialPort I/O comd, AC /USB Switch, Wait, Remark";
                    saveFileDialog1.Title = "Save as ...";
                    saveFileDialog1.Filter = filter;

                    saveFileDialog1.DefaultExt = "xml";
                    saveFileDialog1.FileName = string.Format(@"{0}-{1}.xml", DateTime.Now.ToString("yyyy_MM_dd_HH_mm"), "NEW_Script");

                    //saveFileDialog1.FileName = string.Format(AppPath + @"\{0}-{1}.xml", DateTime.Now.ToString("yyyy_MM_dd_HH_mm"), "NEW_Script");

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        evt_ScriptToolBar_Sortintitem();
                        ConfigModel.Instance.SaveScriptTestSequencefile(saveFileDialog1.FileName, Scriptdatalist, _iterationnumber);

                        MessageAggregator.Instance.SendMessage(new POPNotifyMsgType
                        {
                            Tital = "通知",
                            Message = "已儲存 " + saveFileDialog1.FileName,
                            NotifyType = NotificationType.Notification,

                        });
                    }
                }
            }
        }

        #endregion


        #endregion
    }
}
