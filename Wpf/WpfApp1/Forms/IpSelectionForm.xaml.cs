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

namespace WpfApp1.Forms
{
    /// <summary>
    /// Interaction logic for IpSelectionForm.xaml
    /// </summary>
    public partial class IpSelectionForm : Window
    {
        public IpSelectionForm()
        {
            InitializeComponent();
        }

        private void Open_ip_selection(object sender, RoutedEventArgs e)
        {
            var button = sender as FrameworkElement;
            if (button != null)
            {
                button.ContextMenu.IsOpen = true;
            }
        }

        private void Ip_Selected_Click(object sender, RoutedEventArgs e)
        {
            this.txt_ip.Text = (sender as MenuItem).Header.ToString();
        }
    }
}
