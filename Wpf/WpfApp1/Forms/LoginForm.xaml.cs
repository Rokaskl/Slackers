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
using System.Windows.Shapes;
using WebApi.Dtos;
using System.Diagnostics;
using System.Windows.Navigation;

namespace WpfApp1.Forms
{
    /// <summary>
    /// Interaction logic for LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
        private HttpClient client = Inst.Utils.HttpClient;

        public LoginForm()
        {
            InitializeComponent();
            Username.Focus();
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
            Task<bool> x = Login();
        }

        private async Task<bool> Login()
        {
            try
            {
                var credentials = new UserDto()
                {
                    Username = Username.Text,
                    Password = Password.Password
                };
                var response = await client.PostAsJsonAsync("/Users/authenticate", credentials);
                
                if (response.IsSuccessStatusCode)
                {
                    var user = response.Content.ReadAsAsync<User>().Result;
                    Inst.Utils.User = new User() { id = user.id, firstName = user.firstName, lastName = user.lastName, token = user.token, username = user.username };
                    Inst.Utils.MainWindow.firstNameTextBlock.Text = user.firstName;
                    this.DialogResult = true;
                    this.Close();
                    return true;
                }
                else
                {
                    Username.Text = string.Empty;
                    Password.Password = string.Empty;
                    MessageBox.Show("Register Failed...");
                    return false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
    }
}
