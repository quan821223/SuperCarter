using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using Notification.Wpf;
using SuperCarter.Services;
using SuperCarter.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brush = System.Windows.Media.Brush;

namespace SuperCarter.Model
{
    public class CustomScriptEditor : ViewModelBase
    {
     
        public CustomScriptEditor()
        {
            Fullloop = 1;
            folderViewerlist = new ObservableCollection<Foldertype>();
            evt_Loadscriptroot(AppPath + @"scripts");
            Viewerpath = AppPath + @"scripts";
            OnPropertyChanged(nameof(folderViewerlist));
            OnPropertyChanged(nameof(Viewerpath));

            blockAfolderViewerlist = new ObservableCollection<IFiletype>();
            ctsScrollingcheck= new CancellationTokenSource();


            /// 燈號設定
            DisplaySwitch = Visibility.Visible;
            LabelText = "●";
            ForeColor = new SolidColorBrush(Color.FromRgb(255, 97, 3));
        }

        ~CustomScriptEditor()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region property
        public CSVfile cSVfile { get; set; }
        private UnifiedHostCommandSettype UnifiedHostCommandSet { get; set; } = new UnifiedHostCommandSettype();
        public ObservableCollection<IFiletype> blockAfolderViewerlist { get; set; } = new ObservableCollection<IFiletype>();
        public ObservableCollection<Foldertype> folderViewerlist { get; set; } = new ObservableCollection<Foldertype>();
        public string Viewerpath { get; set; }
        public ObservableCollection<ScriptItemtype> ObsColBlockA1Sequences { get; set; } = new ObservableCollection<ScriptItemtype>();
        public ObservableCollection<ScriptItemtype> ObsColBlockA2Sequences { get; set; } = new ObservableCollection<ScriptItemtype>();
        public ObservableCollection<ScriptItemtype> ObsColBlockB1Sequences { get; set; } = new ObservableCollection<ScriptItemtype>();
        public ObservableCollection<ScriptItemtype> ObsColBlockB2Sequences { get; set; } = new ObservableCollection<ScriptItemtype>();
        public int PowerMode { get; set; }
        public int CommnadID { get; set; } = 0;
        private string sendmsg, recmsg;
        private readonly byte[] RESPONSE_TAIL = { 0x0D, 0x0A };
        public int blockAitemcount { get; set; } = 0;
        public int blockBitemcount { get; set; } = 0;
        public int blockCitemcount { get; set; } = 0;
        public int blockDitemcount { get; set; } = 0;
        private bool _IsEnableExecuteSDMchecklists;
        public bool IsEnableExecuteSDMchecklists
        {
            get => _IsEnableExecuteSDMchecklists;
            set {
                _IsEnableExecuteSDMchecklists = value;
                if(_IsEnableExecuteSDMchecklists)
                    ForeColor = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                else
                    ForeColor = new SolidColorBrush(Color.FromRgb(255, 97, 3));
                OnPropertyChanged(nameof(IsEnableExecuteSDMchecklists));
            }
        }
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
                _ExecuteBlockALoop =  value;
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
                _ExecuteBlockBLoop = value;
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

        #region 參數更新的更新流程 
        public void UpdateDashboardStartThread()
        {
            Thread lowPriorityThread = new Thread(() =>
            {
                while (true)
                {
                    ProcessQueue();

                    // Sleep for a while to prevent busy waiting
                    Thread.Sleep(200);
                }
            });

            lowPriorityThread.Priority = ThreadPriority.Lowest;
            lowPriorityThread.Start();
        }
        private void ProcessQueue()
        {

            while (true)
            {
                while (SendAndReceiveDatabatchQ.TryDequeue(out SendAndReceiveDatabatchcheck bytes))
                {
                    if (bytes != null)
                    {
                        try
                        {
                            ProcessBytes(bytes);
                        }
                        catch (Exception ex)
                        {

                            System.Windows.MessageBox.Show(ex.StackTrace);
                            System.Windows.MessageBox.Show(ex.Message);
                        }
                    }
                }

                // 如果你不打算持續地檢查佇列，那麼這裡可以加上一個小的延遲以避免忙碌等待。
                Thread.Sleep(10);
            }
        }
        private void ProcessBytes(SendAndReceiveDatabatchcheck bytes)
        {
            if (bytes.byte_buffer_Receive.Length > 4 && bytes.byte_buffer_Receive[0] == 0xFA && bytes.byte_buffer_Send[1] == 0x52)
            {
                byte sendByte3 = bytes.byte_buffer_Send[3];
                byte sendByte4 = bytes.byte_buffer_Send[4];
                byte receiveByte2 = bytes.byte_buffer_Receive[2];

                // ... continue with your checks

                switch (sendByte3)
                {
                    case 0x00:
                        HandleBufferSend_00(sendByte4, bytes);
                        break;
                    case 0x01:
                        HandleBufferSend_01(sendByte4, bytes);
                        break;
                    case 0x04:
                        HandleBufferSend_04(sendByte4, bytes);
                        break;
                    case 0x05:
                        HandleBufferSend_05(sendByte4, bytes);
                        break;
                    case 0x06:
                        HandleBufferSend_06(sendByte4, bytes);
                        break;
                }

                App.Current.Dispatcher.Invoke(() =>
                {
                    updateUIobj();      // 刷新項目
                });
            }
        }
        #endregion

        #region icommand event
        private ICommand _LoadscripttoBlockA, _LoadscripttoBlockB, _LoadscripttoBlockC, _LoadscripttoBlockD, _ExecuteSDMchecklistScript, _SaveDynamicMonitorresult;

        public ICommand ExecuteSDMchecklistScript
        {
            get {
                _ExecuteSDMchecklistScript = new RelayCommand(
                    param => evt_ExecuteSDMchecklistScript());
                return _ExecuteSDMchecklistScript;
            }
        
        }
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

