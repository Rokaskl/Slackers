using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Controls.UsersList
{
    /// <summary>
    /// Interaction logic for FriendsListLineControl.xaml
    /// </summary>
    public partial class UsersListLineControl : UserControl
    {
        public UsersListLineControl()
        {
            InitializeComponent();
            var x = this.ContextMenu;
            this.DataContextChanged += UsersListLineControl_DataContextChanged;
        }

        private void UsersListLineControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            while(true)
            {
                Task.Delay(2);
                if (this.ContextMenu != null)
                {

                }
                break;
            }
            if (true)
            {

            }
        }

        private void MenuItems_Clicked(object sender, RoutedEventArgs e)
        {

        }
    }
}
