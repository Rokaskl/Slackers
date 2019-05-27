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
using WebApi.Dtos;
using Newtonsoft.Json;
using WpfApp1.Forms;
using WpfApp1.Pages;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainForm.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private HttpClient client;

        public MainWindow()
        {
            InitializeComponent();
            
            this.MinHeight = 400;
            this.MinWidth = 800;
            Inst.CreateInstance();
            Inst.Utils.MainWindow = this;
            //client = Inst.Utils.HttpClient;
            LoginForm loginForm = new LoginForm();
            if (!(loginForm.ShowDialog() ?? false))
            {
                this.Close();
                return;
            }
            Inst.Utils.CreateTcpServer();
            this.Closed += MainWindow_Closed;//prisisubscribinama po to, kai logino forma jau nebe gali isjungti mainformos
            frame2.NavigationService.Navigate(new Admin());
            frame1.NavigationService.Navigate(new UserPage());   
            roomPage.NavigationService.Navigate(new Pages.RoomPage());
            
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            //Task<bool> x = Logout();//Kai mainwindow yra uzdaromas - reikia i api nusiusti atsijungimo uzklausa.
        }

        private void Logout_Click(object sender, RoutedEventArgs e)//testuot - nesamone kazkokia sitas metodas
        {
            if (Inst.Utils.RoomPage != null)
            {
                (Inst.Utils.RoomPage as RoomPage).Logout();
            }
            Task<bool> x = Logout();
            Inst.Utils.StopTcpServer();
            this.Hide();
            Inst.CreateInstance();
            Inst.Utils.MainWindow = this;
            //client = Inst.Utils.HttpClient;
            LoginForm loginForm = new LoginForm();
            if (!(loginForm.ShowDialog() ?? false))
            {
                this.Close();
                return;
            }
            Inst.Utils.CreateTcpServer();
            frame2.NavigationService.Navigate(new Admin());
            frame1.NavigationService.Navigate(new UserPage());               
            roomPage.NavigationService.Navigate(new Pages.RoomPage());
            this.ShowDialog();
        }
        private async Task<bool> Logout()
        {           
            Inst.Utils.MainWindow.room.Visibility = Visibility.Hidden;
            enableButton();
            Inst.Utils.MainWindow.tabs.SelectedIndex = 0;
            //var response = await client.GetAsync("Users/logout");
            if (/*response.IsSuccessStatusCode*/await Inst.ApiRequests.Logout())
            {
                return true;
            }
            return false;
        }    
        private void enableButton()
        {
            Style style = new Style();
            style.TargetType = typeof(Button);            
            style.BasedOn = (Style)App.Current.FindResource("enable");
            style.Setters.Add(new Setter(Button.IsEnabledProperty,true));
            App.Current.Resources["enable"] = style;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Inst.Utils.RoomPage != null)
            {
                (Inst.Utils.RoomPage as RoomPage).Logout();
            }
            Task<bool> x = Logout();
        }

        private void Account_Click(object sender, RoutedEventArgs e)
        {
            tabs.Visibility = Visibility.Visible;
            tabs.SelectedIndex = 3;
            accountPage.NavigationService.Navigate(new AccountPage());
        }
    }
}
