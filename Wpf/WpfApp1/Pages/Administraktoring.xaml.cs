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

namespace WpfApp1.Pages
{
    /// <summary>
    /// Interaction logic for Administraktoring.xaml
    /// </summary>
    public partial class Administraktoring : Page
    {
        private HttpClient client;
        private RoomDto room;
        

        public Administraktoring(RoomDto room)
        {
            this.room = room;
            client = Inst.Utils.HttpClient;

            InitializeComponent();            
            ListUsers();

            this.Name.Content = room.roomName.Replace(" ",string.Empty);
            this.Name.FontSize = 14;

            ShowUsersTimes(DateTime.Today.AddDays((int)DateTime.Today.DayOfWeek-1),DateTime.Today);
            fromDate.SelectedDate= DateTime.Today.AddDays((int)DateTime.Today.DayOfWeek-1);
            toDate.SelectedDate = DateTime.Today;

            //Random ran = new Random();
            //UNICORNS(ran);

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
        private async void ShowUsersTimes(DateTime from,DateTime to)//Testuot
        {
            string uri = $"TimeTracker/timeroom/{from}/{to.AddDays(1)}/{room.roomId}/{0}";
            var resp = await client.GetAsync(uri);
            if (resp.IsSuccessStatusCode&&roomUsers.ItemsSource!=null)
            {
                Dictionary<int, int> stats = resp.Content.ReadAsAsync<Dictionary<int, int>>().Result;
                List<KeyValuePair<string, int>> _stats = new List<KeyValuePair<string, int>>();

                //_stats.Add(new KeyValuePair<string, int>("TinkisVinkis", 69));
                //_stats.Add(new KeyValuePair<string, int>("Dipsis", 42));
                //_stats.Add(new KeyValuePair<string, int>("Lialia", 0));
                //_stats.Add(new KeyValuePair<string, int>("Pou", 68));

                roomUsers.ItemsSource.Cast<User>().ToList<User>().ForEach(x => _stats.Add(new KeyValuePair<string, int>(x.username, (stats.Where(y => y.Key == Int32.Parse(x.id)).Count() != 0) ? stats.Where(y => y.Key == Int32.Parse(x.id)).First().Value : 0)));

                ((BarSeries)chaha.Series[0]).ItemsSource = _stats.ToArray();
            }
            else MessageBox.Show("Failed to get users stats\nOr no users in room");
        }
        private void kickFromRoom_Click(object sender, RoutedEventArgs e)
        {
            int kickingUserId = Int32.Parse((string)((Button)sender).Tag);
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
             var data = new{ 
                 roomId = room.roomId,
                 userId = user};   
            var res = await client.PutAsJsonAsync("Rooms/kick_user",data);
                if (res.IsSuccessStatusCode)
                {
                    List<User> tempUsers = roomUsers.ItemsSource.Cast<User>().ToList<User>();
                    tempUsers.Remove(tempUsers.Where(x=>Int32.Parse(x.id)==user).First());
                    roomUsers.ItemsSource = tempUsers;
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
                var usersIds = new {ids= room.users.ToList()};
                var res = await client.PostAsJsonAsync("Users/get_list",usersIds);
                List<User> adminR = res.Content.ReadAsAsync<List<User>>().Result;
                
                roomUsers.ItemsSource = adminR;
            }
            catch (Exception)
            {               
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            LoginRoom(room.roomId);
        }
        
        private async void LoginRoom(int roomid)
        {
            
            try
            {
                var response = await client.GetAsync($"/Rooms/login_group/{room.roomId}");
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Joined {room.roomName}");
                    Inst.Utils.MainWindow.roomPage.NavigationService.Navigate(new RoomPage(room,"admin"));
                    Inst.Utils.MainWindow.room.Visibility = Visibility.Visible;                    
                    Inst.Utils.MainWindow.tabs.SelectedIndex = 2;
                    disableButton();
                }
                else
                {
                    MessageBox.Show("Joining failed...");
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
