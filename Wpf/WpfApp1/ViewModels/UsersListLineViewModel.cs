using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfApp1.ViewModels
{
    public class UsersListLineViewModel : INotifyPropertyChanged
    {
        private object photo;
        public object Photo
        {
            get { return photo; }
            set { photo = value; OnPropertyRaised("Photo"); }
        }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }

        private Color status_color;
        public Color StatusColor
        {
            get { return status_color; }
            set { status_color = value;  OnPropertyRaised("StatusColor"); }
        }
        public int UserId { get; set; }
        public string Bio { get; set; }

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
                OnPropertyRaised("Visible");
            }
        }

        public UsersListLineViewModel() { }

        public UsersListLineViewModel(object Photo, string Name, string Lastname, string Username, Brush Brush, int UserId, string Bio)
        {
            this.StatusColor = ((SolidColorBrush)Brush).Color;
            
            this.Name = Name;
            this.Lastname = Lastname;
            this.Username = Username;
            if (Photo as ImageBrush != null)
            {
                this.Photo = Photo;
            }
            else
            {
                if (Photo as Brush != null)
                {
                    this.Photo = (Photo as SolidColorBrush);
                }
                else
                {
                    if (Photo == null)
                    {
                        this.Photo = Brushes.Gray;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(Photo.ToString()))
                        {
                            ImageBrush img_brush = new ImageBrush();
                            img_brush.ImageSource = Inst.PhotoBytes_to_Image(Convert.FromBase64String(Photo.ToString()));
                            this.Photo = img_brush;
                        }
                        else
                        {
                            this.Photo = Brushes.Gray;
                        }
                    }
                }
            }
            this.UserId = UserId;
            this.Bio = Bio;
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
