using Microsoft.Win32;
using Notification.Wpf;
using SuperCarter.Services;
using SuperCarter.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush;

namespace SuperCarter.Model
{
    public class ScheduledScriptEditor : ViewModelBase
    {

        public ScheduledScriptEditor()
        {
            Fullloop = 1;
            BlockA1Interval = 0;
            BlockA2Interval = 0;
            BlockB1Interval = 0;
            BlockB2Interval = 0;
            //folderViewerlist = new ObservableCollection<Foldertype>();
            //evt_Loadscriptroot(AppPath + @"scripts");
            //Viewerpath = AppPath + @"scripts";
            //OnPropertyChanged(nameof(folderViewerlist));
            //OnPropertyChanged(nameof(Viewerpath));
            //blockAfolderViewerlist = new ObservableCollection<IFiletype>();

            /// 燈號設定
            DisplaySwitch = Visibility.Visible;
            LabelText = "●";
            ForeColor = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            RuntimeAlertText = "Idle";
            IsunlockScheduledScriptSetting = true;

        }
        ~ScheduledScriptEditor()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #region properties
        /// <summary>
        /// 燈號控制
        /// </summary>
        public Visibility DisplaySwitch { get; set; } = Visibility.Hidden;
        public string LabelText { get; set; }
        private Brush _ForeColor;

        public Brush ForeColor
        {
            get => _ForeColor;
            set
            {
                _ForeColor = value;
                OnPropertyChanged(nameof(ForeColor));
            }
        }
        public ObservableCollection<ScriptItemtype> BlockObsColSequences { get; set; } = new ObservableCollection<ScriptItemtype>();
        public ObservableCollection<ScriptItemtype> BlockA1ObsColSequences { get; set; } = new ObservableCollection<ScriptItemtype>();
        public ObservableCollection<ScriptItemtype> BlockA2ObsColSequences { get; set; } = new ObservableCollection<ScriptItemtype>();
        public ObservableCollection<ScriptItemtype> BlockB1ObsColSequences { get; set; } = new ObservableCollection<ScriptItemtype>();
        public ObservableCollection<ScriptItemtype> BlockB2ObsColSequences { get; set; } = new ObservableCollection<ScriptItemtype>();
        public ObservableCollection<ScriptItemtype> BlockA1initObsColSequences { get; set; } = new ObservableCollection<ScriptItemtype>();
        public ObservableCollection<ScriptItemtype> BlockA2initObsColSequences { get; set; } = new ObservableCollection<ScriptItemtype>();
        public ObservableCollection<ScriptItemtype> BlockB1initObsColSequences { get; set; } = new ObservableCollection<ScriptItemtype>();
        public ObservableCollection<ScriptItemtype> BlockB2initObsColSequences { get; set; } = new ObservableCollection<ScriptItemtype>();
        private int _ExecuteFullloop = 1;
        public int Fullloop
        {
            get => _ExecuteFullloop;
            set
            {
                _ExecuteFullloop = value < 1 ? 1 : value;
                OnPropertyChanged(nameof(Fullloop));
            }
        }
        public string OpenedBlockScriptPath { get; set; }
        public int SelectedCMD { get; set; }
        public string BlockA1scriptPath { get; set; }
        public string BlockA2scriptPath { get; set; }
        public string BlockB1scriptPath { get; set; }
        public string BlockB2scriptPath { get; set; }
        public int BlockA1Interval { get; set; }
        public int BlockA2Interval { get; set; }
        public int BlockB1Interval { get; set; }
        public int BlockB2Interval { get; set; }
        public int BlockA1scriptLoop { get; set; }
        public int BlockA2scriptLoop { get; set; }
        public int BlockB1scriptLoop { get; set; }
        public int BlockB2scriptLoop { get; set; }
        public string BlockA1initscriptPath { get; set; }
        public string BlockA2initscriptPath { get; set; }
        public string BlockB1initscriptPath { get; set; }
        public string BlockB2initscriptPath { get; set; }
        public int BlockALoop { get; set; }
        public int BlockBLoop { get; set; }
     
        public bool IsEnableDetectnormCurrent { get; set; } = false;
        public int UpperLimitnormCurrentValue { get; set; } = 0;
        public int LowerLimitnormCurrentValue { get; set; } = 0;
        public bool IsEnableDetectsleepCurrent { get; set; } = false;
        public int UpperLimitsleepCurrentValue { get; set; } = 0;
   
