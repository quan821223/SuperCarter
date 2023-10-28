using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperCarter.Services;
using SuperCarter.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop;
using System.Windows;

namespace SuperCarter.ViewModel
{
    public class SuperCarterViewModel : ViewModelBase
    {
        public ObservableCollection<Comporttype> PortTree { get; set; } = new ObservableCollection<Comporttype>();
        public ScriptEditor scriptEditor { get; set; }
  
        public int SelectSerialportIndex { get; set; }

        public SuperCarterViewModel() {

            // 允許定位該事件並且供給給其他視圖使用
            MessageAggregator.Instance.Subscribe(evt_promptlyMessageNotify);
            ConfigModel.OnRaisePopNotification += evt_promptlyMessageNotify;
            nlogMessageAggregator.Instance.Subscribe(evt_popNlogMessageNotify);

            scriptEditor = new ScriptEditor();



            PortTree.Add(new Comporttype
            {
                FriendlyName = "COM",
                Children = SerialPortModel.Instance.GetComPorts()

            });
            SerialCandidator = ConfigbyJSON.Instance.ReadPortCandidatelist();
            OnPropertyChanged(nameof(PortTree));
        }
        ~SuperCarterViewModel()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region Serial port setting 

        private ICommand _SelectItemChangeCommand;
        public ICommand SelectItemChangeCommand
        {
            get
            {
                _SelectItemChangeCommand = new RelayCommand(
                    param => evt_SelectedSerialport((Portdetectedtype)param));
                return _SelectItemChangeCommand;
            }
        }

        // This is public get-only here but you could implement a public setter which
        // also selects the item.
        // Also this should be moved to an instance property on a VM for the whole tree, 
        // otherwise there will be conflicts for more than one tree.
        private static object _selectedItem = null;
        public static object SelectedItem
        {
            get { return _selectedItem; }
            private set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                }
            }
        }
        private void evt_SelectedSerialport(Portdetectedtype _va)
        {
            SelectedItem = this;
            bool IsAllowAddingitem = false;

            if (SerialCandidator.Count < 3)
            {
                if (SerialCandidator.Count < 1)
                {
                    IsAllowAddingitem = true;
                }
                else
                {
                    foreach (var _item in SerialCandidator)
                    {
                        if (!_item.PortName.Contains(_va.PortName))
                        {
                            IsAllowAddingitem = true;
                            logger.Log(NLog.LogLevel.Debug, "catch_" + _va.PortName);
                        }
                        else
                        {
                            IsAllowAddingitem = false;
                            logger.Log(NLog.LogLevel.Debug, "catch_" + _va.PortName + " be denied");
                            MessageBox.Show("無法新增此物件，comport 已被取用", "Information !", MessageBoxButton.OK, MessageBoxImage.Information);
                            break;
                        }
                    }
                }
                if (IsAllowAddingitem)
                {
                    try
                    {
                        SerialCandidator.Add(new Portdetectedtype()
                        {
                            IsUse = false,
                            PortID = 0,
                            FullPortName = _va.FullPortName,
                            PortName = _va.PortName,
                            BaudRateValue = 115200,
                            DataReceivedCasenum = 0
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("無法新增此物件，目前數量 已達上限", "Warning !", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            OnPropertyChangedForStatic(nameof(SerialCandidator));

        }
        public void AutoSaveStatus()
        {
            ConfigbyJSON.Instance.WritePortCandidatelist(SerialCandidator);
        }
        #endregion


        /// <summary>
        /// that is a pop notify message  
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        /// <param name="_NotificationType"></param>      
        /// 
        public void evt_promptlyMessageNotify(POPNotifyMsgType _popmsg)
        {
            var content = new NotificationContent { Title = _popmsg.Tital, Message = _popmsg.Message, Type = _popmsg.NotifyType };

            var clickContent = new NotificationContent
            {
                Title = "DQA notification msg！💛",
                Message = _popmsg.Tital + _popmsg.Message,
                Type = _popmsg.NotifyType
            };
            _notificationManager.Show(content, "WindowArea", onClick: () => _notificationManager.Show(clickContent));

        }
        private readonly NotificationManager _notificationManager = new NotificationManager();

        public void evt_popNlogMessageNotify(nlogtype _popmsg)
        {
            logger.Log(_popmsg.LogLevel, _popmsg.Msg);
        }

    }
}
