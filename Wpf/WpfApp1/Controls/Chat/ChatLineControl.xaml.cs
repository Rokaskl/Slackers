using System.Windows.Controls;

namespace WpfApp1.Controls.Chat
{
    /// <summary>
    /// Interaction logic for ChatLineControl.xaml
    /// </summary>
    public partial class ChatLineControl : UserControl
    {
        public ChatLineControl()
        {
            InitializeComponent();
        }

        private void Grid_RequestBringIntoView(object sender, System.Windows.RequestBringIntoViewEventArgs e)
        {

        }
    }
}
