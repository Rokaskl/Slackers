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
using WpfApp1.Windows;

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

            this.fl_vm.FriendsList.Notifications = Inst.Utils.Notifications.FriendsTabN;
            this.fl_vm.Requests.Notifications = Inst.Utils.Notifications.RequestsTabN;
            this.fl_vm.RequestsOutgoing.Notifications = Inst.Utils.Notifications.RequestsOutgoingTabN;

            found_vm = new UsersListViewModel(new List<string> { "Add" });
            
            InitializeComponent();
            Inst.Utils.Notifications.TabNotificationChanged += Notifications_TabNotificationChanged;
            LoadFriends();
            LoadRequestsIncoming();
            LoadRequestsOutgoing();
            
            //this.search_found_list.DataContext = found_vm;
            this.Closing += FriendsListForm_Closing;
            
            KeepUpdatingStatuses();
        }

        private void Notifications_TabNotificationChanged(object sender, GlobalClasses.Notifications.TabNotificationsChangesEventArgs e)
        {
            switch (e.TabChanged)
            {
                case "Friends":
                    {
                        this.fl_vm.FriendsList.Notifications = e.NotificationCount;
                        break;
                    }
                case "RequestsIncoming":
                    {
                        this.fl_vm.Requests.Notifications = e.NotificationCount;
                        break;
                    }
                case "RequestsOutgoing":
                    {
                        this.fl_vm.RequestsOutgoing.Notifications = e.NotificationCount;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            
        }

        private void FriendsListForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Inst.Utils.MainWindow.Fl_form = null;
            this.keep_updating = false;
            if (this.f_chat_form != null)
            {
                this.f_chat_form.Close();
            }
        }

        private async void LoadFriends()
        {
            List<UsersListLineViewModel> fl = await Inst.ApiRequests.GetFriends(int.Parse(Inst.ApiRequests.User.id));
            if (fl?.Count != 0)
            {
                fl.OrderBy(y => y.StatusColor == Brushes.Green.Color).ToList().ForEach(x =>
                    {
                        if (Inst.Utils.Notifications.MessageNotifications != null && Inst.Utils.Notifications.MessageNotifications.Keys.Contains(x.UserId))
                        {
                           // x.Visibility = true;
                            x.NotificationCount = Inst.Utils.Notifications.MessageNotifications[x.UserId];
                        }
                        else
                        {
                            //x.Visibility = false;//Default value;
                            x.NotificationCount = 0;
                        }

                        fl_vm.FriendsList.Users.Add(x);
                    }
                );
            }
            else
            {
                this.friendless_grid.Visibility = Visibility.Visible;
            }
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

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            ConductSearch();
        }

        private async void ConductSearch()
        {
            if (string.IsNullOrWhiteSpace(this.search_string_txtbox.Text))
            {
                this.found_vm.Users.Clear();
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
                                    this.fl_vm.FriendsList.Users.Add(Inst.Utils.Notifications.PopulateUserWithNotifications(user_vm));
                                }
                            }
                        }
                        break;
                    }
                case "Remove"://Remove friend from friends list
                    {
                        var confirm = new ConfirmWindow($"Remove friend ({user_vm.Username})?");
                        if (confirm.ShowDialog() == false) 
                        {
                            if (confirm.Rezult && await Inst.ApiRequests.SendRequest("remove", user_vm.UserId))
                            {
                                UsersListLineViewModel user_from_friendslist = this.fl_vm.FriendsList.Users.First(x => x.UserId == user_vm.UserId);
                                user_from_friendslist.StatusColor = Brushes.Gray.Color;
                                this.fl_vm.FriendsList.Users.Remove(user_vm);
                                if (this.f_chat_form != null && this.f_chat_form_vm != null)
                                {
                                    this.f_chat_form_vm.RemoveChat(user_vm.UserId);
                                    this.f_chat_form.RemoveTab(user_vm.UserId);
                                }
                                Inst.Utils.Notifications.RemoveMessageNotifications(user_from_friendslist);
                            }
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
                                this.fl_vm.FriendsList.Users.Add(Inst.Utils.Notifications.PopulateUserWithNotifications(user_vm));
                                this.fl_vm.Requests.Users.Remove(user_vm);
                            }
                        }
                        break;
                    }
                case "Send message"://Start chatting (chat pops up)
                    {
                        StartChat(user_vm);
                        break;
                    }
                case "Cancel"://Decline outgoing friendship requests
                    {
                        if(await Inst.ApiRequests.SendRequest("cancel", user_vm.UserId))
                        {
                            this.fl_vm.RequestsOutgoing.Users.Remove(user_vm);
                        }
                        break;
                    }
                default: break;
            }

        }

        public async void HandleTcpSignal(int command, int user_id)
        {
            switch (command)
            {
                case 0://Add
                    {
                        UsersListLineViewModel user = await Inst.ApiRequests.GetUser(user_id);
                        if (!this.fl_vm.Requests.Users.Contains(user))
                        {
                            this.fl_vm.Requests.Users.Add(user);
                            Notify(1, user.Username + " wants to become friends.", user, 0, 1);
                        }
                        break;
                    }
                case 1://Reject
                    {
                        UsersListLineViewModel user = this.fl_vm.RequestsOutgoing.Users.First(x => x.UserId == user_id);
                        this.fl_vm.RequestsOutgoing.Users.Remove(user);
                        Notify(1, user.Username + " has rejected your request.", user, 1);
                        break;
                    }
                case 2://Remove
                    {
                        UsersListLineViewModel user = this.fl_vm.FriendsList.Users.First(x => x.UserId == user_id);
                        Inst.Utils.Notifications.RemoveMessageNotifications(user);
                        this.fl_vm.FriendsList.Users.Remove(user);
                        Notify(1, user.Username + " has removed you.", user, 2);
                        break;
                    }
                case 3://Accept
                    {
                        UsersListLineViewModel user = this.fl_vm.RequestsOutgoing.Users.First(x => x.UserId == user_id);
                        this.fl_vm.RequestsOutgoing.Users.Remove(user);
                        user.StatusColor = (await Inst.ApiRequests.GetUser(user.UserId)).StatusColor;
                        if (!this.fl_vm.FriendsList.Users.Contains(user))
                        {
                            this.fl_vm.FriendsList.Users.Add(Inst.Utils.Notifications.PopulateUserWithNotifications(user));
                            Notify(1, user.Username + " has accepted your request.", user, 3, 0);
                        }
                        break;
                    }
                case 4://Status became online
                    {
                        UsersListLineViewModel user = this.fl_vm.FriendsList.Users.FirstOrDefault(x => x.UserId == user_id);
                        if (user == null)
                        {
                            user = this.fl_vm.Requests.Users.First(x => x.UserId == user_id);
                        }
                        if (user != null)
                        {
                            user.StatusColor = Brushes.Green.Color;
                            Notify(0, user.Username + " logged in.", user, 4);
                        }
                        break;
                    }
                case 5://Status became offline
                    {
                        UsersListLineViewModel user = this.fl_vm.FriendsList.Users.FirstOrDefault(x => x.UserId == user_id);
                        if (user == null)
                        {
                            user = this.fl_vm.Requests.Users.First(x => x.UserId == user_id);
                        }
                        if (user != null)
                        {
                            user.StatusColor = Brushes.Gray.Color;
                            Notify(0, user.Username + " logged out.", user, 5);
                        }
                        break;
                    }
                case 6://Cancel friend request
                    {
                        UsersListLineViewModel user = this.fl_vm.Requests.Users.First(x => x.UserId == user_id);
                        this.fl_vm.Requests.Users.Remove(user);
                        Notify(1, user.Username + " has canceled his request.", user, 6);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        /// <summary>
        /// Add notification number on top of tabs.
        /// 0 - Friends
        /// 1 - Requests (incoming)
        /// 2 - Requests (outgoing)
        /// </summary>
        /// <param name="news_number">Describes what happened</param>
        /// <param name="message">Describes what happened with a message string</param>
        /// <param name="tab_num">Which tab is related with this event</param>
        private void Notify(int news_number, string message, UsersListLineViewModel user, int switch_location, int? tab_num = null)
        {
            if (tab_num != null)
            {
                //fl_vm.Add_Notification((int)tab_num, user.UserId, switch_location);
                Inst.Utils.Notifications.AddNotification(switch_location, user.UserId);
            }
            
            //if (!this.IsVisible)
            //{
            //    System.Media.SystemSounds.Exclamation.Play();
            //    //this
            //}
            //some kind of notification. popup??? sound??? icon with number of notifications in it??? log of notifications???
        }

        private async void Status_Button_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Content.Equals("OFF"))
            {
                if (await Inst.ApiRequests.ChangeStatus(0))
                {
                    this.fl_vm.Owner.StatusColor = Brushes.Gray.Color;
                    (sender as Button).Content = "ON";
                    (sender as Button).Background = Brushes.Green;
                }
            }
            else
            {
                if (await Inst.ApiRequests.ChangeStatus(1))
                {
                    this.fl_vm.Owner.StatusColor = Brushes.Green.Color;
                    (sender as Button).Content = "OFF";
                    (sender as Button).Background = Brushes.Gray;
                }
            }
            
        }

        private void TabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            switch((sender as TabItem).Header)
            {
                case "Friends":
                    {
                        Inst.Utils.Notifications.FriendsTabFocused();
                        //fl_vm.FriendsList.Notifications = Inst.Utils.Notifications.FriendsTabN;
                        break;
                    }
                case "Requests Incoming":
                    {
                        Inst.Utils.Notifications.RequestsTabFocused();
                        //fl_vm.Requests.Notifications = Inst.Utils.Notifications.RequestsTabN;
                        break;
                    }
                case "Requests Outgoing":
                    {
                        Inst.Utils.Notifications.RequestsOutgoingTabFocused();
                        //fl_vm.RequestsOutgoing.Notifications = Inst.Utils.Notifications.RequestsOutgoingTabN;
                        break;
                    }
                //case "Log":
                //    {
                //        Inst.Utils.Notifications.LogsTabFocused();
                //        break;
                //    }
                default:
                    {
                        break;
                    }
            }
        }

        private FriendsChatForm f_chat_form;
        private FriendsChatFormViewModel f_chat_form_vm;

        private void StartChat(UsersListLineViewModel user_vm)
        {
            if (f_chat_form == null)
            {
                f_chat_form = new FriendsChatForm();
                f_chat_form_vm = new FriendsChatFormViewModel();
                f_chat_form.DataContext = f_chat_form_vm;
                f_chat_form.Closing += F_chat_form_Closing;
            }
            f_chat_form.Show();

            ChatViewModel chat_vm;
            if ((chat_vm = f_chat_form_vm.AddChat(user_vm)) != null)
            {
                f_chat_form.AddTab(chat_vm);
                //f_chat_form.FocusLastAddedTab();
            }
            else
            {
                f_chat_form.FocusTab(user_vm.UserId);
            }

            //UsersListLineViewModel user = this.fl_vm.FriendsList.Users.FirstOrDefault(x => x.UserId == user_vm.UserId);
            //if (user != null)
            //{
            //    user.Visibility = false;
            //}
        }

        private void F_chat_form_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.f_chat_form = null;
            this.f_chat_form_vm = null;
        }

        public void NewChatLine(int user_id, string text)
        {
            if (f_chat_form != null && f_chat_form_vm != null)
            {
                f_chat_form_vm.AppendToChat(text, user_id.ToString());
            }
            else
            {
                UsersListLineViewModel user_vm = this.fl_vm.FriendsList.Users.FirstOrDefault(x => x.UserId == user_id);
                if (user_vm != null)
                {
                    //user_vm.Visibility = true;
                    //user_vm.NotificationCount++;
                    Inst.Utils.Notifications.AddMessageNotifications(user_vm);
                }
            }
        }

        private void Search_string_txtbox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ConductSearch();
            }
        }

        private bool keep_updating;

        public object UserListLineViewModel { get; private set; }

        private async void KeepUpdatingStatuses()
        {
            keep_updating = true;

            while (keep_updating)
            {
                await Task.Delay(5000);

                List<KeyValuePair<int, bool>> statuses = await Inst.ApiRequests.GetUserStatuses(this.fl_vm.FriendsList.Users.Select(x => x.UserId).Concat(this.fl_vm.Requests.Users.Select(y => y.UserId)).ToList());
                statuses.ForEach(x =>
                    {
                        UsersListLineViewModel user = this.fl_vm.FriendsList.Users.FirstOrDefault(y => y.UserId == x.Key);
                        if (user == null)
                        {
                            user = this.fl_vm.Requests.Users.FirstOrDefault(y => y.UserId == x.Key);
                        }
                        if (user != null)
                        {
                            user.StatusColor = x.Value ? Brushes.Green.Color : Brushes.Gray.Color;
                        }
                    }
                );
            }
        }
    }
}
