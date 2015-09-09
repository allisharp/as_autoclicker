using AlliSharp;
using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace as_autotalker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IHotKeyOwner
    {
        private  FileLibrary<Clicker> clickers;
        private  Clicker clicker = null;
        private  HotKeyManager hotKeyManager;
        private  CancellationTokenSource tokenSource;
        private  CancellationToken token;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AutoClick()
        {
            while (!token.IsCancellationRequested)
            {
                int x = System.Windows.Forms.Cursor.Position.X;
                int y = System.Windows.Forms.Cursor.Position.Y;
                NativeMethods.mouse_event(clicker.IsRightClick ? (uint)NativeMethods.MOUSEEVENTF_RIGHTDOWN : NativeMethods.MOUSEEVENTF_LEFTDOWN, (uint)x, (uint)y, 0, 0);
                System.Threading.Thread.Sleep(clicker.MouseDownInterval);
                x = System.Windows.Forms.Cursor.Position.X;
                y = System.Windows.Forms.Cursor.Position.Y;
                NativeMethods.mouse_event(clicker.IsRightClick ? (uint)NativeMethods.MOUSEEVENTF_RIGHTUP : NativeMethods.MOUSEEVENTF_LEFTUP, (uint)x, (uint)y, 0, 0);
                System.Threading.Thread.Sleep(clicker.Interval);
            }    
        }
        private void StartClicker()
        {
            if (!clicker.Started)
            {
                clicker.Started = true;
                tokenSource = new CancellationTokenSource();
                token = tokenSource.Token;
                
                Task t = Task.Factory.StartNew(AutoClick, token);
            }
        }

        private void StopClicker()
        {
            if (clicker.Started)
            {
                clicker.Started = false;
                tokenSource.Cancel();
                tokenSource.Dispose();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (clickers == null) //first load
            {
                clickers = new FileLibrary<Clicker>("clickers");
                clickers.LoadLibrary();
                if (clickers.Count > 0)
                {
                    clicker = clickers[0];
                }
                else
                {
                    clicker = new Clicker(5000, 50, 5, 6, false);
                }

                tbClickerStatus.DataContext = clicker;

                hotKeyManager = new HotKeyManager(this, new WindowInteropHelper(this).Handle);

                clicker.hotKeyIdStart = hotKeyManager.RegisterFKey(clicker.FKeyStart);
                clicker.hotKeyIdStop = hotKeyManager.RegisterFKey(clicker.FKeyStop);

                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    this.Title = this.Title + " " + ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(4);
                }
            }
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();


            clickers.Clear();
            if (clicker.hotKeyIdStart != 0) hotKeyManager.UnregisterFKey(clicker.hotKeyIdStart);
            if (clicker.hotKeyIdStop != 0) hotKeyManager.UnregisterFKey(clicker.hotKeyIdStop);
            clickers.Add(clicker);

            clickers.SaveLibrary();
        }

        #region tbInterval numbers only code

        private static bool IsTextAllowed(string text)
        {
            if (text.Contains(Key.Space.ToString())) return false;
            Regex regex = new Regex("[^0-9]+");
            return !regex.IsMatch(text);
        }

        private void tbInterval_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void tbInterval_PastingHandler(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void tbInterval_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = (e.Key == Key.Space);
        }

        #endregion

        public void HotKeyPressed(int id)
        {
            if (clicker != null)
            {
                if (clicker.hotKeyIdStart.Equals(id))
                {
                    if (!clicker.Started)
                    {
                        StartClicker();
                    }
                }

                if (clicker.hotKeyIdStop.Equals(id))
                {
                    if (clicker.Started)
                    {
                        StopClicker();
                    }
                }
            }        
        }

        private void btnGoToAlliSharpCom_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http:\\allisharp.com");
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (clicker.hotKeyIdStart != 0) hotKeyManager.UnregisterFKey(clicker.hotKeyIdStart);
            if (clicker.hotKeyIdStop != 0) hotKeyManager.UnregisterFKey(clicker.hotKeyIdStop);
            if (clicker.Started)
            {
                StopClicker();
            }
            tabDefault.Visibility = Visibility.Collapsed;
            tabEditSettings.Visibility = Visibility.Visible;
            tbInterval.Text = clicker.Interval.ToString();
            tbMouseDown.Text = clicker.MouseDownInterval.ToString();
            cbFKeyStart.SelectedIndex = clicker.FKeyStart - 1;
            cbFKeyStop.SelectedIndex = clicker.FKeyStop - 1;
            cbIsRightClick.IsChecked = clicker.IsRightClick;
        }

        private void btnEditDone_Click(object sender, RoutedEventArgs e)
        {
            if (cbFKeyStart.SelectedIndex == cbFKeyStop.SelectedIndex)
            {
                new AlliSharp.AlliSharpMessageBox(this.Title, "The start and stop hot keys cannot be the same.");
                return;
            }

            int o;
            if (!Int32.TryParse(tbInterval.Text, out o))
            {
                new AlliSharp.AlliSharpMessageBox(this.Title, "Interval value must be numbers only ;)");
                return;
            }
            if (o <= 0)
            {
                new AlliSharp.AlliSharpMessageBox(this.Title, "Please enter a valid interval value (in milliseconds).");
                return;
            }
            if (o <= 50)
            {
                new AlliSharp.AlliSharpMessageBox(this.Title, "Warning: Click intervals are recommended to be atleast 50 milliseconds or slower.");
            }
            int o2;
            if (!Int32.TryParse(tbMouseDown.Text, out o2))
            {
                new AlliSharp.AlliSharpMessageBox(this.Title, "Mouse down interval value must be numbers only ;)");
                return;
            }
            if (o2 <= 0)
            {
                new AlliSharp.AlliSharpMessageBox(this.Title, "Please enter a valid mouse down interval value (in milliseconds).");
                return;
            }
            if (o2 <= 5)
            {
                new AlliSharp.AlliSharpMessageBox(this.Title, "Warning: Mouse down intervals are recommended to be above 5 milliseconds.");
            }

            clicker.Interval = o;
            clicker.MouseDownInterval = o2;
            clicker.FKeyStart = cbFKeyStart.SelectedIndex + 1;
            clicker.FKeyStop = cbFKeyStop.SelectedIndex + 1;
            clicker.IsRightClick = cbIsRightClick.IsChecked.Value;
            tabDefault.Visibility = Visibility.Visible;
            tabEditSettings.Visibility = Visibility.Collapsed;
            clicker.hotKeyIdStart = hotKeyManager.RegisterFKey(clicker.FKeyStart);
            clicker.hotKeyIdStop = hotKeyManager.RegisterFKey(clicker.FKeyStop);
        }     
    }
}
