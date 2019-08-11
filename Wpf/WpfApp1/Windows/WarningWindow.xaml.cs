using System.Windows;

namespace WpfApp1.Windows
{
    /// <summary>
    /// Interaction logic for WarningWindow.xaml
    /// </summary>
    public partial class WarningWindow : Window
    {
        public WarningWindow(string text)
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.label.Text = text;
        }
        private void OK_Click(object sender, RoutedEventArgs e)
        {            
            this.Close();
        }
    }
}
