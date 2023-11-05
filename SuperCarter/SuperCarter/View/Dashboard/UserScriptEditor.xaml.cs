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

namespace SuperCarter.View.Dashboard
{
    /// <summary>
    /// UserScriptEditor.xaml 的互動邏輯
    /// </summary>
    public partial class UserScriptEditor : UserControl
    {
        public UserScriptEditor()
        {
            InitializeComponent();
        }
        private void CurrentBlockscriptfile_TextChanged(object sender, TextChangedEventArgs e)
        {
            // that is to move caret to end of textbox.
            CurrentBlockscriptfile.SelectionLength = 0;
            CurrentBlockscriptfile.Select(CurrentBlockscriptfile.Text.Length, 0);
            Keyboard.Focus(CurrentBlockscriptfile);
        }
    }
}