        #region Scheduled script runtime 
        #region properties
        public static List<SendorExecuteSendType> BlockASequencesList { get; set; } = new List<SendorExecuteSendType>();
        public static List<SendorExecuteSendType> BlockBSequencesList { get; set; } = new List<SendorExecuteSendType>();
        public static List<SendorExecuteSendType> BlockCSequencesList { get; set; } = new List<SendorExecuteSendType>();
        public static List<SendorExecuteSendType> BlockDSequencesList { get; set; } = new List<SendorExecuteSendType>();
        private int _CurLoopValue;
        public int CurLoopValue
        {
            get => _CurLoopValue;
            set
            {
                _CurLoopValue = value;
                float temp = (CurLoopValue * 1.0f / Fullloop) * 100;
                PercentLoopValue = Convert.ToInt32(temp);
                strLoopValue = PercentLoopValue.ToString();
                OnPropertyChanged(nameof(strLoopValue));
            }
        }
        private string _strLoopValue;
        public string strLoopValue
        {
            get { return _strLoopValue; }
            set
            {
                _strLoopValue = string.Format("{0}%", PercentLoopValue);

            }
        }
        private string _Curphase;
        public string Curphase
        {
            get => _Curphase;
            set
            {
                _Curphase = value;
                OnPropertyChanged(nameof(Curphase));
            }
        }
        private int _subloop;
        public int subloop
        {
            get => _subloop;
            set
            {
                _subloop = value;
                OnPropertyChanged(nameof(subloop));
            }
        }
        public int PercentLoopValue { get; set; }
        #endregion
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            string path = string.Format("{0}\\{1}_{2}", FOLDER_RESULT, DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss"), "-outputtestdata.csv");
            cSVfile = new CSVfile(path); // 請替換為您希望保存文件的路徑

            UnifiedHostCommandSet = new UnifiedHostCommandSettype();

            /***         
            *  in  
            * ****/

            CommnadID = 0;

            logger.Log(NLog.LogLevel.Trace, "– Start Custom script schedule.");
            var Startmsg = "– Start Custom script schedule.";
            WritedataToViewTextAggregator.Instance.Updatemsg(new RealtimeMsgQueuetype { msgtype = Msgtype.Message,  msg = Startmsg });
            for (CurLoopValue = 0; CurLoopValue < Fullloop; CurLoopValue++) // 總迴圈
            {
                var msg = string.Format("- Currently on iteration : {0}", CurLoopValue);
                logger.Log(NLog.LogLevel.Trace, msg);                 
                WritedataToViewTextAggregator.Instance.Updatemsg(new RealtimeMsgQueuetype { msgtype = Msgtype.Message, msg = msg });

                // 如果token已被取消，跳出迴圈
                if (cancellationToken.IsCancellationRequested)
                    break;
                OnPropertyChanged(nameof(Fullloop));
                OnPropertyChanged(nameof(CurLoopValue));
                OnPropertyChanged(nameof(PercentLoopValue));
                await BlockWorkstation("A", BlockASequencesList, BlockBSequencesList, ExecuteBlockALoop, blockAscriptDelaytime, blockBscriptDelaytime, cancellationToken);
                //await BlockWorkstation("B", BlockBSequencesList, ExecuteBlockBLoop, blockBscriptDelaytime, cancellationToken);
                await BlockWorkstation("C", BlockCSequencesList, BlockDSequencesList, ExecuteBlockCLoop, blockCscriptDelaytime, blockDscriptDelaytime, cancellationToken);
                //await BlockWorkstation("D", BlockDSequencesList, ExecuteBlockDLoop, blockDscriptDelaytime, cancellationToken);

            }
            ObjectAggregator.Instance.UpdateObject();

            OnPropertyChanged(nameof(Fullloop));
            OnPropertyChanged(nameof(CurLoopValue));
            OnPropertyChanged(nameof(PercentLoopValue));

            logger.Log(NLog.LogLevel.Trace, "– End Custom script schedule.");

            var Endmmsg = "– End Custom script schedule.";
            WritedataToViewTextAggregator.Instance.Updatemsg(new RealtimeMsgQueuetype { msgtype = Msgtype.Message, msg = Endmmsg });
        }
        private async Task BlockWorkstation(string blockName, List<SendorExecuteSendType> sequences1, List<SendorExecuteSendType> sequences2,
                                            int maxLoop, int scriptDelayTime1, int scriptDelayTime2, CancellationToken cancellationToken)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            logger.Log(NLog.LogLevel.Trace, $"– Enter {blockName} step.");
            //RealtimeMsgQueue.Enqueue($"– Enter {blockName} step.");
            cancellationToken.ThrowIfCancellationRequested();
            Curphase = blockName;

            //int blockAcycletime = 360000;
            //int blockA1cycletime = 300000;
            //int blockA2cycletime = 60000;
            //int Timercycle = 3000;
            int blockAcycletime = 180000;
            int blockA1cycletime = 120000;
            int blockA2cycletime = 60000;
            int Timercycle = 3000;

            for (int curLoop = 0; curLoop < maxLoop; curLoop++)
            {
                subloop = curLoop;
                // 
                var blockStartTime = DateTime.Now;

                // Start the block working time loop
                // Execute sequences1 within cycletime
                var seq1Task = ExecuteBlockSequences($"{blockName}_{1}", sequences1, blockA1cycletime, cancellationToken, Timercycle);
                await seq1Task; // Wait for seq1Task to complete

                // Execute sequences2 within cycletime
                var seq2Task = ExecuteBlockSequences($"{blockName}_{2}", sequences2, blockA2cycletime, cancellationToken, Timercycle);
                await seq2Task; // Wait for seq2Task to complete

                // Check if block working time is reached
                if ((DateTime.Now - blockStartTime).TotalMilliseconds >= blockAcycletime)
                {
                    break; // Exit the loop
                }
            }

            stopwatch.Stop();
            var remainingTime = TimeSpan.FromMilliseconds((blockAcycletime) * maxLoop) - stopwatch.Elapsed;
            cancellationToken.ThrowIfCancellationRequested();
            if (remainingTime > TimeSpan.Zero)
            {
                await Task.Delay(remainingTime, cancellationToken);
            }

        }
        private async Task ExecuteBlockSequences(string blockName, List<SendorExecuteSendType> sequences, int scriptDelayTime, CancellationToken cancellationToken, int cycletime)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            cancellationToken.Register(() => cancellationTokenSource.Cancel());

            bool WorkingBreak = false;

            try
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var msg1 = $"- Currently on {blockName} iteration: {blockName}";
                logger.Log(NLog.LogLevel.Trace, msg1);

                var blockStartTime = DateTime.Now;

