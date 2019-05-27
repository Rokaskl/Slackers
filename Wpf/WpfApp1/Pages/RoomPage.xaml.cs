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

namespace WpfApp1.Pages
{
    /// <summary>
    /// Interaction logic for RoomPage.xaml
    /// </summary>
    public partial class RoomPage : Page
    {
        //private HttpClient client;
        private RoomDto room;
        private string prevWindow;
        private Timer timer;
        private bool Changed;

        public RoomPage(){}
        public RoomPage(RoomDto room,string prev)
        {
            Inst.Utils.RoomPage = this;
            this.prevWindow = prev;
            timer = new Timer();
            this.room = room;
            //client = Inst.Utils.HttpClient;
           
            InitializeComponent();
            this.chatbox.ItemContainerGenerator.StatusChanged += ItemContainerGenerator_StatusChanged;
            this.KeyDown += RoomPage_KeyDown;
            this.txt_entry.KeyUp += Txt_entry_KeyUp;

            this.NoteListView.SelectionMode = SelectionMode.Single;
            this.NoteListView.MouseLeftButtonUp += NoteListView_MouseLeftButtonUp;

            ConfigureTimer();

            (this.MembersListView.View as GridView).Columns.Add(new GridViewColumn
            {
                //Header = "Id",
                Header = "Username",
                DisplayMemberBinding = new Binding("username"),
                Width = 100 
            });
            (this.MembersListView.View as GridView).Columns.Add(new GridViewColumn
            {
                Header = "Status",
                DisplayMemberBinding = new Binding("status"),
                Width = 100
            });
            
            InitCmbStatus();
            FillMembers();//pirma karta uzkrauna iskarto.
            FillNotes();
            FillChat();
            Task.Run(() => DisplayMembersR());//toliau naujina info kas 10secs.
        }

        private void ItemContainerGenerator_StatusChanged(object sender, EventArgs e)
        {
            if (Changed && this.chatbox.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                foreach(ChatLine line in (sender as System.Windows.Controls.ItemContainerGenerator).Items)
                {
                    ListViewItem lv = (ListViewItem)chatbox.ItemContainerGenerator.ContainerFromItem(line);
                    if (lv != null)
                    {
                        lv.ToolTip = line.CreateDate.ToString("HH:mm:ss yyyy/MM/dd", CultureInfo.InvariantCulture);
                        if (line.CreatorId.ToString() == Inst.ApiRequests.User.id)
                        {
                            lv.Background = Brushes.LightBlue;
                        }
                        else
                        {
                            lv.Background = Brushes.LightGray;
                        }
                    }
                }
                Changed = false;
            }
        }

