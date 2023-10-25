using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SuperCarter
{
    /// <summary>
    /// App.xaml 的互動邏輯
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            SetLightTheme();
        }

        private void SetLightTheme()
        {
            ResourceDictionary lightTheme = Resources["LightTheme"] as ResourceDictionary;
            if (lightTheme != null)
            {
                Resources.MergedDictionaries.Add(lightTheme);
            }
        }
    }

}