                while ((DateTime.Now - blockStartTime).TotalMilliseconds < scriptDelayTime)
                {
                    var cycleStartTime = DateTime.Now;
                    int sequenceIndex = 0;

                    while ((DateTime.Now - cycleStartTime).TotalMilliseconds < cycletime)
                    {
                        cancellationToken.ThrowIfCancellationRequested();


                        var elapsedTime = (DateTime.Now - cycleStartTime).TotalMilliseconds;
                        var remainingTime = cycletime - (int)elapsedTime;

                        if (sequenceIndex < sequences.Count)
                        {
                            if (sequences[sequenceIndex].PortNum == 9)
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    sequenceIndex++;
                                    if (sequenceIndex >= sequences.Count || cancellationToken.IsCancellationRequested)
                                    {
                                        break; // Exit the loop
                                    }
                                    await SendAndReceivesAsync(sequences[sequenceIndex], cancellationToken);
                                }
                            }
                            else
                            {
                                await SendAndReceivesAsync(sequences[sequenceIndex], cancellationToken);
                                sequenceIndex++;
                            }
                        }

                        if (sequenceIndex >= sequences.Count)
                        {
                            // Check if scriptDelayTime has already expired
                            var remainingScriptTime = scriptDelayTime - (int)(DateTime.Now - blockStartTime).TotalMilliseconds;
                            if (remainingScriptTime <= 0)
                            {
                                return;
                            }

                            await Task.Delay(Math.Min(remainingTime, remainingScriptTime), cancellationToken);
                        }
                    }
                    UnifiedHostCommandSet.Time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ffff");
                    UnifiedHostCommandSet.Loop = CurLoopValue.ToString();
                    UnifiedHostCommandSet.Blockphase = Curphase.ToString();
                    UnifiedHostCommandSet.Blockloop = blockName.ToString();
                    //cSVfile.AppendToCsv(UnifiedHostCommandSet);
                    UnifiedHostCommandSet = new UnifiedHostCommandSettype();
                }
            }
            catch (TaskCanceledException)
            {
                WorkingBreak = true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }

            // Rest of your code...
        }
        #endregion

        #region DynamicMonitorCheck
        public string SDMChecklistscriptXMLPath { get; set; } = "";
        public ICommand SaveDynamicMonitorresult
        {
            get
            {
                _SaveDynamicMonitorresult = new RelayCommand(
                    param => SaveDynamicMonitorResult());
                return _SaveDynamicMonitorresult;
            }
        }
        private ICommand _LoadSDMchecklistScript;
        public ICommand LoadSDMchecklistScript
        {
            get
            {
                _LoadSDMchecklistScript = new RelayCommand(
                      param => LoadSDMchecklistScriptXMLfile());
                return _LoadSDMchecklistScript;
            }
        }

        public CancellationTokenSource ctsScrollingcheck;
        #region
        /// <summary>
        /// 燈號控制
        /// </summary>
        public Visibility DisplaySwitch { get; set; } = Visibility.Hidden;
        public string LabelText { get; set; }
        private Brush _ForeColor;

        public Brush ForeColor {
            get => _ForeColor;
            set
            {
                _ForeColor = value;
                OnPropertyChanged(nameof(ForeColor));   
            } 
        }
        #endregion

        /// <summary>
        /// 運行動態顯示
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StartScrollingCheck(CancellationToken cancellationToken)
        {
            bool WorkingBreak = false;
            try
            {
                CommnadID = 0;
                UnifiedHostCommandSet = new UnifiedHostCommandSettype();
                UnifiedHostCommandSet.IsEnableExecuteSDMcheck = true;
                List<SendorExecuteSendType> sequences = SendorExecuteSequencesList;
                int sequenceIndex = 0;
                var Sendorwatch = new Stopwatch();

                /// 將燈號設定成未完成狀態
                ForeColor = new SolidColorBrush(Color.FromRgb(255, 97, 3));

                while (IsEnableExecuteSDMchecklists)
                {
                    if (sequences[sequenceIndex].PortNum == 9)
                    {
                        var roundtimedelay = sequences[sequenceIndex].Delaytime;
                        Sendorwatch.Restart();
                        for (int j = 0; j < 3; j++)
                        {
                            sequenceIndex++;
                            if (sequenceIndex >= sequences.Count || cancellationToken.IsCancellationRequested)
                            {
                                break; // Exit the loop
                            }
                            await SendAndReceivesAsync(sequences[sequenceIndex], cancellationToken);                             
                        }
                        Sendorwatch.Stop();
                        var remainingSpentTime = roundtimedelay - (int)Sendorwatch.ElapsedMilliseconds;

                        if (remainingSpentTime > 0)
                        {
                            await Task.Delay(remainingSpentTime, cancellationToken);
                        }
                    }
                    else
                    {
                        await SendAndReceivesAsync(sequences[sequenceIndex], cancellationToken);
                        sequenceIndex++;
                    }
                    if (sequenceIndex >= sequences.Count) 
                    {
                        sequenceIndex = 0;
                        ForeColor = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                    }
                }
            
            }
            catch (TaskCanceledException)
            {
                WorkingBreak = true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
           
        }
        #region Event
        public void LoadSDMchecklistScriptXMLfile()
        {
            SDMChecklistscriptXMLPath = ConfigModel.Instance.GetStrScriptpath();

            if (!string.IsNullOrWhiteSpace(SDMChecklistscriptXMLPath))
            {
                OnPropertyChanged(nameof(SDMChecklistscriptXMLPath));
                SendorExecuteSequencesList = new List<SendorExecuteSendType>();
                SendorExecuteSequencesList = ConfigModel.Instance.GetSendorExecuteSequencesList(SDMChecklistscriptXMLPath);

            }
            else
            {
                System.Windows.MessageBox.Show("未偵測到腳本路徑", "Information !", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        public void SaveDynamicMonitorResult()
        {
            cSVfile = new CSVfile();
            cSVfile.AppendToCsv2(UnifiedHostCommandSet);
            IsEnableExecuteSDMchecklists = false;
            OnPropertyChanged(nameof(IsEnableExecuteSDMchecklists));
            // 開啟檔案位置
            string dataPath = AppPath + @"\result\Dynamic";
            Process.Start(new ProcessStartInfo { FileName = dataPath, UseShellExecute = true });

        }
        #endregion


        #endregion

        #region Scheduled detection cmd

        #endregion

        #region event
        private async Task SendAndReceivesAsync(object Sequence, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var command = (SendorExecuteSendType)Sequence;

            if (!DicSerialPort[command.PortNum].IsOpen)
                return;

            byte[] receivedData = new byte[0];

            if (command.SequenceData[0] == 0xFA && command.SequenceData[3] == 0x02 && command.SequenceData[4] == 0x01)
            {
                PowerMode = 1;
                UnifiedHostCommandSet.DUT1PowerMode = "Normal";
                UnifiedHostCommandSet.DUT2PowerMode = "Normal";
                UnifiedHostCommandSet.DUT3PowerMode = "Normal";
            }

            else if (command.SequenceData[0] == 0xFA && command.SequenceData[3] == 0x02 && command.SequenceData[4] == 0x00)
            {
                PowerMode = 0;
                UnifiedHostCommandSet.DUT1PowerMode = "Sleep";
                UnifiedHostCommandSet.DUT2PowerMode = "Sleep";
                UnifiedHostCommandSet.DUT3PowerMode = "Sleep";
            }

            String OutputMsg = String.Format("{0}|{1}|{2}| S | CommandID {4}| {3}",
                DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ffff"),
                SerialPortModel.Instance.PortNameBinding[DicSerialPort[command.PortNum].PortName].ToString().PadLeft(2, ' '),
                DicSerialPort[command.PortNum].PortName.PadLeft(6, ' '),
                command.strSequenceData.Replace(" ", ""),
                CommnadID.ToString().PadLeft(6, ' ')
                );
            logger.Log(NLog.LogLevel.Trace, OutputMsg);
            WritedataToViewTextAggregator.Instance.Updatemsg(new RealtimeMsgQueuetype { msgtype = Msgtype.FromPort, PortNum = command.PortNum, msg = OutputMsg });

          

            DicSerialPort[command.PortNum].Write(command.SequenceData, 0, command.SequenceData.Length);

            // 紀錄讀取 sdm 回傳的資料
            receivedData = await ReadFromPortAsync(Sequence, cancellationToken);

            // update to Dashboard
            if (receivedData.Length > 0)
                RealtimeSDMDataQueue.Enqueue(receivedData);

            
            recmsg = SerialPortModel.Instance.byteToHexStr(receivedData);

            OutputMsg = String.Format("{0}|{1}|{2}| R | CommandID {4}| {3}",
                DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ffff"),
                SerialPortModel.Instance.PortNameBinding[DicSerialPort[command.PortNum].PortName].ToString().PadLeft(2, ' '),
                DicSerialPort[command.PortNum].PortName.PadLeft(6, ' '),
                recmsg.Replace(" ", ""),
              CommnadID.ToString().PadLeft(6, ' ')
                );
            // update to nlog file
            logger.Log(NLog.LogLevel.Trace, OutputMsg);

            WritedataToViewTextAggregator.Instance.Updatemsg(new RealtimeMsgQueuetype { msgtype = Msgtype.FromPort, PortNum = command.PortNum, msg = OutputMsg });
            SendAndReceiveDatabatchQ.Enqueue(new SendAndReceiveDatabatchcheck()
            {
                CommnadID = CommnadID,
                Portnum = command.PortNum,
                strSequenceData_send = command.strSequenceData,
                strSequenceData_Rec = recmsg,
                byte_buffer_Send = command.SequenceData,
                byte_buffer_Receive = receivedData,
            });

            CommnadID += 1;
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
        }

        private async Task<byte[]> ReadFromPortAsync(object Sequence, CancellationToken cancellationToken)
        {
            var command = (SendorExecuteSendType)Sequence;
            const int bufferSize = 1024;
            byte[] buffer = new byte[bufferSize];
            int bufferIndex = 0;
            string data;
            using (var cts = new CancellationTokenSource(100))
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    // 檢查是否有數據可讀
                    if (DicSerialPort[command.PortNum].BytesToRead > 0)
                    {
                        var readByte = (byte)DicSerialPort[command.PortNum].ReadByte();

                        // 檢查是否超過緩存大小
                        if (bufferIndex >= bufferSize)
                        {
                            logger.Log(NLog.LogLevel.Warn, "Buffer overflow detected. Exiting.");
                            break;
                        }

                        buffer[bufferIndex++] = readByte;

                        if (buffer[0] == 0xFA && bufferIndex > 2)
                        {
                            if (buffer[2] - 2 <= bufferIndex)
                            {
                                // 檢查是否收到期望的字元
                                if (bufferIndex > 1 && buffer[bufferIndex - 2] == RESPONSE_TAIL[0] && buffer[bufferIndex - 1] == RESPONSE_TAIL[1])
                                {
                                    byte[] resultBuffer = new byte[bufferIndex];
                                    Array.Copy(buffer, resultBuffer, bufferIndex);

                                    return resultBuffer;
                                }
                            }
                        }
                        else
                        {
                            // 檢查是否收到期望的字元
                            if (bufferIndex > 1 && buffer[bufferIndex - 2] == RESPONSE_TAIL[0] && buffer[bufferIndex - 1] == RESPONSE_TAIL[1])
                            {
                                byte[] resultBuffer = new byte[bufferIndex];
                                Array.Copy(buffer, resultBuffer, bufferIndex);

                                return resultBuffer;
                            }
                        }
                        if (cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                    else
                    {
                        await Task.Delay(10, cancellationToken);
                    }
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                }

                // 如果超時且沒有得到預期響應
                if (bufferIndex > 0)
                {
                    logger.Log(NLog.LogLevel.Trace, "Timeout without receiving expected response. Partial data returned.");
                    Array.Resize(ref buffer, bufferIndex);
                    return buffer;
                }
                else
                {
                    logger.Log(NLog.LogLevel.Trace, "Timeout without any data received.");
                    return Array.Empty<byte>();
                }
            }
        }

        private void evt_ExecuteSDMchecklistScript()
        {
            if (IsEnableExecuteSDMchecklists)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (DicSerialPort[i].IsOpen)
                    {
                        DicSerialPort[i].DataReceived -= new SerialDataReceivedEventHandler(SerialPortModel.Instance.DataReceivedCom);
                        DicSerialPort[i].DiscardInBuffer();
                        DicSerialPort[i].DiscardOutBuffer();
                    }
                }
                if (string.IsNullOrEmpty(SDMChecklistscriptXMLPath) || ! (DicSerialPort[0].IsOpen || DicSerialPort[1].IsOpen || DicSerialPort[2].IsOpen ))
                {
                    IsEnableExecuteSDMchecklists = false;
                    OnPropertyChanged(nameof(IsEnableExecuteSDMchecklists));
                }                      
                else
                {
                    UnifiedHostCommandSet = new UnifiedHostCommandSettype();
                    UnifiedHostCommandSet.IsEnableExecuteSDMcheck = true;

                  
                    ctsScrollingcheck = new CancellationTokenSource();
                    StartScrollingCheck(ctsScrollingcheck.Token);
                }
       
            }
            else
            {
                ctsScrollingcheck.Cancel();
            }
        }

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
                    ObsColBlockA1Sequences = ConfigModel.Instance.GetScriptXMLSequences(loadscriptXMLPath);              // get xml format testsuit 
                    ExecuteBlockALoop = ConfigModel.Instance.GetScriptXMLSequence_itervalue(loadscriptXMLPath);         // get block execute loop
                    blockAscriptDelaytime = ConfigModel.Instance.GetScriptXMLSequence_TotalTime(loadscriptXMLPath);     // get block script total delay time
                    blockAscriptpath = loadscriptXMLPath;
                    blockAitemcount = ObsColBlockA1Sequences.Count;
                    break;
                case 1:
                    blockBscriptpath = null;
                    ObsColBlockA2Sequences = ConfigModel.Instance.GetScriptXMLSequences(loadscriptXMLPath);              // get xml format testsuit 
                    ExecuteBlockBLoop = ConfigModel.Instance.GetScriptXMLSequence_itervalue(loadscriptXMLPath);         // get block execute loop
                    blockBscriptDelaytime = ConfigModel.Instance.GetScriptXMLSequence_TotalTime(loadscriptXMLPath);     // get block script total delay time
                    blockBscriptpath = loadscriptXMLPath;
                    blockBitemcount = ObsColBlockA2Sequences.Count;
                    break;
                case 2:
                    blockCscriptpath = null;
                    ObsColBlockB1Sequences = ConfigModel.Instance.GetScriptXMLSequences(loadscriptXMLPath);              // get xml format testsuit 
                    ExecuteBlockCLoop = ConfigModel.Instance.GetScriptXMLSequence_itervalue(loadscriptXMLPath);         // get block execute loop
                    blockCscriptDelaytime = ConfigModel.Instance.GetScriptXMLSequence_TotalTime(loadscriptXMLPath);     // get block script total delay time
                    blockCscriptpath = loadscriptXMLPath;
                    blockCitemcount = ObsColBlockB1Sequences.Count;
                    break;
                case 3:
                    blockDscriptpath = null;
                    ObsColBlockB2Sequences = ConfigModel.Instance.GetScriptXMLSequences(loadscriptXMLPath);              // get xml format testsuit 
                    ExecuteBlockDLoop = ConfigModel.Instance.GetScriptXMLSequence_itervalue(loadscriptXMLPath);         // get block execute loop
                    blockDscriptDelaytime = ConfigModel.Instance.GetScriptXMLSequence_TotalTime(loadscriptXMLPath);     // get block script total delay time
                    blockDscriptpath = loadscriptXMLPath;
                    blockDitemcount = ObsColBlockB2Sequences.Count;
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

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
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

        #region Monitor dashboard
        private void HandleBufferSend_00(byte sendByte4, SendAndReceiveDatabatchcheck bytes)
        {
            if (sendByte4 == 0x01)
            {
                string msg = string.Format("{0}.{1}", bytes.byte_buffer_Receive[3].ToString(), bytes.byte_buffer_Receive[4].ToString());
          
                if (bytes.byte_buffer_Receive[1] == 1)
                {
                    UnifiedHostCommandSet.DUT1SWversion = msg;
                }
                else if (bytes.byte_buffer_Receive[1] == 2)
                {
                    UnifiedHostCommandSet.DUT2SWversion = msg;
                }
                else if (bytes.byte_buffer_Receive[1] == 3)
                {
                    UnifiedHostCommandSet.DUT3SWversion = msg;
                }
                evt_func_SWversion(bytes.byte_buffer_Receive[1], msg);
            }
            else if (sendByte4 == 0x02)
            {
                string msg = string.Format("{0}.{1}", bytes.byte_buffer_Receive[3].ToString(), bytes.byte_buffer_Receive[4].ToString());
                if (bytes.byte_buffer_Receive[1] == 1)
                {
                    UnifiedHostCommandSet.DUT1HWversion = msg;
                }
                else if (bytes.byte_buffer_Receive[1] == 2)
                {
                    UnifiedHostCommandSet.DUT2HWversion = msg;
                }
                else if (bytes.byte_buffer_Receive[1] == 3)
                {
                    UnifiedHostCommandSet.DUT3HWversion = msg;
                }
                evt_func_HWversion(bytes.byte_buffer_Receive[1], msg);
            }
        }
        private void HandleBufferSend_01(byte sendByte4, SendAndReceiveDatabatchcheck bytes)
        {
            if (sendByte4 == 0x01)
            {
                int bbvalue = Convert.ToInt32(bytes.byte_buffer_Receive[3]);

                evt_func_Backlighbrightness(bytes.byte_buffer_Receive[1], bbvalue);
            }
            // ... other conditions
        }
        private void HandleBufferSend_04(byte sendByte4, SendAndReceiveDatabatchcheck bytes)
        {
            if (sendByte4 == 0x01)
            {
                int DTC = Convert.ToInt32(bytes.byte_buffer_Receive[3]);

                if (bytes.byte_buffer_Receive[1] == 1)
                {
                    UnifiedHostCommandSet.DUT1Diagnostic_raw = bytes.strSequenceData_Rec;
                }
                else if (bytes.byte_buffer_Receive[1] == 2)
                {
                    UnifiedHostCommandSet.DUT2Diagnostic_raw = bytes.strSequenceData_Rec;
                }
                else if (bytes.byte_buffer_Receive[1] == 3)
                {
                    UnifiedHostCommandSet.DUT3Diagnostic_raw = bytes.strSequenceData_Rec;
                }

                evt_func_DTC(bytes.byte_buffer_Receive[1], DTC);
            }

            else if (sendByte4 == 0x02)
            {

                double volvalue1 = (double)(bytes.byte_buffer_Receive[3] << 8 | bytes.byte_buffer_Receive[4]) / 10;
                evt_func_Voltage(bytes.byte_buffer_Receive[1], volvalue1);

            }
            else if (sendByte4 == 0x03)
            {

                int led1temp = Convert.ToInt32(bytes.byte_buffer_Receive[3]);
                int led2temp = Convert.ToInt32(bytes.byte_buffer_Receive[4]);
                int pcbtemp = Convert.ToInt32(bytes.byte_buffer_Receive[5]);
                evt_func_Temp(bytes.byte_buffer_Receive[1], led1temp, led2temp, pcbtemp);

            }

            else if (sendByte4 == 0x04)
            {
                int Ambienttemp = Convert.ToInt32(bytes.byte_buffer_Receive[3]);
                evt_func_T_chamber(bytes.byte_buffer_Receive[1], Ambienttemp);

            }
            else if (sendByte4 == 0x05)
            {
                int adcvalue = bytes.byte_buffer_Receive[3] << 8 + bytes.byte_buffer_Receive[4];

                evt_func_ADC(bytes.byte_buffer_Receive[1], adcvalue);
            }

        }
        private void HandleBufferSend_05(byte sendByte4, SendAndReceiveDatabatchcheck bytes)
        {
            if (PowerMode == 0x01)
            {
                double current1 = Convert.ToDouble(Convert.ToInt32(bytes.byte_buffer_Receive[3].ToString("X2"), 16));
                double current2 = Convert.ToDouble(Convert.ToInt32(bytes.byte_buffer_Receive[4].ToString("X2"), 16)) / 100.0;
                evt_func_normalCurrent(bytes.byte_buffer_Receive[1], current1 + current2);
            }
            else if (PowerMode == 0x00)
            {

                double current1 = Convert.ToDouble(Convert.ToInt32(bytes.byte_buffer_Receive[3].ToString("X2"), 16));
                double current2 = Convert.ToDouble(Convert.ToInt32(bytes.byte_buffer_Receive[4].ToString("X2"), 16)) / 100.0;
                evt_func_sleepCurrent(bytes.byte_buffer_Receive[1], current1 + current2);
            }

        }
        private void HandleBufferSend_06(byte sendByte4, SendAndReceiveDatabatchcheck bytes)
        {
            if (sendByte4 == 0x01)
            {
                //UnifiedHostCommandSet.TouchPoint = bytes.strSequenceData_Rec;
            }


        }
        public void evt_func_SWversion(byte _txb, string _value)
        {

            switch (_txb)
            {
                case 0x01:
                    Port0SDM1SWversion = _value;
                    break;
                case 0x02:
                    Port0SDM2SWversion = _value;
                    break;
                case 0x03:
                    Port0SDM3SWversion = _value;
                    break;
            }
            OnPropertyChanged(nameof(Port0SDM1SWversion));
            OnPropertyChanged(nameof(Port0SDM2SWversion));
            OnPropertyChanged(nameof(Port0SDM3SWversion));


        }
        public void evt_func_HWversion(byte _txb, string _value)
        {
            switch (_txb)
            {
                case 0x01:
                    Port0SDM1HWversion = _value;
                    break;
                case 0x02:
                    Port0SDM2HWversion = _value;
                    break;
                case 0x03:
                    Port0SDM3HWversion = _value;
                    break;
            }
            OnPropertyChanged(nameof(Port0SDM1HWversion));
            OnPropertyChanged(nameof(Port0SDM2HWversion));
            OnPropertyChanged(nameof(Port0SDM3HWversion));
        }

        public void evt_func_DTC(byte _txb, int _value)
        {
            // 0x04 0x01
            switch (_txb)
            {
                case 0x01:
                    UnifiedHostCommandSet.DUT1Diagnostic = _value.ToString();
                    Port0SDM1DTC = _value;
                    break;
                case 0x02:
                    UnifiedHostCommandSet.DUT2Diagnostic = _value.ToString();
                    Port0SDM2DTC = _value;
                    break;
                case 0x03:
                    UnifiedHostCommandSet.DUT3Diagnostic = _value.ToString();
                    Port0SDM3DTC = _value;
                    break;
            }
            OnPropertyChanged(nameof(Port0SDM1DTC));
            OnPropertyChanged(nameof(Port0SDM2DTC));
            OnPropertyChanged(nameof(Port0SDM3DTC));
        }

        public void evt_func_Voltage(byte _txb, double _value)
        {
            // 0x04 0x02
            switch (_txb)
            {
                case 0x01:
                    UnifiedHostCommandSet.DUT1Voltage = _value.ToString();
                    Port0SDM1Voltage = _value;
                    break;
                case 0x02:
                    UnifiedHostCommandSet.DUT2Voltage = _value.ToString();
                    Port0SDM2Voltage = _value;
                    break;
                case 0x03:
                    UnifiedHostCommandSet.DUT3Voltage = _value.ToString();
                    Port0SDM3Voltage = _value;
                    break;
            }
            OnPropertyChanged(nameof(Port0SDM1Voltage));
            OnPropertyChanged(nameof(Port0SDM2Voltage));
            OnPropertyChanged(nameof(Port0SDM3Voltage));
        }

        public void evt_func_Temp(byte _txb, int _LED1, int _LED2, int _PCBA)
        {
            // 0x04 0x03
            string ledAndPCBAData = $"{_LED1}, {_LED2}, {_PCBA}";
            switch (_txb)
            {
                case 0x01:
                    Port0SDM1PCBTemp = _PCBA;
                    Port0SDM1LED1Temp = _LED1;
                    Port0SDM1LED2Temp = _LED2;
                    UnifiedHostCommandSet.DUT1T_LED1_2PCB = ledAndPCBAData;
                    OnPropertyChanged(nameof(Port0SDM1PCBTemp));
                    OnPropertyChanged(nameof(Port0SDM1LED1Temp));
                    OnPropertyChanged(nameof(Port0SDM1LED2Temp));
                    break;
                case 0x02:
                    Port0SDM2PCBTemp = _PCBA;
                    Port0SDM2LED1Temp = _LED1;
                    Port0SDM2LED2Temp = _LED2;
                    UnifiedHostCommandSet.DUT2T_LED1_2PCB = ledAndPCBAData;
                    OnPropertyChanged(nameof(Port0SDM2PCBTemp));
                    OnPropertyChanged(nameof(Port0SDM2LED1Temp));
                    OnPropertyChanged(nameof(Port0SDM2LED2Temp));
                    break;
                case 0x03:
                    Port0SDM3PCBTemp = _PCBA;
                    Port0SDM3LED1Temp = _LED1;
                    Port0SDM3LED2Temp = _LED2;
                    UnifiedHostCommandSet.DUT3T_LED1_2PCB = ledAndPCBAData;
                    OnPropertyChanged(nameof(Port0SDM3PCBTemp));
                    OnPropertyChanged(nameof(Port0SDM3LED1Temp));
                    OnPropertyChanged(nameof(Port0SDM3LED2Temp));
                    break;
            }
        }

        public void evt_func_T_chamber(byte _txb, int _value)
        {
            // 0x04 0x04
            switch (_txb)
            {
                case 0x01:
                    UnifiedHostCommandSet.DUT1T_chamber = _value.ToString();
                    Port0SDM1T_chamber = _value;
                    break;
                case 0x02:
                    UnifiedHostCommandSet.DUT2T_chamber = _value.ToString();
                    Port0SDM2T_chamber = _value;
                    break;
                case 0x03:
                    UnifiedHostCommandSet.DUT3T_chamber = _value.ToString();
                    Port0SDM3T_chamber = _value;
                    break;
            }
            OnPropertyChanged(nameof(Port0SDM1T_chamber));
            OnPropertyChanged(nameof(Port0SDM2T_chamber));
            OnPropertyChanged(nameof(Port0SDM3T_chamber));
        }
       
        public void evt_func_ADC(byte _txb, double _value)
        {
            // 0x04 0x05
            switch (_txb)
            {
                case 0x01:
                    UnifiedHostCommandSet.DUT1Lightsensor = _value.ToString();
                    Port0SDM1ADC = _value;
                    break;
                case 0x02:
                    UnifiedHostCommandSet.DUT2Lightsensor = _value.ToString();
                    Port0SDM2ADC = _value;
                    break;
                case 0x03:
                    UnifiedHostCommandSet.DUT3Lightsensor = _value.ToString();
                    Port0SDM3ADC = _value;
                    break;
            }
            OnPropertyChanged(nameof(Port0SDM1ADC));
            OnPropertyChanged(nameof(Port0SDM2ADC));
            OnPropertyChanged(nameof(Port0SDM3ADC));
        }

        public void evt_func_normalCurrent(byte _txb, double _value)
        {
            var curvalue = Math.Round(_value, 2, MidpointRounding.AwayFromZero);
            switch (_txb)
            {
                case 0x01:
                    UnifiedHostCommandSet.DUT1NormalCurrent = curvalue.ToString();
                    UnifiedHostCommandSet.DUT1SleepCurrent = "";
                    Port0SDM1normalCurrent = curvalue;
                    Port0SDM1sleepCurrent = 0;
                    OnPropertyChanged(nameof(Port0SDM1normalCurrent));
                    OnPropertyChanged(nameof(Port0SDM1sleepCurrent));
                    // 清空較早的Queue
                    if(IsEnableExecuteSDMchecklists)
                    {
                        UnifiedHostCommandSet.CheckoutCurlist(UnifiedHostCommandSet.DUT1CurrentList);
                    }                    
                    break;
                case 0x02:
                    UnifiedHostCommandSet.DUT2NormalCurrent = curvalue.ToString();
                    UnifiedHostCommandSet.DUT2SleepCurrent = "";
                    Port0SDM2normalCurrent = curvalue;
                    Port0SDM2sleepCurrent = 0;
                    OnPropertyChanged(nameof(Port0SDM2normalCurrent));
                    OnPropertyChanged(nameof(Port0SDM2sleepCurrent));
                    // 清空較早的Queue
                    if (IsEnableExecuteSDMchecklists)
                    {
                        UnifiedHostCommandSet.CheckoutCurlist(UnifiedHostCommandSet.DUT2CurrentList);
                    }
                    break;
                case 0x03:
                    UnifiedHostCommandSet.DUT3NormalCurrent = curvalue.ToString();
                    UnifiedHostCommandSet.DUT3SleepCurrent = "";
                    Port0SDM3normalCurrent = curvalue;
                    Port0SDM3sleepCurrent = 0;
                    OnPropertyChanged(nameof(Port0SDM3normalCurrent));
                    OnPropertyChanged(nameof(Port0SDM3sleepCurrent));
                    // 清空較早的Queue
                    if (IsEnableExecuteSDMchecklists)
                    {
                        UnifiedHostCommandSet.CheckoutCurlist(UnifiedHostCommandSet.DUT3CurrentList);
                    }
                    break;
            }

        }

        public void evt_func_sleepCurrent(byte _txb, double _value)
        {
            var curvalue = Math.Round(_value, 2, MidpointRounding.AwayFromZero);
            switch (_txb)
            {
                case 0x01:
                    UnifiedHostCommandSet.DUT1NormalCurrent = "";
                    UnifiedHostCommandSet.DUT1SleepCurrent = curvalue.ToString();
                    Port0SDM1normalCurrent = 0;
                    Port0SDM1sleepCurrent = curvalue;
                    OnPropertyChanged(nameof(Port0SDM1normalCurrent));
                    OnPropertyChanged(nameof(Port0SDM1sleepCurrent));
                    // 清空較早的Queue
                    if (IsEnableExecuteSDMchecklists)
                    {
                        UnifiedHostCommandSet.CheckoutCurlist(UnifiedHostCommandSet.DUT1CurrentList);
                    }
                    break;
                case 0x02:
                    UnifiedHostCommandSet.DUT2NormalCurrent = "";
                    UnifiedHostCommandSet.DUT2SleepCurrent = curvalue.ToString();
                    Port0SDM2normalCurrent = 0;
                    Port0SDM2sleepCurrent = curvalue;
                    OnPropertyChanged(nameof(Port0SDM2normalCurrent));
                    OnPropertyChanged(nameof(Port0SDM2sleepCurrent));
                    // 清空較早的Queue
                    if (IsEnableExecuteSDMchecklists)
                    {
                        UnifiedHostCommandSet.CheckoutCurlist(UnifiedHostCommandSet.DUT2CurrentList);
                    }
                    break;
                case 0x03:
                    UnifiedHostCommandSet.DUT3NormalCurrent = curvalue.ToString();
                    UnifiedHostCommandSet.DUT3SleepCurrent = "";
                    Port0SDM3normalCurrent = 0;
                    Port0SDM3sleepCurrent = curvalue;
                    OnPropertyChanged(nameof(Port0SDM3normalCurrent));
                    OnPropertyChanged(nameof(Port0SDM3sleepCurrent));
                    // 清空較早的Queue
                    if (IsEnableExecuteSDMchecklists)
                    {
                        UnifiedHostCommandSet.CheckoutCurlist(UnifiedHostCommandSet.DUT3CurrentList);
                    }
                    break;
            }
        }

        public void evt_func_Backlighbrightness(byte _txb, int _value)
        {
            switch (_txb)
            {
                case 0x01:
                    UnifiedHostCommandSet.DUT1Brightness = _value.ToString();
                    Port0SDM1Backlighbrightness = _value;
                    break;
                case 0x02:
                    UnifiedHostCommandSet.DUT2Brightness = _value.ToString();
                    Port0SDM2Backlighbrightness = _value;
                    break;
                case 0x03:
                    UnifiedHostCommandSet.DUT3Brightness = _value.ToString();
                    Port0SDM3Backlighbrightness = _value;
                    break;
            }
            OnPropertyChanged(nameof(Port0SDM1Backlighbrightness));
            OnPropertyChanged(nameof(Port0SDM2Backlighbrightness));
            OnPropertyChanged(nameof(Port0SDM3Backlighbrightness));
        }

        public void updateUIobj()
        {
            OnPropertyChanged(nameof(Port0SDM1HWversion));
            OnPropertyChanged(nameof(Port0SDM2HWversion));
            OnPropertyChanged(nameof(Port0SDM3HWversion));
            OnPropertyChanged(nameof(Port0SDM1SWversion));
            OnPropertyChanged(nameof(Port0SDM2SWversion));
            OnPropertyChanged(nameof(Port0SDM3SWversion));
            OnPropertyChanged(nameof(Port0SDM1DTC));
            OnPropertyChanged(nameof(Port0SDM2DTC));
            OnPropertyChanged(nameof(Port0SDM3DTC));
            OnPropertyChanged(nameof(Port0SDM1Voltage));
            OnPropertyChanged(nameof(Port0SDM2Voltage));
            OnPropertyChanged(nameof(Port0SDM3Voltage));
            OnPropertyChanged(nameof(Port0SDM1PCBTemp));
            OnPropertyChanged(nameof(Port0SDM2PCBTemp));
            OnPropertyChanged(nameof(Port0SDM3PCBTemp));
            OnPropertyChanged(nameof(Port0SDM1LED1Temp));
            OnPropertyChanged(nameof(Port0SDM2LED1Temp));
            OnPropertyChanged(nameof(Port0SDM3LED1Temp));
            OnPropertyChanged(nameof(Port0SDM1LED2Temp));
            OnPropertyChanged(nameof(Port0SDM2LED2Temp));
            OnPropertyChanged(nameof(Port0SDM3LED2Temp));
            OnPropertyChanged(nameof(Port0SDM1T_chamber));
            OnPropertyChanged(nameof(Port0SDM2T_chamber));
            OnPropertyChanged(nameof(Port0SDM3T_chamber));
            OnPropertyChanged(nameof(Port0SDM1ADC));
            OnPropertyChanged(nameof(Port0SDM2ADC));
            OnPropertyChanged(nameof(Port0SDM3ADC));
            OnPropertyChanged(nameof(Port0SDM1normalCurrent));
            OnPropertyChanged(nameof(Port0SDM2normalCurrent));
            OnPropertyChanged(nameof(Port0SDM3normalCurrent));
            OnPropertyChanged(nameof(Port0SDM1sleepCurrent));
            OnPropertyChanged(nameof(Port0SDM2sleepCurrent));
            OnPropertyChanged(nameof(Port0SDM3sleepCurrent));
            OnPropertyChanged(nameof(Port0SDM1Backlighbrightness));
            OnPropertyChanged(nameof(Port0SDM2Backlighbrightness));
            OnPropertyChanged(nameof(Port0SDM3Backlighbrightness));

        }
        #endregion
        #region SDM1
        public int Port0SDM1DTC { get; set; }
        public string Port0SDM1SWversion { get; set; } = "___";
        public string Port0SDM1HWversion { get; set; } = "___";
        public string Port0SDM1Status { get; set; } = "___";
        public double Port0SDM1Voltage { get; set; } = 0.0;
        public double Port0SDM1normalCurrent { get; set; } = 0.0;
        public double Port0SDM1sleepCurrent { get; set; } = 0.0;
        public double Port0SDM1Backlighbrightness { get; set; } = 0;
        public int Port0SDM1PCBTemp { get; set; } = 0;
        public int Port0SDM1LED1Temp { get; set; } = 0;
        public int Port0SDM1LED2Temp { get; set; } = 0;
        public int Port0SDM1T_chamber { get; set; } = 0;
        public double Port0SDM1ADC { get; set; } = 0;
        public string Port0SDM1Touchresponse { get; set; } = "___";
        #endregion

        #region SDM2

        public int Port0SDM2DTC { get; set; }
        public string Port0SDM2SWversion { get; set; } = "___";
        public string Port0SDM2HWversion { get; set; } = "___";
        public string Port0SDM2Status { get; set; } = "___";
        public double Port0SDM2Voltage { get; set; } = 0.0;
        public double Port0SDM2normalCurrent { get; set; } = 0.0;
        public double Port0SDM2sleepCurrent { get; set; } = 0.0;
        public int Port0SDM2Backlighbrightness { get; set; } = 0;
        public int Port0SDM2PCBTemp { get; set; } = 0;
        public int Port0SDM2LED1Temp { get; set; } = 0;
        public int Port0SDM2LED2Temp { get; set; } = 0;
        public int Port0SDM2T_chamber { get; set; } = 0;
        public double Port0SDM2ADC { get; set; } = 0;
        public string Port0SDM2Touchresponse { get; set; } = "___";
        #endregion

        #region SDM3
        public int Port0SDM3DTC { get; set; }
        public string Port0SDM3SWversion { get; set; } = "___";
        public string Port0SDM3HWversion { get; set; } = "___";
        public string Port0SDM3Status { get; set; } = "___";
        public double Port0SDM3Voltage { get; set; } = 0.0;
        public double Port0SDM3normalCurrent { get; set; } = 0.0;
        public double Port0SDM3sleepCurrent { get; set; } = 0.0;
        public int Port0SDM3Backlighbrightness { get; set; } = 0;
        public int Port0SDM3PCBTemp { get; set; } = 0;
        public int Port0SDM3LED1Temp { get; set; } = 0;
        public int Port0SDM3LED2Temp { get; set; } = 0;
        public int Port0SDM3T_chamber { get; set; } = 0;
        public double Port0SDM3ADC { get; set; } = 0;
        public string Port0SDM3Touchresponse { get; set; } = "___";
        #endregion
    }
}
