﻿using System;
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
using System.Windows.Shapes;
using WebApi.Dtos;
using System.Diagnostics;
using System.Windows.Navigation;
using System.IO;

namespace WpfApp1.Forms
{
    /// <summary>
    /// Interaction logic for LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
        //private HttpClient client = Inst.Utils.HttpClient;

        public LoginForm()
        {
            InitializeComponent();
            this.KeyDown += LoginForm_KeyDown;
            Username.Focus();
            Username.GotFocus += txtGotFocus;
            Password.GotFocus += txtGotFocus;
        }

        private void txtGotFocus(object sender, RoutedEventArgs e)
        {
            lb_error.Visibility = Visibility.Hidden;
        }

        private void LoginForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnLogin.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                e.Handled = true;
            }
            if (e.Key == Key.F12)
            {
                new RegisterUser().Show();
                e.Handled = true;
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string nick = Username.Text;
            string pw = Password.Password;
            /*Task<bool> x = */Login();
            //e.Handled = true;
        }

        private async void Login()
        {
            try
            {
                var credentials = new UserDto()
                {
                    Username = Username.Text,
                    Password = Password.Password
                };
                //var response = await client.PostAsJsonAsync("/Users/authenticate/0", credentials);
                if (await Inst.ApiRequests.UserLogin(credentials))
                {
                    //var user = response.Content.ReadAsAsync<User>().Result;
                    //Inst.Utils.User = new User() { id = user.id, firstName = user.firstName, lastName = user.lastName, token = user.token, username = user.username };//WTF kam, kodėl
                    if (Inst.ApiRequests.User.firstName.Length > 13)
                    {
                        Inst.Utils.MainWindow.firstNameTextBlock.Text = Inst.ApiRequests.User.firstName.Substring(0, 13);
                    }
                    else
                    {
                        Inst.Utils.MainWindow.firstNameTextBlock.Text = Inst.ApiRequests.User.firstName;
                    }
                    if (await Inst.ApiRequests.UserLoginAddData())
                    {
                        SetProfilePhoto();
                    }

                    this.DialogResult = true;
                    this.Close();
                    //return true;
                }
                else
                {
                    Username.Text = string.Empty;
                    Password.Password = string.Empty;
                    lb_error.Visibility = Visibility.Visible;
                    //return false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //return false;
            }
        }
        private void SetProfilePhoto()
        {
            if (Inst.ApiRequests.AdditionalData != null)
            {
                if (Inst.ApiRequests.AdditionalData.PhotoBytes != null)
                {
                    using (var memstr = new MemoryStream(Inst.ApiRequests.AdditionalData.PhotoBytes))
                    {
                        var image = new BitmapImage();
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad; // here
                        image.StreamSource = memstr;
                        image.EndInit();
                        ImageBrush imgBrush = new ImageBrush();
                        imgBrush.ImageSource = image;
                        Inst.Utils.MainWindow.profilePicture.Fill = imgBrush;
                    }
                }
            }
        }
    }
}
