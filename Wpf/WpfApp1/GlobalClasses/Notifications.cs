using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.ViewModels;

namespace WpfApp1.GlobalClasses
{
    /// <summary>
    /// Intended to be used for all types of notifications around this app.
    /// Notifications access should be global, values bind to controls.
    /// </summary>
    public class Notifications : INotifyPropertyChanged
    {


        public event TabNotificationChangeEventHandler TabNotificationChanged;
        public delegate void TabNotificationChangeEventHandler(object sender, TabNotificationsChangesEventArgs e);
        public virtual void OnTabChanged(TabNotificationsChangesEventArgs e)
        {
            TabNotificationChangeEventHandler handler = TabNotificationChanged;
            handler?.Invoke(this, e);
        }

        public class TabNotificationsChangesEventArgs : EventArgs
        {
            public string TabChanged { get; }
            public int NotificationCount { get; }

            public TabNotificationsChangesEventArgs(string tabName, int not_count)
            {
                this.TabChanged = tabName;
                this.NotificationCount = not_count;
            }
        }

        #region Properties
        private int n_friends_btn;
        public int FriendsBtnN//Bind to friends button in main window.
        {
            get
            {
                return n_friends_btn;
            }
            set
            {
                n_friends_btn = value;
                Inst.Utils.MainWindow.btn_Friends.DataContext = value;
            }
        }

        private int n_logs_btn;
        public int LogsBtnN
        {
            get
            {
                return n_logs_btn;
            }
            set
            {
                n_logs_btn = value;
                Inst.Utils.MainWindow.btn_Logs.DataContext = value;
            }
        }

        private int n_friends_tab;
        public int FriendsTabN
        {
            get
            {
                return n_friends_tab;
            }
            set
            {
                n_friends_tab = value;
                OnPropertyRaised("FriendsTabN");
                OnTabChanged(new TabNotificationsChangesEventArgs("Friends", value));
            }
        }

        private int n_requests_tab;
        public int RequestsTabN
        {
            get
            {
                return n_requests_tab;
            }
            set
            {
                n_requests_tab = value;
                OnPropertyRaised("RequestsTabN");
                OnTabChanged(new TabNotificationsChangesEventArgs("RequestsIncoming", value));
            }
        }

        private int n_requestsout_tab;
        public int RequestsOutgoingTabN
        {
            get
            {
                return n_requestsout_tab;
            }
            set
            {
                n_requestsout_tab = value;
                OnPropertyRaised("RequestsOutgoingTabN");
                OnTabChanged(new TabNotificationsChangesEventArgs("RequestsOutgoing", value));
            }
        }

        //private int n_logs_tab;
        //public int LogsTabN
        //{
        //    get
        //    {
        //        return n_logs_tab;
        //    }
        //    set
        //    {
        //        n_logs_tab = value;
        //    }
        //}

        private List<int> n_friends_list_remove;
        public List<int> FriendsRemovedN
        {
            get
            {
                return n_friends_list_remove;
            }
            set
            {
                n_friends_list_remove = value;
            }
        }

        public int FriendsRemovedNCount
        {
            get
            {
                return this.n_friends_list_remove.Count;
            }
        }

        private List<int> n_friends_list_add;
        public List<int> FriendsAddN
        {
            get
            {
                return n_friends_list_add;
            }
            set
            {
                n_friends_list_add = value;
            }
        }

        public int FriendsAddNCount
        {
            get
            {
                return this.n_friends_list_add.Count;
            }
        }

        private List<int> n_friends_list_messages;
        public List<int> FriendsMessageN
        {
            get
            {
                return n_friends_list_messages;
            }
            set
            {
                n_friends_list_messages = value;
            }
        }

        public int FriendsMessageNCount
        {
            get
            {
                return this.n_friends_list_messages.Count;
            }
        }

        private Dictionary<int, int> messageNotifications;
        public Dictionary<int, int> MessageNotifications
        {
            get
            {
                return messageNotifications;
            }
            set
            {
                messageNotifications = value;
            }
        }
        #endregion

        public Notifications()
        {
            n_friends_list_remove = new List<int>();
            n_friends_list_add = new List<int>();
            n_friends_list_messages = new List<int>();
            messageNotifications = new Dictionary<int, int>();

            GetNotifications();
        }

