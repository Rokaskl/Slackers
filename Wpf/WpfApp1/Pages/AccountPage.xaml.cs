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

namespace WpfApp1.Pages
{
    /// <summary>
    /// Interaction logic for AccountPage.xaml
    /// </summary>
    public partial class AccountPage : Page
    {
        public AccountPage()
        {
            InitializeComponent();
            showInfo();
        }

        private void showInfo()
        {
            txtUsername.Text = Inst.ApiRequests.User.username;
            txtName.Text = Inst.ApiRequests.User.firstName;
            txtLastName.Text = Inst.ApiRequests.User.lastName;
        }

        private void BtnChangePass_Click(object sender, RoutedEventArgs e)
        {
            PassChangingForm passform = new PassChangingForm();
            passform.Show();
        }
    }
}
