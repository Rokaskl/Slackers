using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WpfApp1.ViewModels;

namespace WpfApp1.Controls.Chat.DesignTimeStaticClasses
{
    public class DesignTimeChatStatic : ChatViewModel
    {
        public static DesignTimeChatStatic Instance => new DesignTimeChatStatic();

        public DesignTimeChatStatic()
        {
            this.ChatLines = new ObservableCollection<ChatLineViewModel>()
            {
                new ChatLineViewModel("a", "texxxxxt", Brushes.AliceBlue, DateTime.Now),
                new ChatLineViewModel("b", "tehsrthxxxxxt", Brushes.LightGray, DateTime.Now),
                new ChatLineViewModel("jonas", "texxahteterhxxxt", Brushes.LightGray, DateTime.Now),
                new ChatLineViewModel("tom smith", "texxxxnsgfnfgnfgnxt", Brushes.LightGray, DateTime.Now),
                new ChatLineViewModel("Dan", "texxxxnsgfnfgnfgnxt gareigjkl mgklagh kaerhgl jgaerhg; raeghurghoaerg erghoieargheahg earn gjmbklk atblt't + g3r1e3galkern gleakrgh lkjgrakehglkerhglk aerglkeahr lgaerlkg haerlk ghalerkg haergjapek rj;lgkaerhgkl;", Brushes.LightGray, DateTime.Now),
            };
        }
    }
}
