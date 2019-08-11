using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WebApi.Dtos;
using WpfApp1.Pages;
using WebApi.Entities;
using WpfApp1.Windows;
using WpfApp1.Controls;
using System.Threading.Tasks;

namespace WpfApp1.Forms
{
    /// <summary>
    /// Interaction logic for UserPage.xaml
    /// </summary>
    public partial class UserPage : Page
    {
        //private HttpClient client;
        //private Dictionary<string, string> SelectedRoom;
        public UserPage()
        {         
            //client = Inst.Utils.HttpClient;
            InitializeComponent();
            this.DataContext = Inst.ApiRequests.disable_enable; 
            this.gif.Source = Inst.LoadingGifSource;
            Inst.Utils.UserPage = this;
            ShowRooms();
        }

        private void BtnLoginRoom_Click(object sender, RoutedEventArgs e)
        {           
            if (Inst.Utils.IsLoginEnabled)
            {
                RoomDto temp = ((RoomDto)((ButtonCornering)sender).Tag);
                LoginRoom(temp); 
            }                      
        }

        public void UpdateRoomsListView()
        {
            ShowRooms();
        }

        private async void ShowRooms()
        {
            roomsListView.Visibility = Visibility.Hidden;
            gif.Visibility = Visibility.Visible;
            gif.Play();
            try
            {
                List<Dictionary<string, object>> roomsInfo = new List<Dictionary<string, object>>();
                List<RoomDto> userR = await Inst.ApiRequests.UserGetRooms();
                if (userR != null)
                    foreach (var item in userR)
                    {
                        Dictionary<string, object> tempRoom = new Dictionary<string, object>();
                        tempRoom.Add("roomName", item.roomName);
                        tempRoom.Add("room", item);
                        AdditionalData data = await /*GetAddData*/Inst.ApiRequests.GetRoomAddData(item.roomId);
                        tempRoom.Add("bio", data.Biography);
                        ImageBrush imgBrush = new ImageBrush();
                        if (data != null)
                        {
                            if (data.PhotoBytes != null)
                            {
                                using (var memstr = new MemoryStream(data.PhotoBytes))
                                {
                                    var image = new BitmapImage();
                                    image.BeginInit();
                                    image.CacheOption = BitmapCacheOption.OnLoad;
                                    image.StreamSource = memstr;
                                    image.EndInit();
                                    imgBrush.ImageSource = image;
                                    tempRoom.Add("photo", imgBrush);
                                }
                            }
                            else
                            {
                                tempRoom.Add("photo", Brushes.LightGray);
                            }
                        }                        
                        roomsInfo.Add(tempRoom);
                    }
                this.roomsListView.ItemsSource = roomsInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            roomsListView.Visibility = Visibility.Visible;
            gif.Visibility = Visibility.Hidden;
            gif.Stop();
            gif.Pause();
        }
        //private async void ShowRooms()
        //{            
        //    try
        //    {
        //        RoomsList.Items.Clear();
        //        //var res2 = await client.GetAsync("Rooms/user_get_rooms");
        //        //List<Dictionary<string, string>> userR = res2.Content.ReadAsAsync<List<Dictionary<string, string>>>().Result;                
        //        List<RoomDto> userR = await Inst.ApiRequests.UserGetRooms();//res2.Content.ReadAsAsync<List<RoomDto>>().Result;                
        //        if(userR!=null)
        //        foreach (var item in userR)
        //        {
                    
        //            Button btn = new Button();
        //            btn.Content = "Login";
        //            btn.Click +=BtnLoginRoom_Click;
        //            btn.Tag = item;
        //            btn.Margin = new Thickness(2,2,2,2);
        //            btn.HorizontalAlignment = HorizontalAlignment.Center;
        //            btn.VerticalAlignment = VerticalAlignment.Center;                    
        //            btn.Style = (Style)App.Current.Resources["enable"];
                    
        //            Label name = new Label();
        //            name.Content = item.roomName;
        //            name.Margin = new Thickness(2,2,2,2);
        //            name.VerticalAlignment = VerticalAlignment.Center;
        //            name.HorizontalAlignment = HorizontalAlignment.Center;

        //            Ellipse roomElipse = new Ellipse();
        //            roomElipse.Width = 50;
        //            roomElipse.Height = 50;

        //            AdditionalData data = await /*GetAddData*/Inst.ApiRequests.GetRoomAddData(item.roomId);                    
        //            ImageBrush imgBrush = new ImageBrush();
        //            if (data!=null)
        //            {
        //                if (data.PhotoBytes!=null)
        //                {
        //                    using (var memstr = new MemoryStream(data.PhotoBytes))
        //                    {
        //                        var image = new BitmapImage();
        //                        image.BeginInit();
        //                        image.CacheOption = BitmapCacheOption.OnLoad;
        //                        image.StreamSource = memstr;
        //                        image.EndInit();  
        //                        imgBrush.ImageSource = image;
        //                        roomElipse.Fill = imgBrush;
        //                    }
        //                }
        //            }
                    
        //            StackPanel roomPanel = new StackPanel();
        //            roomPanel.Orientation = Orientation.Horizontal;
        //            roomPanel.Children.Add(roomElipse);
        //            roomPanel.Children.Add(name);
        //            roomPanel.Children.Add(btn);
                    
        //            Button leave = new Button();
        //            leave.Content = "Leave";
        //                leave.Click += Leave_Click; ;
        //            leave.Tag = item;
        //            leave.Margin = new Thickness(2,2,2,2);
        //            leave.HorizontalAlignment = HorizontalAlignment.Center;
        //            leave.VerticalAlignment = VerticalAlignment.Center;                    
        //            leave.Style = (Style)App.Current.Resources["bad"];

        //            roomPanel.Children.Add(leave);
        //            RoomsList.Items.Add(roomPanel);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.ToString());
        //    }
        //}

        private void Leave_Click(object sender, RoutedEventArgs e)
        {
            var confirm = new ConfirmWindow($"Leave room {((RoomDto)((ButtonCornering)sender).Tag).roomName} ?");
            if (confirm.ShowDialog()==false)
            {
                if (confirm.Rezult)
                {
                    Leave(((RoomDto)((ButtonCornering)sender).Tag).roomId);
                }
            }            
        }
        private async void Leave(int roomId)
        {
            if (!await Inst.ApiRequests.LeaveRoom(roomId))
            {
                MessageBox.Show("Failed");
            }     
        }
        //private async Task<AdditionalData> GetAddData(int id)
        //{
        //    var resp = await client.GetAsync($"AdditionalDatas/{id}/{false}");
        //    if (resp.IsSuccessStatusCode)
        //    {
        //        AdditionalData data = resp.Content.ReadAsAsync<AdditionalData>().Result;
        //        return data;
        //    }
        //    return null;
        //}

        private async void LoginRoom(RoomDto roomid)
        {
            //List<Dictionary<string,string>> temp = userRooms.Items.Cast<Dictionary<string,string>>().Where(x =>Int32.Parse(x["roomId"])==roomid).ToList<Dictionary<string,string>>();

            //Dictionary<string, string> room = temp[0];
            try
            {
                //var response = await client.GetAsync($"/Rooms/login_group/{roomid.roomId}");
                if (/*response.IsSuccessStatusCode*/await Inst.ApiRequests.LoginRoom(roomid.roomId))
                {                
                    //MessageBox.Show($"Joined {roomid.roomName}");
                    Inst.Utils.MainWindow.roomPage.NavigationService.Navigate(new RoomPage(roomid,"user"));
                    Inst.Utils.MainWindow.room.Visibility = Visibility.Visible;                    
                    Inst.Utils.MainWindow.tabs.SelectedIndex = 2;  
                    Inst.Utils.IsLoginEnabled = false;
                    disableButton();
                }
                else
                {
                    //MessageBox.Show("Joining failed...");
                }
                //var response = await client.GetAsync($"/Rooms/login_group/{room["roomId"]}");
                //if (response.IsSuccessStatusCode)
                //{
                //    MessageBox.Show($"Joined {room["roomName"]}");
                //    Inst.Utils.MainWindow.roomPage.NavigationService.Navigate(new RoomPage(new RoomDto() { roomAdminId = Int32.Parse(room["roomAdminId"].ToString()), roomId = Convert.ToInt32(room["roomId"]), roomName = room["roomName"].ToString() },"user"));
                //    Inst.Utils.MainWindow.room.Visibility = Visibility.Visible;                    
                //    Inst.Utils.MainWindow.tabs.SelectedIndex = 2;  
                //    disableButton();
                //}
                //else
                //{
                //    MessageBox.Show("Joining failed...");
                //}

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private void disableButton()
        {
            Style style = new Style();
            style.TargetType = typeof(Button);
            style.BasedOn = (Style)App.Current.FindResource("enable");
            style.Setters.Add(new Setter(Button.IsEnabledProperty,false));
            App.Current.Resources["enable"] = style; 
        }
        private void BtnJoinRoom_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.guidText.Text))
            {
                return;
            }
            JoinRoom(this.guidText.Text);
        }

        private async void JoinRoom(string guid)
        {
            try
            {
                //var response = await client.PutAsJsonAsync($"/Rooms/join_group", new { guid = guid });
                if (/*response.IsSuccessStatusCode*/await Inst.ApiRequests.JoinGroup(guid))
                {
                    //MessageBox.Show($"successfuly became a member");
                    //Inst.Utils.MainWindow.frame.NavigationService.Navigate(new RoomPage(new RoomDto() { roomAdminId = Int32.Parse(room["roomAdminId"].ToString()), roomId = Int32.Parse(room["roomId"].ToString()), roomName = room["roomName"].ToString() }));
                    ShowRooms();
                    this.guidText.Text = string.Empty;
                }
                else
                {
                    MessageBox.Show("Available room was not found.");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        private void Gif_MediaEnded(object sender, RoutedEventArgs e)
        {
            gif.Position = new TimeSpan(0, 0, 0, 1);
            gif.Play();
        }
    } 
}
