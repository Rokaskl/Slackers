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
            this.Owner = new UsersListLineViewModel(Image, null, null, Username, Brushes.Green, UserId, Bio);
            this.FriendsList = new UsersListViewModel(new List<string> { "Send message", "Remove"});
            this.Requests = new UsersListViewModel(new List<string> { "Accept", "Reject" });
            this.RequestsOutgoing = new UsersListViewModel(new List<string> { "Cancel" });
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public enum OwnerStatus
    {
        Online = 0,
        Offline = 1//Invisible
    }
}
