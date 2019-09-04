using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp1.ViewModels
{
    public class FriendsListViewModel : INotifyPropertyChanged
    {
        public UsersListLineViewModel Owner { get; set; }
        public UsersListViewModel FriendsList { get; set; }
        public UsersListViewModel Requests { get; set; }
        public UsersListViewModel RequestsOutgoing { get; set; }
        public OwnerStatus Status { get; set; }
        //public ObservableCollection<UsersListLineViewModel> Friends { get; set; }

        public FriendsListViewModel()
        {

        }

        public FriendsListViewModel(object Image, string Username, int UserId, string Bio)
        {
            this.Owner = new UsersListLineViewModel(Image, null, null, Username, Brushes.Green, UserId, null);//Bio is set to null deliberately so button would be invisible on the UsersListLineControl in FriendsListFrom(Owner view model).
            this.FriendsList = new UsersListViewModel(new List<string> { "Send message", "Call", "Remove"});
            this.Requests = new UsersListViewModel(new List<string> { "Accept", "Reject" });
            this.RequestsOutgoing = new UsersListViewModel(new List<string> { "Cancel" });
        }

        //public void Add_Notification(int tab_num, int user_id, int action)
        //{
        //    switch (tab_num)
        //    {
        //        case 0:
        //            {
        //                FriendsList.Notifications++;
        //                Inst.Utils.Notifications.FriendsTabN++;
        //                Inst.Utils.Notifications.FriendsBtnN++;
        //                break;
        //            }
        //        case 1:
        //            {
        //                Requests.Notifications++;
        //                Inst.Utils.Notifications.RequestsTabN++;
        //                Inst.Utils.Notifications.FriendsBtnN++;
        //                break;
        //            }
        //        case 2:
        //            {
        //                RequestsOutgoing.Notifications++;
        //                Inst.Utils.Notifications.RequestsOutgoingTabN++;
        //                Inst.Utils.Notifications.FriendsBtnN++;
        //                break;
        //            }
        //        default:
        //            {
        //                break;
        //            }
        //    }
        //}

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
    }

    public enum OwnerStatus
    {
        Online = 0,
        Offline = 1//Invisible
    }
}
