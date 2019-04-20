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

namespace WpfApp1.Pages
{
    /// <summary>
    /// Interaction logic for RoomPage.xaml
    /// </summary>
    public partial class RoomPage : Page
    {
        HttpClient client;
        RoomDto room;
        public RoomPage(RoomDto room)
        {
            this.room = room;
            InitializeComponent();
            client = Inst.Utils.HttpClient;
            //this.MembersListView.DisplayMemberPath = "username";
            (this.MembersListView.View as GridView).Columns.Add(new GridViewColumn
            {
                //Header = "Id",
                DisplayMemberBinding = new Binding("username")
            });
            (this.MembersListView.View as GridView).Columns.Add(new GridViewColumn
            {
                //Header = "Name",
                DisplayMemberBinding = new Binding("status")
            });
            FillMembers();//pirma karta uzkrauna iskarto.
            Task.Run(() => DisplayMembers());//toliau naujina info kas 10secs.
        }

        private void btnStartStop_Click(object sender, RoutedEventArgs e)
        {
            if (true)
            {
                StartTimer();
                StopTimer();
            }
        }

        private async void StartTimer()
        {
            try
            {
                var response = await client.GetAsync($"/TimeTracker/Start/{Inst.Utils.User.id}");
                if (response.IsSuccessStatusCode)
                {
                    //start timer
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
                var response = await client.GetAsync($"/TimeTracker/Stop/{Inst.Utils.User.id}");
                if (response.IsSuccessStatusCode)
                {
                    //start timer
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
                            Inst.Utils.MainWindow.frame.Navigate(new RoomsPage());
                            end = false;
                        }
                        else
                        {
                            resp.ForEach(x =>
                            {
                                Dictionary<string, string> respDict = new Dictionary<string, string>();
                                foreach (var key in x.GetValue("key").ToObject<Dictionary<string, string>>())
                                {
                                    respDict.Add(key.Key, key.Value);
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

        private void ChangeStatuses(List<Dictionary<string, string>> members)
        {
            //this.MembersListView.
            //nuspalvinti ar kazkaip kitaip pavaizduoti ListView'e statusus useriu.
            foreach (ListViewItem item in this.MembersListView.Items)
            {
                //switch (item)
                //    {
                //    case 
                //}
                if (true)
                {

                }
            }
            
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
                    StopTimer();
                    Inst.Utils.MainWindow.frame.NavigationService.Navigate(new RoomsPage());
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
