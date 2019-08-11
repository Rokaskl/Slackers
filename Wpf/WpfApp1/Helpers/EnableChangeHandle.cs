using System.ComponentModel;

namespace WpfApp1.Helpers
{
    //[ImplementPropertyChanged]
    public class EnableChangeHandle : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        private bool isEnabled = true;

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }
            set
            {
                isEnabled = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsEnabled"));
            }
        }

    }
}
