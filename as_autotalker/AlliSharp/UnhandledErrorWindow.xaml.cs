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

namespace AlliSharp
{
    /// <summary>
    /// Interaction logic for UnhandledErrorWindow.xaml
    /// </summary>
    public partial class UnhandledErrorWindow : Window
    {
        public bool Terminate = false;

        public UnhandledErrorWindow(Exception e)
        {
            InitializeComponent();
            ErrorInfo.Text = e.ToString();
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ErrorInfo.Text);
        }

        public bool ShowError()
        {
            this.ShowDialog();
            return Terminate;
        }

        private void TerminateProcess_Click(object sender, RoutedEventArgs e)
        {
            Terminate = true;
            this.Close();
        }
    }
}

