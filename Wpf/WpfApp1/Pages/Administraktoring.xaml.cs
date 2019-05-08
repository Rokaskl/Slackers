using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using WebApi.Dtos;

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
            ListUsers();

            InitializeComponent();
        }
        private void kickFromRoom_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private async void ListUsers()
        {

                var res = await client.PostAsJsonAsync("Rooms/get_list",room.users.ToList());
                List<User> adminR = res.Content.ReadAsAsync<List<User>>().Result;
                //adminRooms.ItemsSource = adminR;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            Inst.Utils.MainWindow.frame2.NavigationService.Navigate(new RoomPage(room));
        }
    }
}
