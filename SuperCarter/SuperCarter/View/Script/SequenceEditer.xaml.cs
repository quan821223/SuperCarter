using AvalonDock.Layout;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SuperCarter.View.Script
{
    /// <summary>
    /// SequenceEditer.xaml 的互動邏輯
    /// </summary>
    public partial class SequenceEditer : UserControl
    {
        public SequenceEditer()
        {
            InitializeComponent();
        }
        private void CMDprotocal_Click(object sender, RoutedEventArgs e)
        {
            SequencesProtocal _SequencesProtocal = new SequencesProtocal();
            _SequencesProtocal.Show();
        }
        private void SequenceScript_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            //(this.DataContext as SMSViewModel).serialPortViewModelBase.SelectedScriptitem = SequenceScript.SelectedIndex;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //CollectionViewSource.GetDefaultView(SequenceScript.ItemsSource).Refresh();
        }
        private void bt_savetestsequence_Click(object sender, RoutedEventArgs e)
        {
            SequenceScript.CommitEdit(DataGridEditingUnit.Row, true);
        }
        private void Textscriptpath_TextChanged(object sender, TextChangedEventArgs e)
        {
            // that is to move caret to end of textbox.
            Textscriptpath.SelectionLength = 0;
            Textscriptpath.Select(Textscriptpath.Text.Length, 0);
            Keyboard.Focus(Textscriptpath);
        }
        private void ScriptToolBar_SortScriptitem_Click(object sender, RoutedEventArgs e)
        {
            //var Grid = (DataGrid)sender;
            SequenceScript.CommitEdit(DataGridEditingUnit.Row, true);
            SequenceScript.CancelEdit(DataGridEditingUnit.Cell);
            SequenceScript.CancelEdit(DataGridEditingUnit.Row);

            //(this.DataContext as SuperCarterViewModel).serialPortViewModelBase.evt_ScriptToolBar_Sortintitem();


            CollectionViewSource.GetDefaultView(SequenceScript.ItemsSource).Refresh();
        }
        private void bt_Sequenceboxwindow_Click(object sender, RoutedEventArgs e)
        {
            var toolWindow_Sequencebox = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == "toolWindow_Sequencebox");
            if (toolWindow_Sequencebox.IsHidden)
                toolWindow_Sequencebox.Show();
            else if (toolWindow_Sequencebox.IsVisible)
                toolWindow_Sequencebox.IsActive = true;
            else
                toolWindow_Sequencebox.AddToLayout(dockManager, AnchorableShowStrategy.Bottom | AnchorableShowStrategy.Most);
        }
        private void bt_Sequenceswindow_Click(object sender, RoutedEventArgs e)
        {
            var toolWindow_Sequences = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == "toolWindow_Sequences");
            if (toolWindow_Sequences.IsHidden)
                toolWindow_Sequences.Show();
            else if (toolWindow_Sequences.IsVisible)
                toolWindow_Sequences.IsActive = true;
            else
                toolWindow_Sequences.AddToLayout(dockManager, AnchorableShowStrategy.Bottom | AnchorableShowStrategy.Most);
        }

        private void bt_OutputTextwindow_Click(object sender, RoutedEventArgs e)
        {
            var toolWindow_Output = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == "toolWindow_Output");
            if (toolWindow_Output.IsHidden)
                toolWindow_Output.Show();
            else if (toolWindow_Output.IsVisible)
                toolWindow_Output.IsActive = true;
            else
                toolWindow_Output.AddToLayout(dockManager, AnchorableShowStrategy.Bottom | AnchorableShowStrategy.Most);
        }
    }
}
