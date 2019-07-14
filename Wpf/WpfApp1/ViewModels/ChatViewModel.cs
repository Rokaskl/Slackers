using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ChatLineViewModel> ChatLines { get; set; }
        //public bool Constantly_changing_property { get; set; }

        public ChatViewModel()
        {
            ChatLines = new ObservableCollection<ChatLineViewModel>();
            //this.PropertyChanged += ChatViewModel_PropertyChanged;
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

        public event PropertyChangedEventHandler PropertyChanged;


    }
}
