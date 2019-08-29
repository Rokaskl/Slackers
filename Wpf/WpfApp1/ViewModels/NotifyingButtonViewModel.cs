using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    class NotifyingButtonViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int notification_count;
        public int Notifications
        {
            get
            {
                return notification_count;
            }
            set
            {
                notification_count = value;
                OnPropertyRaised("Notifications");
                if (notification_count == 0 && visible)
                {
                    Visibility = false;
                }
                else
                {
                    if (notification_count > 0 && !visible)
                    {
                        Visibility = true;
                    }
                }
            }
        }

        private bool visible;
        public bool Visibility
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
                OnPropertyRaised("Visibility");
            }
        }

        public NotifyingButtonViewModel()
        {
           
        }

        private void OnPropertyRaised(string propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }
    }
}
