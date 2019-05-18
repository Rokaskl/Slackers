using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
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
//using Json.Net;
using WebApi.Dtos;
using Newtonsoft.Json;
using WpfApp1.Forms;
using System.Globalization;
using WpfApp1.Pages;

namespace WpfApp1.Forms
{
    /// <summary>
    /// Interaction logic for UserPage.xaml
    /// </summary>
    public partial class UserPage : Page
    {
        private HttpClient client;
        //private Dictionary<string, string> SelectedRoom;
        public UserPage()
        {         
            client = Inst.Utils.HttpClient;
            ShowRooms();
            InitializeComponent();
            
            userRooms.SelectionMode = SelectionMode.Single;
        }



        
        private void BtnLoginRoom_Click(object sender, RoutedEventArgs e)
        {  
            LoginRoom(Int32.Parse((string)((Button)sender).Tag));           
        }


        private async void ShowRooms()
        {
            try
            {

                var res2 = await client.GetAsync("Rooms/user_get_rooms");
                List<Dictionary<string, string>> userR = res2.Content.ReadAsAsync<List<Dictionary<string, string>>>().Result;
                userRooms.ItemsSource = userR;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        private async void LoginRoom(int roomid)
        {
            List<Dictionary<string,string>> temp = userRooms.Items.Cast<Dictionary<string,string>>().Where(x =>Int32.Parse(x["roomId"])==roomid).ToList<Dictionary<string,string>>();

            Dictionary<string, string> room = temp[0];
            try
            {
                var response = await client.GetAsync($"/Rooms/login_group/{room["roomId"]}");
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Joined {room["roomName"]}");
                    Inst.Utils.MainWindow.roomPage.NavigationService.Navigate(new RoomPage(new RoomDto() { roomAdminId = Int32.Parse(room["roomAdminId"].ToString()), roomId = Convert.ToInt32(room["roomId"]), roomName = room["roomName"].ToString() },"user"));
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
            style.BasedOn = (Style)App.Current.FindResource("enable");
            style.Setters.Add(new Setter(Button.IsEnabledProperty,false));
            App.Current.Resources["enable"] = style;
        }
        private void BtnJoinRoom_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.guidText.Text))
            {
                return;
            }
            JoinRoom(this.guidText.Text);
        }

        private async void JoinRoom(string guid)
        {
            try
            {
                var response = await client.PutAsJsonAsync($"/Rooms/join_group", new { guid = guid });
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"successfuly became a member");
                    //Inst.Utils.MainWindow.frame.NavigationService.Navigate(new RoomPage(new RoomDto() { roomAdminId = Int32.Parse(room["roomAdminId"].ToString()), roomId = Int32.Parse(room["roomId"].ToString()), roomName = room["roomName"].ToString() }));
                    ShowRooms();
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
    } 
}
