using System;
using System.Windows.Media;

namespace WpfApp1.ViewModels
{
    public class ChatLineViewModel
    {
        public string NickName { get; set; }
        public string Text { get; set; }
        public SolidColorBrush Brush { get; set; }
        public string CreateDate { get; set; }


        public ChatLineViewModel(string NickName, string Text, SolidColorBrush Brush, DateTime CreateDate)
        {
            this.NickName = NickName;
            this.Text = Text;
            this.Brush = Brush;
            this.CreateDate = CreateDate.ToString("hh:mm:ss yyyy-MM-dd");
        }

        public ChatLineViewModel(ChatLine chatline, bool local = false)
        {
            this.NickName = chatline.Username;
            this.Text = chatline.Text;
            if (chatline.CreatorId == int.Parse(Inst.ApiRequests.User.id) || local)
            {
                this.Brush = Brushes.AliceBlue;
            }
            else
            {
                this.Brush = Brushes.LightGray;
            }
            this.CreateDate = chatline.CreateDate.ToString("hh:mm:ss yyyy-MM-dd");
        }

        public ChatLineViewModel() { }
    }
}
