using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using WebApi.Dtos;
using WebApi.Entities;
using WpfApp1.Forms;

namespace WpfApp1.Pages
{
    /// <summary>
    /// Interaction logic for AccountPage.xaml
    /// </summary>
    public partial class AccountPage : Page
    {
        private byte[] photo = null;
        public AccountPage()
        {
            InitializeComponent();
            ShowInfo();
            SetBio();
            SetProfilePhoto();
        }
        private void SetBio()
        {
            if (Inst.ApiRequests.AdditionalData.Biography!=null)
            {
             this.bio.Document.Blocks.Clear();
             this.bio.Document.Blocks.Add(new Paragraph(new Run(Inst.ApiRequests.AdditionalData.Biography)));      
            }
        }
        private void ShowInfo()
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

        private void BtnChangePhoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".jpg";
            dialog.Filter = "Text documents (.jpg)|*.jpg";
            if (dialog.ShowDialog().HasValue && File.Exists(dialog.FileName))
            {
                photo = File.ReadAllBytes(dialog.FileName);
                ChangeData();
            }
        }

        private async void ChangeData()
        {
            int userId = Int32.Parse(Inst.ApiRequests.User.id);
            AdditionalData data = new AdditionalData(userId, true, "", photo);
            if (await Inst.ApiRequests.UpdateAddDataUser(data))
            {
                SetProfilePhoto();
                MessageBox.Show("Photo upload succsesful");
            }
            else
            {
                MessageBox.Show("Photo upload failed");
            }
        }

        private async void SetProfilePhoto()
        {
            if (await Inst.ApiRequests.UserLoginAddData())
            {
                if (Inst.ApiRequests.AdditionalData != null)
                {
                    if (Inst.ApiRequests.AdditionalData.PhotoBytes != null)
                    {
                        using (var memstr = new MemoryStream(Inst.ApiRequests.AdditionalData.PhotoBytes))
                        {
                            var image = new BitmapImage();
                            image.BeginInit();
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.StreamSource = memstr;
                            image.EndInit();
                            profilePicture.ImageSource = image;
                            ImageBrush imgBrush = new ImageBrush();
                            imgBrush.ImageSource = image;
                            Inst.Utils.MainWindow.profilePicture.Fill = imgBrush; 
                        }
                    }
                }
            }
        }

        private void SubmitBio_Click(object sender, RoutedEventArgs e)
        {
            UpdateBio();
        }
        private async void UpdateBio()
        {
            if (await Inst.ApiRequests.UpdateAddDataUser(new AdditionalData(Int32.Parse(Inst.ApiRequests.User.id),true,(new TextRange(bio.Document.ContentStart,bio.Document.ContentEnd)).Text,null)))
            {
                await Inst.ApiRequests.UserLoginAddData();
                SetBio();
                //MessageBox.Show("Photo upload succsesful");
            }
            else
            {
                MessageBox.Show("Bio update failed");
            }
        }
    }
}
