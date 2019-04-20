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
                    FillMembers();
                    time = (int)stopWatch.Elapsed.TotalSeconds;
                }
            }
        }

        private async void FillMembers()
        {
            try
            {
                var response = await client.GetAsync($"/Rooms/group/{this.room.roomId}");
                if (response.IsSuccessStatusCode)
                {
                    this.Dispatcher.Invoke((Action)(() =>
                    {
                        List<Dictionary<string, string>> respListDict = new List<Dictionary<string, string>>();
                        List<Newtonsoft.Json.Linq.JObject> resp = response.Content.ReadAsAsync<List<Newtonsoft.Json.Linq.JObject>>().Result;
                        resp.ForEach(x =>
                        {
                            Dictionary<string, string> respDict = new Dictionary<string, string>();
                            foreach (var key in x.GetValue("key").ToObject<Dictionary<string, string>>())
                            {
                                respDict.Add(key.Key, key.Value);
                            }
                            respDict.Add("status", x.GetValue("value").ToObject<KeyValuePair<string, string>>().Value);
                            respListDict.Add(respDict);
                        });
                        this.MembersListView.ItemsSource = respListDict;
                        
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
