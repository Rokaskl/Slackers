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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string nick = txt1.Text;
            string pw = txt2.Text;
            Task<bool> x = Login();
        }

        private async Task<bool> Login()
        {
            //client.BaseAddress = uri;
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var credentials = new UserDto()
                {
                    Username = txt1.Text,
                    Password = txt2.Text
                };
                var response = await client.PostAsJsonAsync("/Users/authenticate", credentials);
                
                if (response.IsSuccessStatusCode)
                {
                    var user = response.Content.ReadAsAsync<User>().Result;
                    //response.Content.
                    Inst.Utils.Token = user.token;
                    //Inst.Utils.Token = response.
                    this.DialogResult = true;
                    this.Close();
                    return true;
                }
                else
                {
                    txt1.Text = string.Empty;
                    txt2.Text = string.Empty;
                    MessageBox.Show("Register Failed...");
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
                //Console.WriteLine(ex.ToString());
            }
        }
    }
}