        public bool IsEnableDetectDiag { get; set; } = false;
        public bool IsEnableDetectLightsensor { get; set; } = false;
        public int UpperLimitLightsensorValue { get; set; } = 0;
        public int LowerLimitLightsensorValue { get; set; } = 0;
        public bool IsEnableTouchfinger { get; set; } = false;
        public bool IsEnableTouchXY { get; set; } = false;

        private bool _isAnimationEnabled;
        public bool IsAnimationEnabled
        {
            get { return _isAnimationEnabled; }
            set
            {
                if (_isAnimationEnabled != value)
                {
                    _isAnimationEnabled = value;
                    OnPropertyChanged(nameof(IsAnimationEnabled));
                }
            }
        }
        private string _runtimeAlertText;
        public string RuntimeAlertText
        {
            get { return _runtimeAlertText; }
            set
            {
                if (_runtimeAlertText != value)
                {
                    _runtimeAlertText = value;

                    if (_runtimeAlertText == "Loading")
                    {
                        IsAnimationEnabled = true;
                    }
                    else if (_runtimeAlertText == "Idle")
                    {
                        IsAnimationEnabled = false;
                    }

                    OnPropertyChanged(nameof(RuntimeAlertText));
                }
            }
        }
        private bool _IsunlockScheduledScriptSetting = true;
        public bool IsunlockScheduledScriptSetting
        {
            get { return _IsunlockScheduledScriptSetting; }
            set
            {
                if (_IsunlockScheduledScriptSetting != value)
                {
                    _IsunlockScheduledScriptSetting = value;
                    OnPropertyChanged(nameof(IsunlockScheduledScriptSetting));
                }
            }
        }

        public string sub_scriptbrowsing { get; set; }

        #endregion

        #region icommands
        private ICommand _ClearPageProperties, _SaveasSubscript, _ClearSubscript, _Saveasscript;
        private ICommand _LoadBlockScript, _PreViewscript, _Loadscript;
        //public ICommand LoadBlockscript
        //{
        //    get
        //    {
        //        _Loadscript = new RelayCommand(
        //            param => ;
        //        return _Loadscript;
        //    }
        //}
        public ICommand ViewBlockscript
        {
            get
            {
                _PreViewscript = new RelayCommand(
                   async param => await evt_ViewBlockscript(param));
                return _PreViewscript;
            }
        }

        public ICommand ClearPageProperties
        {
            get
            {
                _ClearPageProperties = new RelayCommand(
                    param => evt_ClearPageProperties());
                return _ClearPageProperties;
            }
        }
        public ICommand SaveasSubscript
        {
            get
            {
                _SaveasSubscript = new RelayCommand(
                    param => evt_SaveasSubscript());
                return _SaveasSubscript;
            }
        }
        public ICommand ClearSubscript
        {
            get
            {
                _ClearSubscript = new RelayCommand(
                    param => evt_ClearSubscript());
                return _ClearSubscript;
            }
        }
        public ICommand Saveasscript
        {
            get {
                _Saveasscript = new RelayCommand(
                        param => evt_SaveasScript());
                return _Saveasscript;

            }
        }
        public ICommand Loadscript
        {
            get
            {
                _Saveasscript = new RelayCommand(
                        param => evt_SaveasScript());
                return _Saveasscript;

            }
        }

        #endregion

