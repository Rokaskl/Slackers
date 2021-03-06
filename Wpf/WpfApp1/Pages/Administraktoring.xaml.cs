﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WebApi.Dtos;
using System.Windows.Controls.DataVisualization.Charting;
using System.IO;
using WebApi.Entities;
using System.Windows.Media.Imaging;
using WpfApp1.Windows;
using WpfApp1.Controls;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Interaction logic for Administraktoring.xaml
    /// </summary>
    public partial class Administraktoring : Page
    {
        //private HttpClient client;
        private RoomDto room;
        private List<User> users = new List<User>();  
        //private EnableChangeHandle kkk = new EnableChangeHandle();


        public Administraktoring(RoomDto room)
        {
            this.room = room;
            //client = Inst.Utils.HttpClient;

            InitializeComponent();
            gif.Source = Inst.LoadingGifSource;
            this.DataContext = Inst.ApiRequests.disable_enable;
            ListUsers();
            this.IsVisibleChanged += Administraktoring_IsVisibleChanged;
            this.Name.Content = room.roomName.Replace(" ",string.Empty);
            this.Name.FontSize = 14;

            ShowUsersTimes(DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek),DateTime.Today);
            fromDate.SelectedDate= DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek + 1);
            toDate.SelectedDate = DateTime.Today.AddDays(1);

            GetAddDataRoom();

            Task.Run(() => LiveChart());

            //Random ran = new Random();
            //UNICORNS(ran);

        }

        private bool cancel = false;

        public void StopChart()
        {
            cancel = true;
        }

        private void Administraktoring_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)// became visible
            {
                UpdateChart();
            }
        }

        private async void LiveChart()
        {
            while (!cancel)
            {
                await Task.Delay(30000);
                this.Dispatcher.Invoke(() =>
                {
                    UpdateChart();
                });
            }

        }

        public void UpdateMembersListView()
        {            
            ListUsers();
        }

        private async void UNICORNS(Random ran)
        {
            Color color = new Color();
            List<int> font = new List<int>();font.Add(12);font.Add(15); 
            while (true)
            {            
            await Task.Delay(50);
            color.R = Convert.ToByte(ran.Next(225));
            color.G =Convert.ToByte(ran.Next(225));
            color.B = Convert.ToByte(ran.Next(225));
            color.ScA = 1;
            Brush brush = new SolidColorBrush(color);
            
            Brush b = Brushes.Red;
             
            
            this.Name.Foreground = brush;
            this.Name.FontSize = font[0];
                font.Reverse();
            }
            
        }
        //private async Task<AdditionalData> GetAddDataUser(int id)
        //{
        //    var resp = await client.GetAsync($"AdditionalDatas/{id}/{true}");
        //    if (resp.IsSuccessStatusCode)
        //    {
        //         return resp.Content.ReadAsAsync<AdditionalData>().Result;
        //    }
        //    return null;
        //}
        private async void GetAddDataRoom()
        {
            //var resp = await client.GetAsync($"AdditionalDatas/{room.roomId}/{false}");
            AdditionalData data = await Inst.ApiRequests.GetRoomAddData(room.roomId);
            //if (resp.IsSuccessStatusCode)
            //{
            //AdditionalData data = resp.Content.ReadAsAsync<AdditionalData>().Result;
            if (data != null)
            {
                if (data.PhotoBytes != null)
                {
                    using (var memstr = new MemoryStream(data.PhotoBytes))
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad; // here
                        image.StreamSource = memstr;
                        image.EndInit();
                        ImageBrush imgBrush = new ImageBrush();
                        imgBrush.ImageSource = image;
                        this.elipsePhoto.Fill = imgBrush;
                    }
                }
            }
            //}
        }
        private async void ShowUsersTimes(DateTime from,DateTime to)//Testuot
        {
            //string uri = $"TimeTracker/timeroom/{from}/{to}/{room.roomId}/{0}";
            //var resp = await client.GetAsync(uri);
            Dictionary<int, int> stats = await Inst.ApiRequests.GetTimes(from,to,room.roomId);
            if (/*resp.IsSuccessStatusCode*/stats!=null)
            {
                //Dictionary<int, int> stats = resp.Content.ReadAsAsync<Dictionary<int, int>>().Result;
                List<KeyValuePair<string, int>> _stats = new List<KeyValuePair<string, int>>();
                                                
                users.Add(Inst.ApiRequests.User);

                //_stats.Add(new KeyValuePair<string, int>("TinkisVinkis", 69));
                //_stats.Add(new KeyValuePair<string, int>("Dipsis", 42));
                //_stats.Add(new KeyValuePair<string, int>("Lialia", 0));
                //_stats.Add(new KeyValuePair<string, int>("Pou", 68));
                if(users!=null&&!users.Contains(null))
                users.ForEach(x => _stats.Add(new KeyValuePair<string, int>(x.username, (stats.Where(y => y.Key == Int32.Parse(x.id)).Count() != 0) ? stats.Where(y => y.Key == Int32.Parse(x.id)).First().Value : 0)));
                users.Remove(Inst.ApiRequests.User);

                ((BarSeries)chaha.Series[0]).ItemsSource = _stats.ToArray();
            }
            //else MessageBox.Show("Failed to get users stats\nOr no users in room");
        }
        private void kickFromRoom_Click(object sender, RoutedEventArgs e)
        {
            User kickingUser = ((User)((ButtonCornering)sender).Tag);
            var confirm = new ConfirmWindow($"Are you sure?\nKick {kickingUser.username} from room?");
            if (confirm.ShowDialog() == false)
            {
                if (confirm.Rezult)
                {
                    Kick(Int32.Parse(kickingUser.id));
                }
            }
        }
        private async void Kick(int user)
        {
            try
            {  
             //var data = new{ 
             //    roomId = room.roomId,
             //    userId = user};   
             //   var res = await client.PutAsJsonAsync("Rooms/kick_user",data);
                if (/*res.IsSuccessStatusCode*/await Inst.ApiRequests.KickUser(user,room.roomId))
                {
                    
                    //List<int> temp = room.users.ToList<int>();
                    //temp.Remove(user);
                    //room.users = temp.ToArray();
                    
                }
                ListUsers();
            }
            catch (Exception)
            {
                MessageBox.Show("Something went wrong");
            }
        }
        private async void ListUsers()
        {
            this.usersListView.Visibility = Visibility.Hidden;
            gif.Visibility = Visibility.Visible;
            gif.Play();
            try
            {
                //var usersIds = new {ids= room.users.ToList()};
                //var res = await client.PostAsJsonAsync("Users/get_list",usersIds);
                //users = res.Content.ReadAsAsync<List<User>>().Result;
                users = await Inst.ApiRequests.GetUsersList(room.roomId);
                if (users != null)
                {
                    List<Dictionary<string,object>> usersListview = new List<Dictionary<string, object>>();
                    foreach (var item in users.ToList())
                    {                        
                        Dictionary<string,object> tempUser = new Dictionary<string, object>();
                        tempUser.Add("username",item.username);
                        AdditionalData data = await Inst.ApiRequests.GetUserAddData(Int32.Parse(item.id));
                        tempUser.Add("bio",data.Biography);
                        ImageBrush imgBrush = new ImageBrush();
                        if (data != null)
                        {
                            if (data.PhotoBytes != null)
                            {
                                using (var memstr = new MemoryStream(data.PhotoBytes))
                                {
                                    var image = new BitmapImage();
                                    image.BeginInit();
                                    image.CacheOption = BitmapCacheOption.OnLoad;
                                    image.StreamSource = memstr;
                                    image.EndInit();
                                    imgBrush.ImageSource = image;
                                    tempUser.Add("photo",imgBrush);
                                }
                            }
                            else
                            {
                                tempUser.Add("photo",Brushes.LightGray);
                            }
                        }
                        tempUser.Add("user",item);
                        usersListview.Add(tempUser);
                    }
                    this.usersListView.ItemsSource = usersListview;
                }
                else
                {
                    users = new List<User>();
                }
            }
            catch (Exception exception)
            {               
            }
            gif.Visibility = Visibility.Hidden;
            this.usersListView.Visibility = Visibility.Visible;
            gif.Stop();
            gif.Pause();
        }
        //private async void ListUsers()
        //{
        //    try
        //    {
        //        //var usersIds = new {ids= room.users.ToList()};
        //        //var res = await client.PostAsJsonAsync("Users/get_list",usersIds);
        //        //users = res.Content.ReadAsAsync<List<User>>().Result;
        //        users = await Inst.ApiRequests.GetUsersList(room.roomId);
        //        if (users != null)
        //        {
        //            foreach (var item in users.ToList())
        //            {
        //                Button btn = new Button();
        //                btn.Content = "Kick";
        //                btn.Click += kickFromRoom_Click;
        //                btn.Tag = item;
        //                btn.Margin = new Thickness(2, 2, 2, 2);
        //                btn.HorizontalAlignment = HorizontalAlignment.Center;
        //                btn.VerticalAlignment = VerticalAlignment.Center;
        //                btn.Style = (Style)App.Current.Resources["bad"];

        //                Label name = new Label();
        //                name.Content = item.username;
        //                name.Margin = new Thickness(2, 2, 2, 2);
        //                name.VerticalAlignment = VerticalAlignment.Center;
        //                name.HorizontalAlignment = HorizontalAlignment.Center;

        //                Ellipse roomElipse = new Ellipse();
        //                roomElipse.Width = 50;
        //                roomElipse.Height = 50;

        //                //AdditionalData data = await GetAddDataUser(Int32.Parse(item.id));
        //                AdditionalData data = await Inst.ApiRequests.GetUserAddData(Int32.Parse(item.id));
        //                ImageBrush imgBrush = new ImageBrush();
        //                roomElipse.Fill = Brushes.LightGray;
        //                if (data != null)
        //                {
        //                    if (data.PhotoBytes != null)
        //                    {
        //                        using (var memstr = new MemoryStream(data.PhotoBytes))
        //                        {
        //                            var image = new BitmapImage();
        //                            image.BeginInit();
        //                            image.CacheOption = BitmapCacheOption.OnLoad;
        //                            image.StreamSource = memstr;
        //                            image.EndInit();
        //                            imgBrush.ImageSource = image;
        //                            roomElipse.Fill = imgBrush;
        //                        }
        //                    }
        //                }

        //                StackPanel roomPanel = new StackPanel();
        //                roomPanel.Orientation = Orientation.Horizontal;
        //                roomPanel.Children.Add(roomElipse);
        //                roomPanel.Children.Add(name);
        //                roomPanel.Children.Add(btn);
        //                if (!usersList.Items.Contains(roomPanel))
        //                {
        //                    usersList.Items.Add(roomPanel);
        //                }                        
        //            }
        //        }
        //        else
        //        {
        //            users = new List<User>();
        //        }
        //    }
        //    catch (Exception exception)
        //    {               
        //    }
        //}
        //private async void ListUsers()
        //{
        //    try
        //    {
        //        var usersIds = new {ids= room.users.ToList()};
        //        var res = await client.PostAsJsonAsync("Users/get_list",usersIds);
        //        List<User> adminR = res.Content.ReadAsAsync<List<User>>().Result;
                
        //        roomUsers.ItemsSource = adminR;
        //    }
        //    catch (Exception)
        //    {               
        //    }
        //}

        private void Login_Click(object sender, RoutedEventArgs e)
        {            
            LoginRoom(room.roomId);
        }
        
        private async void LoginRoom(int roomid)
        {            
            try
            {
                //var response = await client.GetAsync($"/Rooms/login_group/{room.roomId}");
                if (/*response.IsSuccessStatusCode*/await Inst.ApiRequests.LoginRoom(room.roomId))
                {
                    //MessageBox.Show($"Joined {room.roomName}");
                    Inst.Utils.MainWindow.roomPage.NavigationService.Navigate(new RoomPage(room,"admin"));
                    Inst.Utils.MainWindow.room.Visibility = Visibility.Visible;
                    Inst.Utils.MainWindow.room.Header = $"Room {room.roomName}";
                    Inst.Utils.MainWindow.tabs.SelectedIndex = 2;
                    Inst.Utils.IsLoginEnabled = false;
                    //disableButton();
                }
                else
                {
                    //MessageBox.Show("Joining failed...");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        //private void disableButton()
        //{
        //    Style style = new Style();
        //    style.TargetType = typeof(Button);
        //    style.BasedOn = (Style)App.Current.FindResource("enable");;
        //    style.Setters.Add(new Setter(Button.IsEnabledProperty,false));
        //    App.Current.Resources["enable"] = style;
        //}
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Inst.Utils.MainWindow.frame2.NavigationService.Navigate(new Forms.Admin());
            StopChart();
            Inst.Utils.Administraktoring = null;
            this.IsVisibleChanged -= Administraktoring_IsVisibleChanged;
        }
        private void Copy_guid_Click(object sender,RoutedEventArgs e)
        {
            Clipboard.SetText(room.guid);
            MessageBox.Show("Guid copyed to clipboard");
        }

        private void GetStats_Click(object sender, RoutedEventArgs e)
        {
            UpdateChart();
        }

        private void UpdateChart()
        {
            if (this.fromDate.SelectedDate != null && this.toDate.SelectedDate != null)
            {
                DateTime from = this.fromDate.SelectedDate.Value;
                DateTime to = this.toDate.SelectedDate.Value;
                ShowUsersTimes(from, to);
            }
        }

        private void Btn_guidChange_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Change room's guid?", "", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                ChangeRoomGuid();
            }
        }

        private async void ChangeRoomGuid()
        {
            try
            {
                if (await Inst.ApiRequests.ChangeRoomGuid(room))
                {
                    MessageBox.Show("Guid was successfuly changed!");
                }
                else
                {
                    MessageBox.Show("Could not change room's guid!");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void Gif_MediaEnded(object sender, RoutedEventArgs e)
        {
            gif.Position = new TimeSpan(0, 0, 0, 1);
            gif.Play();
        }
    }
}
