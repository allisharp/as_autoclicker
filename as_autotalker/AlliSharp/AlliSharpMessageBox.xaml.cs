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
    /// Interaction logic for AlliSharpMessageBox.xaml
    /// </summary>
    public partial class AlliSharpMessageBox : Window
    {
        public AlliSharpMessageBox(string title, string customMessage)
        {
            InitializeComponent();
            this.Title = title;
            tbMessage.Text = customMessage;
            System.Media.SystemSounds.Asterisk.Play();
            this.ShowDialog();
        }

        public AlliSharpMessageBox(string title, Inline[] customMessage)
        {
            InitializeComponent();
            this.Title = title;
            tbMessage.Inlines.Clear();
            foreach (Inline il in customMessage)
            {
                tbMessage.Inlines.Add(il);
            }
            System.Media.SystemSounds.Asterisk.Play();
            this.ShowDialog();
        }

        private void btnOkay_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