        #region Event
        private void evt_LoadBlockscript() {

          

            using (var openFileDialog1 = new System.Windows.Forms.OpenFileDialog())
            {
                string myPath = AppPath + @"\script\";
                string strpath = null;
                // 設定OpenFileDialog屬性
                openFileDialog1.Title = "選擇要開啟的 XML 檔案";
                openFileDialog1.Filter = "xml Files (.xml)|*.xml|All Files (*.*)|*.*";
                openFileDialog1.FilterIndex = 1;
                openFileDialog1.Multiselect = true;
                openFileDialog1.InitialDirectory = myPath;

                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                    strpath = openFileDialog1.FileName; //取得檔名
                    OpenedBlockScriptPath = strpath;
                    ConfigModel.Instance.GetScriptXMLTestSuite(this);
                    evt_objfresh();
                }
                OnPropertyChanged(nameof(OpenedBlockScriptPath));
                
            }               

        }
        public void evt_OpenedBlockScriptPath() { }
        private void evt_SaveasScript() {

            using (var savefiledialog = new System.Windows.Forms.SaveFileDialog())
            {
                string filter = "xml file (*.xml)|*.xml| All Files (*.*)|*.*";
                const string header = "Command ,Times/ Keyword#, Interval, COM PORT/Pin, Function,Sub -func., SerialPort I/O comd, AC /USB Switch, Wait, Remark";
                savefiledialog.Title = "Save as ...";
                savefiledialog.Filter = filter;

                savefiledialog.DefaultExt = "xml";
                savefiledialog.FileName = string.Format(@"{0}-{1}.xml", DateTime.Now.ToString("yyyy_MM_dd_HH_mm"), "NEW_Script");

                if (savefiledialog.ShowDialog() == DialogResult.OK)
                {
                    ConfigModel.Instance.evt_SaveScriptTestSuitefile(savefiledialog.FileName, this);
                    MessageAggregator.Instance.SendMessage(new POPNotifyMsgType
                    {
                        Tital = "通知",
                        Message = "已儲存 " + savefiledialog.FileName,
                        NotifyType = NotificationType.Notification,

                    });
                }

            }
        }

        private void OnSequencesUpdated(object sender, SequencesUpdatedEventArgs e)
        {
            // 更新 UI 或执行其他相关操作
            BlockObsColSequences = e.Sequences;
        }
        public async Task evt_ViewBlockscript(object _selectedblock)
        {
            ForeColor = new SolidColorBrush(Color.FromRgb(255, 97, 3));
            OnPropertyChanged(nameof(ForeColor));
            RuntimeAlertText = "Loading";
            IsunlockScheduledScriptSetting = false;
            await Task.Delay(10);
            // 订阅事件
            ConfigModel.Instance.SequencesUpdated += OnSequencesUpdated;

            try
            {
                // 异步加载脚本
                //await ConfigModel.Instance.LoadScriptAsync(GetScriptPathForBlock(_selectedblock));

                BlockObsColSequences.Clear();

                var tempItems = ConfigModel.Instance.GetScriptXMLSequences(GetScriptPathForBlock(_selectedblock));
          

                if (tempItems != null && tempItems.Any())
                {
                    // 如果ObservableCollection支持AddRange则使用，否则使用foreach
                    foreach (var item in tempItems)
                        BlockObsColSequences.Add(item);

                    var testmsg = new POPNotifyMsgType
                    {
                        Tital = "通知",
                        Message = "已開啟" + GetScriptPathForBlock(_selectedblock),
                        NotifyType = NotificationType.Notification,
                    };

                    MessageAggregator.Instance.SendMessage(testmsg);
                }
          
            }
            finally
            {

                ConfigModel.Instance.SequencesUpdated -= OnSequencesUpdated;

                // 更新UI的部分
                ForeColor = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                OnPropertyChanged(nameof(ForeColor));
                RuntimeAlertText = "Idle";
                IsunlockScheduledScriptSetting = true;
            }
        }
     
        private string GetScriptPathForBlock(object _selectedblock)
        {
            // 根據 _selectedblock 返回相應的腳本路徑
            // 這裡僅提供示例，您應根據您的邏輯實現此方法
            switch (Convert.ToInt32(_selectedblock))
            {
                case 0:
                    sub_scriptbrowsing = BlockA1initscriptPath;
                    break;
                case 1:
                    sub_scriptbrowsing = BlockA2initscriptPath;
                    break;               
                case 2:
                    sub_scriptbrowsing = BlockB1initscriptPath;
                    break;                 
                case 3:
                    sub_scriptbrowsing = BlockB2initscriptPath;
                    break;               
                case 4:
                    sub_scriptbrowsing = BlockA1scriptPath;
                    break;
                case 5:
                    sub_scriptbrowsing = BlockA2scriptPath;
                    break;
                case 6:
                    sub_scriptbrowsing = BlockB1scriptPath;
                    break;
                case 7:
                    sub_scriptbrowsing = BlockB2scriptPath;
                    break;
                default:
                    // 處理未知的 _selectedblock 值
                    return string.Empty;
            }
            OnPropertyChanged(nameof(sub_scriptbrowsing));
            return sub_scriptbrowsing;
        }

