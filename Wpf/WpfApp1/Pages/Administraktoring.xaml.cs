using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WebApi.Dtos;
using System.Windows.Controls.DataVisualization.Charting;
using System.IO;
using WebApi.Entities;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
        

        public Administraktoring(RoomDto room)
        {
            this.room = room;
            //client = Inst.Utils.HttpClient;

            InitializeComponent();
            Inst.Utils.MembersChanged += Utils_MembersChanged;
            ListUsers();

            this.Name.Content = room.roomName;
            this.Name.FontSize = 14;

            ShowUsersTimes(DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek),DateTime.Today);
            fromDate.SelectedDate= DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            toDate.SelectedDate = DateTime.Today;

            GetAddDataRoom();

            //Random ran = new Random();
            //UNICORNS(ran);

        }

        private void Utils_MembersChanged(object sender, EventArgs e)
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

                users.ForEach(x => _stats.Add(new KeyValuePair<string, int>(x.username, (stats.Where(y => y.Key == Int32.Parse(x.id)).Count() != 0) ? stats.Where(y => y.Key == Int32.Parse(x.id)).First().Value : 0)));
                users.Remove(Inst.ApiRequests.User);

                ((BarSeries)chaha.Series[0]).ItemsSource = _stats.ToArray();
            }
            //else MessageBox.Show("Failed to get users stats\nOr no users in room");
        }
        private void kickFromRoom_Click(object sender, RoutedEventArgs e)
        {
            int kickingUserId = Int32.Parse(((User)((Button)sender).Tag).id);
            Window confirm = new Window();
            confirm.Title = "Kick user";
            confirm.Width = 250;
            confirm.Height = 250;
            

            StackPanel pan = new StackPanel{ Orientation = Orientation.Vertical};
            StackPanel pan2 = new StackPanel{Orientation = Orientation.Horizontal};
            
            Button ok = new Button();
            ok.Margin = new Thickness(10,15,15,10);
            ok.Content = "Yes";
            ok.Width = 50;
            ok.Click += (s,ev)=>
            {
                Kick(kickingUserId);
                confirm.Close();
            };
            
            Button cancer = new Button();
            cancer.Content = "No";            
            cancer.Width = 50;
            cancer.Margin = new Thickness(10,15,15,10);
            cancer.Click += (s,ev) =>
            {
                confirm.Close();
            };
            cancer.Style = (Style)App.Current.Resources["bad"];

            pan2.Children.Add(ok);
            pan2.Children.Add(cancer);
                        
            pan.VerticalAlignment = VerticalAlignment.Center;
            pan.HorizontalAlignment = HorizontalAlignment.Center;
            Label lab = new Label();
            lab.Content = "Are you sure?\nKick from room?";
            lab.HorizontalAlignment = HorizontalAlignment.Center;
            pan.Children.Add(lab);
            pan.Children.Add(pan2);

            confirm.Content = pan;
            confirm.ShowDialog();
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
                    this.usersList.Items.Clear();
                    List<int> temp = room.users.ToList<int>();
                    temp.Remove(user);
                    room.users = temp.ToArray();
                    ListUsers();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Something went wrong");
            }
        }
        private async void ListUsers()
        {
            try
            {
                //var usersIds = new {ids= room.users.ToList()};
                //var res = await client.PostAsJsonAsync("Users/get_list",usersIds);
                //users = res.Content.ReadAsAsync<List<User>>().Result;
                users = await Inst.ApiRequests.GetUsersList(room.users.ToList());
                if(users!=null)
                foreach (var item in users)
                {
                    Button btn = new Button();
                    btn.Content = "Kick";
                    btn.Click += kickFromRoom_Click;
                    btn.Tag = item;
                    btn.Margin = new Thickness(2, 2, 2, 2);
                    btn.HorizontalAlignment = HorizontalAlignment.Center;
                    btn.VerticalAlignment = VerticalAlignment.Center;
                    btn.Style = (Style)App.Current.Resources["bad"];                    

                    Label name = new Label();
                    name.Content = item.username;
                    name.Margin = new Thickness(2, 2, 2, 2);
                    name.VerticalAlignment = VerticalAlignment.Center;
                    name.HorizontalAlignment = HorizontalAlignment.Center;

                    Ellipse roomElipse = new Ellipse();
                    roomElipse.Width = 50;
                    roomElipse.Height = 50;

                    //AdditionalData data = await GetAddDataUser(Int32.Parse(item.id));
                    AdditionalData data = await Inst.ApiRequests.GetUserAddData(Int32.Parse(item.id));
                    ImageBrush imgBrush = new ImageBrush();
                    roomElipse.Fill = Brushes.LightGray;
                    if (data != null)
                    {
                        if (data.PhotoBytes!=null)
                        {
                            using (var memstr = new MemoryStream(data.PhotoBytes))
                            {
                                var image = new BitmapImage();
                                image.BeginInit();
                                image.CacheOption = BitmapCacheOption.OnLoad;
                                image.StreamSource = memstr;
                                image.EndInit();
                                imgBrush.ImageSource = image;
                                roomElipse.Fill = imgBrush;
                            }
                        }                                              
                    }
                    
                    StackPanel roomPanel = new StackPanel();
                    roomPanel.Orientation = Orientation.Horizontal;
                    roomPanel.Children.Add(roomElipse);
                    roomPanel.Children.Add(name);
                    roomPanel.Children.Add(btn);

                    usersList.Items.Add(roomPanel);
                }                

            }
            catch (Exception)
            {               
            }
        }
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
                    Inst.Utils.MainWindow.tabs.SelectedIndex = 2;
                    Inst.Utils.IsLoginEnabled = false;
                    disableButton();
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
        private void disableButton()
        {
            Style style = new Style();
            style.TargetType = typeof(Button);
            style.BasedOn = (Style)App.Current.FindResource("enable");;
            style.Setters.Add(new Setter(Button.IsEnabledProperty,false));
            App.Current.Resources["enable"] = style;
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Inst.Utils.MainWindow.frame2.NavigationService.Navigate(new Forms.Admin());
        }
        private void Copy_guid_Click(object sender,RoutedEventArgs e)
        {
            Clipboard.SetText(room.guid);
            MessageBox.Show("Guid copyed to clipboard");
        }

        private void GetStats_Click(object sender, RoutedEventArgs e)
        {
            if (this.fromDate.SelectedDate!=null&&this.toDate.SelectedDate!=null)
            {
                DateTime from = this.fromDate.SelectedDate.Value;
                DateTime to = this.toDate.SelectedDate.Value;
                ShowUsersTimes(from,to);
            }
        }
    }
}
