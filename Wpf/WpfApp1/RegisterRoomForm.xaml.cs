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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for RegisterRoomForm.xaml
    /// </summary>
    public partial class RegisterRoomForm : Window
    {
        private int userId;
        private byte[] photo;
        private HttpClient client;
        private int rooomId = -1;

        public RegisterRoomForm(int userId)
        {
            this.userId = userId;
            client = Inst.Utils.HttpClient;
            InitializeComponent();
        }

        private void UploadPicture_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.DefaultExt = ".jpg";
            dialog.Filter = "Text documents (.jpg)|*.jpg";
            if(dialog.ShowDialog().HasValue)
            {
                photo = File.ReadAllBytes(dialog.FileName);
                file.Content = dialog.FileName;
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {           
            CreateAndUpload();
            this.Close();
        }
        private async void CreateAndUpload()
        {
            await registerroom(RoomName.Text);
            await Create();  
        }
        private async Task Create()
        {  
            if (rooomId!=-1)
            {
                string Bio = (new TextRange(RoomBio.Document.ContentStart,RoomBio.Document.ContentEnd)).Text;
                AdditionalData data = new AdditionalData(rooomId,false,Bio,photo);
                var response = await client.PostAsJsonAsync("AdditionalDatas", data);
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Photo upload succsesful");
                }
                else
                {
                    MessageBox.Show("Photo upload failed");
                }
            }            
        }
        private async Task registerroom(string name)
        {
            try
            {
                var club = new
                {
                    roomName = name
                };
                var response = await client.PostAsJsonAsync("/rooms/register", club);
                if (response.IsSuccessStatusCode)
                {                    
                    RoomDto temp = response.Content.ReadAsAsync<RoomDto>().Result;
                    MessageBox.Show("register successfully");  
                    rooomId = temp.roomId;
                }
                else
                {
                    MessageBox.Show("register failed...");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }            
        }
    }
}
