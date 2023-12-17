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

namespace SuperCarter.View.Dashboard.ImgUI
{
    /// <summary>
    /// navi_img2.xaml 的互動邏輯
    /// </summary>
    public partial class navi_img2 : UserControl
    {
        public navi_img2()
        {
            InitializeComponent();
        }
        private void radiobt_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;

            // 根據選取的RadioButton設定圖片路徑
            if (radioButton == radiobt1)
            {
                mainimg.Source = new BitmapImage(new Uri("腳本設定.png", UriKind.Relative));
            }
            else if (radioButton == radiobt2)
            {
                mainimg.Source = new BitmapImage(new Uri("複合型腳本設置.png", UriKind.Relative));
            }
            else if (radioButton == radiobt3)
            {
                mainimg.Source = new BitmapImage(new Uri( "複合型子腳本匯入與查看.png", UriKind.Relative));
            }
            else if (radioButton == radiobt4)
            {
                mainimg.Source = new BitmapImage(new Uri("運行操作.png", UriKind.Relative));
            }
        }
    }

}
