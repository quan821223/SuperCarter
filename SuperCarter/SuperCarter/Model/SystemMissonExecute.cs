using SuperCarter.Services;
using SuperCarter.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SuperCarter.Model
{
    public class SystemMissonExecute : ViewModelBase
    {
        public delegate void RealtimeMessageDelegate(int Portid, string msg);
        public static event RealtimeMessageDelegate OnRaisePopNotification;
        public void StartThread()
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
                while (RealtimeMsgQueue.TryDequeue(out RealtimeMsgQueuetype data))
                {
                    if (data != null)
                    {
                        try
                        {
                            ProcessBytes(data);
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
        private void ProcessBytes(RealtimeMsgQueuetype data)
        {      // 更新資訊的代碼
            App.Current.Dispatcher.BeginInvoke(() =>
            {
                // 更新資訊的具體實現               
                WritedataToViewTextAggregator.Instance.Updatemsg( data);

            });
        }

    }
}
