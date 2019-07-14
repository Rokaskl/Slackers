using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfApp1.Forms;
using WpfApp1.Pages;

namespace WpfApp1
{
    public static class Inst
    {
        public static Utils Utils;
        public static ApiRequests ApiRequests;

        public static void CreateInstance()
        {
            Utils = new Utils();
            ApiRequests = new ApiRequests();
        }

        public static T GetChildOfType<T>(this DependencyObject depObj)
    where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
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
        private Room room;

        public Utils()
        {
            this.client = new HttpClient();
            //this.url = new Uri("http://localhost:4000");
            this.url = new Uri("http://192.168.0.142:10102");
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

        public Room Room
        {
            get => room;
            set => room = value;
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

    public class Room
    {
        public int Id;
        public List<User> users;//Joininus naujam useriui, listas lieka senas
        public string Room_Name;

        public Room(int id, string name)
        {
            this.Id = id;
            this.Room_Name = name;
            SetUsersList();
        }

        public async void SetUsersList()
        {
            users = await Inst.ApiRequests.GetUsersList(Id, true);
        }

        public string GetUsername(int id)
        {
            return users.FirstOrDefault(x => x.id == id.ToString())?.username;
        }
    }
}
