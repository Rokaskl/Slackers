using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WpfApp1.ViewModels;

namespace WpfApp1.Controls.Chat.DesignTimeStaticClasses
{
    /// <summary>
    /// <see cref="ChatLineViewModel"/>
    /// </summary>
    public class DesignTimeChatLineStatic : ChatLineViewModel
    {
        public static DesignTimeChatLineStatic chatline => new DesignTimeChatLineStatic();

        public DesignTimeChatLineStatic()
        {
            this.NickName = "tim";
            this.Text = "hello world! blah blah blah blah";
            this.Brush = Brushes.White;
            this.CreateDate = DateTime.Now.ToString("hh:mm:ss yyyy-MM-dd");
        }

    }
}
