using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.ViewModels
{
    public class UsersListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //public double OpacityMask_gradient { get; set; }//Test paskui...
        public ObservableCollection<UsersListLineViewModel> Users { get; set; }
        //public bool Constantly_changing_property { get; set; }
        public ObservableCollection<string> ContextMenuItems { get; set; }

        private int notifications = 0;
        public int Notifications
        {
            get { return notifications; }
            set
            {
                notifications = value;
                OnPropertyRaised("Notifications");
                if (value == 0 && visibility)
                {
                    Visibility = false;
                }
                else
                {
                    if (value > 0 && !visibility)
                    {
                        Visibility = true;
                    }
                }
            }
        }

        private bool visibility = false;
        public bool Visibility
        {
            get
            {
                return visibility;
            }
            set
            {
                visibility = value;
                OnPropertyRaised("Visibility");
            }
        }

        public UsersListViewModel()
        {
            Users = new ObservableCollection<UsersListLineViewModel>();
            ContextMenuItems = new ObservableCollection<string>();
            //this.PropertyChanged += ChatViewModel_PropertyChanged;
        }

        public UsersListViewModel(List<string> cm_items)
        {
            Users = new ObservableCollection<UsersListLineViewModel>();
            ContextMenuItems = new ObservableCollection<string>();
            cm_items.ForEach(x => ContextMenuItems.Add(x));
            //this.PropertyChanged += ChatViewModel_PropertyChanged;
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
