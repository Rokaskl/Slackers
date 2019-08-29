﻿using System.Threading.Tasks;
using System.Windows;
using WpfApp1.Pages;
using System.Diagnostics;
using WpfApp1.Windows;
using System.Collections.Generic;
using WpfApp1.Forms;
using WpfApp1.ViewModels;
using System.Windows.Media;
using System;
using System.Linq;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainForm.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        //private HttpClient client;
        private KeyValuePair<string, int> ip_port;
        private NotifyingButtonViewModel btn_Friends_vm;

        public MainWindow()
        {
            ip_port = Inst.Ip_selection_debugmode();

            InitializeComponent();
            
            this.MinHeight = 400;
            this.MinWidth = 800;

            this.btn_Friends_vm = new NotifyingButtonViewModel();

            this.btn_Friends.DataContext = this.btn_Friends_vm;
            (this.btn_Friends.Content as System.Windows.Controls.Grid).GetChildOfType<System.Windows.Controls.Button>().Click += FriendsList_Button_Click;

            Inst.CreateInstance(ip_port);
            Inst.Utils.MainWindow = this;
            //client = Inst.Utils.HttpClient;
            LoginWindow loginForm = new LoginWindow();            
            loginForm.Closing -= Window_Closing;
            if (!(loginForm.ShowDialog() ?? false))
            {
                this.Close();
                return;
            }
            this.Closing += Window_Closing;
            Inst.Utils.CreateTcpServer();
            //this.Closed += MainWindow_Closed;//prisisubscribinama po to, kai logino forma jau nebe gali isjungti mainformos
            frame2.NavigationService.Navigate(new Admin());
            frame1.NavigationService.Navigate(new UserPage());   
            roomPage.NavigationService.Navigate(new RoomPage());
            SetupNotifications();
        }        

        //private void MainWindow_Closed(object sender, EventArgs e)
        //{
            
        //    //Task<bool> x = Logout();//Kai mainwindow yra uzdaromas - reikia i api nusiusti atsijungimo uzklausa.
        //}

        private void Logout_Click(object sender, RoutedEventArgs e)//testuot - nesamone kazkokia sitas metodas
        {
            if (Inst.Utils.RoomPage != null)
            {
                (Inst.Utils.RoomPage as RoomPage).Logout();
            }
            Task<bool> x = Logout();
            Inst.Utils.StopTcpServer();
            this.Hide();
            Inst.CreateInstance(ip_port);
            Inst.Utils.MainWindow = this;
            this.Closing -=Window_Closing;
            //client = Inst.Utils.HttpClient;
            LoginWindow loginForm = new LoginWindow();
            if (!(loginForm.ShowDialog() ?? false))
            {
                this.Close();
                return;
            }
            this.Closing +=Window_Closing;
            Inst.Utils.CreateTcpServer();
            frame2.NavigationService.Navigate(new Admin());
            frame1.NavigationService.Navigate(new UserPage());               
            roomPage.NavigationService.Navigate(new Pages.RoomPage());
            this.ShowDialog();
        }
        private async Task<bool> Logout()
        {           
            Inst.Utils.MainWindow.room.Visibility = Visibility.Hidden;            
            Inst.Utils.MainWindow.tabs.SelectedIndex = 0;
            //var response = await client.GetAsync("Users/logout");
            if (/*response.IsSuccessStatusCode*/await Inst.ApiRequests.Logout())
            {                
                Inst.Utils.Administraktoring = null;
                return true;
            }
            return false;
        }    
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var confirm = new ConfirmWindow("Logout and exit?");
            if (confirm.ShowDialog() == false)
            {
                if (confirm.Rezult)
                {
                    if (Inst.Utils.RoomPage != null)
                    {
                        (Inst.Utils.RoomPage as RoomPage).Logout();
                    }
                    Task<bool> x = Logout();
                    if (this.Fl_form != null)
                    {
                        this.Fl_form.Close();
                    }
                    SaveNotificationsToServer();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void Account_Click(object sender, RoutedEventArgs e)
        {
            tabs.Visibility = Visibility.Visible;
            tabs.SelectedIndex = 3;
            accountPage.NavigationService.Navigate(new AccountPage());
        }

        private void Web_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("http://localhost:8080/login"));
            e.Handled = true;
        }

        public FriendsListForm Fl_form;
        private int Notification_count = 0;
        public List<int> notifications = new List<int>();
        public List<int> friends_notif;
        
        private async void SetupNotifications()
        {
            notifications = await Inst.ApiRequests.GetNotifications();
            if (notifications.Count >= 8)
            {
                Notification_count = notifications.Sum();
                this.btn_Friends_vm.Notifications = Notification_count;
            }
            if (notifications.Count > 8)
            {
                friends_notif = notifications.Skip(8).ToList();
            }
        }

        private async void SaveNotificationsToServer()
        {
            await Inst.ApiRequests.SaveNotificationsToServer(notifications);
        }

        public void HandleSignal_for_Fl_form(int command, int user_id)
        {
            if (this.Fl_form != null)
            {
                this.Fl_form.HandleTcpSignal(command, user_id);
            }
            else
            {
                if (command < 7)
                {
                    System.Media.SystemSounds.Exclamation.Play();
                    Notification_count++;
                    notifications[command]++;
                    this.btn_Friends_vm.Notifications = Notification_count;
                }
            }
        }

        public void HandleSignal_for_Fl_forms_friendschat_form(string text, int user_id)
        {
            if (this.Fl_form != null)
            {
                this.Fl_form.NewChatLine(user_id, text);
            }
            else
            {
                System.Media.SystemSounds.Exclamation.Play();
                Notification_count++;
                notifications.Add(user_id);
                this.btn_Friends_vm.Notifications = Notification_count;
            }
        }

        private void FriendsList_Button_Click(object sender, RoutedEventArgs e)
        {
            //(new FriendsListForm(Inst.ApiRequests.AdditionalData.)).Show();
            if (this.Fl_form == null)
            {
                ImageBrush img = new ImageBrush();
                img.ImageSource = Inst.PhotoBytes_to_Image(Inst.ApiRequests.AdditionalData.PhotoBytes);
                FriendsListViewModel fl_vm = new FriendsListViewModel(img, Inst.ApiRequests.User.username, int.Parse(Inst.ApiRequests.User.id), Inst.ApiRequests.AdditionalData.Biography);
                FriendsListForm fl_form = new FriendsListForm(fl_vm, notifications, this.friends_notif);
                this.Fl_form = fl_form;
                fl_form.Show();
            }
            else
            {
                this.Fl_form.Focus();
            }
            this.btn_Friends_vm.Notifications = 0;
        }
    }
}
