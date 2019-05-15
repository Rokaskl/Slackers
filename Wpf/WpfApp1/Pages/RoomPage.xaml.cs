﻿using System;
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

namespace WpfApp1.Pages
{
    /// <summary>
    /// Interaction logic for RoomPage.xaml
    /// </summary>
    public partial class RoomPage : Page
    {
        private HttpClient client;
        private RoomDto room;
        private string prevWindow;
        private Timer timer;

        public RoomPage(RoomDto room,string prev)
        {
            this.prevWindow = prev;
            timer = new Timer();
            this.room = room;
            client = Inst.Utils.HttpClient;
           
            InitializeComponent();
            this.KeyDown += RoomPage_KeyDown;

            this.NoteListView.SelectionMode = SelectionMode.Single;
            this.NoteListView.MouseLeftButtonUp += NoteListView_MouseLeftButtonUp;

            ConfigureTimer();

            (this.MembersListView.View as GridView).Columns.Add(new GridViewColumn
            {
                //Header = "Id",
                DisplayMemberBinding = new Binding("username"),
                Width = 100 
            });
            (this.MembersListView.View as GridView).Columns.Add(new GridViewColumn
            {
                //Header = "Name",
                DisplayMemberBinding = new Binding("status"),
                Width = 100
            });
            
            InitCmbStatus();
            FillMembers();//pirma karta uzkrauna iskarto.
            FillNotes();
            Task.Run(() => DisplayMembers());//toliau naujina info kas 10secs.
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
                            DisplaySelectedStatus(await client.GetAsync($"/Rooms/status/{this.room.roomId}/A"), "Active");
                            break;
                        }
                    case "Away":
                        {
                            DisplaySelectedStatus(await client.GetAsync($"/Rooms/status/{this.room.roomId}/B"), "Away");
                            break;
                        }
                    case "Don't disturb":
                        {
                            DisplaySelectedStatus(await client.GetAsync($"/Rooms/status/{this.room.roomId}/C"), "Don't disturb");
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
                void DisplaySelectedStatus(HttpResponseMessage response, string item)
                {
                    if (response.IsSuccessStatusCode)
                    {
                        this.cmbStatus.SelectedItem = item;
                        foreach(var x in this.MembersListView.Items)
                        {
                            if (x.GetType().GetProperty("username").GetValue(x).ToString() == Inst.Utils.User.username)
                            {
                                int index = this.MembersListView.Items.IndexOf(x);
                                this.MembersListView.Items.Remove(x);
                                this.MembersListView.Items.Insert(index, new { username = Inst.Utils.User.username, status = item});
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
            timer.Margin = new Thickness(656, 330, 0, 0);
            timer.VerticalAlignment = VerticalAlignment.Top;
            timer.Width = 95;
            this.pageGrid.Children.Add(timer);
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
                var response = await client.GetAsync($"/TimeTracker/mark/{room.roomId}/1");
                if (response.IsSuccessStatusCode)
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
                var response = await client.GetAsync($"/TimeTracker/mark/{room.roomId}/0");
                if (response.IsSuccessStatusCode)
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

        private async Task<bool> FillMembers()
        {
            bool end = true;
            try
            {
                var response = await client.GetAsync($"/Rooms/group/{this.room.roomId}");
                if (response.IsSuccessStatusCode)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        List<Dictionary<string, string>> respListDict = new List<Dictionary<string, string>>();
                        List<Newtonsoft.Json.Linq.JObject> resp = response.Content.ReadAsAsync<List<Newtonsoft.Json.Linq.JObject>>().Result;
                        if (resp == null)
                        {
                            Inst.Utils.MainWindow.frame1.Navigate(new UserPage());
                            end = false;
                        }
                        else
                        {
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
                        }
                    }));
                    //this.MembersListView
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

        private async void LogoutFromRoom()
        {
            try
            {
                var response = await client.GetAsync($"/Rooms/logout_group/{room.roomId}");
                if (response.IsSuccessStatusCode)
                {
                    if (this.timer.IsRunning)
                    {
                        StopTimer();
                    }
                    if (prevWindow=="admin")
                    {                        
                    Inst.Utils.MainWindow.frame2.NavigationService.Navigate(new Admin());
                    }
                    else
                    Inst.Utils.MainWindow.frame1.NavigationService.Navigate(new UserPage());
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

        private async void FillNotes()
        {
            PopupsClose();

            try
            {
                var response = await client.GetAsync($"/Notes/{room.roomId}");
                if (response.IsSuccessStatusCode)
                {
                    //notelist.ItemsSource = response;
                    //List<Dictionary<string, string>> info = response.Content.ReadAsAsync<Newtonsoft.Json.Linq.JArray>().Result.ToObject(typeof(List<Dictionary<string, string>>));
                    List<Note> info = response.Content.ReadAsAsync<List<Note>>().Result;
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
                var response = await client.PostAsJsonAsync<Dictionary<string, string>>($"/Notes/submit", info);
                if (response.IsSuccessStatusCode)
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
                var response = await client.PostAsJsonAsync<Note>($"/Notes/modify", note);
                if (response.IsSuccessStatusCode)
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
                var response = await client.GetAsync($"/Notes/delete/{roomId}/{noteId}");
                if (response.IsSuccessStatusCode)
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
            popup.AllowsTransparency = true;
            popup.Height = 400;
            popup.Width = 500;
            popup.Placement = PlacementMode.Mouse;
            StackPanel panel = new StackPanel();
            TextBox textbox = new TextBox();
            textbox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            textbox.TextWrapping = TextWrapping.Wrap;
            popup.Child = panel;
            TextBox header = new TextBox() { Text = string.IsNullOrWhiteSpace(pheader) ? "Header" : pheader, Background = Brushes.Aqua };
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
            textbox.Background = Brushes.AliceBlue;
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
    }
}