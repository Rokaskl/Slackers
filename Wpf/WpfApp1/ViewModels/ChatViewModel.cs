using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WpfApp1.Controls.Chat;

namespace WpfApp1.ViewModels
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ChatLineViewModel> ChatLines { get; set; }
        public ChatControl ChatControl { get; set; }

        private UsersListLineViewModel end_user_vm;
        public UsersListLineViewModel End_user_vm
        {
            get
            {
                return end_user_vm;
            }
            set
            {
                end_user_vm = value;
                OnPropertyRaised("End_user_vm");
            }
        }

        public bool Room;

        private int id;
        public int Id// id of the user with which the chat is held.
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                SetNickname();
            }
        }

        private string nickname;
        public string Nickname
        {
            get
            {
                return nickname;
            }
        }

        //private int notifications = 0;
        //public int Notifications
        //{
        //    get { return notifications; }
        //    set
        //    {
        //        notifications = value;
        //        OnPropertyRaised("Notifications");
        //        if (value == 0 && visibility)
        //        {
        //            Visibility = false;
        //        }
        //        else
        //        {
        //            if (value > 0 && !visibility)
        //            {
        //                Visibility = true;
        //            }
        //        }
        //    }
        //}

        //private bool visibility = false;
        //public bool Visibility
        //{
        //    get
        //    {
        //        return visibility;
        //    }
        //    set
        //    {
        //        visibility = value;
        //        OnPropertyRaised("Visibility");
        //    }
        //}

        //public bool Constantly_changing_property { get; set; }

        public ChatViewModel()
        {
            ChatLines = new ObservableCollection<ChatLineViewModel>();
            this.Room = true;
            //this.PropertyChanged += ChatViewModel_PropertyChanged;
        }

        public ChatViewModel(UsersListLineViewModel user_vm)//Might need whole user_vm in the future.
        {
            ChatLines = new ObservableCollection<ChatLineViewModel>();
            this.nickname = user_vm.Username;
            this.id = user_vm.UserId;
            this.Room = false;
            this.End_user_vm = user_vm;
        }

        //private void ChatViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{

        //}

        public void Add(ChatLineViewModel chatline)
        {
            ChatLines.Add(chatline);
            //Constantly_changing_property = !Constantly_changing_property;
            //PropertyChangedEventHandler handler = PropertyChanged;
            //handler.Invoke(this, new PropertyChangedEventArgs("ChatLines"));
        }

        private async void SetNickname()
        {
            this.nickname = await Inst.ApiRequests.GetUserNickname(id);
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
}
