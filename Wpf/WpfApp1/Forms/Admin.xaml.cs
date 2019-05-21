﻿using System;
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
using WebApi.Entities;

namespace WpfApp1.Forms
{
    /// <summary>
    
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

            //adminRooms.SelectionMode = SelectionMode.Single;
            //adminRooms.SelectionChanged += AdminRooms_SelectionChanged;
            ////adminRooms.MouseDown += AdminRooms_MouseDown;
            //adminRooms.MouseRightButtonUp += AdminRooms_MouseRightButtonUp;
            //adminRooms.Items.
        }

        private void UserRooms_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //btnLoginRoom.Content = "Login " + (e.AddedItems[0] as RoomDto).roomName;
            //SelectedRoom = (e.AddedItems[0] as RoomDto);
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
            //btnLoginRoom.Content = "Login " + (e.AddedItems[0] as RoomDto).roomName;
            SelectedRoom = (e.AddedItems[0] as RoomDto);
            
        }

        
        private void BtnLoginRoom_Click(object sender, RoutedEventArgs e)
        {
            //LoginRoom(adminRooms.SelectedItem as Dictionary<string, object>);
            SelectedRoom = (RoomDto)((Button)sender).Tag;
            LoginRoom();
        }

        private void BtnRegisterRoom_Click(object sender, RoutedEventArgs e)
        {
            new RegisterRoomForm(Int32.Parse(Inst.Utils.User.id)).Show();

            //if (roomNameText.Text != "")
            //RegisterRoom(roomNameText.Text);
            //else

            // MessageBox.Show("Theres no room name");
        }
        private async void ShowRooms()
        {            
            try
            {
                var res2 = await client.GetAsync("Rooms/admin_get_rooms");
                //List<Dictionary<string, string>> userR = res2.Content.ReadAsAsync<List<Dictionary<string, string>>>().Result;                
                List<RoomDto> userR = res2.Content.ReadAsAsync<List<RoomDto>>().Result;                
                foreach (var item in userR)
                {
                    
                    Button btn = new Button();
                    btn.Content = "Login";
                    btn.Click += BtnLoginRoom_Click;
                    btn.Tag = item;
                    btn.Margin = new Thickness(2,2,2,2);
                    btn.HorizontalAlignment = HorizontalAlignment.Center;
                    btn.VerticalAlignment = VerticalAlignment.Center;

                    Button btnDelete = new Button();
                    btnDelete.Content = "Delete";
                    btnDelete.Click += RemoveRoom_Click;
                    btnDelete.Tag = item;
                    btnDelete.Margin = new Thickness(2,2,2,2);
                    btnDelete.HorizontalAlignment = HorizontalAlignment.Center;
                    btnDelete.VerticalAlignment = VerticalAlignment.Center;
                    btnDelete.Style = (Style)App.Current.Resources["bad"];
                    
                    Label name = new Label();
                    name.Content = item.roomName;
                    name.Margin = new Thickness(2,2,2,2);
                    name.VerticalAlignment = VerticalAlignment.Center;
                    name.HorizontalAlignment = HorizontalAlignment.Center;

                    Ellipse roomElipse = new Ellipse();
                    roomElipse.Width = 50;
                    roomElipse.Height = 50;

                    AdditionalData data = await GetAddData(item.roomId);                    
                    ImageBrush imgBrush = new ImageBrush();
                    if (data!=null)
                    {
                        using (var memstr = new MemoryStream(data.PhotoBytes))
                        {
                            var image = new BitmapImage();
                            image.BeginInit();
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.StreamSource = memstr;
                            image.EndInit();  
                            imgBrush.ImageSource = image;
                        }
                    }
                    roomElipse.Fill = imgBrush;
                    StackPanel roomPanel = new StackPanel();
                    roomPanel.Orientation = Orientation.Horizontal;
                    roomPanel.Children.Add(roomElipse);
                    roomPanel.Children.Add(name);
                    roomPanel.Children.Add(btn);
                    roomPanel.Children.Add(btnDelete);

                    RoomsList.Items.Add(roomPanel);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private async Task<AdditionalData> GetAddData(int id)
        {
            var resp = await client.GetAsync($"AdditionalDatas/{id}/{false}");
            if (resp.IsSuccessStatusCode)
            {
                AdditionalData data = resp.Content.ReadAsAsync<AdditionalData>().Result;
                return data;
            }
            return null;
        }
        //private async void ShowRooms()
        //{
        //    try
        //    {
        //        var res = await client.GetAsync("Rooms/admin_get_rooms");
        //        List<RoomDto> adminR = res.Content.ReadAsAsync<List<RoomDto>>().Result;
        //        adminRooms.ItemsSource = adminR;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //    }
        //}

        //private async void RegisterRoom(String name)
        //{
        //    try
        //    {
        //       
        //        //var club = new RoomDto()
        //        //{
        //        //    roomName = name
        //        //};
        //        //var response = await client.PostAsJsonAsync("/Rooms/register", club);
        //        //if (response.IsSuccessStatusCode)
        //        //{
        //        //    MessageBox.Show("Register Successfully");
        //        //    ShowRooms();
        //        //}
        //        //else
        //        //{
        //        //    MessageBox.Show("Register Failed...");
        //        //}

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //    }
        //}

        private async void LoginRoom()
        {
            RoomDto room = SelectedRoom;
            try
            {
                var response = await client.GetAsync($"/Rooms/login_group/{room.roomId}");
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"Joined {room.roomName}");
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
                    MessageBox.Show("Joining failed...");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        private void LoginAsAdmin_Click(object sender, RoutedEventArgs e)
        {
            RoomDto roomId = (RoomDto)((Button)sender).Tag;
            SelectedRoom = roomId;
            LoginRoom();
        }
        
        private void RemoveRoom_Click(object sender, RoutedEventArgs e)
        {
            RoomDto temp = (RoomDto)((Button)sender).Tag;
            SelectedRoom = temp;
            
            Window confirm = new Window();
            confirm.Title = "Delete room";
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
                DeleteRoom();
                confirm.Close();
            };
            
            Button cancer = new Button();
            cancer.Content = "No";   
            cancer.Style = (Style)App.Current.Resources["bad"];
            cancer.Margin = new Thickness(10,15,15,10);
            cancer.Width = 50;
            cancer.Click += (s,ev) =>
            {
                confirm.Close();
            };
            
            pan2.Children.Add(ok);
            pan2.Children.Add(cancer);
                        
            pan.VerticalAlignment = VerticalAlignment.Center;
            pan.HorizontalAlignment = HorizontalAlignment.Center;
            Label lab = new Label();
            lab.Content = $"Are you sure?\nDelete {SelectedRoom.roomName.Replace(" ",string.Empty)} room?";
            lab.HorizontalAlignment = HorizontalAlignment.Center;
            pan.Children.Add(lab);
            pan.Children.Add(pan2);

            confirm.Content = pan;
            confirm.ShowDialog();
        }
        private async void DeleteRoom()
        {
            var res = await client.DeleteAsync($"Rooms/{SelectedRoom.roomId}");
            if (res.IsSuccessStatusCode)
            {
                MessageBox.Show($"Room {SelectedRoom.roomName} successful deleted");
                RoomsList.Items.Clear();
                ShowRooms();
                //List<RoomDto> rooms = adminRooms.Items.Cast<RoomDto>().ToList<RoomDto>();
                //rooms.Remove(SelectedRoom);
                //adminRooms.ItemsSource = rooms;
            }
            else
                MessageBox.Show("Delete failed");
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RoomsList.Items.Clear();
            ShowRooms();
        }
    }
}
