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

namespace WpfApp1.Windows
{
    /// <summary>
    /// Interaction logic for PassChangingWindow.xaml
    /// </summary>
    public partial class PassChangingWindow : Window
    {
        public PassChangingWindow()
        {
            InitializeComponent();
            newPassword.Focus();
            this.KeyDown += PassChangingForm_KeyDown;
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
                            Id = int.Parse(Inst.ApiRequests.User.id),
                            Username = Inst.ApiRequests.User.username,
                            FirstName = Inst.ApiRequests.User.firstName,
                            LastName = Inst.ApiRequests.User.lastName,
                            Password = newPassword.Password
                        };
                        //var response = await Inst.Utils.HttpClient.PutAsJsonAsync("/Users/" + Inst.Utils.User.id, info);

                        if (/*response.IsSuccessStatusCode*/await Inst.ApiRequests.UpdateAccount(info))
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

        private void PassChangingForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnChangePass.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                e.Handled = true;
            }
        }
    }
}