        public void evt_SaveasSubscript()
        {
            if (BlockObsColSequences == null)
                return;
            if (BlockObsColSequences.Count > 0)
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
                        ConfigModel.Instance.SaveScriptTestSequencefile(saveFileDialog1.FileName, BlockObsColSequences);

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
        public void evt_ClearSubscript()
        {
            BlockObsColSequences.Clear();
            sub_scriptbrowsing = "";
            OnPropertyChanged(nameof(sub_scriptbrowsing));
            OnPropertyChanged(nameof(BlockObsColSequences));
        }

        public void evt_ScriptToolBar_Sortintitem()
        {
            ObservableCollection<ScriptItemtype> _BlockObsColSequences = new ObservableCollection<ScriptItemtype>();
            _BlockObsColSequences = BlockObsColSequences;
            if (BlockObsColSequences == null)
                return;
            if (BlockObsColSequences.Count > 0)
            {
                for (int id = 0; id < Scriptdatalist.Count; id++)
                {
                    BlockObsColSequences[id].ID = id + 1;
                }
                BlockObsColSequences = null;
                BlockObsColSequences = _BlockObsColSequences;
                OnPropertyChangedForStatic(nameof(BlockObsColSequences));
            }

        }

        private void evt_LoadscripttoBlock(object _selectedblock)
        {
            var loadscriptXMLPath = ConfigModel.Instance.GetStrScriptpath();

            // Check the path location; if it's an empty string, the process will be terminated. 
            if (string.IsNullOrWhiteSpace(loadscriptXMLPath))
                return;
            switch (Convert.ToInt32(_selectedblock))
            {
                case 0:
                    BlockA1initObsColSequences = null; 
                    BlockA1initObsColSequences = new ObservableCollection<ScriptItemtype>(ConfigModel.Instance.GetScriptXMLSequences(loadscriptXMLPath));
                    BlockA1initscriptPath = loadscriptXMLPath;
                    OnPropertyChanged(nameof(BlockA1initscriptPath));
                    break;
                case 1:
                    BlockA2initObsColSequences = new ObservableCollection<ScriptItemtype>(ConfigModel.Instance.GetScriptXMLSequences(loadscriptXMLPath));
                    BlockA2initscriptPath = loadscriptXMLPath;
                    OnPropertyChanged(nameof(BlockA2initscriptPath));
                    break;
                case 2:
                    BlockB1initObsColSequences = ConfigModel.Instance.GetScriptXMLSequences(loadscriptXMLPath);              // get xml format testsuit 
                    BlockB1initscriptPath = loadscriptXMLPath;
                    OnPropertyChanged(nameof(BlockB1initscriptPath));
                    break;
                case 3:
                    BlockB2initObsColSequences = ConfigModel.Instance.GetScriptXMLSequences(loadscriptXMLPath);              // get xml format testsuit 
                    BlockB2initscriptPath = loadscriptXMLPath;
                    OnPropertyChanged(nameof(BlockB2initscriptPath));
                    break;
                case 4:
                    BlockA1ObsColSequences = ConfigModel.Instance.GetScriptXMLSequences(loadscriptXMLPath);              // get xml format testsuit 
                    BlockA1scriptPath = loadscriptXMLPath;
                    OnPropertyChanged(nameof(BlockA1scriptPath));
                    break;
                case 5:
                    BlockA2ObsColSequences = ConfigModel.Instance.GetScriptXMLSequences(loadscriptXMLPath);              // get xml format testsuit 
                    BlockA2scriptPath = loadscriptXMLPath;
                    OnPropertyChanged(nameof(BlockA2scriptPath));
                    break;
                case 6:
                    BlockB1ObsColSequences = ConfigModel.Instance.GetScriptXMLSequences(loadscriptXMLPath);              // get xml format testsuit 
                    BlockB1scriptPath = loadscriptXMLPath;
                    OnPropertyChanged(nameof(BlockB1scriptPath));
                    break;
                case 7:
                    BlockB2ObsColSequences = ConfigModel.Instance.GetScriptXMLSequences(loadscriptXMLPath);              // get xml format testsuit 
                    BlockB2scriptPath = loadscriptXMLPath;
                    OnPropertyChanged(nameof(BlockB2scriptPath));
                    break;

            }
            //evt_ReleasetoRefresh();
        }

        private void evt_ClearPageProperties()
        {
            BlockA1scriptPath = null;
            BlockA2scriptPath = null;
            BlockB1scriptPath = null;
            BlockB2scriptPath = null;
            BlockA1Interval = 0;
            BlockA2Interval = 0;
            BlockB1Interval = 0;
            BlockB2Interval = 0;
            BlockA1scriptLoop = 0;
            BlockA2scriptLoop = 0;
            BlockB1scriptLoop = 0;
            BlockB2scriptLoop = 0;
            BlockA1initscriptPath = null;
            BlockA2initscriptPath = null;
            BlockB1initscriptPath = null;
            BlockB2initscriptPath = null;
            BlockALoop = 0;
            BlockBLoop = 0;
            BlockA1ObsColSequences = new ObservableCollection<ScriptItemtype>();
            BlockA2ObsColSequences = new ObservableCollection<ScriptItemtype>();
            BlockB1ObsColSequences = new ObservableCollection<ScriptItemtype>();
            BlockB2ObsColSequences = new ObservableCollection<ScriptItemtype>();
            BlockA1initObsColSequences = new ObservableCollection<ScriptItemtype>();
            BlockA2initObsColSequences = new ObservableCollection<ScriptItemtype>();
            BlockB1initObsColSequences = new ObservableCollection<ScriptItemtype>();
            BlockB2initObsColSequences = new ObservableCollection<ScriptItemtype>();
            BlockObsColSequences.Clear();

            IsEnableDetectnormCurrent = false;
            UpperLimitnormCurrentValue = 0;
            LowerLimitnormCurrentValue = 0;
            IsEnableDetectsleepCurrent = false;
            UpperLimitsleepCurrentValue = 0;
            IsEnableDetectDiag = false;
            IsEnableDetectLightsensor = false;
            UpperLimitLightsensorValue = 0;
            LowerLimitLightsensorValue = 0;
            IsEnableTouchfinger = false;
            IsEnableTouchXY = false;

            evt_objfresh();

        }
        public void evt_objfresh()
        {
            OnPropertyChanged(nameof(IsEnableDetectnormCurrent));
            OnPropertyChanged(nameof(UpperLimitnormCurrentValue));
            OnPropertyChanged(nameof(LowerLimitnormCurrentValue));
            OnPropertyChanged(nameof(IsEnableDetectsleepCurrent));
            OnPropertyChanged(nameof(UpperLimitsleepCurrentValue));
            OnPropertyChanged(nameof(IsEnableDetectDiag));
            OnPropertyChanged(nameof(IsEnableDetectLightsensor));
            OnPropertyChanged(nameof(UpperLimitLightsensorValue));
            OnPropertyChanged(nameof(LowerLimitLightsensorValue));
            OnPropertyChanged(nameof(IsEnableTouchfinger));
            OnPropertyChanged(nameof(IsEnableTouchXY));
            OnPropertyChanged(nameof(BlockA1scriptPath));
            OnPropertyChanged(nameof(BlockA2scriptPath));
            OnPropertyChanged(nameof(BlockB1scriptPath));
            OnPropertyChanged(nameof(BlockB2scriptPath));
            OnPropertyChanged(nameof(BlockA1Interval));
            OnPropertyChanged(nameof(BlockA2Interval));
            OnPropertyChanged(nameof(BlockB1Interval));
            OnPropertyChanged(nameof(BlockB2Interval));
            OnPropertyChanged(nameof(BlockA1scriptLoop));
            OnPropertyChanged(nameof(BlockA2scriptLoop));
            OnPropertyChanged(nameof(BlockB1scriptLoop));
            OnPropertyChanged(nameof(BlockB2scriptLoop));
            OnPropertyChanged(nameof(BlockA1initscriptPath));
            OnPropertyChanged(nameof(BlockA2initscriptPath));
            OnPropertyChanged(nameof(BlockB1initscriptPath));
            OnPropertyChanged(nameof(BlockB2initscriptPath));
            OnPropertyChanged(nameof(BlockALoop));
            OnPropertyChanged(nameof(BlockBLoop));
            OnPropertyChanged(nameof(BlockObsColSequences));
        }
        #endregion

    }
}
