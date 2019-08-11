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

namespace WpfApp1.Windows
{
    /// <summary>
    /// Interaction logic for ConfirmWindow.xaml
    /// </summary>
    public partial class ConfirmWindow : Window
    {
        public bool confirm = false;
        public ConfirmWindow(string labelText)
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            this.label.Text = labelText;
        }

        private void Yes_Click(object sender, RoutedEventArgs e)
        {
            confirm = true;
            this.Close();
        }

        private void No_Click(object sender, RoutedEventArgs e)
        {
            confirm = false;
            this.Close();
        }
        public bool Rezult
        {
            get{return confirm;}
        }
    }
}