        private async void GetNotifications()
        {
            //Logu notificationu kiekis gali buti atrinktas labai papratai - pries atsijungiant uzfiksuoti kiek viso logu yra, prisijungus suskaiciuoti kiek nauju, o ju skirtumas - logu notificationai.
            //Friends notificationai - visi add/remove, teks serve prie byte[] pridedineti.
            //Requests Outgoing - kiek buvo cancelinta, isimta. Galima uzfiksuoti atsijungus.
            //Requests Incoming - kiek nauju atsirado. Jei dingo tai ignoruoti. Tad irgi galima uzfiksuoti atsijungus.
            //Kurie useriai parase - prideti serve.

            //Parsisiunciu paskutini issaugota irasa visu.
            //Parsisiunciu info esama
            //Palyginu.

            //serve notificationai yra istisai pridedami. Atsijungus nusiunciama su likusiais ir naujas irasas pakeicia serve. Kol useris atsijunges toliau pridedami notificationai.

            //Senu notificationu irasu parsisiuntimas

            //Logu parsisiuntimas
            List<int> notif_list = await Inst.ApiRequests.GetGeneralNotifications();
            if (notif_list?.Count == 3)
            {
                this.LogsBtnN = notif_list[2];
                //this.LogsTabN = this.LogsBtnN;//Temporary
                this.RequestsTabN = notif_list[0];
                this.RequestsOutgoingTabN = notif_list[1];
            }
            else
            {
                this.LogsBtnN = 0;
                this.RequestsOutgoingTabN = 0;
                this.RequestsTabN = 0;
            }

            this.FriendsBtnN = this.n_requests_tab + this.n_requestsout_tab;

            List<Tuple<int, int>> notifications = await Inst.ApiRequests.GetNotifications();
            if (notifications != null && notifications.Count > 0)
            {
                foreach (Tuple<int, int> item in notifications)
                {

                    AddNotification(item.Item2, item.Item1);

                    //switch (item.Item2)
                    //{
                    //    case 0:
                    //        {
                    //            //request incoming ++ 
                                
                    //            break;
                    //        }
                    //    case 1:
                    //        {
                    //            //request outgoing --
                    //            break;
                    //        }
                    //    case 2:
                    //        {
                    //            //friends -- (removed)
                    //            n_friends_list_remove.Add(item.Item1);
                    //            break;
                    //        }
                    //    case 3:
                    //        {
                    //            //friends ++ (added/accepted your request)
                    //            n_friends_list_add.Add(item.Item1);
                    //            break;
                    //        }
                    //    case 6:
                    //        {
                    //            //requests incoming -- (canceled)
                    //            break;
                    //        }
                    //    case 7:
                    //        {
                    //            //messages ++
                    //            n_friends_list_messages.Add(item.Item1);
                    //            break;
                    //        }
                    //    default:
                    //        {
                    //            break;
                    //        }
                    //}
                }

                //ReduceSuccessfullRequestNotifications();
            }

            //this.FriendsBtnN = this.n_requests_tab + this.n_requestsout_tab + this.n_friends_list_remove.Count + this.n_friends_list_add.Count + this.n_friends_list_messages.Count;
        }

        public void SaveNotifications()
        {
            PersistentNotifications pn = new PersistentNotifications();
            pn.Log_count = LogsBtnN;
            pn.RequestsIncoming_count = RequestsTabN;
            pn.RequestsOutgoing_count = RequestsOutgoingTabN;

            //this.n_friends_list_remove.Concat(this.n_friends_list_add).Concat(this.n_friends_list_messages).ToList().ForEach(pn.Friends_changes)
            this.n_friends_list_remove.ForEach(x => pn.Friends_changes.Add(new Tuple<int, int>(x, 0)));
            this.n_friends_list_add.ForEach(x => pn.Friends_changes.Add(new Tuple<int, int>(x, 1)));
            this.n_friends_list_messages.ForEach(x => pn.Friends_changes.Add(new Tuple<int, int>(x, 2)));

            Inst.ApiRequests.SaveNotificationsToServer(pn);
        }

        public void FriendsTabFocused()
        {
            this.FriendsTabN -= (this.n_friends_list_remove.Count + this.n_friends_list_add.Count);
            this.FriendsBtnN -= (this.n_friends_list_remove.Count + this.n_friends_list_add.Count);
            this.n_friends_list_remove.Clear();
            this.n_friends_list_add.Clear();
        }

