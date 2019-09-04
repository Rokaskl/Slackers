using System.Windows;
using System.Windows.Controls;
using WpfApp1.ViewModels;

namespace WpfApp1.Controls.UsersList
{
    /// <summary>
    /// Interaction logic for FriendsListControl.xaml
    /// </summary>
    public partial class UsersListControl : UserControl
    {
        private UsersListLineViewModel last_clicked_user;

        public UsersListControl()
        {
            InitializeComponent();

            this.DataContextChanged += UsersListControl_DataContextChanged;
        }

        private void UsersListControl_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            foreach (var item in this.Resources.Values)
            {
                if (item as ContextMenu != null && (this.DataContext as UsersListViewModel) != null)
                {
                    (item as ContextMenu).ItemsSource = (this.DataContext as UsersListViewModel).ContextMenuItems;
                }
            }
        }

        //private void UsersListLineControl_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        //{
        //    this.last_clicked_user = ((e.Source as UsersListLineControl).DataContext as UsersListLineViewModel);
        //}

        private void MenuItems_Clicked(object sender, RoutedEventArgs e)
        {
            Inst.Utils.MainWindow.Fl_form.HandleUserControlsChange((sender as MenuItem).Header.ToString(), this.last_clicked_user);
        }

        private void UsersListLineControl_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2 && ((this.Parent as Grid)?.Parent as Grid)?.Parent as WpfApp1.Pages.RoomPage == null && (this.Parent as TabItem)?.Header?.ToString() == "Friends")
            {
                Inst.Utils.MainWindow.Fl_form.HandleUserControlsChange("Send message", this.last_clicked_user);
            }
        }

        private void UsersListLineControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.last_clicked_user = ((e.Source as UsersListLineControl).DataContext as UsersListLineViewModel);
        }
    }
}
