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
            Task.Run(() => DisplayMembers());//toliau naujina info kas 10secs.
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
            //< Label Content = "Label" HorizontalAlignment = "Left" Margin = "656,330,0,0" VerticalAlignment = "Top" Width = "95" />
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
    }
}
