using SuperCarter.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static SuperCarter.ViewModel.SuperCarterViewModel;
using MaterialDesignThemes;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.IO;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Documents;
using GongSolutions.Wpf.DragDrop;
using System.Windows.Shapes;
using System.Xml;
using Newtonsoft.Json.Linq;
using SuperCarter.Services;
using SuperCarter.Model;
using SuperCarter.Services;
using SuperCarter.ViewModel;
using SuperCarter;
using System.Windows.Interop;

namespace SuperCarter.Model
{


    public class CustomscriptExecution : ViewModelBase
    {
        public ObservableCollection<Foldertype> folderViewerlist { get; set; } = new ObservableCollection<Foldertype>();
        private ObservableCollection<IFiletype> _blockAfolderViewerlist;
        //public SerialPortViewModelBase serialPortViewModelBase { get; set; }
        public int CommnadID { get; set; } = 0;
        public CSVfile cSVfile { get; set; }

        /// <summary>
        /// 1: Normal mode 0: Sleep mode
        /// </summary>
        public int PowerMode { get; set; }
        public CustomscriptExecution()
        {


        }
        ~CustomscriptExecution()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public int MonitoringIntervaltime { get; set; } = 3000;
        public static List<SendorExecuteSendType> BlockA1SequencesList { get; set; } = new List<SendorExecuteSendType>();
        public static List<SendorExecuteSendType> BlockA2SequencesList { get; set; } = new List<SendorExecuteSendType>();
        public static List<SendorExecuteSendType> BlockB1SequencesList { get; set; } = new List<SendorExecuteSendType>();
        public static List<SendorExecuteSendType> BlockB2SequencesList { get; set; } = new List<SendorExecuteSendType>();

        #region property
        private int _ExecuteFullloop, _BlockALoop, _BlockBLoop, _ExecuteBlockCLoop, _ExecuteBlockDLoop;
        private double _Estimateruntimefullblock, _EstimateruntimeforblockA, _EstimateruntimeforblockB, _EstimateruntimeforblockC, _EstimateruntimeforblockD;

        public string blockAscriptpath { get; set; }
        public string blockBscriptpath { get; set; }
        public string blockCscriptpath { get; set; }
        public string blockDscriptpath { get; set; }
      
        public int BlockALoop 
        {
            get => _BlockALoop;
            set { 
                _BlockALoop = value;
                OnPropertyChanged(nameof(EstimateBlockAtotaltime));
            }
        
        }
        private int _EstimateBlockAtotaltime = 0;
        public int EstimateBlockAtotaltime 
        { 
            get {
                _EstimateBlockAtotaltime = (BlockA1Interval + BlockA2Interval) * BlockALoop;
           
                OnPropertyChanged(nameof(EstimateBlockAtotaltime));
                return _EstimateBlockAtotaltime;
            }
            
        }
        public int BlockBLoop { get; set; }
        public int ExecuteBlockCLoop { get; set; }
        public int ExecuteBlockDLoop { get; set; }


        private int _BlockA1Interval, _subloop, _BlockA2Interval, _blockCscriptDelaytime, _blockDscriptDelaytime;
        public int blockAcurloop { get; set; }
        public int blockBcurloop { get; set; }
        public int blockCcurloop { get; set; }
        public int blockDcurloop { get; set; }
        public int BlockA1Interval { get; set; }
        public int BlockA2Interval { get; set; }
        public int blockCscriptDelaytime { get; set; }
        public int blockDscriptDelaytime { get; set; }
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
        public int subloop
        {
            get => _subloop;
            set
            {
                _subloop = value;
                OnPropertyChanged(nameof(subloop));
            }
        }
        public int Fullloop
        {
            get => _ExecuteFullloop;
            set
            {
                _ExecuteFullloop = value < 1 ? 1 : value;
                OnPropertyChanged(nameof(Fullloop));
            }
        }

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
        public int PercentLoopValue { get; set; }
        private readonly byte[] RESPONSE_TAIL = { 0x0D, 0x0A };
        #endregion

        #region icommand properties


        #endregion

