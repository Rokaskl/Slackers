using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Controls.Chat;

namespace WpfApp1.ViewModels
{
    class FriendsChatFormViewModel : INotifyPropertyChanged
    {
        public List<ChatViewModel> Chats { get; set; }

        public FriendsChatFormViewModel()
        {
            Chats = new List<ChatViewModel>();
        }
        
        public ChatViewModel AddChat(UsersListLineViewModel user_vm)
        {
            if (Chats.All(x => x.Id != user_vm.UserId))
            {
                ChatViewModel chat = new ChatViewModel(user_vm);
                Chats.Add(chat);
                return chat;
            }
            else
            {
                return null;
            }
            //chat.ChatControl.FillChat();
        }

        public bool RemoveChat(ChatViewModel chat_vm)
        {
            this.Chats.Remove(chat_vm);
            return true;
        }

        public bool RemoveChat(int id)
        {
            this.Chats.Remove(this.Chats.Find(x => x.Id == id));
            return true;
        }

        public void AppendToChat(string text, string creator_id)
        {
            ChatViewModel chat = Chats.FirstOrDefault(x => x.Id == int.Parse(creator_id));
            if (chat != null)
            {
                chat.ChatControl.AppendChatLine(text, chat.Nickname, DateTime.Now, pCreatorId: int.Parse(creator_id));
            }   
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
