using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp1.ViewModels
{
    public class ChatLineViewModel
    {
        public string NickName { get; set; }
        public string Text { get; set; }
        public SolidColorBrush Brush { get; set; }
        public string CreateDate { get; set; }
        public string Time { get; set; }
        public object Photo { get; set; }


        public ChatLineViewModel(string NickName, string Text, SolidColorBrush Brush, DateTime CreateDate, BitmapImage Image)
        {
            this.NickName = NickName;
            this.Text = Text;
            this.Brush = Brush;
            this.CreateDate = CreateDate.ToString("hh:mm:ss yyyy-MM-dd");
            this.Time = CreateDate.ToString("hh:mm");
            this.Photo = new ImageBrush();

            if (Image == null)
            {
                this.Photo = Brushes.LightGray;
            }
            else
            {
                (this.Photo as ImageBrush).ImageSource = Image;
            }
        }

        public ChatLineViewModel(ChatLine chatline, bool local = false)
        {
            this.NickName = chatline.Username;
            this.Text = chatline.Text;
            if (chatline.CreatorId == int.Parse(Inst.ApiRequests.User.id) || local)
            {
                this.Brush = Brushes.White;
            }
            else
            {
                this.Brush = Brushes.AliceBlue;
            }
            this.CreateDate = chatline.CreateDate.ToString("hh:mm:ss yyyy-MM-dd");
            this.Time = chatline.CreateDate.ToString("hh:mm");
            this.Photo = new ImageBrush();

            if (chatline.Image == null)
            {
                this.Photo = Brushes.LightGray;
            }
            else
            {
                (this.Photo as ImageBrush).ImageSource = chatline.Image;
            }
        }

        public ChatLineViewModel() { }
    }
}