        #region  events
        public void evt_ReleasetoRefresh()
        {
            OnPropertyChanged(nameof(PercentLoopValue));
            OnPropertyChanged(nameof(CurLoopValue));
            OnPropertyChanged(nameof(PercentLoopValue));
            OnPropertyChanged(nameof(Fullloop));
            OnPropertyChanged(nameof(BlockA1Interval));
            OnPropertyChanged(nameof(BlockA2Interval));
            OnPropertyChanged(nameof(blockCscriptDelaytime));
            OnPropertyChanged(nameof(blockDscriptDelaytime));
            OnPropertyChanged(nameof(BlockALoop));
            OnPropertyChanged(nameof(BlockBLoop));
            OnPropertyChanged(nameof(ExecuteBlockCLoop));
            OnPropertyChanged(nameof(ExecuteBlockDLoop));
            OnPropertyChanged(nameof(blockAscriptpath));
            OnPropertyChanged(nameof(blockBscriptpath));
            OnPropertyChanged(nameof(blockCscriptpath));
            OnPropertyChanged(nameof(blockDscriptpath));
        }

        #endregion


        public async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.Log(NLog.LogLevel.Trace, "– Stop Custom script schedule.");
            RealtimeMsgQueue.Enqueue(new RealtimeMsgQueuetype { msgtype = Msgtype.Message,  msg = "– Stop Custom script schedule." });
            await StopAsync(cancellationToken);
        }
        private UnifiedHostCommandSettype UnifiedHostCommandSet = new UnifiedHostCommandSettype();


