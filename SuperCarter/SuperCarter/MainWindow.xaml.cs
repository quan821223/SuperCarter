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

namespace SuperCarter
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private bool _isDarkTheme = false;

        private bool IsLightThemeActive()
        {
            return Application.Current.Resources.MergedDictionaries.Contains(Application.Current.Resources["LightTheme"] as ResourceDictionary);
        }
        private void OnToggleThemeClicked(object sender, RoutedEventArgs e)
        {
            if (IsLightThemeActive())
            {
                Application.Current.Resources.MergedDictionaries.Remove(Application.Current.Resources["LightTheme"] as ResourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(Application.Current.Resources["DarkTheme"] as ResourceDictionary);
            }
            else
            {
                Application.Current.Resources.MergedDictionaries.Remove(Application.Current.Resources["DarkTheme"] as ResourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(Application.Current.Resources["LightTheme"] as ResourceDictionary);
            }

            // 確認主題是否已經切換
            if (IsLightThemeActive())
            {
                MessageBox.Show("目前是亮色主題");
            }
            else
            {
                MessageBox.Show("目前是暗色主題");
            }
        }
    }
}
