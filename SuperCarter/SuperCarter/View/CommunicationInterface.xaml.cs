using SuperCarter.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SuperCarter.View
{
    /// <summary>
    /// CommunicationInterface.xaml 的互動邏輯
    /// </summary>
    public partial class CommunicationInterface : Window
    {
        public CommunicationInterface()
        {
            InitializeComponent();
        }
        private void SerialPortCandidateDatagrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            //if (SerialPortCandidateDatagrid.ItemsSource is not null)
            //    CollectionViewSource.GetDefaultView(SerialPortCandidateDatagrid.ItemsSource).Refresh();
            //SelectSerialportIndex = SerialPortCandidateDatagrid.SelectedIndex;
            (this.DataContext as SuperCarterViewModel).serialportmanager.SelectSerialportIndex = SerialPortCandidateDatagrid.SelectedIndex;
        }
        private void SavePortsetting_Click(object sender, RoutedEventArgs e)
        {
            SerialPortCandidateDatagrid.CommitEdit();
        }

        private void treeView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
      
        }
    }
}
