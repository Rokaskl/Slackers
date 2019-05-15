﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
            this.GUID.Content = room.guid;
            this.MinHeight = 400;
            this.MinWidth = 800;
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
            ok.Click += (s,ev)=>
            {
                //Kick(kickingUserId);
                confirm.Close();
            };
            
            Button cancer = new Button();
            cancer.Content = "No";            
            cancer.Margin = new Thickness(10,15,15,10);
            cancer.Click += (s,ev) =>
            {
                confirm.Close();
            };
            
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
                    roomUsers.Items.Remove(user);
                }
            }
            catch (Exception)
            {
                throw;
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
            Inst.Utils.MainWindow.frame2.NavigationService.Navigate(new RoomPage(room,"admin"));
        }
    }
}