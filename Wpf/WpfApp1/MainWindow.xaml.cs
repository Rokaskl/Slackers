using System.Threading.Tasks;
using System.Windows;
using WpfApp1.Pages;
using System.Diagnostics;
using WpfApp1.Windows;
using System.Collections.Generic;
using WpfApp1.Forms;

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

        public MainWindow()
        {
            ip_port = Inst.Ip_selection_debugmode();

            InitializeComponent();
            
            this.MinHeight = 400;
            this.MinWidth = 800;
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
    }
}
