using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Net.Http;
using WebApi.Dtos;
using WpfApp1.Forms;
using System.Diagnostics;
using WpfApp1.TimerControl;
using System.Reflection;

namespace WpfApp1.Forms
{
    /// <summary>
    /// Interaction logic for PassChangeForm.xaml
    /// </summary>
    public partial class PassChangeForm : Window
    {
        public PassChangeForm()
        {
            InitializeComponent();
        }

        private async void ChangePass()
        {
            try
            {
                if (!string.IsNullOrEmpty(newPassword.Password) && !string.IsNullOrEmpty(repeatedNewPassword.Password))
                {
                    if (newPassword.Password == repeatedNewPassword.Password)
                    {
                        var info = new UserDto
                        {
                            Id = int.Parse(Inst.Utils.User.id),
                            Username = Inst.Utils.User.username,
                            FirstName = Inst.Utils.User.firstName,
                            LastName = Inst.Utils.User.lastName,
                            Password = newPassword.Password
                        };
                        var response = await Inst.Utils.HttpClient.PutAsJsonAsync("/Users/" + Inst.Utils.User.id, info);

                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("Password changed successfully");
                            this.Close();
                        }
                        else
                            MessageBox.Show("Password change failed");
                    }
                    else
                        MessageBox.Show("Passwords don't match");
                }
                else
                    MessageBox.Show("Empty fields");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void BtnChangePass_Click(object sender, RoutedEventArgs e)
        {
            ChangePass();
        }
    }
}
