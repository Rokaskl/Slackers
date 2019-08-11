using Microsoft.Win32;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public static class Inst
    {
        public static Utils Utils;
        public static ApiRequests ApiRequests;
        public static Uri LoadingGifSource = new Uri(Directory.GetCurrentDirectory()+@"\..\..\Media/496.gif");
        

        public static void CreateInstance()
        {
            Utils = new Utils();
            ApiRequests = new ApiRequests();
        }
    }

    public class Utils
    {
        private bool isLoginEnabled = true;
        private HttpClient client;
        private Uri url;
        //private User user;
        private MainWindow mainWindow;
        private Page roomPage;
        private Page roomOverViewPage;
        private Page userPage;
        private Page adminPage;
        private TcpDock tcp_client;

        public Utils()
        {
            this.client = new HttpClient();
            this.url = new Uri("http://localhost:4000");
            client.BaseAddress = this.url;
            roomPage = null;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public void CreateTcpServer()
        {
            tcp_client = new TcpDock();
        }

        public void StopTcpServer()
        {
            tcp_client.Stop();
        }

        //public HttpClient HttpClient
        //{
        //    get => client;
        //}

        //public Uri Url
        //{
        //    get => url;
        //}

        //public User User
        //{
        //    get => user;
        //    set
        //    {
        //        user = value;
        //        this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + value.token);
        //        Task.Run(() => PingR());
        //    }
        //}
        public bool IsLoginEnabled
        {
            get => isLoginEnabled;
            set => isLoginEnabled = value;
        }

        public MainWindow MainWindow
        {
            get => mainWindow;
            set => mainWindow = value;
        }

        public Page RoomPage
        {
            get => roomPage;
            set => roomPage = value;
        }

        public Page Administraktoring
        {
            get => roomOverViewPage;
            set => roomOverViewPage = value;
        }

        public Page UserPage
        {
            get => userPage;
            set => userPage = value;
        }

        public Page AdminPage
        {
            get => adminPage;
            set => adminPage = value;
        }
        public byte[] UploadPhoto(ref string fileName)
        {
            OpenFileDialog dialog = new OpenFileDialog();            
            dialog.Filter = "Picture file (.jpg)|*.jpg|(.bmp)|*.bmp|(.png)|*.png|(.tif)|*.tif|(.jfif)|*.jfif|(.gif)|*.gif";
            if(dialog.ShowDialog().HasValue&&File.Exists(dialog.FileName)&&(dialog.FileName.EndsWith(".jfif")||dialog.FileName.EndsWith(".jpg")||dialog.FileName.EndsWith(".bmp")||dialog.FileName.EndsWith(".png")||dialog.FileName.EndsWith(".tif")||dialog.FileName.EndsWith(".gif")))
            {
                if ( (new FileInfo(dialog.FileName).Length)/1024/1024<1)
                {
                    fileName = dialog.FileName;
                    return File.ReadAllBytes(dialog.FileName);
                }
                else
                {
                    MessageBox.Show("Photo too big");
                    return null;
                }
            }
             else
            {
                return null;
            }
        }
        //private async void Ping()
        //{            
        //    try
        //    {
        //        int time = 0;
        //        Stopwatch stopWatch = new Stopwatch();
        //        stopWatch.Start();
        //        while (true)
        //        {
        //            if (time + 10 <= stopWatch.Elapsed.TotalSeconds)
        //            {
        //                var response = await client.GetAsync($"TimeOut/ping/{Inst.ApiRequests.User.id}");
        //                if (!response.IsSuccessStatusCode)
        //                {
        //                    Inst.Utils.MainWindow.frame1.NavigationService.Navigate(new RoomsPage());//got kicked

        //                }
        //                time = (int)stopWatch.Elapsed.TotalSeconds;
        //            }
        //        }
        //    }
        //    catch(Exception exception)
        //    {
        //        Console.WriteLine(exception.ToString());
        //        return;
        //    }
        //}
        //private async void PingR()
        //{            
        //    try
        //    {
        //        await Task.Delay(10000);
        //        var response = await client.GetAsync($"TimeOut/ping/{Inst.ApiRequests.User.id}");
        //        if (!response.IsSuccessStatusCode)
        //        {
        //            Inst.Utils.MainWindow.frame1.NavigationService.Navigate(new RoomsPage());//got kicked

        //        }
        //        PingR();
        //    }
        //    catch(Exception exception)
        //    {
        //        Console.WriteLine(exception.ToString());
        //        return;
        //    }
        //}

        //public event EventHandler MembersChanged;
        ////private delegate void MyChangesEventHandler(object sender, ChangesEventArgs e);
        //public virtual void RaiseMembersChangedEvent(object sender, EventArgs e)
        //{
        //    EventHandler handler = MembersChanged;
        //    handler?.Invoke(this, e);
        //    //if (MessageChanged != null)
        //    //{
        //    //    MessageChanged(this, e);
        //    //}
        //}
    }

    public class User
    {
        public string id { get; set; }
        public string username { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string token { get; set; }
    }
    public class Note
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int CreatorId { get; set; }
        public string Message { get; set; }
        public int Status { get; set; }
        public string Header { get; set; }

        public override string ToString()
        {
            return String.Format($"{Id} - {Header}");
        }
    }
    public class ChatLine
    {
        public string Id { get; set; }
        public int RoomId { get; set; }
        public int CreatorId { get; set; }
        public DateTime CreateDate { get; set; }
        public string Text { get; set; }
        public string Username { get; set; }

        public override string ToString()
        {
            return String.Format($"{Username}: {Text}");
        }
    }
}