        private void Txt_entry_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SubmitEntry();
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
                        foreach(var x in this.MembersListView.Items)
                        {
                            if (x.GetType().GetProperty("username").GetValue(x).ToString() == Inst.ApiRequests.User.username)
                            {
                                int index = this.MembersListView.Items.IndexOf(x);
                                this.MembersListView.Items.Remove(x);
                                this.MembersListView.Items.Insert(index, new { username = Inst.ApiRequests.User.username, status = item});
                                break;
                            }
                        }
                    }
                } 
            }
            catch (Exception exception)
            {

                Console.WriteLine(exception.ToString());
            }
        }

        private void ConfigureTimer()
        {
            
            timer.HorizontalAlignment = HorizontalAlignment.Left;
            timer.Margin = new Thickness(0, 0, 0, 0);
            timer.VerticalAlignment = VerticalAlignment.Top;
            timer.Width = 95;
            timer.FlowDirection = FlowDirection.LeftToRight;
            this.botomStack.Children.Add(timer);
            
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

        private void DisplayMembers()
        {
            int time = 0;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            while (true)
            {
                if (time + 10 <= stopWatch.Elapsed.TotalSeconds)
                {
                    if (!FillMembers().Result)
                    {
                        break;
                    }
                    time = (int)stopWatch.Elapsed.TotalSeconds;
                }
            }
        }
        private async Task DisplayMembersR()
        {
            while (true)
            {
                    await Task.Delay(10000);
                    if (!FillMembers().Result)
                    {
                        break;
                    }                    
            }
        }
        private async Task<bool> FillMembers()
        {
            bool end = true;
            try
            {
                //var response = await client.GetAsync($"/Rooms/group/{this.room.roomId}");
                List<Newtonsoft.Json.Linq.JObject> resp = await Inst.ApiRequests.GetGroupMembers(this.room.roomId);
                if (/*response.IsSuccessStatusCode*/resp!=null)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        List<Dictionary<string, string>> respListDict = new List<Dictionary<string, string>>();
                        //List<Newtonsoft.Json.Linq.JObject> resp = response.Content.ReadAsAsync<List<Newtonsoft.Json.Linq.JObject>>().Result;

                        resp.ForEach(x =>
                        {
                            Dictionary<string, string> respDict = new Dictionary<string, string>();
                            foreach (var key in x.GetValue("key").ToObject<Dictionary<string, object>>())
                            {
                                if (key.Value?.ToString() != null)
                                {
                                    respDict.Add(key.Key, key.Value.ToString());
                                }
                            }
                            respDict.Add("status", x.GetValue("value").ToObject<string>());
                            respListDict.Add(respDict);
                        });
                        this.MembersListView.Items.Clear();

                        //(this.MembersListView.View as GridView).Columns.Add(new GridViewColumn());
                        //this.MembersGrid.
                        //DependencyProperty dp = DependencyProperty.Register("username", typeof(string), typeof(Dictionary<string, string>));
                        respListDict.ForEach(x =>
                        {
                                //Brush b = Brushes.Gray;
                                string status = string.Empty;
                            switch (x["status"])
                            {
                                case "A":
                                    {
                                            //b = Brushes.Green;
                                            status = "Active";
                                        break;
                                    }
                                case "B":
                                    {
                                            //b = Brushes.Yellow;
                                            status = "Away";
                                        break;
                                    }
                                case "C":
                                    {
                                            //b = Brushes.Red;
                                            status = "Don't disturb!";
                                        break;
                                    }
                                default: break;
                            }
                                //ListViewItem lvi = new ListViewItem() { /*Content = "username",*/ Background = b };
                                //lvi.SetValue(dp, x);
                                this.MembersListView.Items.Add(new { username = x["username"], status = status });
                        });
                        //this.MembersListView.ItemsSource = respListDict;
                        //ChangeStatuses(respListDict);

                    }));
                    //this.MembersListView
                }
                else if (resp == null)
                {
                    Inst.Utils.MainWindow.frame1.Navigate(new UserPage());
                    end = false;
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
            return end;
        }

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
                    if (this.timer.IsRunning)
                    {
                        StopTimer();
                    }                    
                    Inst.Utils.MainWindow.room.Visibility = Visibility.Hidden;                    
                    enableButton();
                    Inst.Utils.MainWindow.frame1.Refresh();
                    Inst.Utils.MainWindow.frame2.Refresh();
                    if (prevWindow=="admin")
                    {                           
                    Inst.Utils.MainWindow.tabs.SelectedIndex = 0;
                    //Inst.Utils.MainWindow.frame2.NavigationService.Navigate(new Admin());
                    }
                    else                        
                    Inst.Utils.MainWindow.tabs.SelectedIndex = 1;
                    //Inst.Utils.MainWindow.frame1.NavigationService.Navigate(new UserPage());
                    Inst.Utils.RoomPage = null;
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
        private void enableButton()
        {
            Inst.Utils.IsLoginEnabled = true;
            Style style = new Style();
            style.TargetType = typeof(Button);            
            style.BasedOn = (Style)App.Current.FindResource("enable");
            style.Setters.Add(new Setter(Button.IsEnabledProperty,true));
            App.Current.Resources["enable"] = style;
        }

        public void UpdateNoteListView()
        {
            FillNotes();
        }

        public void UpdateUsersListView()
        {
            FillMembers();
        }

        public void UpdateGroupChat()
        {
            FillChat();
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
                List<ChatLine> data = await Inst.ApiRequests.GetChat(this.room.roomId);
                if (data!=null)
                {
                    //List<ChatLine> data = response.Content.ReadAsAsync<List<ChatLine>>().Result;
                    chatbox.Items.Clear();
                    
                    foreach (ChatLine line in data.OrderBy(x => x.CreateDate))
                    {
                        chatbox.Items.Add(line);
                    }
                    Changed = true;
                    chatbox.ScrollIntoView(data.OrderBy(x => x.CreateDate).LastOrDefault());
                    chatbox.UpdateLayout();
                }
                else
                {
                    MessageBox.Show("Something went wrong! Could not populate chat");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void Background_Changed(object sender, EventArgs e)
        {
            (sender as ListViewItem).Background.Changed -= Background_Changed;
        }

        private void Btn_txtenter_Click(object sender, RoutedEventArgs e)
        {
            SubmitEntry();
        }

        private async void SubmitEntry()
        {
            if (string.IsNullOrWhiteSpace(txt_entry.Text))
            {
                return;
            }

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
    }
}