        public async Task StartAsync(CancellationToken cancellationToken)
        {
           
            cSVfile = new CSVfile(); // 請替換為您希望保存文件的路徑

            UnifiedHostCommandSet = new UnifiedHostCommandSettype();


            /***         
            *  in  
            * ****/
            CommnadID = 0;

            logger.Log(NLog.LogLevel.Trace, "– Start Custom script schedule.");
            RealtimeMsgQueue.Enqueue(new RealtimeMsgQueuetype { msgtype = Msgtype.Message, msg = "– Start Custom script schedule." });
            for (CurLoopValue = 0; CurLoopValue < Fullloop; CurLoopValue++) // 總迴圈
            {
                var msg = string.Format("- Currently on iteration : {0}", CurLoopValue);
                logger.Log(NLog.LogLevel.Trace, msg);
          
                RealtimeMsgQueue.Enqueue(new RealtimeMsgQueuetype { msgtype = Msgtype.Message, msg = msg });
                // 如果token已被取消，跳出迴圈
                if (cancellationToken.IsCancellationRequested)
                    break;
                OnPropertyChanged(nameof(Fullloop));
                OnPropertyChanged(nameof(CurLoopValue));
                OnPropertyChanged(nameof(PercentLoopValue));
                await BlockWorkstation("A", BlockA1SequencesList, BlockA2SequencesList, BlockALoop, BlockA1Interval, BlockA2Interval, cancellationToken);
                //await BlockWorkstation("B", BlockA2SequencesList, BlockBLoop, BlockA2Interval, cancellationToken);
                await BlockWorkstation("C", BlockB1SequencesList, BlockB2SequencesList, ExecuteBlockCLoop, blockCscriptDelaytime, blockDscriptDelaytime, cancellationToken);
                //await BlockWorkstation("D", BlockB2SequencesList, ExecuteBlockDLoop, blockDscriptDelaytime, cancellationToken);

            }
            ObjectAggregator.Instance.UpdateObject();

            OnPropertyChanged(nameof(Fullloop));
            OnPropertyChanged(nameof(CurLoopValue));
            OnPropertyChanged(nameof(PercentLoopValue));

            logger.Log(NLog.LogLevel.Trace, "– End Custom script schedule.");
            RealtimeMsgQueue.Enqueue(new RealtimeMsgQueuetype { msgtype = Msgtype.Message,  msg = "– End Custom script schedule." });

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
        

            for (int curLoop = 0; curLoop < maxLoop; curLoop++)
            {
                subloop = curLoop;
          
                // 
                var blockStartTime = DateTime.Now;

                // Start the block working time loop
                // Execute sequences1 within cycletime
                var seq1Task = ExecuteBlockSequences($"{blockName}_{1}", sequences1, blockA1cycletime, curLoop, cancellationToken, MonitoringIntervaltime);
                await seq1Task; // Wait for seq1Task to complete

                // Execute sequences2 within cycletime
                var seq2Task = ExecuteBlockSequences($"{blockName}_{2}", sequences2, blockA2cycletime, curLoop, cancellationToken, MonitoringIntervaltime);
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
        private async Task ExecuteBlockSequences(string blockName, List<SendorExecuteSendType> sequences, int scriptDelayTime, int curLoop, CancellationToken cancellationToken, int cycletime)
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
                        //cancellationToken.ThrowIfCancellationRequested();
                        if ( cancellationToken.IsCancellationRequested)
                        {
                            UnifiedHostCommandSet.Time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ffff");
                            UnifiedHostCommandSet.Loop = CurLoopValue.ToString();
                            UnifiedHostCommandSet.Blockphase = Curphase.ToString();
                            UnifiedHostCommandSet.Blockloop = curLoop.ToString();
                            cSVfile.AppendToCsv(UnifiedHostCommandSet);
                            UnifiedHostCommandSet = new UnifiedHostCommandSettype();


                            break; // Exit the loop
                        }

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

                            UnifiedHostCommandSet.Time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ffff");
                            UnifiedHostCommandSet.Loop = CurLoopValue.ToString();
                            UnifiedHostCommandSet.Blockphase = Curphase.ToString();
                            UnifiedHostCommandSet.Blockloop = curLoop.ToString();
                            cSVfile.AppendToCsv(UnifiedHostCommandSet);
                            UnifiedHostCommandSet = new UnifiedHostCommandSettype();
                        }
                    }
             
                }
            }
            catch (TaskCanceledException)
            {
                WorkingBreak = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                cancellationTokenSource.Dispose();
            }

            // Rest of your code...
        }








        //private async Task ExecuteBlockSequences(string blockName, List<SendorExecuteSendType> sequences, int scriptDelayTime, CancellationToken cancellationToken, int cycletime)
        //{
        //    cancellationToken.ThrowIfCancellationRequested();
        //    bool WorkingBreak = false;

        //    try
        //    {
        //        var stopwatch = new Stopwatch();
        //        stopwatch.Start();
        //        var msg1 = $"- Currently on {blockName} iteration: {blockName}";
        //        logger.Log(NLog.LogLevel.Trace, msg1);

        //        var blockStartTime = DateTime.Now;
        //        int sequenceIndex = 0;

        //        bool isWaiting = false;

        //        // Start the block working time loop for this block

        //        while ((DateTime.Now - blockStartTime).TotalMilliseconds < scriptDelayTime)
        //        {

        //            //if (cancellationToken.IsCancellationRequested || sequenceIndex >= sequences.Count)
        //            if (cancellationToken.IsCancellationRequested )
        //            {
        //                break; // Exit the loop
        //            }

        //            if (sequences[sequenceIndex].SendMsgdata == "FA 57 00 02 01" || sequences[sequenceIndex].SendMsgdata == "FA 57 00 02 00")
        //            {
        //                isWaiting = true; // Set the flag to indicate the need to wait for this command
        //            }

        //            if (sequences[sequenceIndex].PortNum == 9)
        //            {
        //                for (int j = 0; j < 3; j++)
        //                {
        //                    sequenceIndex++;
        //                    if (sequenceIndex >= sequences.Count || cancellationToken.IsCancellationRequested)
        //                    {
        //                        break; // Exit the loop
        //                    }
        //                    await SendAndReceivesAsync(sequences[sequenceIndex], cancellationToken);
        //                }
        //            }
        //            else
        //            {
        //                await SendAndReceivesAsync(sequences[sequenceIndex], cancellationToken);
        //                sequenceIndex++;
        //            }

        //            if (isWaiting)
        //            {
        //                var elapsedTime = (DateTime.Now - blockStartTime).TotalMilliseconds;
        //                if (elapsedTime < cycletime)
        //                {
        //                    // Calculate the time spent in the block
        //                    var timeSpentInBlock = (int)elapsedTime;

        //                    // Calculate the remaining time based on cycletime
        //                    var remainingTime = cycletime - timeSpentInBlock;

        //                    if (remainingTime > 0)
        //                    {
        //                        await Task.Delay(remainingTime, cancellationToken);
        //                    }
        //                }

        //                isWaiting = false; // Reset the flag after waiting
        //            }
        //        }
        //    }
        //    catch (TaskCanceledException)
        //    {
        //        WorkingBreak = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }

        //    if (WorkingBreak)
        //    {
        //        logger.Log(NLog.LogLevel.Trace, $"– the runtime was canceled in {Curphase}.");
        //        RealtimeMsgQueue.Enqueue($"– the runtime was canceled in {Curphase}.");
        //    }
        //    else
        //    {
        //        UnifiedHostCommandSet.Time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ffff");
        //        UnifiedHostCommandSet.Loop = CurLoopValue.ToString();
        //        UnifiedHostCommandSet.Blockphase = Curphase.ToString();
        //        UnifiedHostCommandSet.Blockloop = blockName.ToString();
        //        cSVfile.AppendToCsv(UnifiedHostCommandSet);
        //        UnifiedHostCommandSet = new UnifiedHostCommandSettype();
        //    }
        //}


        //private async Task ExecuteBlockSequences(string blockName, List<SendorExecuteSendType> sequences, int scriptDelayTime, CancellationToken cancellationToken)
        //{

        //    cancellationToken.ThrowIfCancellationRequested();
        //    bool WorkingBreak = false;

        //    try
        //    {
        //        var stopwatch = new Stopwatch();
        //        stopwatch.Start();
        //        var Sendorwatch = new Stopwatch();

        //        var msg1 = $"- Currently on {blockName} iteration: {blockName}";
        //        logger.Log(NLog.LogLevel.Trace, msg1);


        //        for (int i = 0; i < sequences.Count; i++)
        //        {
        //            if (cancellationToken.IsCancellationRequested) break;

        //            if (sequences[i].PortNum == 9)
        //            {
        //                var roundtimedelay = sequences[i].Delaytime;

        //                Sendorwatch.Restart();
        //                for (int j = 0; j < 3; j++)
        //                {
        //                    i++;
        //                    await SendAndReceivesAsync(sequences[i], cancellationToken);
        //                }

        //                Sendorwatch.Stop();
        //                var remainingSpentTime = roundtimedelay - (int)Sendorwatch.ElapsedMilliseconds;

        //                if (remainingSpentTime > 0)
        //                {
        //                    await Task.Delay(remainingSpentTime, cancellationToken);
        //                }
        //            }
        //            else
        //            {
        //                var roundtimedelay = sequences[i].Delaytime;
        //                Sendorwatch.Restart();
        //                await SendAndReceivesAsync(sequences[i], cancellationToken);
        //                Sendorwatch.Stop();
        //                var remainingSpentTime = roundtimedelay - (int)Sendorwatch.ElapsedMilliseconds;

        //                if (remainingSpentTime > 0)
        //                {
        //                    await Task.Delay(remainingSpentTime, cancellationToken);
        //                }
        //            }
        //        }
        //        await Task.Delay(3000, cancellationToken);

        //        stopwatch.Stop();
        //        var remainingTime = TimeSpan.FromMilliseconds(scriptDelayTime) - stopwatch.Elapsed;
        //        cancellationToken.ThrowIfCancellationRequested();
        //        if (remainingTime > TimeSpan.Zero)
        //        {
        //            await Task.Delay(remainingTime, cancellationToken);
        //        }

        //    }
        //    catch (TaskCanceledException)
        //    {
        //        WorkingBreak = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }

        //    if (WorkingBreak)
        //    {
        //        logger.Log(NLog.LogLevel.Trace, $"– the runtime was canceled in {Curphase}.");
        //        RealtimeMsgQueue.Enqueue($"– the runtime was canceled in {Curphase}.");
        //    }
        //    else
        //    {
        //        UnifiedHostCommandSet.Time = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ffff");
        //        UnifiedHostCommandSet.Loop = CurLoopValue.ToString();
        //        UnifiedHostCommandSet.Blockphase = Curphase.ToString();
        //        UnifiedHostCommandSet.Blockloop = blockName.ToString();
        //        cSVfile.AppendToCsv(UnifiedHostCommandSet);
        //        UnifiedHostCommandSet = new UnifiedHostCommandSettype();
        //    }

        //}
        private string sendmsg, recmsg;
        /// <summary>
        /// 發送訊號至待測物
        /// </summary>
        /// <param name="Sequence"></param>
        /// <returns></returns>
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
            RealtimeMsgQueue.Enqueue(new RealtimeMsgQueuetype { msgtype = Msgtype.FromPort, PortNum = command.PortNum , msg = OutputMsg });

            DicSerialPort[command.PortNum].Write(command.SequenceData, 0, command.SequenceData.Length);

            // 紀錄讀取 sdm 回傳的資料
            receivedData = await ReadFromPortAsync6(Sequence, cancellationToken);
            // update to Dashboard
            if (receivedData.Length > 0)
                RealtimeSDMDataQueue.Enqueue(receivedData);
            // update to nlog file
            recmsg = SerialPortModel.Instance.byteToHexStr(receivedData);

            OutputMsg = String.Format("{0}|{1}|{2}| R | CommandID {4}| {3}",
                DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ffff"),
                SerialPortModel.Instance.PortNameBinding[DicSerialPort[command.PortNum].PortName].ToString().PadLeft(2, ' '),
                DicSerialPort[command.PortNum].PortName.PadLeft(6, ' '),
                recmsg.Replace(" ", ""),
              CommnadID.ToString().PadLeft(6, ' ')
                );
            logger.Log(NLog.LogLevel.Trace, OutputMsg);
            RealtimeMsgQueue.Enqueue(new RealtimeMsgQueuetype { msgtype = Msgtype.FromPort, PortNum = command.PortNum, msg = OutputMsg });

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

        private async Task<byte[]> ReadFromPortAsync6(object Sequence, CancellationToken cancellationToken)
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
                    if (DicSerialPort[0].BytesToRead > 0)
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
        /// <summary>
        /// 透過路徑載入相關的變數
        /// </summary>
        /// <param name="ScriptPath"> 此為腳本路徑 </param>
        public void LoadBlockScript(string ScriptPath)
        {
            try
            {
                // Loading from a file, you can also load from a stream

                XmlDocument ScriptionXML = new XmlDocument();

                ScriptionXML.Load(ScriptPath);

                List<string> list = new List<string>() { "TestSuiteA", "TestSuiteB", "TestSuiteC", "TestSuiteD" };

                XmlNode root = ScriptionXML.SelectSingleNode("TestSuites");

                Fullloop = Convert.ToInt32(root.Attributes["Fullloop"]?.Value);

                foreach (var ite in list)
                {
                    string strblock = "TestSuites/" + ite + "/TestSequence";
                    XmlNode block = ScriptionXML.SelectSingleNode(strblock);
                    string strrequisites = "TestSuites/" + ite + "/Prerequisites";
                    XmlNode requisites = ScriptionXML.SelectSingleNode(strrequisites);

                    XmlElement element = (XmlElement)block;
                    ObservableCollection<ScriptItemtype> obsTemp = new ObservableCollection<ScriptItemtype>();
                    List<SendorExecuteSendType> Temp = new List<SendorExecuteSendType>();
                    //取得節點內的欄位
                    foreach (XmlElement node in element)
                    {
                        String ID = node.Attributes["ID"].Value ?? "";
                        String PortNum = node.Attributes["PortNum"]?.Value ?? "all";
                        String Nodename = node.Attributes["Nodename"]?.Value ?? "";
                        String MSGname = node.Attributes["MSGname"]?.Value ?? "";
                        String Sequence = node.Attributes["Sequence"]?.Value ?? "";
                        String Delaytime = node.Attributes["Delaytime"]?.Value ?? "200";
                        String RecSequence = node.Attributes["RecSequence"]?.Value ?? "";
                        String HashCodevalue = node.Attributes["HashCodevalue"]?.Value ?? "";
                        String Loop = node.Attributes["Loop"]?.Value ?? "1";

                        if (PortNum == "all")
                        {
                            Temp.Add(new SendorExecuteSendType()
                            {
                                PortNum = 9,
                                SequenceData = new byte[0],
                                intDataLen = 0,
                                strDataLen = "0",
                                Delaytime = Convert.ToInt32(Delaytime),
                                Loop = 1,
                                SendMsgdata = String.Format("ID:{0}|Port:{1}|S| {2}",
                               ID.PadLeft(3, ' '),
                                  9,
                                  "delay time")
                            });
                            Temp.Add(new SendorExecuteSendType()
                            {
                                PortNum = 0,
                                SequenceData = SerialPortModel.Instance.HexStrToByte(Sequence),
                                strSequenceData = Sequence,
                                intDataLen = SerialPortModel.Instance.HexStrToByte(Sequence).Length,
                                strDataLen = SerialPortModel.Instance.HexStrToByte(Sequence).Length.ToString(),
                                Delaytime = 0,
                                Loop = Convert.ToInt32(Loop),
                                SendMsgdata = String.Format("ID:{0}|Port:{1}|S| {2}",
                                                          ID.PadLeft(3, ' '),
                                                            0,
                                                            Sequence.Replace(" ", ""))
                            });
                            Temp.Add(new SendorExecuteSendType()
                            {
                                PortNum = 1,
                                SequenceData = SerialPortModel.Instance.HexStrToByte(Sequence),
                                strSequenceData = Sequence,
                                intDataLen = SerialPortModel.Instance.HexStrToByte(Sequence).Length,
                                strDataLen = SerialPortModel.Instance.HexStrToByte(Sequence).Length.ToString(),
                                Delaytime = 0,
                                Loop = Convert.ToInt32(Loop),
                                SendMsgdata = String.Format("ID:{0}|Port:{1}|S| {2}",
                                                        ID.PadLeft(3, ' '),
                                                            1,
                                                            Sequence.Replace(" ", ""))
                            });
                            Temp.Add(new SendorExecuteSendType()
                            {
                                PortNum = 2,
                                SequenceData = SerialPortModel.Instance.HexStrToByte(Sequence),
                                strSequenceData = Sequence,
                                intDataLen = SerialPortModel.Instance.HexStrToByte(Sequence).Length,
                                strDataLen = SerialPortModel.Instance.HexStrToByte(Sequence).Length.ToString(),
                                Delaytime = 0,
                                Loop = Convert.ToInt32(Loop),
                                SendMsgdata = String.Format("ID:{0}|Port:{1}|S| {2}",
                                                         ID.PadLeft(3, ' '),
                                                            2,
                                                            Sequence.Replace(" ", ""))
                            });

                        }
                        else
                        {
                            Temp.Add(new SendorExecuteSendType()
                            {
                                PortNum = Convert.ToInt32(PortNum),
                                strSequenceData = Sequence,
                                intDataLen = SerialPortModel.Instance.HexStrToByte(Sequence).Length,
                                strDataLen = SerialPortModel.Instance.HexStrToByte(Sequence).Length.ToString(),
                                Delaytime = Convert.ToInt32(Delaytime),
                                Loop = Convert.ToInt32(Loop),
                                SendMsgdata = String.Format("ID:{0}|Port:{1}|S| {2}",
                                                       ID.PadLeft(3, ' '),
                                                            PortNum,
                                                            Sequence.Replace(" ", ""))
                            });
                        }

                    }
                    if (ite == "TestSuiteA")
                    {
                        //BlockALoop = Convert.ToInt32(block.Attributes["IterValue"]?.Value);
                        BlockA1Interval = Convert.ToInt32(block.Attributes["TotalTime"]?.Value) + 3000;
                        blockAscriptpath = requisites.Attributes["Path"]?.Value ?? "";
                        BlockA1SequencesList = Temp;


                    }
                    else if (ite == "TestSuiteB")
                    {
                        //BlockBLoop = Convert.ToInt32(block.Attributes["IterValue"]?.Value);
                        BlockA2Interval = Convert.ToInt32(block.Attributes["TotalTime"]?.Value) + 3000;
                        blockBscriptpath = requisites.Attributes["Path"]?.Value ?? "";
                        BlockA2SequencesList = Temp;
                    }
                    else if (ite == "TestSuiteC")
                    {
                        //ExecuteBlockCLoop = Convert.ToInt32(block.Attributes["IterValue"]?.Value);
                        blockCscriptDelaytime = Convert.ToInt32(block.Attributes["TotalTime"]?.Value) + 3000;
                        blockCscriptpath = requisites.Attributes["Path"]?.Value ?? "";

                        BlockB1SequencesList = Temp;
                    }
                    else if (ite == "TestSuiteD")
                    {
                        //ExecuteBlockDLoop = Convert.ToInt32(block.Attributes["IterValue"]?.Value);
                        blockDscriptDelaytime = Convert.ToInt32(block.Attributes["TotalTime"]?.Value) + 3000;
                        blockDscriptpath = requisites.Attributes["Path"]?.Value ?? "";
                        BlockB2SequencesList = Temp;
                    }
                }

                //return _ScriptEditor;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //return new ScriptEditor();
            }
        }


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

                            MessageBox.Show(ex.StackTrace);
                            MessageBox.Show(ex.Message);
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

                //App.Current.Dispatcher.Invoke(() =>
                //{
                //    serialPortViewModelBase.updateUIobj();
                //});
            }
        }
        private void HandleBufferSend_00(byte sendByte4, SendAndReceiveDatabatchcheck bytes)
        {
            if (sendByte4 == 0x01)
            {
                string msg = string.Format("{0}.{1}", bytes.byte_buffer_Receive[3].ToString(), bytes.byte_buffer_Receive[4].ToString());
                evt_func_SWversion(bytes.byte_buffer_Receive[1], msg);
            }
            else if (sendByte4 == 0x02)
            {
                string msg = string.Format("{0}.{1}", bytes.byte_buffer_Receive[3].ToString(), bytes.byte_buffer_Receive[4].ToString());
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
        #region Assert func to variable
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
            switch (_txb)
            {
                case 0x01:
                    UnifiedHostCommandSet.DUT1NormalCurrent = _value.ToString();
                    UnifiedHostCommandSet.DUT1SleepCurrent = "";
                    Port0SDM1normalCurrent = _value;
                    Port0SDM1sleepCurrent = 0;
                    OnPropertyChanged(nameof(Port0SDM1normalCurrent));
                    OnPropertyChanged(nameof(Port0SDM1sleepCurrent));
                    break;
                case 0x02:
                    UnifiedHostCommandSet.DUT2NormalCurrent = _value.ToString();
                    UnifiedHostCommandSet.DUT2SleepCurrent = "";
                    Port0SDM2normalCurrent = _value;
                    Port0SDM2sleepCurrent = 0;
                    OnPropertyChanged(nameof(Port0SDM2normalCurrent));
                    OnPropertyChanged(nameof(Port0SDM2sleepCurrent));
                    break;
                case 0x03:
                    UnifiedHostCommandSet.DUT3NormalCurrent = _value.ToString();
                    UnifiedHostCommandSet.DUT3SleepCurrent = "";
                    Port0SDM3normalCurrent = _value;
                    Port0SDM3sleepCurrent = 0;
                    OnPropertyChanged(nameof(Port0SDM3normalCurrent));
                    OnPropertyChanged(nameof(Port0SDM3sleepCurrent));
                    break;
            }

        }
        public void evt_func_sleepCurrent(byte _txb, double _value)
        {
            switch (_txb)
            {
                case 0x01:
                    UnifiedHostCommandSet.DUT1NormalCurrent = "";
                    UnifiedHostCommandSet.DUT1SleepCurrent = _value.ToString();
                    Port0SDM1normalCurrent = 0;
                    Port0SDM1sleepCurrent = _value;

                    OnPropertyChanged(nameof(Port0SDM1normalCurrent));
                    OnPropertyChanged(nameof(Port0SDM1sleepCurrent));
                    break;
                case 0x02:
                    UnifiedHostCommandSet.DUT2NormalCurrent = "";
                    UnifiedHostCommandSet.DUT2SleepCurrent = _value.ToString();
                    Port0SDM2normalCurrent = 0;
                    Port0SDM2sleepCurrent = _value;
                    OnPropertyChanged(nameof(Port0SDM2normalCurrent));
                    OnPropertyChanged(nameof(Port0SDM2sleepCurrent));
                    break;
                case 0x03:
                    UnifiedHostCommandSet.DUT3NormalCurrent = _value.ToString();
                    UnifiedHostCommandSet.DUT3SleepCurrent = "";
                    Port0SDM3normalCurrent = 0;
                    Port0SDM3sleepCurrent = _value;
                    OnPropertyChanged(nameof(Port0SDM3normalCurrent));
                    OnPropertyChanged(nameof(Port0SDM3sleepCurrent));
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

        #endregion
        private static object _lock = new object();
        private string _path;

    }

}
