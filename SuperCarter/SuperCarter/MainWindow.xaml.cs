using AvalonDock.Layout;
using SuperCarter.Model;
using SuperCarter.View;
using SuperCarter.View.Dashboard;
using SuperCarter.View.Script;
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
using SuperCarter.Services;
using Notification.Wpf;
using System.Diagnostics;

namespace SuperCarter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static SuperCarterViewModel SuperCarterVM { get; set; }
      
        public SequenceEditer sequenceEditer { get; set; }
        public Overview overview { get; set; }
        public CommunicationInterface communicationInterface { get; set; }
        public Applicationtable applicationList { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            SuperCarterVM = new SuperCarterViewModel();          
            sequenceEditer = new SequenceEditer();
            applicationList = new Applicationtable();
            overview = new Overview();
            // AppPaneGroup.Background = new SolidColorBrush(Colors.Transparent);
            AppPaneGroup.Hide();
            dockManager.Background = new SolidColorBrush(Colors.Transparent);
            GridPrincipal.Children.Clear();
            // hiding all text views.
            /// TextViewPortIII_Output
            var TextViewPortIII_Output = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == "TextViewPortIII_Output");
            if (TextViewPortIII_Output.IsEnabled)
            {
                TextViewPortIII_Output.Hide();            
            }                      

            /// TextViewPortII_Output
            var TextViewPortII_Output = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == "TextViewPortII_Output");
            if (TextViewPortII_Output.IsEnabled) {
                TextViewPortII_Output.Hide();
            }                 

            /// TextViewPortI_Output
            var TextViewPortI_Output = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == "TextViewPortI_Output");
            if (TextViewPortI_Output.IsEnabled) {
                TextViewPortI_Output.Hide();
            }                     

            /// AllViewText_Output
            var AllViewText_Output = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == "AllViewText_Output");
            if (AllViewText_Output.IsEnabled) {
                AllViewText_Output.Hide();
            }               

            this.DataContext = SuperCarterVM;
           
        }

        private void ListViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ListViewMenu.SelectedIndex + 1;
            switch (index)
            {
                case 1:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(applicationList);
                    AppPaneGroup.Show();
                    dockManager.Background = new SolidColorBrush(Colors.White);
                    // dockManager.Background = new SolidColorBrush(Colors.Transparent);
                    break;
                case 2:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(sequenceEditer);
                    AppPaneGroup.Show();
                    dockManager.Background = new SolidColorBrush(Colors.White);
                    if (dockManager.Visibility != Visibility.Visible)
                    {
                        dockManager.Visibility = Visibility.Visible;
                    }
                 
                    break;
                case 3:
                    GridPrincipal.Children.Clear();
                    GridPrincipal.Children.Add(overview);
                    dockManager.Background = new SolidColorBrush(Colors.White);
                    AppPaneGroup.Show();
                    if (dockManager.Visibility != Visibility.Visible)
                        dockManager.Visibility = Visibility.Visible;
                    break;
            }
        }
        private void ClearTextViewPortII_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as SuperCarterViewModel).TextViewPortII = "";
        }

        private void ClearAllViewText_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as SuperCarterViewModel).AllViewText = "";
        }

        private void ClearTextViewPortIII_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as SuperCarterViewModel).TextViewPortIII = "";
        }
        private void ClearTextViewPortI_Click(object sender, RoutedEventArgs e)
        {
            (this.DataContext as SuperCarterViewModel).TextViewPortI = "";
        }
        private void AllViewText_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (AllViewText.LineCount > 500)
                (this.DataContext as SuperCarterViewModel).AllViewText = "";
            AllViewText.ScrollToEnd();

        }
        private void TextViewPortI_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextViewPortI.LineCount > 500)
                (this.DataContext as SuperCarterViewModel).TextViewPortI = "";
            TextViewPortI.ScrollToEnd();
        }
        private void TextViewPortII_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextViewPortII.LineCount > 500)
                (this.DataContext as SuperCarterViewModel).TextViewPortII = "";
            TextViewPortII.ScrollToEnd();
        }
        private void TextViewPortIII_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextViewPortIII.LineCount > 500)
                (this.DataContext as SuperCarterViewModel).TextViewPortIII = "";
            TextViewPortIII.ScrollToEnd();
        }

        private void PortSettingMenuItem_Click(object sender, RoutedEventArgs e)
        {
            communicationInterface = new CommunicationInterface();
            communicationInterface.DataContext = SuperCarterVM;
            communicationInterface.Show();
      
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //ConfigbyJSON.logger.Log(NLog.LogLevel.Debug, "Auto_save_candidaters_process...");
            nlogMessageAggregator.Instance.SendMessage(new nlogtype
            {
                LogLevel = NLog.LogLevel.Debug,
                Msg = "Auto_save_candidaters_process..."
            });
            SuperCarterVM.serialportmanager.AutoSaveStatus();
            System.Environment.Exit(0);
        }

        private void PathofLogItem_Click(object sender, RoutedEventArgs e)
        {
            string dataPath = SuperCarterVM.AppPath + @"logs";
            Process.Start(new ProcessStartInfo { FileName = dataPath, UseShellExecute = true });
        }

        private void PathofDefaultscript_Click(object sender, RoutedEventArgs e)
        {
            string dataPath = SuperCarterVM.AppPath + @"scripts";
            Process.Start(new ProcessStartInfo { FileName = dataPath, UseShellExecute = true });
        }
        private void PathofDataItem_Click(object sender, RoutedEventArgs e)
        {
            string dataPath = SuperCarterVM.AppPath + @"result";
            Process.Start(new ProcessStartInfo { FileName = dataPath, UseShellExecute = true });
        }
        private void CalcMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("calc.exe");
        }


        private void appwindow_Click(object sender, RoutedEventArgs e)
        {
            if (dockManager.Visibility != Visibility.Visible)
                dockManager.Visibility = Visibility.Visible;

            var Application_window = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == "Application_window");

            if (Application_window.IsHidden)
                Application_window.Show();
            else if (Application_window.IsVisible)
                Application_window.IsActive = true;
            else if (!Application_window.IsActive)
                Application_window.Show();
            else
            {
                Application_window.AddToLayout(dockManager, AnchorableShowStrategy.Bottom | AnchorableShowStrategy.Most);
                Application_window.Show();
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
        

        }

        private void OpenAllview_Click(object sender, RoutedEventArgs e)
        {
            // TextViewPortIII_Output
            var TextViewPortIII_Output = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == "TextViewPortIII_Output");
            if (TextViewPortIII_Output.IsHidden)
                TextViewPortIII_Output.Show();
            else if (TextViewPortIII_Output.IsEnabled)
                TextViewPortIII_Output.Show();
            else if (!TextViewPortIII_Output.IsActive)
                TextViewPortIII_Output.Show();
            else
            {
                TextViewPortIII_Output.AddToLayout(dockManager, AnchorableShowStrategy.Bottom | AnchorableShowStrategy.Most);
                TextViewPortIII_Output.Show();
            }
            // TextViewPortII_Output
            var TextViewPortII_Output = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == "TextViewPortII_Output");
            if (TextViewPortII_Output.IsHidden)
                TextViewPortII_Output.Show();
            else if (TextViewPortII_Output.IsEnabled)
                TextViewPortII_Output.Show();
            else if (!TextViewPortII_Output.IsActive)
                TextViewPortII_Output.Show();
            else
            {
                TextViewPortII_Output.AddToLayout(dockManager, AnchorableShowStrategy.Bottom | AnchorableShowStrategy.Most);
                TextViewPortII_Output.Show();
            }
            // TextViewPortI_Output
            var TextViewPortI_Output = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == "TextViewPortI_Output");
            if (TextViewPortI_Output.IsHidden)
                TextViewPortI_Output.Show();
            else if (TextViewPortI_Output.IsEnabled)
                TextViewPortI_Output.Show();
            else if (!TextViewPortI_Output.IsActive)
                TextViewPortI_Output.Show();
            else
            {
                TextViewPortI_Output.AddToLayout(dockManager, AnchorableShowStrategy.Bottom | AnchorableShowStrategy.Most);
                TextViewPortI_Output.Show();
            }
            // AllViewText_Output
            var AllViewText_Output = dockManager.Layout.Descendents().OfType<LayoutAnchorable>().Single(a => a.ContentId == "AllViewText_Output");
            if (AllViewText_Output.IsHidden)
                AllViewText_Output.Show();
            else if (AllViewText_Output.IsEnabled)
                AllViewText_Output.Show();
            else if (!AllViewText_Output.IsActive)
                AllViewText_Output.Show();
            else
            {
                AllViewText_Output.AddToLayout(dockManager, AnchorableShowStrategy.Bottom | AnchorableShowStrategy.Most);
                AllViewText_Output.Show();
            }
        }


    }
}