        public void RequestsTabFocused()
        {
            this.FriendsBtnN -= this.RequestsTabN;
            this.RequestsTabN = 0;
        }

        public void RequestsOutgoingTabFocused()
        {
            this.FriendsBtnN -= RequestsOutgoingTabN;
            RequestsOutgoingTabN = 0;
        }

        public void AddNotification(int command, int user_id)
        {
            if (command >= 0)
            {


                switch (command)
                {
                    case 0:
                        {
                            AddNotificationRequestsIncoming();
                            break;
                        }
                    case 1:
                        {
                            AddNotificationRequestsOutgoing();
                            break;
                        }
                    case 2:
                        {
                            AddNotificationFriendsRemove(user_id);
                            break;
                        }
                    case 3:
                        {
                            AddNotificationFriendsAdd(user_id);
                            break;
                        }
                    case 4:
                        {
                            break;
                        }
                    case 5:
                        {
                            break;
                        }
                    case 6:
                        {
                            Inst.Utils.Notifications.RequestsTabN++;
                            Inst.Utils.Notifications.FriendsBtnN++;
                            break;
                        }
                    case 7:
                        {
                            Inst.Utils.Notifications.LogsBtnN++;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
            else
            {
                AddNotificationFriendsMessage(Math.Abs(command));
            }
        }

        public void AddNotificationRequestsIncoming()
        {
            this.RequestsTabN++;
            this.FriendsBtnN++;
        }

        public void AddNotificationRequestsOutgoing()
        {
            this.RequestsOutgoingTabN++;
            this.FriendsBtnN++;
        }

        public void AddNotificationFriendsAdd(int user_id)
        {
            this.FriendsAddN.Add(user_id);
            this.FriendsTabN++;
            this.FriendsBtnN++;
        }

        public void AddNotificationFriendsRemove(int user_id)
        {
            this.FriendsRemovedN.Add(user_id);
            this.FriendsTabN++;
            this.FriendsBtnN++;
        }

        public void AddNotificationFriendsMessage(int user_id)
        {
            this.FriendsMessageN.Add(user_id);
            if (this.messageNotifications.Keys.Contains(user_id))
            {
                this.messageNotifications[user_id]++;
            }
            else
            {
                this.messageNotifications.Add(user_id, 1);
                this.FriendsTabN++;
                this.FriendsBtnN++;
            }
        }

        //private void ReduceSuccessfullRequestNotifications()
        //{
        //    if (this.FriendsAddNCount > 0)
        //    {
        //        this.RequestsOutgoingTabN -= this.n_friends_list_add.Count;
        //    }
        //}

        public void RemoveMessageNotifications(UsersListLineViewModel user)
        {
            if (this.messageNotifications.Keys.Contains(user.UserId))
            {
                user.NotificationCount = 0;
                this.messageNotifications.Remove(user.UserId);
                this.FriendsMessageN.Remove(user.UserId);
                this.FriendsTabN--;
                this.FriendsBtnN--;
            }
        }

        public void UserWasRemoved(UsersListLineViewModel user)
        {
            if (this.messageNotifications.Keys.Contains(user.UserId))
            {
                this.FriendsTabN--;
                this.FriendsBtnN--;
            }
        }

        public void AddMessageNotifications(UsersListLineViewModel user)
        {
            if (this.messageNotifications.Keys.Contains(user.UserId))
            {
                this.messageNotifications[user.UserId]++;
                this.FriendsMessageN.Add(user.UserId);
            }
            else
            {
                AddNotificationFriendsMessage(user.UserId);
            }
            user.NotificationCount++;
        }

        public UsersListLineViewModel PopulateUserWithNotifications(UsersListLineViewModel user)
        {
            if (this.MessageNotifications.Keys.Contains(user.UserId))
            {
                user.NotificationCount = this.MessageNotifications[user.UserId];
                this.FriendsTabN++;
                this.FriendsBtnN++;
            }
            return user;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
    }

    public class PersistentNotifications
    {
        public int Log_count { get; set; }
        public int RequestsIncoming_count { get; set; }
        public int RequestsOutgoing_count { get; set; }
        public List<Tuple<int, int>> Friends_changes { get; set; }//0 - Removed; 1 - Added; 2 - Wrote a message.

        public PersistentNotifications()
        {
            Friends_changes = new List<Tuple<int, int>>();
        }
    }
}
