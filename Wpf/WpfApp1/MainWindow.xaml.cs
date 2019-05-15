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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainForm.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HttpClient client;
        public MainWindow()
        {
            InitializeComponent();
            
            this.MinHeight = 400;
            this.MinWidth = 800;
            Inst.CreateInstance();
            Inst.Utils.MainWindow = this;
            client = Inst.Utils.HttpClient;
            LoginForm loginForm = new LoginForm();
            if (!(loginForm.ShowDialog() ?? false))
            {
                this.Close();
                return;
            }
            this.Closed += MainWindow_Closed;//prisisubscribinama po to, kai logino forma jau nebegali isjungti mainformos
            frame2.NavigationService.Navigate(new Admin());
            frame1.NavigationService.Navigate(new UserPage());           

        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            //Kai mainwindow yra uzdaromas - reikia i api nusiusti atsijungimo uzklausa.
        }

        private void Logout_Click(object sender, RoutedEventArgs e)//testuot - nesamone kazkokia sitas metodas
        {  
            this.Hide();
            Inst.CreateInstance();
            Inst.Utils.MainWindow = this;
            client = Inst.Utils.HttpClient;
            LoginForm loginForm = new LoginForm();
            if (!(loginForm.ShowDialog() ?? false))
            {
                this.Close();
                return;
            }            
            frame2.NavigationService.Navigate(new Admin());
            frame1.NavigationService.Navigate(new UserPage());     
            this.ShowDialog();
        }
    }
}
