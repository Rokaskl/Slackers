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
using Json.Net;
using WebApi.Dtos;
using  Newtonsoft.Json;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Uri uri= new Uri("http://localhost:4000");
        public MainWindow()
        {
            InitializeComponent();
        }
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            roomMaker(1,1,"GayClub",new int[]{1,2,3,4,5,6});
            //register("tadas","Šiknaspardis","suldubulduVabaliukai","gayClub");
        }
        private void btnAuthorize_Click(object sender, RoutedEventArgs e)
        {
            Task<HttpClient> sesion= authenticate("gayClub","suldubulduVabaliukai");
            
        }

        public async void register(string firstName, string lastName, string password,string username)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = uri;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var dude = new  UserDto()
                {
                    Id = 1,
                    FirstName = firstName,
                    LastName = lastName,
                    Password = password,
                    Username = username
                };
                var response = await client.PostAsJsonAsync("/Users/register", dude);
                if (response.IsSuccessStatusCode)  
                {  
                    MessageBox.Show("Register Successfully");  
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
        public async void roomMaker(int id,int Aid,string name,int[] users)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = uri;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var club = new  RoomDto()
                {
                    roomId = id,
                    roomAdminId = Aid,
                    roomName = name,
                    users = users
                };
                var response = await client.PostAsJsonAsync("/Rooms", club);
                if (response.IsSuccessStatusCode)  
                {  
                    MessageBox.Show("Register Successfully");  
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
        public async Task<HttpClient> authenticate(string username, string password)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = uri;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var dude = new UserDto()
            {
                Username = username,
                Password = password
            };
            var response = await client.PostAsJsonAsync("/Users/authenticate",dude);
            if (response.IsSuccessStatusCode)  
            {  
                var user = response.Content.ReadAsAsync<User>().Result;
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + user.token);
                return client;
            }  
            else  
            {  
                MessageBox.Show("Login Failed...");  
            }
            return null;
        }
        public class User
        {
            public string id { get; set; }
            public  string username { get; set; }
            public  string firstName { get; set; }
            public string lastName { get; set; }
            public  string token { get; set; }
        }
    }
}