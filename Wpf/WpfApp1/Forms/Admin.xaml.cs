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
using Newtonsoft.Json.Linq;

namespace WpfApp1.Forms
{
    /// <summary>
    /// Interaction logic for RoomsPage.xaml
    /// </summary>
    public partial class Admin : Page
    {
        private HttpClient client;
        private RoomDto SelectedRoom;

        public Admin()
        {
            client = Inst.Utils.HttpClient;
            ShowRooms();
            InitializeComponent();

            adminRooms.SelectionMode = SelectionMode.Single;
            adminRooms.SelectionChanged += AdminRooms_SelectionChanged;
            //adminRooms.MouseDown += AdminRooms_MouseDown;
            adminRooms.MouseRightButtonUp += AdminRooms_MouseRightButtonUp;
            //adminRooms.Items.
        }

        private void UserRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnLoginRoom.Content = "Login " + (e.AddedItems[0] as RoomDto).roomName;
            SelectedRoom = (e.AddedItems[0] as RoomDto);
        }

        private void AdminRooms_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show(((sender as ListView).SelectedItem as Dictionary<string, object>)["guid"].ToString());
            if ((sender as ListView).SelectedItem != null)
            {
                Clipboard.SetText(((sender as ListView).SelectedItem as RoomDto).guid);
                MessageBox.Show(String.Format("Guid copied to clipboard",((sender as ListView).SelectedItem as RoomDto).guid));
            }
        }

        private void AdminRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show((e.AddedItems[0] as Dictionary<string, object>)["roomName"].ToString());
            btnLoginRoom.Content = "Login " + (e.AddedItems[0] as RoomDto).roomName;
            SelectedRoom = (e.AddedItems[0] as RoomDto);
            
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
                List<RoomDto> adminR = res.Content.ReadAsAsync<List<RoomDto>>().Result;
                adminRooms.ItemsSource = adminR;
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
                    MessageBox.Show("Register Successfully");
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
            RoomDto room = SelectedRoom;
            try
            {
                var response = await client.GetAsync($"/Rooms/login_group/{room.roomId}");
                if (response.IsSuccessStatusCode)
                {
                    //MessageBox.Show($"Joined {room.roomName}");
                    Inst.Utils.MainWindow.frame2.NavigationService.Navigate(new Administraktoring(room));
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
        private int[] GetArray(string array)
        {            
            List<int> temp = new List<int>();
            return temp.ToArray();
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
