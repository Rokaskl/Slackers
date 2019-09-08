using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.ViewModels;

namespace WpfApp1.Controls.Chat
{
    /// <summary>
    /// Interaction logic for ChatControl.xaml
    /// </summary>
    public partial class ChatControl : UserControl
    {
        public ScrollViewer ScrollViewer { get; set; }

        private ChatViewModel chat_vm;
        public ChatViewModel ChatViewModel
        {
            get
            {
                return chat_vm;
            }
            set
            {
                chat_vm = value;
                this.chat_vm.ChatControl = this;
            }
        }

        private bool room;

        private bool chat_flag = false;
        public int room_id { get; set; }
        public int items_per_page { get; set; }
        public bool chat_filled;//determines if chat was filled. Chat must be filled only once.

        public ChatControl()
        {
            InitializeComponent();
            this.current_page = 0;
            this.items_per_page = 20;
            this.DataContextChanged += ChatControl_DataContextChanged;
            this.GotFocus += ChatControl_GotFocus;
        }

        //To delete notifications
        private void ChatControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.chat_vm != null && this.chat_vm.End_user_vm?.NotificationCount > 0 && this.ScrollViewer != null && this.ScrollViewer.VerticalOffset == this.ScrollViewer.ScrollableHeight)
            {
                Inst.Utils.Notifications.RemoveMessageNotifications(this.chat_vm.End_user_vm);
            }
        }

        private async void ChatControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.ChatViewModel == null)
            {
                this.ChatViewModel = this.DataContext as ChatViewModel;
                this.room = this.chat_vm.Room;
                if (!chat_filled)
                {
                    FillChat();
                }
            }

            while (!chat_flag && this.ScrollViewer?.ScrollableHeight == 0)
            {
                await LoadPage();
                if (this.ScrollViewer.ScrollableHeight > 0)
                {
                    chat_flag = true;
                }
            }
        }

        //public ChatControl(int room_id, int items_per_page)
        //{
        //    InitializeComponent();
        //    this.room_id = room_id;
        //    this.items_per_page = items_per_page;
        //    this.current_page = 0;

        //    //this.KeyDown += Txt_entry_KeyUp;

        //    //FillChat???
        //}

        private void Btn_txtenter_Click(object sender, RoutedEventArgs e)
        {
            ProcessEntry();
        }

        private void ProcessEntry()
        {
            if (string.IsNullOrWhiteSpace(txt_entry.Text))
            {
                return;
            }
            AppendChatLine(txt_entry.Text, Inst.ApiRequests.User.username, DateTime.Now, pCreatorId: int.Parse(Inst.ApiRequests.User.id));
            SubmitEntry();
        }

        private void Txt_entry_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && this.txt_entry.IsFocused)//Sutvarkyti, kad visi vienu metu nesubmitintu.
            {
                ProcessEntry();
            }
        }

        private async void SubmitEntry()
        {
            try
            {
                //var response = await client.PostAsJsonAsync<string>($"/ChatLine/create/{this.room.roomId}", txt_entry.Text);
                if (/*response.IsSuccessStatusCode*/await Inst.ApiRequests.CreateEntry(txt_entry.Text, GetContextId(), this.room))
                {
                    txt_entry.Text = string.Empty;
                }
                else
                {
                    MessageBox.Show("Something went wrong! Could not create chat entry");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async void FillChat()
        {
            try
            {
                //var response = await client.GetAsync($"/ChatLine/lines/{this.room.roomId}");
                List<ChatLine> data = await Inst.ApiRequests.GetChat(GetContextId(), 0, this.items_per_page, this.room);
                if (data != null)
                {
                    data.ForEach(async x =>
                    {
                        x.Profile_image = await Inst.Utils.FindImageOf(x.CreatorId);
                    });
                    foreach (ChatLine line in data.OrderBy(x => x.CreateDate))
                    {
                        //line.Image = await FindImageOf(line.CreatorId);
                        await AppendChatLine(line.Text, line.Username, line.CreateDate, pImage: line.Profile_image, pCreatorId: line.CreatorId);
                        //chatbox.ChatLines.Add(new ChatLineViewModel(line));
                    }
                    await Task.Delay(1);
                    this.UpdateLayout();
                    GetScrollViewer();
                    this.chat_filled = true;
                }
                else
                {
                    MessageBox.Show("Something went wrong! Could not populate chat");
                }

                if (this.DataContext == null)
                {
                    this.DataContext = ChatViewModel;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task AppendChatLine(string text, string username, DateTime CreateDate, BitmapImage pImage = null, int pCreatorId = -1, bool Insert = false)
        {
            if (ChatViewModel.ChatLines.Count > 0)
            {
                ChatLineViewModel chatline_vm;
                if (Insert)
                {
                    chatline_vm = ChatViewModel.ChatLines.First();
                }
                else
                {
                    chatline_vm = ChatViewModel.ChatLines.Last();
                }

                DateTime last_chatline_createtime = DateTime.Parse(chatline_vm.CreateDate);
                DateTime added_chatline_createtime = CreateDate;
                if (chatline_vm.CreatorId == pCreatorId && last_chatline_createtime.Date == added_chatline_createtime.Date && last_chatline_createtime.Hour == added_chatline_createtime.Hour && last_chatline_createtime.Minute == added_chatline_createtime.Minute)
                {
                    ChatViewModel.ChatLines.Remove(chatline_vm);

                    if (Insert)
                    {
                        ChatViewModel.ChatLines.Insert(0, chatline_vm);
                        chatline_vm.Text = text + Environment.NewLine + chatline_vm.Text;
                    }
                    else
                    {
                        chatline_vm.Text += Environment.NewLine + text;
                        ChatViewModel.ChatLines.Add(chatline_vm);
                    }
                }
                else
                {
                    await AddChatLine();
                }

                if ((!Insert && DateTime.Parse(chatline_vm.CreateDate) > CreateDate)/* || (Insert && DateTime.Parse(chatline_vm.CreateDate) < CreateDate)*/)
                {
                    ReorderChatLines();
                }
            }
            else
            {
                await AddChatLine();
            }

            async Task AddChatLine()
            {
                ChatLine line = new ChatLine() { Id = null, CreateDate = CreateDate, Username = username, CreatorId = pCreatorId, RoomId = GetContextId(), Text = text };
                if (pImage == null)
                {
                    if (this.chat_vm != null && pCreatorId == ChatViewModel.Id)
                    {
                        line.Profile_image = (this.chat_vm.End_user_vm.Photo as ImageBrush)?.ImageSource as BitmapImage;
                    }
                    if (line.Profile_image == null)
                    {
                        line.Profile_image = await Inst.Utils.FindImageOf(pCreatorId);
                    }
                }
                else
                {
                    line.Profile_image = pImage;
                }

                if (Insert)
                {
                    ChatViewModel.ChatLines.Insert(0, new ChatLineViewModel(line));
                }
                else
                {
                    ChatViewModel.Add(new ChatLineViewModel(line));
                }
            }

            if (this.ScrollViewer != null)
            {
                HandleChatControlView(pCreatorId == int.Parse(Inst.ApiRequests.User.id), was_inserted: Insert);
            }
            else
            {
                GetScrollViewer();
            }
        }

        private async void GetScrollViewer()
        {
            //this.ScrollViewer = this.GetChildOfType<ScrollViewer>();
            
            if (this.ScrollViewer == null)
            {
                this.ScrollViewer = this.ChatScrollViewer;
                this.ScrollViewer.ScrollToEnd();
                this.ScrollViewer.ScrollChanged += RoomPage_ScrollChangedAsync;
            }
        }

        /// <summary>
        /// Handles ChatControl view after a new chat entry was appended.
        /// </summary>
        private void HandleChatControlView(bool creator_is_local, bool was_inserted = false)
        {
            if (this.ScrollViewer.VerticalOffset + this.ScrollViewer.ViewportHeight < this.ScrollViewer.ExtentHeight - this.ScrollViewer.ViewportHeight)
            {
                if (!creator_is_local && !was_inserted)
                {
                    System.Media.SystemSounds.Beep.Play();
                    if (!this.room)
                    {
                        Inst.Utils.Notifications.AddMessageNotifications(this.chat_vm.End_user_vm);
                    }
                    //Kazkokias vizualias priemones padaryti? Kontroliu mirgsejima..?
                }
                else
                {
                    if (creator_is_local && !was_inserted)
                    {
                        this.ScrollViewer.ScrollToEnd();
                    }
                }
            }
            else
            {
                this.ScrollViewer.ScrollToEnd();
            }
        }

        private async void RoomPage_ScrollChangedAsync(object sender, ScrollChangedEventArgs e)
        {
            if (e.OriginalSource.Equals(this.ScrollViewer))
            {


                if (e.VerticalOffset == 0)
                {
                    if (e.ExtentHeightChange > 0)
                    {
                        this.ScrollViewer.ScrollToVerticalOffset(e.ExtentHeightChange + e.ExtentHeightChange > 0 ? e.ViewportHeight + e.ExtentHeightChange - e.ViewportHeight : 0);
                    }
                    else
                    {
                        await LoadPage();
                    }
                }
                else
                {
                    if (this.chat_vm.End_user_vm?.NotificationCount > 0 && e.VerticalOffset == this.ScrollViewer.ScrollableHeight)
                    {
                        Inst.Utils.Notifications.RemoveMessageNotifications(this.chat_vm.End_user_vm);
                    }
                }
            }
        }

        private int current_page;
        private bool finished_loading_chat_page = true;

        public async Task LoadPage()
        {
            if (!finished_loading_chat_page)
            {
                return;
            }
            finished_loading_chat_page = false;
            List<ChatLine> data = await Inst.ApiRequests.GetChat(GetContextId(), current_page + 1, this.items_per_page, this.room);
            if (data != null && data.Count > 0)
            {
                //data.ForEach(async x =>
                //        {
                //            x.Image = await FindImageOf(x.CreatorId);
                //            chatbox.ChatLines.Insert(0, new ChatLineViewModel(x));
                //        }
                //            );
                data.ForEach(async x =>
                {
                    x.Profile_image = await Inst.Utils.FindImageOf(x.CreatorId);
                });
                data.ForEach(async x => await AppendChatLine(x.Text, x.Username, x.CreateDate, pImage: x.Profile_image, pCreatorId: x.CreatorId, Insert: true));
                current_page++;
            }
            else
            {
                //All chatline were loaded.
                chat_flag = true;
            }
            finished_loading_chat_page = true;
        }

        public void SetRoom_bool(bool value)
        {
            this.room = value;
        }

        private int GetContextId()
        {
            if (room)
            {
                return this.room_id;
            }
            else
            {
                return this.ChatViewModel.Id;
            }
        }

        private void ReorderChatLines()
        {

            List<ChatLineViewModel> chatlines_to_reorder = new List<ChatLineViewModel>();

            for (int i = this.ChatViewModel.ChatLines.Count - 1; i > 0; i--)
            {
                if (DateTime.Parse(this.ChatViewModel.ChatLines[i - 1].CreateDate) > DateTime.Parse(this.ChatViewModel.ChatLines[i].CreateDate))
                {
                    chatlines_to_reorder.Add(this.ChatViewModel.ChatLines[i]);
                    chatlines_to_reorder.Add(this.ChatViewModel.ChatLines[i - 1]);
                }
            }

            if (chatlines_to_reorder.Count > 0)
            {
                chatlines_to_reorder.Distinct().OrderBy(x => DateTime.Parse(x.CreateDate)).ToList().ForEach(y => AppendChatLine(y.Text, y.NickName, DateTime.Parse(y.CreateDate), (y.Photo as ImageBrush).ImageSource as BitmapImage, pCreatorId: y.CreatorId));
            }
        }
    }
}
