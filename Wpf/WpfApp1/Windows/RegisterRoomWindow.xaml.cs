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

namespace WpfApp1.Windows
{
    /// <summary>
    /// Interaction logic for RegisterRoomWindow.xaml
    /// </summary>
    public partial class RegisterRoomWindow : Window
    {
        private int userId = -1;
        private byte[] photo = null;
        private int rooomId = -1;
        private Admin adminPage;

        public RegisterRoomWindow(int userId, Admin adminPage)
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.userId = userId;
            InitializeComponent();
            //this.Closed += RegisterRoomForm_Closed;
            this.adminPage = adminPage;
        }

        public RegisterRoomWindow(RoomDto room, Admin adminPage)
        {
            this.adminPage = adminPage;
            this.userId = 0;
            this.rooomId = room.roomId;
            InitializeComponent();
            //this.Closed += RegisterRoomForm_Closed;
            this.RoomName.Text = room.roomName;
            this.RoomName.IsReadOnly = true;
            this.RoomBio.Document.Blocks.Clear();
            getAddData(room.roomId);
            //this.create.Content = "Submit edit";

        }

        //private void RegisterRoomForm_Closed(object sender, EventArgs e)
        //{
        //    //this.adminPage.UpdateRoomView();
        //}

        public async void getAddData(int roomId)
        {
            //var resp = await client.GetAsync($"AdditionalDatas/{roomId}/{false}");
            AdditionalData tempData = await Inst.ApiRequests.GetRoomAddData(roomId);
            if (/*resp.IsSuccessStatusCode*/tempData!=null)
            {
                //AdditionalData tempData = resp.Content.ReadAsAsync<AdditionalData>().Result;
                this.RoomBio.Document.Blocks.Add(new Paragraph(new Run(tempData.Biography)));
            }
        }
        private void UploadPicture_Click(object sender, RoutedEventArgs e)
        {
            string fileName = "";
            photo = Inst.Utils.UploadPhoto(ref fileName);
            file.Content = fileName;
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {           
            CreateAndUpload();
            this.Close();
        }
        private async void CreateAndUpload()
        {
            if (userId!=0)
            {
                await registerroom(RoomName.Text);
            }            
            await Create();  
        }
        private async Task Create()
        {  
            if (rooomId!=-1||userId==0)
            {
                //string Bio = (new TextRange(RoomBio.Document.ContentStart,RoomBio.Document.ContentEnd)).Text;
                AdditionalData data = new AdditionalData(rooomId,false,(new TextRange(RoomBio.Document.ContentStart,RoomBio.Document.ContentEnd)).Text,photo);
                //var response = await client.PostAsJsonAsync("AdditionalDatas", data);
                if (await Inst.ApiRequests.UpdateAddDataRoom(data))
                {
                    //MessageBox.Show("Photo upload succsesful");
                }
                else
                {
                    //MessageBox.Show("Photo upload failed");
                }
            }            
        }
        private async Task registerroom(string name)
        {
            try
            {
                //var club = new
                //{
                //    roomName = name
                //};
                //var response = await client.PostAsJsonAsync("/rooms/register", club);
                RoomDto temp = await Inst.ApiRequests.RegisterRoom(name);
                if (/*response.IsSuccessStatusCode*/temp!=null)
                {                    
                    //RoomDto temp = response.Content.ReadAsAsync<RoomDto>().Result;
                    //MessageBox.Show("register successfully");  
                    rooomId = temp.roomId;
                }
                //else
                //{
                //    //MessageBox.Show("register failed...");
                //}

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }            
        }
    }
}
