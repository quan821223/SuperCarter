using SuperCarter.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SuperCarter.Model
{
    public class ScriptEditor : ViewModelBase
    {
        
        public ScriptEditor()
        {
            Fullloop = 1;
            BlockA1scriptruntime = 0;
            BlockA2scriptruntime = 0;
            BlockB1scriptruntime = 0;
            BlockB2scriptruntime = 0;
        //folderViewerlist = new ObservableCollection<Foldertype>();
        //evt_Loadscriptroot(AppPath + @"scripts");
        //Viewerpath = AppPath + @"scripts";
        //OnPropertyChanged(nameof(folderViewerlist));
        //OnPropertyChanged(nameof(Viewerpath));

        //blockAfolderViewerlist = new ObservableCollection<IFiletype>();
    }
        ~ScriptEditor()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #region properties
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
        public string BlockA1scriptPath { get; set; }
        public string BlockA2scriptPath { get; set; }
        public string BlockB1scriptPath { get; set; }
        public string BlockB2scriptPath { get; set; }

        public int BlockA1scriptruntime { get; set; }
        public int BlockA2scriptruntime { get; set; }
        public int BlockB1scriptruntime { get; set; }
        public int BlockB2scriptruntime { get; set; }
        public int BlockA1scriptLoop { get; set; }
        public int BlockA2scriptLoop { get; set; }
        public int BlockB1scriptLoop { get; set; }
        public int BlockB2scriptLoop { get; set; }
        public string BlockA1initscriptPath { get; set; }
        public string BlockA2initscriptPath { get; set; }
        public string BlockB1initscriptPath { get; set; }
        public string BlockB2initscriptPath { get; set; }
        public int BlockAscriptLoop { get; set; }
        public int BlockBscriptLoop { get; set; }

        #endregion

        #region icommands
        private ICommand _ClearPageProperties;
        private ICommand _LoadBlockA1Script, _LoadBlockA2Script, _LoadBlockB1Script, _LoadBlockB2Script ;
        private ICommand _LoadBlockA1initScript, _LoadBlockA2initScript, _LoadBlockB1initScript, _LoadBlockB2initScript;
        private ICommand _ViewBlockA1Script, _ViewBlockA2Script, _ViewBlockB1Script, _ViewBlockB2Script;
        private ICommand _ViewBlockA1initScript, _ViewBlockA2initScript, _ViewBlockB1initScript, _ViewBlockB2initScript;
        public ICommand LoadBlockA1Script {
            get
            {
                _LoadBlockA1Script = new RelayCommand(
                    param => evt_LoadscripttoBlock(0));
                return _LoadBlockA1Script;
            }
        }             
        public ICommand LoadBlockA2Script
        {
            get
            {
                _LoadBlockA2Script = new RelayCommand(
                    param => evt_LoadscripttoBlock(1));
                return _LoadBlockA2Script;
            }
        }
        public ICommand LoadBlockB1Script
        {
            get
            {
                _LoadBlockB1Script = new RelayCommand(
                    param => evt_LoadscripttoBlock(2));
                return _LoadBlockB1Script;
            }
        }
        public ICommand LoadBlockB2Script
        {
            get
            {
                _LoadBlockB2Script = new RelayCommand(
                    param => evt_LoadscripttoBlock(3));
                return _LoadBlockB2Script;
            }
        }
        //
        public ICommand LoadBlockA1initScript
        {
            get
            {
                _LoadBlockA1initScript = new RelayCommand(
                    param => evt_LoadscripttoBlock(4));
                return _LoadBlockA1initScript;
            }
        }
        public ICommand LoadBlockA2initScript
        {
            get
            {
                _LoadBlockA2initScript = new RelayCommand(
                    param => evt_LoadscripttoBlock(5));
                return _LoadBlockA2initScript;
            }
        }
        public ICommand LoadBlockB1initScript
        {
            get
            {
                _LoadBlockB1initScript = new RelayCommand(
                    param => evt_LoadscripttoBlock(6));
                return _LoadBlockB1initScript;
            }
        }
        public ICommand LoadBlockB2initScript
        {
            get
            {
                _LoadBlockB2initScript = new RelayCommand(
                    param => evt_LoadscripttoBlock(7));
                return _LoadBlockB2initScript;
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

        #endregion

        #region Event
 
        public void evt_LoadscripttoBlock(int _selectedblock)
        {
            var loadscriptXMLPath = ConfigModel.Instance.GetStrScriptpath();

            // Check the path location; if it's an empty string, the process will be terminated. 
            if (string.IsNullOrWhiteSpace(loadscriptXMLPath))
                return;
            switch (_selectedblock)
            {
                case 0:
                    BlockA1ObsColSequences = ConfigModel.Instance.GetScriptXMLSequences(loadscriptXMLPath);              // get xml format testsuit 
                    BlockA1scriptPath = loadscriptXMLPath;
                    OnPropertyChanged(nameof(BlockA1scriptPath));
                    break;
                case 1:
                    BlockA2ObsColSequences = ConfigModel.Instance.GetScriptXMLSequences(loadscriptXMLPath);              // get xml format testsuit 
                    BlockA2scriptPath = loadscriptXMLPath;
                    OnPropertyChanged(nameof(BlockA2scriptPath));
                    break;
                case 2:
                    BlockB1ObsColSequences = ConfigModel.Instance.GetScriptXMLSequences(loadscriptXMLPath);              // get xml format testsuit 
                    BlockB1scriptPath = loadscriptXMLPath;
                    OnPropertyChanged(nameof(BlockB1scriptPath));
                    break;
                case 3:
                    BlockB2ObsColSequences = ConfigModel.Instance.GetScriptXMLSequences(loadscriptXMLPath);              // get xml format testsuit 
                    BlockB2scriptPath = loadscriptXMLPath;
                    OnPropertyChanged(nameof(BlockB2scriptPath));
                    break;
                case 4:
                    BlockA1initObsColSequences = ConfigModel.Instance.GetScriptXMLSequences(loadscriptXMLPath);              // get xml format testsuit 
                    BlockA1initscriptPath = loadscriptXMLPath;
                    OnPropertyChanged(nameof(BlockA1initscriptPath));
                    break;
                case 5:
                    BlockA2initObsColSequences = ConfigModel.Instance.GetScriptXMLSequences(loadscriptXMLPath);              // get xml format testsuit 
                    BlockA2initscriptPath = loadscriptXMLPath;
                    OnPropertyChanged(nameof(BlockA2initscriptPath));
                    break;
                case 6:
                    BlockB1initObsColSequences = ConfigModel.Instance.GetScriptXMLSequences(loadscriptXMLPath);              // get xml format testsuit 
                    BlockB1initscriptPath = loadscriptXMLPath;
                    OnPropertyChanged(nameof(BlockB1initscriptPath));
                    break;
                case 7:
                    BlockB2initObsColSequences = ConfigModel.Instance.GetScriptXMLSequences(loadscriptXMLPath);              // get xml format testsuit 
                    BlockB2initscriptPath = loadscriptXMLPath;
                    OnPropertyChanged(nameof(BlockB2initscriptPath));
                    break;
            }
            //evt_ReleasetoRefresh();
        }
        public void evt_ClearPageProperties()
        {
            BlockA1scriptPath = null;
            BlockA2scriptPath = null;
            BlockB1scriptPath = null;
            BlockB2scriptPath = null;
            BlockA1scriptruntime = 0;
            BlockA2scriptruntime = 0;
            BlockB1scriptruntime = 0;
            BlockB2scriptruntime = 0;
            BlockA1scriptLoop = 0;
            BlockA2scriptLoop = 0;
            BlockB1scriptLoop = 0;
            BlockB2scriptLoop = 0;
            BlockA1initscriptPath = null;
            BlockA2initscriptPath = null;
            BlockB1initscriptPath = null;
            BlockB2initscriptPath = null;
            BlockAscriptLoop = 0;
            BlockBscriptLoop = 0;
            BlockA1ObsColSequences = new ObservableCollection<ScriptItemtype>();
            BlockA2ObsColSequences = new ObservableCollection<ScriptItemtype>();
            BlockB1ObsColSequences = new ObservableCollection<ScriptItemtype>();
            BlockB2ObsColSequences = new ObservableCollection<ScriptItemtype>();
            BlockA1initObsColSequences = new ObservableCollection<ScriptItemtype>();
            BlockA2initObsColSequences = new ObservableCollection<ScriptItemtype>();
            BlockB1initObsColSequences = new ObservableCollection<ScriptItemtype>();
            BlockB2initObsColSequences = new ObservableCollection<ScriptItemtype>();
  
            OnPropertyChanged(nameof(BlockA1scriptPath));
            OnPropertyChanged(nameof(BlockA2scriptPath));
            OnPropertyChanged(nameof(BlockB1scriptPath));
            OnPropertyChanged(nameof(BlockB2scriptPath));
            OnPropertyChanged(nameof(BlockA1scriptruntime));
            OnPropertyChanged(nameof(BlockA2scriptruntime));
            OnPropertyChanged(nameof(BlockB1scriptruntime));
            OnPropertyChanged(nameof(BlockB2scriptruntime));
            OnPropertyChanged(nameof(BlockA1scriptLoop));
            OnPropertyChanged(nameof(BlockA2scriptLoop));
            OnPropertyChanged(nameof(BlockB1scriptLoop));
            OnPropertyChanged(nameof(BlockB2scriptLoop));
            OnPropertyChanged(nameof(BlockA1initscriptPath));
            OnPropertyChanged(nameof(BlockA2initscriptPath));
            OnPropertyChanged(nameof(BlockB1initscriptPath));
            OnPropertyChanged(nameof(BlockB2initscriptPath));
            OnPropertyChanged(nameof(BlockAscriptLoop));
            OnPropertyChanged(nameof(BlockBscriptLoop));

         
        }
        #endregion

    }
}
