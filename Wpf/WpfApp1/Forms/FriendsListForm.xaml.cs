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
using WpfApp1.ViewModels;

namespace WpfApp1.Forms
{
    /// <summary>
    /// Interaction logic for FriendsListForm.xaml
    /// </summary>
    public partial class FriendsListForm : Window
    {
        private FriendsListViewModel fl_vm;
        private UsersListViewModel found_vm;

        public FriendsListForm(FriendsListViewModel vm)
        {
            this.DataContext = vm;
            this.fl_vm = vm;
            found_vm = new UsersListViewModel(new List<string> { "Add" });
            
            InitializeComponent();
            LoadFriends();
            LoadRequestsIncoming();
            LoadRequestsOutgoing();
            //this.search_found_list.DataContext = found_vm;
            this.Closing += FriendsListForm_Closing;

        }

        private void FriendsListForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Inst.Utils.MainWindow.Fl_form = null;
        }

        private async void LoadFriends()
        {
            List<UsersListLineViewModel> fl = await Inst.ApiRequests.GetFriends(int.Parse(Inst.ApiRequests.User.id));
            if (fl?.Count != 0)
            {
                fl.OrderBy(y => y.StatusColor == Brushes.Green.Color).ToList().ForEach(x => fl_vm.FriendsList.Users.Add(x));
            }
            else
            {
                this.friendless_grid.Visibility = Visibility.Visible;
            }

            //this.UpdateLayout();//Reikalingas?
        }

        private async void LoadRequestsIncoming()
        {
            List<UsersListLineViewModel> freqs = await Inst.ApiRequests.GetFriendRequests(int.Parse(Inst.ApiRequests.User.id), "in");
            if (freqs?.Count != 0)
            {
                freqs.OrderBy(y => y.StatusColor == Brushes.Green.Color).ToList().ForEach(x => fl_vm.Requests.Users.Add(x));
            }
            else
            {
                //There are no requests.
            }
        }

        private async void LoadRequestsOutgoing()
        {
            List<UsersListLineViewModel> freqs = await Inst.ApiRequests.GetFriendRequests(int.Parse(Inst.ApiRequests.User.id), "out");
            if (freqs?.Count != 0)
            {
                freqs.OrderBy(y => y.StatusColor == Brushes.Green.Color).ToList().ForEach(x => fl_vm.RequestsOutgoing.Users.Add(x));
            }
            else
            {
                //There are no requests.
            }
        }

        private void SearchFriends_Click(object sender, RoutedEventArgs e)
        {
            this.friendless_grid.Visibility = Visibility.Hidden;
            this.search_tab.BringIntoView();
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.search_string_txtbox.Text))
            {
                return;
            }

            List<UsersListLineViewModel> found = await Inst.ApiRequests.SearchFriends(this.search_string_txtbox.Text);
            if (found != null)
            {
                if (found.Count != 0)
                {
                    //found_vm = new UsersListViewModel();
                    found_vm.Users.Clear();
                    found.ForEach(x => found_vm.Users.Add(x));
                    //this.search_found_list.DataContext = found_vm;
                    //this.search_found_list.DataContext = found_vm;
                }
                else
                {
                    if (found.Count == 0)
                    {
                        //found_vm = new UsersListViewModel();
                        //found_vm.Users = new System.Collections.ObjectModel.ObservableCollection<UsersListLineViewModel>();
                        //found_vm.Users.RemoveAt(0);
                        found_vm.Users.Clear();
                    }
                }
                this.search_found_list.DataContext = found_vm;
                this.search_found_list.UpdateLayout();
            }
        }

        public async void HandleUserControlsChange(string command, UsersListLineViewModel user_vm)
        {
            switch (command)
            {
                case "Add"://Add user to your friends list(send friendship request)
                    {
                        if (await Inst.ApiRequests.SendRequest("add", user_vm.UserId))
                        {
                            if (!this.fl_vm.RequestsOutgoing.Users.Contains(user_vm))
                            {
                                if (!this.fl_vm.Requests.Users.Contains(user_vm))
                                {
                                    this.fl_vm.RequestsOutgoing.Users.Add(user_vm);
                                }
                                else
                                {
                                    this.fl_vm.Requests.Users.Remove(user_vm);
                                    this.fl_vm.FriendsList.Users.Add(user_vm);
                                }
                            }
                        }
                        break;
                    }
                case "Remove"://Remove friend from friends list
                    {
                        if(await Inst.ApiRequests.SendRequest("remove", user_vm.UserId))
                        {
                            this.fl_vm.FriendsList.Users.Remove(user_vm);
                        }
                        break;
                    }
                case "Reject"://Reject friendship request
                    {
                        if(await Inst.ApiRequests.SendRequest("reject", user_vm.UserId))
                        {
                            this.fl_vm.Requests.Users.Remove(user_vm);
                        }
                        break;
                    }
                case "Accept"://Accept friendship request
                    {
                        if(await Inst.ApiRequests.SendRequest("accept", user_vm.UserId))
                        {
                            if (!this.fl_vm.FriendsList.Users.Contains(user_vm))
                            {
                                this.fl_vm.FriendsList.Users.Add(user_vm);
                                this.fl_vm.Requests.Users.Remove(user_vm);
                            }
                        }
                        break;
                    }
                case "Send message"://Start chatting (chat pops up)
                    {
                        if (true)
                        {

                        }
                        break;
                    }
                case "Cancel"://Decline outgoing friendship requests
                    {
                        if(await Inst.ApiRequests.SendRequest("reject", user_vm.UserId))
                        {
                            this.fl_vm.RequestsOutgoing.Users.Remove(user_vm);
                        }
                        break;
                    }
                default: break;
            }

        }
    }
}
