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

namespace WpfApp1.Forms
{
    /// <summary>
    /// Interaction logic for RoomsPage.xaml
    /// </summary>
    public partial class RoomsPage : Page
    {
        private HttpClient client;
        public RoomsPage()
        {
            client = Inst.Utils.HttpClient;
            ShowRooms();
            InitializeComponent();
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
                List<Dictionary<string, string>> adminR = res.Content.ReadAsAsync<List<Dictionary<string, string>>>().Result;
                adminRooms.ItemsSource = adminR;

                /*var res2 = await client.GetAsync("Rooms/user_get_rooms");
                List<Dictionary<string, string>> userR = res2.Content.ReadAsAsync<List<Dictionary<string, string>>>().Result;
                userRooms.ItemsSource = userR;*/
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
    }
}
