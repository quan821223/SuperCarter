using Notification.Wpf;
using SuperCarter.Services;
using SuperCarter.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SuperCarter.Model
{
    
    public class Comporttype : INode
    {
        public string FriendlyName { get; set; }
        public ObservableCollection<Portdetectedtype> Children { get; set; } = new ObservableCollection<Portdetectedtype>();

    }
    public class Foldertype : INode
    {
        public Foldertype()
        {
            this.Children = new ObservableCollection<INode>();
        }

        public string FriendlyName { get; set; }
        public BitmapSource MyIcon { get; set; }
        public string FullPathName { get; set; }
        public ObservableCollection<INode> Children { get; set; } = new ObservableCollection<INode>();
        public bool IsExpanded { get; set; }
        public bool IncludeFileChildren { get; set; }

    }

    public interface INode
    {
        string FriendlyName { get; }
    }
    public class IFiletype : INode
    {
        public string FriendlyName { get; set; }
        public BitmapSource MyIcon { get; set; }
        public string FullPathName { get; set; }
        public bool IsExpanded { get; set; }
        public bool IncludeFileChildren { get; set; }
    }

    public class ScriptEditor : ViewModelBase
    {
        public ScriptEditor()
        {
            Fullloop = 1;
            folderViewerlist = new ObservableCollection<Foldertype>();
            evt_Loadscriptroot(AppPath + @"scripts");
            Viewerpath = AppPath + @"scripts";
            OnPropertyChanged(nameof(folderViewerlist));
            OnPropertyChanged(nameof(Viewerpath));

            blockAfolderViewerlist = new ObservableCollection<IFiletype>();
        }
        ~ScriptEditor()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region property
        public ObservableCollection<IFiletype> blockAfolderViewerlist { get; set; } = new ObservableCollection<IFiletype>();
        public ObservableCollection<Foldertype> folderViewerlist { get; set; } = new ObservableCollection<Foldertype>();
        public string Viewerpath { get; set; }
        public ObservableCollection<ScriptItemtype> ObsColBlockASequences { get; set; } = new ObservableCollection<ScriptItemtype>();
        public ObservableCollection<ScriptItemtype> ObsColBlockBSequences { get; set; } = new ObservableCollection<ScriptItemtype>();
        public ObservableCollection<ScriptItemtype> ObsColBlockCSequences { get; set; } = new ObservableCollection<ScriptItemtype>();
        public ObservableCollection<ScriptItemtype> ObsColBlockDSequences { get; set; } = new ObservableCollection<ScriptItemtype>();
        public int blockAitemcount { get; set; } = 0;
        public int blockBitemcount { get; set; } = 0;
        public int blockCitemcount { get; set; } = 0;
        public int blockDitemcount { get; set; } = 0;

        //public static List<SendorExecuteSendType> BlockASequencesList { get; set; } = new List<SendorExecuteSendType>();
        //public static List<SendorExecuteSendType> BlockBSequencesList { get; set; } = new List<SendorExecuteSendType>();
        //public static List<SendorExecuteSendType> BlockCSequencesList { get; set; } = new List<SendorExecuteSendType>();
        //public static List<SendorExecuteSendType> BlockDSequencesList { get; set; } = new List<SendorExecuteSendType>();
        public string Openblockfilepath { get; set; }
        public double Estimateruntimefullblock
        {
            get => _Estimateruntimefullblock;
            set
            {
                _Estimateruntimefullblock = Convert.ToDouble(value);
                _Estimateruntimefullblock = 0.0;
                _Estimateruntimefullblock += EstimateruntimeforblockA;
                _Estimateruntimefullblock += EstimateruntimeforblockB;
                _Estimateruntimefullblock += EstimateruntimeforblockC;
                _Estimateruntimefullblock += EstimateruntimeforblockD;
                _Estimateruntimefullblock *= Fullloop;
            }
        }

        public int Fullloop
        {
            get => _ExecuteFullloop;
            set
            {
                _ExecuteFullloop = value < 1 ? 1 : value;
                Estimateruntimefullblock = 0.0;
                Estimateruntimefullblock += EstimateruntimeforblockA;
                Estimateruntimefullblock += EstimateruntimeforblockB;
                Estimateruntimefullblock += EstimateruntimeforblockC;
                Estimateruntimefullblock += EstimateruntimeforblockD;
                Estimateruntimefullblock *= _ExecuteFullloop;
                OnPropertyChanged(nameof(Estimateruntimefullblock));
                OnPropertyChanged(nameof(Fullloop));
            }
        }
        public string blockAscriptpath { get; set; }
        public string blockBscriptpath { get; set; }
        public string blockCscriptpath { get; set; }
        public string blockDscriptpath { get; set; }

        private int _ExecuteFullloop, _ExecuteBlockALoop, _ExecuteBlockBLoop, _ExecuteBlockCLoop, _ExecuteBlockDLoop;
        private double _Estimateruntimefullblock, _EstimateruntimeforblockA, _EstimateruntimeforblockB, _EstimateruntimeforblockC, _EstimateruntimeforblockD;
        public int ExecuteBlockALoop
        {
            get => _ExecuteBlockALoop;
            set
            {
                _ExecuteBlockALoop = value < 1 ? 1 : value;
                EstimateruntimeforblockA = blockAscriptDelaytime;
                OnPropertyChanged(nameof(EstimateruntimeforblockA));
                Fullloop = Fullloop;
            }
        }
        public int ExecuteBlockBLoop
        {
            get => _ExecuteBlockBLoop;
            set
            {
                _ExecuteBlockBLoop = value < 1 ? 1 : value;
                EstimateruntimeforblockB = blockBscriptDelaytime;
                OnPropertyChanged(nameof(EstimateruntimeforblockB));
                Fullloop = Fullloop;
            }
        }
        public int ExecuteBlockCLoop
        {
            get => _ExecuteBlockCLoop;
            set
            {
                _ExecuteBlockCLoop = value < 1 ? 1 : value;
                EstimateruntimeforblockC = blockCscriptDelaytime;
                OnPropertyChanged(nameof(EstimateruntimeforblockC));
                Fullloop = Fullloop;
            }
        }
        public int ExecuteBlockDLoop
        {
            get => _ExecuteBlockDLoop;
            set
            {
                _ExecuteBlockDLoop = value < 1 ? 1 : value;
                EstimateruntimeforblockD = blockDscriptDelaytime;
                OnPropertyChanged(nameof(EstimateruntimeforblockD));
                Fullloop = Fullloop;
            }
        }
        public double EstimateruntimeforblockA
        {
            get => _EstimateruntimeforblockA;
            set
            {
                _EstimateruntimeforblockA = Convert.ToDouble(value);
                _EstimateruntimeforblockA /= 60000;
                _EstimateruntimeforblockA *= ExecuteBlockALoop;
            }
        }
        public double EstimateruntimeforblockB
        {
            get => _EstimateruntimeforblockB;
            set
            {
                _EstimateruntimeforblockB = Convert.ToDouble(value);
                _EstimateruntimeforblockB /= 60000;
                _EstimateruntimeforblockB *= ExecuteBlockBLoop;
            }
        }
        public double EstimateruntimeforblockC
        {
            get => _EstimateruntimeforblockC;
            set
            {
                _EstimateruntimeforblockC = Convert.ToDouble(value);
                _EstimateruntimeforblockC /= 60000;
                _EstimateruntimeforblockC *= ExecuteBlockCLoop;
            }
        }
        public double EstimateruntimeforblockD
        {
            get => _EstimateruntimeforblockD;
            set
            {
                _EstimateruntimeforblockD = Convert.ToDouble(value);
                _EstimateruntimeforblockD /= 60000;
                _EstimateruntimeforblockD *= ExecuteBlockDLoop;
            }
        }

        private int _blockAscriptDelaytime,
        _blockBscriptDelaytime, _blockCscriptDelaytime,
        _blockDscriptDelaytime;
        public int blockAscriptDelaytime
        {
            get => _blockAscriptDelaytime;
            set
            {
                _blockAscriptDelaytime = value;
                EstimateruntimeforblockA = _blockAscriptDelaytime;
            }
        }
        public int blockBscriptDelaytime
        {
            get => _blockBscriptDelaytime;
            set
            {
                _blockBscriptDelaytime = value;
                EstimateruntimeforblockB = _blockBscriptDelaytime;
            }
        }
        public int blockCscriptDelaytime
        {
            get => _blockCscriptDelaytime;
            set
            {
                _blockCscriptDelaytime = value;
                EstimateruntimeforblockC = _blockCscriptDelaytime;
            }
        }
        public int blockDscriptDelaytime
        {
            get => _blockDscriptDelaytime;
            set
            {
                _blockDscriptDelaytime = value;
                EstimateruntimeforblockD = _blockDscriptDelaytime;
            }
        }
        #endregion 

        #region icommand event
        private ICommand _LoadscripttoBlockA, _LoadscripttoBlockB, _LoadscripttoBlockC, _LoadscripttoBlockD;
        public ICommand LoadscripttoBlockA
        {
            get
            {
                _LoadscripttoBlockA = new RelayCommand(
                    param => evt_LoadscripttoBlockA(0));
                return _LoadscripttoBlockA;
            }
        }
        public ICommand LoadscripttoBlockB
        {
            get
            {
                _LoadscripttoBlockB = new RelayCommand(
                    param => evt_LoadscripttoBlockA(1));
                return _LoadscripttoBlockB;
            }
        }
        public ICommand LoadscripttoBlockC
        {
            get
            {
                _LoadscripttoBlockC = new RelayCommand(
                    param => evt_LoadscripttoBlockA(2));
                return _LoadscripttoBlockC;
            }
        }
        public ICommand LoadscripttoBlockD
        {
            get
            {
                _LoadscripttoBlockD = new RelayCommand(
                    param => evt_LoadscripttoBlockA(3));
                return _LoadscripttoBlockD;
            }
        }

        private ICommand _SettinViewPath, _ReleasetoRefresh;
        public ICommand SettinViewPath
        {
            get
            {
                _SettinViewPath = new RelayCommand(
                                param => evt_selectViewpath());
                return _SettinViewPath;
            }
        }
        private ICommand _SelectScriptpathCommand;
        public ICommand SelectScriptpathCommand
        {
            get
            {
                _SelectScriptpathCommand = new RelayCommand(
                    param => evt_SelectedScriptItem((IFiletype)param));
                return _SelectScriptpathCommand;
            }
        }
        public ICommand ReleasetoRefresh
        {
            get
            {
                _ReleasetoRefresh = new RelayCommand(
                    param => evt_ReleasetoRefresh());
                return _ReleasetoRefresh;
            }
        }
        private ICommand _SaveBlockprocedure;
        public ICommand SaveBlockprocedure
        {
            get
            {
                _SaveBlockprocedure = new RelayCommand(
                    param => evt_SaveBlockTestSuitsToXMLfile());
                return _SaveBlockprocedure;
            }
        }


        #endregion

        #region event

        private void evt_SelectedScriptItem(IFiletype _va)
        {
            blockAscriptpath = _va.FullPathName;
            blockAfolderViewerlist.Add(new IFiletype
            {
                FriendlyName = _va.FriendlyName,
                FullPathName = _va.FullPathName,
                IncludeFileChildren = false,
                IsExpanded = false,

            });
        }

        public void evt_selectViewpath()
        {
            string myPath = AppPath;
            string PATHDDSSCRIPT = null;
            string foldername = null;
            folderViewerlist = new ObservableCollection<Foldertype>();
            using (var openFileDialog1 = new System.Windows.Forms.FolderBrowserDialog())
            {
                openFileDialog1.Description = "請選擇欲顯示的資料夾路徑";
                // 設定OpenFileDialog屬性
                openFileDialog1.SelectedPath = myPath;

                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    PATHDDSSCRIPT = openFileDialog1.SelectedPath; //取得檔名
                    var strarr = PATHDDSSCRIPT.Split('\\').ToList();
                    foldername = strarr[strarr.Count - 1];
                }
                evt_Loadscriptroot(PATHDDSSCRIPT);
                Viewerpath = PATHDDSSCRIPT;
            }
            OnPropertyChanged(nameof(folderViewerlist));
            OnPropertyChanged(nameof(Viewerpath));
        }

        public void evt_Loadscriptroot(string _path)
        {
            string foldername;
            if (_path == null) return;
            else
            {

                var strarr = _path.Split('\\').ToList();
                foldername = strarr[strarr.Count - 1];

                folderViewerlist.Add(new Foldertype
                {
                    FriendlyName = foldername,
                    FullPathName = Viewerpath + _path,
                    IncludeFileChildren = false,
                    IsExpanded = false,
                    Children = evt_addfolderNodes(_path)
                });
            }
        }
        public ObservableCollection<INode> evt_addfolderNodes(string _path)
        {
            try
            {
                ObservableCollection<INode> _obj = new ObservableCollection<INode>();

                foreach (string fname in System.IO.Directory.GetFileSystemEntries(_path))
                {
                    // get the file attributes for file or directory
                    // FileAttributes attr = File.GetAttributes(Viewerpath + fname);
                    var strarr = fname.Split('\\').ToList();
                    var foldername = strarr[strarr.Count - 1];
                    //detect whether its a directory or file

                    if (Directory.Exists(fname))
                    {
                        _obj.Add(new Foldertype
                        {
                            FriendlyName = foldername,
                            FullPathName = fname,
                            IncludeFileChildren = false,
                            IsExpanded = false,
                            Children = evt_addfolderNodes(fname)

                        });
                    }
                    else
                    {
                        // MessageBox.Show("2");
                        _obj.Add(new IFiletype
                        {
                            FriendlyName = foldername,
                            FullPathName = fname,
                            IncludeFileChildren = false,
                            IsExpanded = false,

                        });
                    }
                }
                return _obj;
            }
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show(ex.Message);
                return new ObservableCollection<INode>();
            }
        }
        public void evt_LoadscripttoBlockA(int _selectedblock)
        {
            var loadscriptXMLPath = ConfigModel.Instance.GetStrScriptpath();

            // Check the path location; if it's an empty string, the process will be terminated. 
            if (string.IsNullOrWhiteSpace(loadscriptXMLPath))
                return;
            switch (_selectedblock)
            {
                case 0:
                    blockAscriptpath = null;
                    ObsColBlockASequences = ConfigModel.Instance.GetScriptXMLSequences(loadscriptXMLPath);              // get xml format testsuit 
                    ExecuteBlockALoop = ConfigModel.Instance.GetScriptXMLSequence_itervalue(loadscriptXMLPath);         // get block execute loop
                    blockAscriptDelaytime = ConfigModel.Instance.GetScriptXMLSequence_TotalTime(loadscriptXMLPath);     // get block script total delay time
                    blockAscriptpath = loadscriptXMLPath;
                    blockAitemcount = ObsColBlockASequences.Count;
                    break;
                case 1:
                    blockBscriptpath = null;
                    ObsColBlockBSequences = ConfigModel.Instance.GetScriptXMLSequences(loadscriptXMLPath);              // get xml format testsuit 
                    ExecuteBlockBLoop = ConfigModel.Instance.GetScriptXMLSequence_itervalue(loadscriptXMLPath);         // get block execute loop
                    blockBscriptDelaytime = ConfigModel.Instance.GetScriptXMLSequence_TotalTime(loadscriptXMLPath);     // get block script total delay time
                    blockBscriptpath = loadscriptXMLPath;
                    blockBitemcount = ObsColBlockBSequences.Count;
                    break;
                case 2:
                    blockCscriptpath = null;
                    ObsColBlockCSequences = ConfigModel.Instance.GetScriptXMLSequences(loadscriptXMLPath);              // get xml format testsuit 
                    ExecuteBlockCLoop = ConfigModel.Instance.GetScriptXMLSequence_itervalue(loadscriptXMLPath);         // get block execute loop
                    blockCscriptDelaytime = ConfigModel.Instance.GetScriptXMLSequence_TotalTime(loadscriptXMLPath);     // get block script total delay time
                    blockCscriptpath = loadscriptXMLPath;
                    blockCitemcount = ObsColBlockCSequences.Count;
                    break;
                case 3:
                    blockDscriptpath = null;
                    ObsColBlockDSequences = ConfigModel.Instance.GetScriptXMLSequences(loadscriptXMLPath);              // get xml format testsuit 
                    ExecuteBlockDLoop = ConfigModel.Instance.GetScriptXMLSequence_itervalue(loadscriptXMLPath);         // get block execute loop
                    blockDscriptDelaytime = ConfigModel.Instance.GetScriptXMLSequence_TotalTime(loadscriptXMLPath);     // get block script total delay time
                    blockDscriptpath = loadscriptXMLPath;
                    blockDitemcount = ObsColBlockDSequences.Count;
                    break;
            }
            evt_ReleasetoRefresh();
        }
        public void evt_ReleasetoRefresh()
        {
            OnPropertyChanged(nameof(Openblockfilepath));
            OnPropertyChanged(nameof(Fullloop));
            OnPropertyChanged(nameof(EstimateruntimeforblockA));
            OnPropertyChanged(nameof(EstimateruntimeforblockB));
            OnPropertyChanged(nameof(EstimateruntimeforblockC));
            OnPropertyChanged(nameof(EstimateruntimeforblockD));
            OnPropertyChanged(nameof(blockAscriptDelaytime));
            OnPropertyChanged(nameof(blockBscriptDelaytime));
            OnPropertyChanged(nameof(blockCscriptDelaytime));
            OnPropertyChanged(nameof(blockDscriptDelaytime));
            OnPropertyChanged(nameof(ExecuteBlockALoop));
            OnPropertyChanged(nameof(ExecuteBlockBLoop));
            OnPropertyChanged(nameof(ExecuteBlockCLoop));
            OnPropertyChanged(nameof(ExecuteBlockDLoop));
            OnPropertyChanged(nameof(blockAscriptpath));
            OnPropertyChanged(nameof(blockBscriptpath));
            OnPropertyChanged(nameof(blockCscriptpath));
            OnPropertyChanged(nameof(blockDscriptpath));

            OnPropertyChanged(nameof(blockAitemcount));
            OnPropertyChanged(nameof(blockBitemcount));
            OnPropertyChanged(nameof(blockCitemcount));
            OnPropertyChanged(nameof(blockDitemcount));


            Estimateruntimefullblock = 0;
            OnPropertyChanged(nameof(Estimateruntimefullblock));
        }

        private void evt_SaveBlockTestSuitsToXMLfile()
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

                    //ConfigModel.Instance.SaveScriptTestSequencefile(saveFileDialog1.FileName, Scriptdatalist);
                    ConfigModel.Instance.evt_SaveScriptTestSuitefile(saveFileDialog1.FileName, this);
                    MessageAggregator.Instance.SendMessage(new POPNotifyMsgType
                    {
                        Tital = "通知",
                        Message = "已儲存 " + saveFileDialog1.FileName,
                        NotifyType = NotificationType.Notification,

                    });
                }
            }

        }
        public string evt_Openfile()
        {
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
                    Openblockfilepath = strpath;
                    ConfigModel.Instance.GetScriptXMLTestSuite(this);
                    evt_ReleasetoRefresh();
                }
                return strpath;
            }
        }


        #endregion

    }
}
