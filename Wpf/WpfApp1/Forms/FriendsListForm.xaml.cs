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

        public FriendsListForm(FriendsListViewModel vm, List<int> notifications, List<int> friend_notif)
        {
            this.DataContext = vm;
            this.fl_vm = vm;

            if (notifications.Count > 0)
            {
                this.fl_vm.FriendsList.Notifications = notifications[3];
                this.fl_vm.Requests.Notifications = notifications[0];
                this.fl_vm.RequestsOutgoing.Notifications = notifications[1];
                //this.fl_vm.RequestsOutgoing.Notifications = notifications[];
                //Log notifications...
            }

            found_vm = new UsersListViewModel(new List<string> { "Add" });
            
            InitializeComponent();
            LoadFriends(friend_notif);
            LoadRequestsIncoming();
            LoadRequestsOutgoing();
            this.fl_vm.LogLines = new System.Collections.ObjectModel.ObservableCollection<LogLine>();
            LoadLog();
            this.LogTab_ScrollViewer.ScrollChanged += LogTab_ScrollViewer_ScrollChanged;
            //this.search_found_list.DataContext = found_vm;
            this.Closing += FriendsListForm_Closing;
            
            KeepUpdatingStatuses();
        }

        private void LogTab_ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer scv = (sender as ScrollViewer);
            if (scv.VerticalOffset == scv.ScrollableHeight)
            {
                LoadLog();
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

        private async void LoadFriends(List<int> friend_notif)
        {
            List<UsersListLineViewModel> fl = await Inst.ApiRequests.GetFriends(int.Parse(Inst.ApiRequests.User.id));
            if (fl?.Count != 0)
            {
                fl.OrderBy(y => y.StatusColor == Brushes.Green.Color).ToList().ForEach(x =>
                    {
                        if (friends_notif != null && friends_notif.Contains(x.UserId))
                        {
                            x.Visibility = true;
                        }
                        else
                        {
                            x.Visibility = false;//Default value;
                        }

                        fl_vm.FriendsList.Users.Add(x);
                    }
                );
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

        private int current_log_page = 0;
        private int log_count_per_page = 20;

        private async void LoadLog()
        {
            List<LogLine> logs = await Inst.ApiRequests.GetLogLines(current_log_page * log_count_per_page, log_count_per_page);
            current_log_page++;
            logs.ForEach(async x => await Inst.Utils.PopulateLogLinesWithNames(x));
            logs.ForEach(x => this.fl_vm.LogLines.Add(x));
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
                                    this.fl_vm.FriendsList.Users.Add(user_vm);
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
                                f_chat_form_vm.RemoveChat(user_vm.UserId);
                                this.f_chat_form.RemoveTab(user_vm.UserId);
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
                                this.fl_vm.FriendsList.Users.Add(user_vm);
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
                            Notify(1, user.Username + " wants to become friends.", 1);
                        }
                        break;
                    }
                case 1://Reject
                    {
                        UsersListLineViewModel user = this.fl_vm.RequestsOutgoing.Users.First(x => x.UserId == user_id);
                        this.fl_vm.RequestsOutgoing.Users.Remove(user);
                        Notify(1, user.Username + " has rejected your request.", null);
                        break;
                    }
                case 2://Remove
                    {
                        UsersListLineViewModel user = this.fl_vm.FriendsList.Users.First(x => x.UserId == user_id);
                        this.fl_vm.FriendsList.Users.Remove(user);
                        Notify(1, user.Username + " has removed you.", null);
                        break;
                    }
                case 3://Accept
                    {
                        UsersListLineViewModel user = this.fl_vm.RequestsOutgoing.Users.First(x => x.UserId == user_id);
                        this.fl_vm.RequestsOutgoing.Users.Remove(user);
                        user.StatusColor = (await Inst.ApiRequests.GetUser(user.UserId)).StatusColor;
                        if (!this.fl_vm.FriendsList.Users.Contains(user))
                        {
                            this.fl_vm.FriendsList.Users.Add(user);
                            Notify(1, user.Username + " has accepted your request.", 0);
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
                            Notify(0, user.Username + " logged in.", null);
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
                            Notify(0, user.Username + " logged out.", null);
                        }
                        break;
                    }
                case 6://Cancel friend request
                    {
                        UsersListLineViewModel user = this.fl_vm.Requests.Users.First(x => x.UserId == user_id);
                        this.fl_vm.Requests.Users.Remove(user);
                        Notify(1, user.Username + " has canceled his request.", null);
                        break;
                    }
                case 7:
                    {
                        LogLine log_line = await Inst.ApiRequests.GetLogLine(user_id);
                        await Inst.Utils.PopulateLogLinesWithNames(log_line);
                        this.fl_vm.LogLines.Insert(0, log_line);
                        
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private int notifications_count = 0;
        public List<int> friends_notif;

        /// <summary>
        /// Add notification number on top of tabs.
        /// 0 - Friends
        /// 1 - Requests (incoming)
        /// 2 - Requests (outgoing)
        /// </summary>
        /// <param name="news_number">Describes what happened</param>
        /// <param name="message">Describes what happened with a message string</param>
        /// <param name="tab_num">Which tab is related with this event</param>
        private void Notify(int news_number, string message, int? tab_num = null)
        {
            notifications_count += news_number;
            if (tab_num != null)
            {
                fl_vm.Add_Notification((int)tab_num);
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
                        fl_vm.FriendsList.Notifications = 0;
                        if (Inst.Utils.MainWindow.notifications.Count > 0)
                        {
                            Inst.Utils.MainWindow.notifications[3] = 0;
                            Inst.Utils.MainWindow.notifications[2] = 0;
                        }
                        break;
                    }
                case "Requests Incoming":
                    {
                        fl_vm.Requests.Notifications = 0;
                        if (Inst.Utils.MainWindow.notifications.Count > 0)
                        {
                            Inst.Utils.MainWindow.notifications[0] = 0;
                            Inst.Utils.MainWindow.notifications[6] = 0;
                        }
                        break;
                    }
                case "Requests Outgoing":
                    {
                        fl_vm.RequestsOutgoing.Notifications = 0;
                        if (Inst.Utils.MainWindow.notifications.Count > 0)
                        {
                            Inst.Utils.MainWindow.notifications[1] = 0;
                        }
                        break;
                    }
                case "Log":
                    {
                        //fl_vm.LogLines.no
                        if (Inst.Utils.MainWindow.notifications.Count > 0)
                        {
                            Inst.Utils.MainWindow.notifications[7] = 0;
                        }
                        break;
                    }
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

            UsersListLineViewModel user = this.fl_vm.FriendsList.Users.FirstOrDefault(x => x.UserId == user_vm.UserId);
            if (user != null)
            {
                user.Visibility = false;
            }
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
                    user_vm.Visibility = true;
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
