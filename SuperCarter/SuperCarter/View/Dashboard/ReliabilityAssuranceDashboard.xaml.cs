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
    /// ReliabilityAssuranceDashboard.xaml 的互動邏輯
    /// </summary>
    public partial class ReliabilityAssuranceDashboard : UserControl
    {
        public ReliabilityAssuranceDashboard()
        {
            InitializeComponent();
        }
        private void SDMChecklistscriptXMLPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            // that is to move caret to end of textbox.
            SDMChecklistscriptXMLPath.SelectionLength = 0;
            SDMChecklistscriptXMLPath.Select(SDMChecklistscriptXMLPath.Text.Length, 0);
            Keyboard.Focus(SDMChecklistscriptXMLPath);
        }
        private void ScheduledscriptScriptPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            // that is to move caret to end of textbox.
            ScheduledscriptScriptPath.SelectionLength = 0;
            ScheduledscriptScriptPath.Select(ScheduledscriptScriptPath.Text.Length, 0);
            Keyboard.Focus(ScheduledscriptScriptPath);
        }
    }

}
