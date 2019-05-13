using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Forms;

namespace WpfApp1
{
    public static class Inst
    {
        public static Utils Utils;

        public static void CreateInstance()
        {
            Utils = new Utils();
        }
    }

    public class Utils
    {
        private HttpClient client;
        private Uri url;
        private User user;
        private MainWindow mainWindow;

        public Utils()
        {
            this.client = new HttpClient();
            this.url = new Uri("http://localhost:4000");
            client.BaseAddress = this.url;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public HttpClient HttpClient
        {
            get => client;
        }

        public Uri Url
        {
            get => url;
        }

        public User User
        {
            get => user;
            set
            {
                user = value;
                this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + value.token);
                Task.Run(() => PingR());
            }
        }

        public MainWindow MainWindow
        {
            get => mainWindow;
            set => mainWindow = value;
        }

        private async void Ping()
        {
            
            try
            {
                int time = 0;
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                while (true)
                {
                    if (time + 10 <= stopWatch.Elapsed.TotalSeconds)
                    {
                        var response = await client.GetAsync($"TimeOut/ping/{Inst.Utils.User.id}");
                        if (!response.IsSuccessStatusCode)
                        {
                            Inst.Utils.MainWindow.frame1.NavigationService.Navigate(new RoomsPage());//got kicked

                        }
                        time = (int)stopWatch.Elapsed.TotalSeconds;
                    }
                }
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
                return;
            }
        }
        private async void PingR()
        {            
            try
            {
                await Task.Delay(10000);
                var response = await client.GetAsync($"TimeOut/ping/{Inst.Utils.User.id}");
                if (!response.IsSuccessStatusCode)
                {
                    Inst.Utils.MainWindow.frame1.NavigationService.Navigate(new RoomsPage());//got kicked

                }
                PingR();
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
                return;
            }
        }
    }

    public class User
    {
        public string id { get; set; }
        public string username { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string token { get; set; }
    }
}
