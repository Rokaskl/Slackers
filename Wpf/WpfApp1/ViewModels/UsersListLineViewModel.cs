using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfApp1.ViewModels
{
    public class UsersListLineViewModel
    {
        public object Photo { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Username { get; set; }
        //public SolidColorBrush StatusColor { get; set; }
        public Color StatusColor { get; set; }
        public int UserId { get; set; }
        public string Bio { get; set; }

        public UsersListLineViewModel() { }

        public UsersListLineViewModel(object Photo, string Name, string Lastname, string Username, Brush Brush, int UserId, string Bio)
        {
            this.StatusColor = ((SolidColorBrush)Brush).Color;
            
            this.Name = Name;
            this.Lastname = Lastname;
            this.Username = Username;
            if (Photo == null)
            {
                this.Photo = Brushes.Gray;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(Photo.ToString()))
                {
                    this.Photo = Inst.PhotoBytes_to_Image(Encoding.UTF8.GetBytes(Photo.ToString()));
                }
                else
                {
                    this.Photo = Brushes.Gray;
                }
            }
            this.UserId = UserId;
            this.Bio = Bio;
        }
    }
}
