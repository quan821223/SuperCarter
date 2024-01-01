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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SuperCarter.View
{
    /// <summary>
    /// Applicationtable.xaml 的互動邏輯
    /// </summary>
    public partial class Applicationtable : UserControl
    {
        public Applicationtable()
        {
            InitializeComponent();
        }
        private void bt_OpenISPapp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var exeFilePath = @".\src\ISP\ISP.exe";
                string arg = null;

                System.Diagnostics.Process diagAction = new System.Diagnostics.Process();
                diagAction.StartInfo.FileName = exeFilePath;
                diagAction.StartInfo.Arguments = arg;
                diagAction.StartInfo.CreateNoWindow = true;
                diagAction.StartInfo.UseShellExecute = false;
                diagAction.Start();
                // diagAction.WaitForExit();//關鍵，等待外部程式退出後才能往下執行
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }
        }

        private void bt_OpenTouchpadapp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var exeFilePath = @".\src\Touchpad\TouchScratchpadMaster.exe";
                string arg = null;

                System.Diagnostics.Process diagAction = new System.Diagnostics.Process();
                diagAction.StartInfo.FileName = exeFilePath;
                diagAction.StartInfo.Arguments = arg;
                diagAction.StartInfo.CreateNoWindow = true;
                diagAction.StartInfo.UseShellExecute = false;
                diagAction.Start();
                /// diagAction.WaitForExit();//關鍵，等待外部程式退出後才能往下執行
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }
        }
    }
}
