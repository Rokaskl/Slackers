using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
using WebApi.Entities;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for RegisterUser.xaml
    /// </summary>
    public partial class RegisterUser : Window
    {
        
        private byte[] photo = null;
        private int userId = -1;

        public RegisterUser()
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            CheckPass();
        }
        private void CheckPass()
        {
            if (password.Password==passwordConfirm.Password&&password.Password.Equals(passwordConfirm.Password))
            {
                waitForRegister();
            }
            else
            {
                MessageBox.Show("Check password");
                
            }
        }
        private async void waitForRegister()
        {
            await register();
            await addInfo();            
        }
        private async Task register()
        {
            var user = new
            {
                FirstName=firstName.Text,
                LastName=lastName.Text,
                Username=userName.Text,
                Password=password.Password
            };
            User userResp = await Inst.ApiRequests.Register(user);
            if (userResp!=null)
            {
               this.userId = Int32.Parse(userResp.id);
            }
            else 
                MessageBox.Show("Failed");
        }
        private async Task addInfo()
        {  
            if (userId!=-1)
            {
                AdditionalData data = new AdditionalData(userId,true,(new TextRange(bio.Document.ContentStart,bio.Document.ContentEnd)).Text,photo);                
                if (await Inst.ApiRequests.UpdateAddDataUser(data))
                {      
                    this.Close();
                    ///MessageBox.Show("Photo upload succsesful");
                }
                else
                {
                    MessageBox.Show("Photo and bio upload failed");
                }
            }            
        }
        private void UploadPicture_Click(object sender, RoutedEventArgs e)
        {
            string fileName = "";
            photo = Inst.Utils.UploadPhoto(ref fileName);
            file.Content = fileName;
        }
    }
}
