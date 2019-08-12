using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using WebApi.Dtos;
using WpfApp1.Forms;
using System.Diagnostics;
using WpfApp1.TimerControl;
using System.Reflection;
using System.Globalization;
using System.IO;
using WebApi.Entities;
using WpfApp1.ViewModels;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Interaction logic for RoomPage.xaml
    /// </summary>
    public partial class RoomPage : Page
    {
        //private HttpClient client;
        private AdditionalData roomAddData;
        private RoomDto room;
        private string prevWindow;
        private Timer timer;
        private Button btnStartStop;
        //private bool Changed;
        private bool breakDaRules = false;
        private int items_per_page = 10;
        private ChatViewModel chatbox = new ChatViewModel();
        private UsersListViewModel usersbox = new UsersListViewModel();
        private List<Tuple<int, BitmapImage>> User_images;
        /// <summary>
        /// This flag indicates that all the existing chatlines are loaded or scrollviewer is obtained. Default = false; when done = true;
        /// </summary>
        private bool chat_flag;


        public RoomPage(){}
        public RoomPage(RoomDto room,string prev)
        {
            Inst.Utils.RoomPage = this;
            this.prevWindow = prev;
            timer = new Timer();
            this.room = room;
            Inst.Utils.Room = new Room(room.roomId, room.roomName);
            this.User_images = new List<Tuple<int, BitmapImage>>() { new Tuple<int, BitmapImage>(int.Parse(Inst.ApiRequests.User.id), Inst.PhotoBytes_to_Image(Inst.ApiRequests.AdditionalData.PhotoBytes)) };
            //client = Inst.Utils.HttpClient;

            //chatbox = new ChatViewModel();
            

            InitializeComponent();

            

            SetupChatBox();//Nieko nedaro
            RoomInfo();
            ListUsers();
            //ListUsersStack();

            FillNotes();
            FillChat();

            //this.ChatControl.DataContext = chatbox;

            //this.ChatControl.SetResourceReference((ChatControl.DataContext as DependencyProperty), "chatbox");

            //this.chatbox.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
            this.KeyDown += RoomPage_KeyDown;
            this.txt_entry.KeyUp += Txt_entry_KeyUp;
            this.NoteListView.SelectionMode = SelectionMode.Single;
            this.NoteListView.MouseLeftButtonUp += NoteListView_MouseLeftButtonUp;
            //this.usersListView.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged1;

            //(this.MembersListView.View as GridView).Columns.Add(new GridViewColumn
            //{
            //    //Header = "Id",
            //    Header = "Username",
            //    DisplayMemberBinding = new Binding("username"),
            //    Width = 100 
            //});
            //(this.MembersListView.View as GridView).Columns.Add(new GridViewColumn
            //{
            //    Header = "Status",
            //    DisplayMemberBinding = new Binding("status"),
            //    Width = 100
            //});

            InitCmbStatus();
            ConfigureBotStack();
            //FillMembers();//pirma karta uzkrauna iskarto.
            

            Task.Run(() => DisplayMembersR());//toliau naujina info kas 10secs.
        }

        private void SetupChatBox()
        {
            //this.chatbox.ItemTemplate =
        }

        //private void ItemContainerGenerator_StatusChanged1(object sender, EventArgs e)
        //{
        //    if (this.usersListView.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
        //    {
        //        foreach(Dictionary<string,object> line in (sender as System.Windows.Controls.ItemContainerGenerator).Items)
        //        {
        //            ListViewItem lv = (ListViewItem)usersListView.ItemContainerGenerator.ContainerFromItem(line);
        //            if (lv != null)
        //            {
        //                if (line["userId"].ToString() == Inst.ApiRequests.User.id)
        //                {
        //                    lv.Background = Brushes.AliceBlue;
        //                }
        //            }
        //        }
        //    }
        //}

        //private void UsersListView_MouseLeave(object sender, MouseEventArgs e)
        //{
        //    //CloseBioPop();
        //}

        private Popup userBioPOP;
        private Popup roomBioPOP;

        //private void UsersListView_MouseEnter(object sender, RoutedEventArgs e)
        //{
        //    if ((string)((Ellipse)sender).Tag!=null )
        //    {
        //        userBioPOP = CreateBioPopup(((Ellipse)sender).Tag.ToString());
        //    }            
        //}

        private Popup CreateBioPopup(string text)
        {
            Popup popup = new Popup();
            popup.MouseLeave += Bio_LostFocus;
            popup.Width = 150;
            popup.Height = 100;

            StackPanel stack = new StackPanel();
            stack.VerticalAlignment = VerticalAlignment.Stretch;
            stack.HorizontalAlignment = HorizontalAlignment.Stretch;
            stack.Background = Brushes.LightGray;

            TextBlock bio = new TextBlock();
            bio.Text = text;
            stack.Children.Add(bio);
            popup.Child = stack;

            popup.StaysOpen = false;
            popup.Placement = PlacementMode.MousePoint;
            popup.Visibility = Visibility.Visible;
            popup.PopupAnimation = PopupAnimation.Slide;
            popup.IsOpen = true;
            return popup;
        }

        private void CloseBioPop()
        {
            if (userBioPOP != null)
            {
                userBioPOP.IsOpen=false;
            }
        }

        private void Bio_LostFocus(object sender, RoutedEventArgs e)
        {
            (sender as Popup).IsOpen = false;
            (sender as Popup).MouseLeave -= Bio_LostFocus;
        }

        private async void RoomInfo()
        {
            this.roomAddData = await Inst.ApiRequests.GetRoomAddData(this.room.roomId);
            Ellipse roomPhoto = new Ellipse();
            roomPhoto.Width = 50;
            roomPhoto.Height = 50;
            //roomPhoto.Margin = new Thickness(2, 2, 2, 2);
            roomPhoto.MouseLeftButtonUp += RoomPhoto_MouseLeftButtonUp;
            if(roomAddData!=null&&roomAddData.PhotoBytes!=null)
            using (var memstr = new MemoryStream(roomAddData.PhotoBytes))
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad; // here
                        image.StreamSource = memstr;
                        image.EndInit();
                        ImageBrush imgBrush = new ImageBrush();
                        imgBrush.ImageSource = image;
                        roomPhoto.Fill = imgBrush;
                    //Test
                    this.room_page_background_imgbrush.ImageSource = image;
                    //Test
                }
            roomPhoto.Tag = roomAddData.Biography;
            this.roomInfo.Children.Add(roomPhoto);
            Label roomNameLabel = new Label();
            roomNameLabel.Content = this.room.roomName;
            roomNameLabel.FontSize = 20;
            roomNameLabel.Foreground = Brushes.Black;
            roomNameLabel.Margin = new Thickness(2, 2, 2, 2);
            this.roomInfo.Children.Add(roomNameLabel);

            
        }

        private void RoomPhoto_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            roomBioPOP = CreateBioPopup(this.roomAddData.Biography);
        }

        //private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        //{
        //    if (Changed && this.chatbox.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
        //    {
        //        foreach(ChatLine line in (sender as System.Windows.Controls.ItemContainerGenerator).Items)
        //        {
        //            ListBoxItem lv = (ListBoxItem)chatbox.ItemContainerGenerator.ContainerFromItem(line);
        //            if (lv != null)
        //            {
        //                lv.ToolTip = line.CreateDate.ToString("HH:mm:ss yyyy/MM/dd", CultureInfo.InvariantCulture);
        //                if (line.CreatorId.ToString() == Inst.ApiRequests.User.id)
        //                {
        //                    lv.Background = Brushes.LightBlue;
        //                }
        //                else
        //                {
        //                    lv.Background = Brushes.LightGray;
        //                }
        //            }
        //        }
        //        Changed = false;
        //    }
        //}

        private void Txt_entry_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessEntry();
            }
        }

        private void NoteListView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ListViewItem lv = (ListViewItem)NoteListView.ItemContainerGenerator.ContainerFromItem((sender as ListView).SelectedItem);
            if (lv != null)
            {
                if (popuptemp.popup != null)
                {
                    popuptemp.popup.IsOpen = false;
                }
                popuptemp = PopupEdit((sender as ListView).SelectedItem as Note);
                popuptemp.popup.IsOpen = true;
            }
            
            
        }

        private void RoomPage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)//Refresh notes list
            {
                FillNotes();
            }
            if (e.Key == Key.Delete)
            {
                DeleteNote();
            }
            if (e.Key == Key.Escape)
            {
                PopupsClose();
            }
        }

        private void InitCmbStatus()
        {
            this.cmbStatus.Items.Add("Active");
            this.cmbStatus.Items.Add("Away");
            this.cmbStatus.Items.Add("Don't disturb");
            this.cmbStatus.SelectedItem = "Active";
            this.cmbStatus.SelectionChanged += CmbStatus_SelectionChanged;
        }

        private void CmbStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeStatus(e.AddedItems[0].ToString());
        }

        private async void ChangeStatus(string status)
        {
            try
            {
                switch (status)
                {
                    case "Active":
                        {
                            DisplaySelectedStatus(await Inst.ApiRequests.UpdateStatus(this.room.roomId,'A')/*client.GetAsync($"/Rooms/status/{this.room.roomId}/A")*/, "Active");
                            break;
                        }
                    case "Away":
                        {
                            DisplaySelectedStatus(await Inst.ApiRequests.UpdateStatus(this.room.roomId,'B')/*await client.GetAsync($"/Rooms/status/{this.room.roomId}/B")*/, "Away");
                            break;
                        }
                    case "Don't disturb":
                        {
                            DisplaySelectedStatus(await Inst.ApiRequests.UpdateStatus(this.room.roomId,'C')/*await client.GetAsync($"/Rooms/status/{this.room.roomId}/C")*/, "Don't disturb");
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                void DisplaySelectedStatus(/*HttpResponseMessage*/bool response, string item)
                {
                    if (response/*.IsSuccessStatusCode*/)
                    {
                        this.cmbStatus.SelectedItem = item;
                        //foreach(var x in this.MembersListView.Items)
                        //{
                        //    if (x.GetType().GetProperty("username").GetValue(x).ToString() == Inst.ApiRequests.User.username)
                        //    {
                        //        int index = this.MembersListView.Items.IndexOf(x);
                        //        this.MembersListView.Items.Remove(x);
                        //        this.MembersListView.Items.Insert(index, new { username = Inst.ApiRequests.User.username, status = item});
                        //        break;
                        //    }
                        //}
                    }
                } 
            }
            catch (Exception exception)
            {

                Console.WriteLine(exception.ToString());
            }
        }

        private void ConfigureBotStack()
        {
            btnStartStop = new Button();
            btnStartStop.Content = "Start!";
            btnStartStop.VerticalAlignment = VerticalAlignment.Center;
            btnStartStop.HorizontalAlignment = HorizontalAlignment.Stretch;
            btnStartStop.Click += btnStartStop_Click;
            btnStartStop.Height = 25;
            btnStartStop.Width = 50;
            
            timer.HorizontalAlignment = HorizontalAlignment.Stretch;
            timer.Margin = new Thickness(0, 0, 0, 0);
            timer.VerticalAlignment = VerticalAlignment.Center;
            timer.FlowDirection = FlowDirection.LeftToRight;
            this.botomStack.Children.Add(timer);
            this.botomStack.Children.Add(btnStartStop);
            
        }

        private void btnStartStop_Click(object sender, RoutedEventArgs e)
        {
            if ((e.OriginalSource as Button).Content.ToString() == "Start!")
            {
                StartTimer();
                //(e.OriginalSource as Button).Content = "Stop!";
            }
            else
            {
                StopTimer();
                //(e.OriginalSource as Button).Content = "Start!";
            }
        }

        private async void StartTimer()
        {
            try
            {
                //var response = await client.GetAsync($"/TimeTracker/mark/{room.roomId}/1");
                if (/*response.IsSuccessStatusCode*/await Inst.ApiRequests.TimerMark(room.roomId,1))
                {
                    this.timer.Start();
                    this.btnStartStop.Content = "Stop!";
                }
                else
                {
                    MessageBox.Show("Something went wrong!");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async void StopTimer()
        {
            try
            {
                //var response = await client.GetAsync($"/TimeTracker/mark/{room.roomId}/0");
                if (/*response.IsSuccessStatusCode*/await Inst.ApiRequests.TimerMark(room.roomId,0))
                {
                    this.timer.Stop();
                    this.btnStartStop.Content = "Start!";
                }
                else
                {
                    MessageBox.Show("Something went wrong!");
                    //return false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //return false;
            }
        }

        //private void DisplayMembers()
        //{
        //    int time = 0;
        //    Stopwatch stopWatch = new Stopwatch();
        //    stopWatch.Start();
        //    while (true)
        //    {
        //        if (time + 10 <= stopWatch.Elapsed.TotalSeconds)
        //        {
        //            if (true/*!FillMembers().Result*/)
        //            {
        //                break;
        //            }
        //            time = (int)stopWatch.Elapsed.TotalSeconds;
        //        }
        //    }
        //}
        private async Task DisplayMembersR()
        {
            while (true)
            {
                await Task.Delay(10000);

                await this.Dispatcher.Invoke(async () =>
                {
                    await ListUsers();
                });
                //ListUsersStack();
                
                //if (this.usersListView.Items.Count == 0/*!FillMembers().Result*/)
                //{
                //    break;
                //}
            }
        }
        //private async Task<bool> FillMembers()
        //{
        //    bool end = true;
        //    try
        //    {
        //        //var response = await client.GetAsync($"/Rooms/group/{this.room.roomId}");
        //        List<Dictionary<User,string> >resp = await Inst.ApiRequests.GetGroupMembers(this.room.roomId);                
        //        if (/*response.IsSuccessStatusCode*/resp!=null)
        //        {
        //            this.Dispatcher.Invoke((Action)(() =>
        //            {
        //                List<Dictionary<string, string>> respListDict = new List<Dictionary<string, string>>();
        //                //List<Newtonsoft.Json.Linq.JObject> resp = response.Content.ReadAsAsync<List<Newtonsoft.Json.Linq.JObject>>().Result;

        //                resp.ForEach(x =>
        //                {
        //                    Dictionary<string, string> respDict = new Dictionary<string, string>();
        //                    foreach (var key in x)
        //                    {
        //                        if (key.Value?.ToString() != null)
        //                        {
        //                            respDict.Add(key.Key, key.Value.ToString());
        //                        }
        //                    }
        //                    respDict.Add("status", x.GetValue("value").ToObject<string>());
        //                    respListDict.Add(respDict);
        //                });
        //               // this.MembersListView.Items.Clear();

        //                //(this.MembersListView.View as GridView).Columns.Add(new GridViewColumn());
        //                //this.MembersGrid.
        //                //DependencyProperty dp = DependencyProperty.Register("username", typeof(string), typeof(Dictionary<string, string>));
        //                respListDict.ForEach(x =>
        //                {
        //                        //Brush b = Brushes.Gray;
        //                        string status = string.Empty;
        //                    switch (x["status"])
        //                    {
        //                        case "A":
        //                            {
        //                                    //b = Brushes.Green;
        //                                    status = "Active";
        //                                break;
        //                            }
        //                        case "B":
        //                            {
        //                                    //b = Brushes.Yellow;
        //                                    status = "Away";
        //                                break;
        //                            }
        //                        case "C":
        //                            {
        //                                    //b = Brushes.Red;
        //                                    status = "Don't disturb!";
        //                                break;
        //                            }
        //                        default: break;
        //                    }
        //                        //ListViewItem lvi = new ListViewItem() { /*Content = "username",*/ Background = b };
        //                        //lvi.SetValue(dp, x);
        //                        //this.MembersListView.Items.Add(new { username = x["username"], status = status });
        //                });
        //                //this.MembersListView.ItemsSource = respListDict;
        //                //ChangeStatuses(respListDict);

        //            }));
        //            //this.MembersListView
        //        }
        //        else if (resp == null)
        //        {
        //            Inst.Utils.MainWindow.frame1.Navigate(new UserPage());
        //            end = false;
        //        }
        //        else
        //        {
        //            MessageBox.Show("Something went wrong!");                    
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //    }
        //    return end;
        //}

        private void BtnLogoutRoom_Click(object sender, RoutedEventArgs e)
        {
            LogoutFromRoom();
        }

        public  bool Logout()
        {
            LogoutFromRoom();
            return true;
        }

        private async void LogoutFromRoom()
        {
            try
            {
                //var response = await client.GetAsync($"/Rooms/logout_group/{room.roomId}");
                if (/*response.IsSuccessStatusCode*/await Inst.ApiRequests.LogoutGroup(this.room.roomId))
                {
                    Close();
                }
                else
                {
                    MessageBox.Show("Something went wrong!");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }

        public void Close()
        {
            if (this.timer.IsRunning)
            {
                StopTimer();
            }
            Inst.Utils.MainWindow.room.Visibility = Visibility.Hidden;
            Inst.Utils.MainWindow.frame1.Refresh();
            Inst.Utils.MainWindow.frame2.Refresh();
            if (prevWindow == "admin")
            {
                Inst.Utils.MainWindow.tabs.SelectedIndex = 0;
                //Inst.Utils.MainWindow.frame2.NavigationService.Navigate(new Admin());
            }
            else
                Inst.Utils.MainWindow.tabs.SelectedIndex = 1;
            //Inst.Utils.MainWindow.frame1.NavigationService.Navigate(new UserPage());
            Inst.Utils.RoomPage = null;
        }

        public void UpdateNoteListView()
        {
            FillNotes();
        }

        private async Task<BitmapImage> FindImageOf(int id)
        {
            Tuple<int, BitmapImage> item;
            if ((item = User_images.FirstOrDefault(x => x.Item1 == id)) != null)
            {
                return item.Item2;
            }
            else
            {
                AdditionalData addata = await Inst.ApiRequests.GetUserAddData(id);
                byte[] result = addata.PhotoBytes;
                //BitmapImage image = Inst.PhotoBytes_to_Image(Inst.ApiRequests.GetUserAddData(id).Result?.PhotoBytes);//Problem
                BitmapImage image = Inst.PhotoBytes_to_Image(result);
                User_images.Add(new Tuple<int, BitmapImage>(id, image));
                return image;
            }
        }

        public void UpdateUsersListView()
        {
            this.Dispatcher.Invoke(ListUsers);
            Inst.Utils.Room.SetUsersList();//Galima butu naudoti is ListUsers metodo gauta info.
            //FillMembers();
        }

        public void UpdateGroupChat(string text, string username, string creator_id)
        {
            AppendChatLine(text, username, DateTime.Now, pCreatorId : int.Parse(creator_id));
            //FillChat();
        }

        private async Task AppendChatLine(string text, string username, DateTime CreateDate, BitmapImage pImage = null, int pCreatorId = -1, bool Insert = false)
        {
            if (this.chatbox.ChatLines.Count > 0)
            {
                ChatLineViewModel chatline_vm;
                if (Insert)
                {
                    chatline_vm = this.chatbox.ChatLines.First();
                }
                else
                {
                    chatline_vm = this.chatbox.ChatLines.Last();
                }
                 
                DateTime last_chatline_createtime = DateTime.Parse(chatline_vm.CreateDate);
                DateTime added_chatline_createtime = CreateDate;
                if (chatline_vm.CreatorId == pCreatorId && last_chatline_createtime.Date == added_chatline_createtime.Date && last_chatline_createtime.Hour == added_chatline_createtime.Hour && last_chatline_createtime.Minute == added_chatline_createtime.Minute)
                {
                    this.chatbox.ChatLines.Remove(chatline_vm);
                    
                    if (Insert)
                    {
                        this.chatbox.ChatLines.Insert(0, chatline_vm);
                        chatline_vm.Text = text + Environment.NewLine + chatline_vm.Text;
                    }
                    else
                    {
                        chatline_vm.Text += Environment.NewLine + text;
                        this.chatbox.ChatLines.Add(chatline_vm);
                    }
                }
                else
                {
                    await AddChatLine();
                }
            }
            else
            {
                await AddChatLine();
            }

            async Task AddChatLine()
            {
                ChatLine line = new ChatLine() { Id = null, CreateDate = CreateDate, Username = username, CreatorId = pCreatorId, RoomId = this.room.roomId, Text = text};
                if (pImage == null)
                {
                    line.Profile_image = await FindImageOf(pCreatorId);
                }
                else
                {
                    line.Profile_image = pImage;
                }

                if (Insert)
                {
                    this.chatbox.ChatLines.Insert(0, new ChatLineViewModel(line));
                }
                else
                {
                    this.chatbox.Add(new ChatLineViewModel(line));
                }
            }

            if (this.ChatControl.ScrollViewer != null)
            {
                HandleChatControlView(pCreatorId == int.Parse(Inst.ApiRequests.User.id), was_inserted : Insert);
            }
            else
            {
                GetScrollViewer();
            }
        }

        private void GetScrollViewer()
        {
            this.ChatControl.ScrollViewer = this.ChatControl.GetChildOfType<ScrollViewer>();
            if (this.ChatControl.ScrollViewer != null)
            {
                this.ChatControl.ScrollViewer.ScrollToEnd();
                this.ChatControl.ScrollViewer.ScrollChanged += RoomPage_ScrollChangedAsync;

            }

            if (!chat_flag && this.ChatControl.ScrollViewer.ScrollableHeight == 0)
            {
                LoadPage();
                if (this.ChatControl.ScrollViewer.ScrollableHeight > 0)
                {
                    chat_flag = true;
                }
            }
        }

        /// <summary>
        /// Handles ChatControl view after a new chat entry was appended.
        /// </summary>
        private void HandleChatControlView(bool creator_is_local, bool was_inserted = false)
        {
            if (this.ChatControl.ScrollViewer.VerticalOffset + this.ChatControl.ScrollViewer.ViewportHeight < this.ChatControl.ScrollViewer.ExtentHeight - this.ChatControl.ScrollViewer.ViewportHeight)
            {
                if (!creator_is_local && !was_inserted)
                {
                    System.Media.SystemSounds.Beep.Play();
                    //Kazkokias vizualias priemones padaryti? Kontroliu mirgsejima..?
                }
            }
            else
            {
                this.ChatControl.ScrollViewer.ScrollToEnd();
            }
        }

        private async void FillNotes()
        {
            PopupsClose();

            try
            {
                //var response = await client.GetAsync($"/Notes/{room.roomId}");
                 List<Note> info = await Inst.ApiRequests.GetRoomNotes(this.room.roomId);
                if (/*response.IsSuccessStatusCode*/info!=null)
                {
                    //notelist.ItemsSource = response;
                    //List<Dictionary<string, string>> info = response.Content.ReadAsAsync<Newtonsoft.Json.Linq.JArray>().Result.ToObject(typeof(List<Dictionary<string, string>>));
                    //List<Note> info = response.Content.ReadAsAsync<List<Note>>().Result;
                    NoteListView.ItemsSource = info;
                }
                else
                {
                    MessageBox.Show("Something went wrong! Could not update Notes list");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async void SubmitNote()
        {
            if (string.IsNullOrWhiteSpace(popupnew.body.Text))
            {
                return;
            }

            Dictionary<string, string> info = new Dictionary<string, string>() { { "roomId", this.room.roomId.ToString() }, {"message", popupnew.body.Text }, { "header", popupnew.header.Text} };
            

            try
            {
                //var response = await client.PostAsJsonAsync<Dictionary<string, string>>($"/Notes/submit", info);
                if (/*response.IsSuccessStatusCode*/await Inst.ApiRequests.SubmitNote(info))
                {
                    FillNotes();
                }
                else
                {
                    MessageBox.Show("Something went wrong! Could not submit Note");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async void ModifyNote(Note note)
        {
            try
            {
                //var response = await client.PostAsJsonAsync<Note>($"/Notes/modify", note);
                if (/*response.IsSuccessStatusCode*/await Inst.ApiRequests.ModNote(note))
                {
                    //FillNotes();
                }
                else
                {
                    MessageBox.Show("Something went wrong! Could not submit Note");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void DeleteNote()
        {
            if (this.NoteListView.SelectedItem != null)
            {
                Note note = this.NoteListView.SelectedItem as Note;
                if(MessageBox.Show($"Delete Note (ID:{note.Id}) ?", "Warning!", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
                {
                    SubmitDelete(this.room.roomId, note.Id);
                }
            }
            
        }

        private async void SubmitDelete(int roomId, int noteId)
        {
            try
            {
                //var response = await client.GetAsync($"/Notes/delete/{roomId}/{noteId}");
                if (/*response.IsSuccessStatusCode*/await Inst.ApiRequests.DelNote(roomId,noteId))
                {
                    FillNotes();
                }
                else
                {
                    MessageBox.Show("Something went wrong! Could not delete Note");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void Btn_addnote_Click(object sender, RoutedEventArgs e)
        {
            if (popupnew.popup == null)
            {
                popupnew = PopupEdit();
            }
            popupnew.popup.IsOpen = true;
        }

        private MyPopup popupnew;
        private MyPopup popuptemp;

        struct MyPopup
        {
            public Note note;
            public Popup popup;
            public TextBox header;
            public TextBox body;
        };

        private MyPopup PopupEdit(Note note = null)
        {
            string pheader = string.Empty;
            string pmessage = string.Empty;

            if (note != null)
            {
                pheader = note.Header;
                pmessage = note.Message;
            }
            
            Popup popup = new Popup();
            popup.StaysOpen = false;
            popup.AllowsTransparency = true;
            popup.Height = 400;
            popup.Width = 500;
            popup.Placement = PlacementMode.Mouse;
            StackPanel panel = new StackPanel();
            TextBox textbox = new TextBox();
            textbox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            textbox.TextWrapping = TextWrapping.Wrap;
            popup.Child = panel;
            TextBox header = new TextBox() { Text = string.IsNullOrWhiteSpace(pheader) ? "Header" : pheader, Background = Brushes.LightGray };
            panel.Children.Add(header);
            panel.Children.Add(textbox);
            
            if (note == null)
            {
                Button btnSubmit = new Button() { Content = "Submit" };
                btnSubmit.Click += (s, e) => {
                    if (MessageBox.Show("Submit?", "", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Cancel) == MessageBoxResult.Yes)
                    {
                        SubmitNote();
                    }
                };
                panel.Children.Add(btnSubmit);
            }
            else
            {
                Button btnModify = new Button() { Content = "Modify" };
                btnModify.Click += (s, e) => {
                    if (MessageBox.Show("Modify?", "", MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Cancel) == MessageBoxResult.Yes)
                    {
                        note.Header = popuptemp.header.Text;
                        note.Message = popuptemp.body.Text;
                        ModifyNote(note);
                    }
                };
                panel.Children.Add(btnModify);
            }

            textbox.Height = 350;
            textbox.Width = 500;
            textbox.Background = Brushes.White;
            textbox.VerticalContentAlignment = VerticalAlignment.Top;
            textbox.AcceptsReturn = true;
            textbox.AcceptsTab = true;
            popup.PopupAnimation = PopupAnimation.Fade;
            popup.Visibility = System.Windows.Visibility.Visible;
            popup.KeyDown += Popup_KeyDown;
            textbox.Text = string.IsNullOrWhiteSpace(pmessage) ? string.Empty : pmessage;
            return new MyPopup() { popup = popup, body = textbox, header = header, note = note};
        }

        private void Popup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                (sender as Popup).IsOpen = false;
            }
        }

        private void PopupsClose()
        {
            if (popupnew.popup != null)
                popupnew.popup.IsOpen = false;
            if (popuptemp.popup != null)
                popuptemp.popup.IsOpen = false;
        }

        private async void FillChat()
        {
            try
            {
                //var response = await client.GetAsync($"/ChatLine/lines/{this.room.roomId}");
                List<ChatLine> data = await Inst.ApiRequests.GetChat(this.room.roomId, 0, items_per_page);
                if (data!=null)
                {
                    data.ForEach(async x => 
                    {
                        x.Profile_image = await FindImageOf(x.CreatorId);
                    });
                    foreach (ChatLine line in data.OrderBy(x => x.CreateDate))
                    {
                        //line.Image = await FindImageOf(line.CreatorId);
                        await AppendChatLine(line.Text, line.Username, line.CreateDate, pImage: line.Profile_image, pCreatorId : line.CreatorId);
                        //chatbox.ChatLines.Add(new ChatLineViewModel(line));
                    }
                    await Task.Delay(1);
                    this.ChatControl.UpdateLayout();
                    GetScrollViewer();
                }
                else
                {
                    MessageBox.Show("Something went wrong! Could not populate chat");
                }

                current_page = 0;

                this.ChatControl.DataContext = chatbox;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Nukelia ListViewItemu teksta ir padidina pati itema atitinkamai. Tai padaro pirmiems items_per_page itemams
        /// </summary>
        //private void FormatListViewItems()
        //{
        //    for (int i = 0; i < items_per_page; i++)
        //    {
        //        ListViewItem lwi = (chatbox.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem);
        //        string visual_text = (lwi.Content as ChatLine).Username + (lwi.Content as ChatLine).Text;
        //        lwi.
        //    }
        //}

        //private void ChatControl_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        //{

        //}

        private async void RoomPage_ScrollChangedAsync(object sender, ScrollChangedEventArgs e)
        {
            if (e.VerticalOffset == 0)
            {
                if (e.ExtentHeightChange > 0)
                {
                    this.ChatControl.ScrollViewer.ScrollToVerticalOffset(e.ExtentHeightChange + e.ExtentHeightChange > 0 ? e.ViewportHeight + e.ExtentHeightChange - e.ViewportHeight : 0);
                }
                else
                {
                    await LoadPage();
                }
            }
        }

        private int current_page;
        private bool finished_loading_chat_page = true;

        private async Task LoadPage()
        {
            if (!finished_loading_chat_page)
            {
                return;
            }
            finished_loading_chat_page = false;
            List<ChatLine> data = await Inst.ApiRequests.GetChat(this.room.roomId, current_page + 1, items_per_page);
            if (data != null)
            {
                //data.ForEach(async x =>
                //        {
                //            x.Image = await FindImageOf(x.CreatorId);
                //            chatbox.ChatLines.Insert(0, new ChatLineViewModel(x));
                //        }
                //            );
                data.ForEach(async x =>
                {
                    x.Profile_image = await FindImageOf(x.CreatorId);
                });
                data.ForEach(async x => await AppendChatLine(x.Text, x.Username, x.CreateDate, pImage : x.Profile_image, pCreatorId : x.CreatorId, Insert: true));
                current_page++;
            }
            else
            {
                //All chatline were loaded.
                chat_flag = true;
            }
            finished_loading_chat_page = true;
        }

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
            AppendChatLine(txt_entry.Text, Inst.ApiRequests.User.username, DateTime.Now, pCreatorId : int.Parse(Inst.ApiRequests.User.id));
            SubmitEntry();
        }

        private async void SubmitEntry()
        {
            try
            {
                //var response = await client.PostAsJsonAsync<string>($"/ChatLine/create/{this.room.roomId}", txt_entry.Text);
                if (/*response.IsSuccessStatusCode*/await Inst.ApiRequests.CreateEntry(txt_entry.Text,this.room.roomId))
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
        public async Task<bool> ListUsers()//Neefektyviai atnaujinamas listas. Reiketu po viena prideti/ismesti, kaip daroma kitur.
        {
            List<Newtonsoft.Json.Linq.JObject> users = await Inst.ApiRequests.GetGroupMembers(this.room.roomId);
            if (users == null)
            {
                return false;
            }
            List<Dictionary<string, object>> usersList = new List<Dictionary<string, object>>();
            foreach (var item in users)
            {
                UserDto temp = item["key"].ToObject<UserDto>();
                AdditionalData userAddData = await Inst.ApiRequests.GetUserAddData(temp.Id);
                if (userAddData == null)
                {
                    usersList.Add(UsersStatus(null, item["value"].ToObject<string>(), temp.Username,temp.Id.ToString()));
                }
                else
                    usersList.Add(UsersStatus(userAddData, item["value"].ToObject<string>(), temp.Username,temp.Id.ToString()));
            }
            this.usersbox = new UsersListViewModel();//Laikinai
            usersList.ForEach(x => usersbox.Users.Add(new UsersListLineViewModel(x["photo"], null, null, x["username"]?.ToString(), x["status"] as Brush, int.Parse(x["userId"]?.ToString()), x["bio"].ToString())));
            this.usersList.DataContext = this.usersbox;
            //this.usersList. = usersList;
            //this.usersListView.UpdateLayout();
            return true;
        }
        private Dictionary<string,object> UsersStatus(AdditionalData addData,string statusChar,string userName,string id)
        {
            Dictionary<string,object> temp = new Dictionary<string, object>();
            temp.Add("bio",addData.Biography);
            temp.Add("userId",id);
            if(addData.PhotoBytes!=null)
            using (var memstr = new MemoryStream(addData.PhotoBytes))
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad; // here
                        image.StreamSource = memstr;
                        image.EndInit();
                        ImageBrush imgBrush = new ImageBrush();
                        imgBrush.ImageSource = image;
                        temp.Add("photo",imgBrush);
                    if (User_images.All(x => x.Item1 != int.Parse(id)))//Should replace existing if found...
                    {
                        User_images.Add(new Tuple<int, BitmapImage>(int.Parse(id), image));
                    }
            }
            else
            {
                temp.Add("photo", Brushes.LightGray);
            }
            Brush status;          
             switch (statusChar)
            {
                case "A":
                    {
                        //b = Brushes.Green;
                        status = Brushes.Green;
                        break;
                    }
                case "B":
                    {
                        //b = Brushes.Yellow;
                        status = Brushes.Yellow;
                        break;
                    }
                case "C":
                    {
                        //b = Brushes.Red;
                        status = Brushes.Red;
                        break;
                    }
                default: 
                    status = Brushes.Gray;
                    break;
            }     
            temp.Add("status",status);
            temp.Add("username",userName);
            return temp;
        }
        //public async void ListUsersStack()
        //{
        //    usersList.Items.Clear();
        //    List<Newtonsoft.Json.Linq.JObject> users = await Inst.ApiRequests.GetGroupMembers(this.room.roomId);
        //    if (users == null)
        //    {
        //        breakDaRules =  true;
        //    }            
        //    foreach (var item in users)
        //    {
        //        UserDto temp = item["key"].ToObject<UserDto>();
        //        AdditionalData userAddData = await Inst.ApiRequests.GetUserAddData(temp.Id);
        //        if (userAddData == null)
        //        {
        //            usersList.Items.Add(UsersStatus(null, item["value"].ToObject<string>(), temp.Username));
        //        }
        //        else
        //            usersList.Items.Add(UsersStatus(userAddData, item["value"].ToObject<string>(), temp.Username));
        //    }            
        //    breakDaRules = false;
        //}
        private StackPanel UsersStatuses(byte[] photoBytes,string username,string stat)
        {
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            Ellipse photo = new Ellipse();
            photo.Width = 25;
            photo.Height = 25;
            if(photoBytes!=null)
            using (var memstr = new MemoryStream(photoBytes))
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad; // here
                        image.StreamSource = memstr;
                        image.EndInit();
                        ImageBrush imgBrush = new ImageBrush();
                        imgBrush.ImageSource = image;
                        photo.Fill = imgBrush;
            }
            stack.Children.Add(photo);
            Label userName = new Label();
            userName.Content = username;
            Brush status;          
             switch (stat)
            {
                case "A":
                    {
                        //b = Brushes.Green;
                        status = Brushes.Green;
                        break;
                    }
                case "B":
                    {
                        //b = Brushes.Yellow;
                        status = Brushes.Yellow;
                        break;
                    }
                case "C":
                    {
                        //b = Brushes.Red;
                        status = Brushes.Red;
                        break;
                    }
                default: 
                    status = Brushes.Gray;
                    break;
            }     
            Ellipse statusPhoto = new Ellipse();
            statusPhoto.Width = 10;
            statusPhoto.Height = 10;
            statusPhoto.Fill = status;
            stack.Children.Add(statusPhoto);
            return stack;
        }
    }

    class AddStroke : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)((Dictionary<string,object>)value)["userId"]==Inst.ApiRequests.User.id.ToString())
            {
                return 5;
            }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
