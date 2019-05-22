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
    /// Interaction logic for RoomsPage.xaml
    /// </summary>
    public partial class RoomsPage : Page
    {
        private HttpClient client;
        private Dictionary<string, string> SelectedRoom;

        public RoomsPage()
        {
            client = Inst.Utils.HttpClient;
            ShowRooms();
            InitializeComponent();

            adminRooms.SelectionMode = SelectionMode.Single;
            userRooms.SelectionMode = SelectionMode.Single;
            adminRooms.SelectionChanged += AdminRooms_SelectionChanged;
            userRooms.SelectionChanged += UserRooms_SelectionChanged;
            //adminRooms.MouseDown += AdminRooms_MouseDown;
            adminRooms.MouseRightButtonUp += AdminRooms_MouseRightButtonUp;
            //adminRooms.Items.
        }

        private void UserRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnLoginRoom.Content = "Login " + (e.AddedItems[0] as Dictionary<string, string>)["roomName"].ToString();
            SelectedRoom = (e.AddedItems[0] as Dictionary<string, string>);
        }

        private void AdminRooms_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show(((sender as ListView).SelectedItem as Dictionary<string, object>)["guid"].ToString());
            if ((sender as ListView).SelectedItem != null)
            {
                Window info = new Window();
                StackPanel panel = new StackPanel { Orientation = Orientation.Vertical };
                panel.Children.Add(new Label() { Content = "Guid" });
                panel.Children.Add(new TextBox() { Text = ((sender as ListView).SelectedItem as Dictionary<string, object>)["guid"].ToString() });
                info.Content = panel;
                info.ShowDialog();
            }
        }

        private void AdminRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show((e.AddedItems[0] as Dictionary<string, object>)["roomName"].ToString());
            btnLoginRoom.Content = "Login " + (e.AddedItems[0] as Dictionary<string, object>)["roomName"].ToString();
            SelectedRoom = new Dictionary<string, string>();
            foreach (var x in (e.AddedItems[0] as Dictionary<string, object>))
            {
                if (x.Value?.ToString() != null)
                {
                    SelectedRoom.Add(x.Key, x.Value.ToString());
                }   
            }
        }

        
        private void BtnLoginRoom_Click(object sender, RoutedEventArgs e)
        {
            //LoginRoom(adminRooms.SelectedItem as Dictionary<string, object>);
            LoginRoom();
        }

        private void BtnRegisterRoom_Click(object sender, RoutedEventArgs e)
        {
            if (roomNameText.Text != "")
                RegisterRoom(roomNameText.Text);
            else
                MessageBox.Show("Theres no room name");
        }

        private async void ShowRooms()
        {
            try
                    {
                var res = await client.GetAsync("Rooms/admin_get_rooms");
                List<Dictionary<string, object>> adminR = res.Content.ReadAsAsync<List<Dictionary<string, object>>>().Result;
                adminRooms.ItemsSource = adminR;

                var res2 = await client.GetAsync("Rooms/user_get_rooms");
                List<Dictionary<string, string>> userR = res2.Content.ReadAsAsync<List<Dictionary<string, string>>>().Result;
                userRooms.ItemsSource = userR;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async void RegisterRoom(String name)
        {
            try
            {
                var club = new RoomDto()
                {
                    roomName = name
                };
                var response = await client.PostAsJsonAsync("/Rooms/register", club);
                if (response.IsSuccessStatusCode)
                {
                    //MessageBox.Show("Register Successfully");
                    ShowRooms();
                }
                else
                {
                    MessageBox.Show("Register Failed...");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async void LoginRoom()
        {
            Dictionary<string, string> room = SelectedRoom;
            try
            {
                var response = await client.GetAsync($"/Rooms/login_group/{room["roomId"]}");
                if (response.IsSuccessStatusCode)
                {
                    //MessageBox.Show($"Joined {room["roomName"]}");
                   // Inst.Utils.MainWindow.frame1.NavigationService.Navigate(new RoomPage(new RoomDto() { roomAdminId = Int32.Parse(room["roomAdminId"].ToString()), roomId = Int32.Parse(room["roomId"].ToString()), roomName = room["roomName"].ToString() }));
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
                    //MessageBox.Show($"successfuly became a member");
                    //Inst.Utils.MainWindow.frame.NavigationService.Navigate(new RoomPage(new RoomDto() { roomAdminId = Int32.Parse(room["roomAdminId"].ToString()), roomId = Int32.Parse(room["roomId"].ToString()), roomName = room["roomName"].ToString() }));
                    ShowRooms();
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
    }
}
