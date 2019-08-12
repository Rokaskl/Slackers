using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WebApi.Dtos;
using WebApi.Entities;
using WpfApp1.Forms;
using WpfApp1.Helpers;
using WpfApp1.Windows;
using WpfApp1.ViewModels;

namespace WpfApp1
{
    public class ApiRequests
    {
        #region 
        private HttpClient client;
        private Uri url;
        private User user;
        private AdditionalData additionalData;
        public EnableChangeHandle disable_enable;
        #endregion

        public ApiRequests(KeyValuePair<string, int> ip_port)
        {
            this.client = new HttpClient();
            //this.url = new Uri("http://localhost:4000");
            this.url = new Uri($"http://{ip_port.Key}:{ip_port.Value}");
            this.client.BaseAddress = url;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            disable_enable = new EnableChangeHandle();
        }

        #region Chat
        public async Task<List<ChatLine>> GetChat(int roomId, int page, int items_per_page)
        {
            if (roomId<1)
            {
                return null;
            }
            var response = await client.GetAsync($"/ChatLine/lines/{roomId}/{page}/{items_per_page}");
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<List<ChatLine>>().Result;
            }
            return null;
        }
        public async Task<bool> CreateEntry(string entry,int roomId)
        {
            if (entry==null)
            {
                return false;
            }
            var response = await client.PostAsJsonAsync<string>($"/ChatLine/create/{roomId}", entry);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Notes requests
        public async Task<bool> SubmitNote(Dictionary<string, string> info)
        {
            if (info==null)
            {
                return false;
            }
            var response = await client.PostAsJsonAsync<Dictionary<string, string>>($"/Notes/submit", info);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }        
        public async Task<bool> ModNote(Note note)
        {
            if (note==null)
            {
                return false;
            }
            var response = await client.PostAsJsonAsync<Note>($"/Notes/modify", note);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> DelNote(int roomId,int noteId)
        {
            if (roomId<1||noteId<1)
            {
                return false;
            }
            var response = await client.GetAsync($"/Notes/delete/{roomId}/{noteId}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
        public async Task<List<Note>> GetRoomNotes(int roomId)
        {
            if (roomId<1)
            {
                return null;
            }
            var response = await client.GetAsync($"/Notes/{roomId}");
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<List<Note>>().Result;
            }
            return null;
        }
        #endregion

        #region Room requests, time marks
        public async Task<bool> LogoutGroup(int roomId)
        {
            if (roomId<1)            
                return false;
            var response = await client.GetAsync($"/Rooms/logout_group/{roomId}");
            if (response.IsSuccessStatusCode)
            {
                disable_enable.IsEnabled = true;
                return true;
            }
            return false;
        }
        public async Task<List<Newtonsoft.Json.Linq.JObject>> GetGroupMembers(int roomId)
        {
            if (roomId<1)
            {
                return null;
            }
            var response = await client.GetAsync($"/Rooms/group/{roomId}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<List<Newtonsoft.Json.Linq.JObject>>();
            }
            return null;
        }
        public async Task<bool> TimerMark(int roomId,int markType)
        {
            if (roomId<1||markType<0||markType>1)
            {
                return false;
            }
            var response = await client.GetAsync($"/TimeTracker/mark/{roomId}/{markType}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> UpdateStatus(int roomId,char status)
        {
            if (roomId<1)
            {
                return false;
            }
            var res = await client.GetAsync($"/Rooms/status/{roomId}/{status}");
            if (res.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> LoginRoom(int id)
        {
            if (id<1)
            {
                return false;
            }
            var response = await client.GetAsync($"/Rooms/login_group/{id}");
            if (response.IsSuccessStatusCode)
            {
                disable_enable.IsEnabled = false;
                return true;
            }
            return false;
        }

        //public async Task<List<User>> GetRoomUserList(int id)
        //{
        //    var response = await client.GetAsync($"/Rooms/group_members/{id}");
        //    if (response.IsSuccessStatusCode)
        //    {
        //        return await response.Content.ReadAsAsync<List<User>>();
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}
        #endregion

        #region User, account
        public async Task<bool> UpdateAddDataUser(AdditionalData data)
        {
            if (data==null)
            {
                return false;
            }
            var response = await client.PostAsJsonAsync("AdditionalDatas", data);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
        public async Task<User> Register(object user)
        {
            if (user==null)
            {
                return null;
            }
            var resp = await client.PostAsJsonAsync("Users/register",user);
            if (resp.IsSuccessStatusCode)
            {
                return resp.Content.ReadAsAsync<User>().Result;
            }
            var warning = new WarningWindow(resp.Content.ReadAsAsync<string>().Result);
            warning.ShowDialog();
            return null;
        }
        public async Task<bool> Logout()
        {
            var response = await client.GetAsync("Users/logout");
            if (response.IsSuccessStatusCode)
            {
                this.additionalData = null;
                return true;
            }
            return false;
        }
        public async Task<bool> UpdateAccount(UserDto user)
        {
            if (user==null)
            {
                return false;
            }
            var response = await this.client.PutAsJsonAsync("/Users/" + Int32.Parse(this.user.id), user);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> UserLogin(UserDto credentials)
        {
            if (credentials==null)
            {
                return false;
            }
            var response = await this.client.PostAsJsonAsync("/Users/authenticate/0", credentials);
            if (response.IsSuccessStatusCode)
            {
                User = response.Content.ReadAsAsync<User>().Result;
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<List<RoomDto>> UserGetRooms()
        {
            var res = await client.GetAsync("Rooms/user_get_rooms");
            if (res.IsSuccessStatusCode)
            {
                return res.Content.ReadAsAsync<List<RoomDto>>().Result;
            }
            return null;
        }
        public async Task<bool> LeaveRoom(int roomId)
        {
            if (roomId<1)
            {
                return false;
            }
            var resp = await client.PutAsJsonAsync("Rooms/leave",new {roomId=roomId });
            if (resp.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> JoinGroup(string guid)
        {
            if (guid==null)
            {
                return false;
            }
            var response = await client.PutAsJsonAsync($"/Rooms/join_group", new { guid = guid });
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> UserLoginAddData()
        {
            var resp = await this.client.GetAsync($"AdditionalDatas/{this.user.id}/{true}");
            if (resp.IsSuccessStatusCode)
            {
                this.additionalData = resp.Content.ReadAsAsync<AdditionalData>().Result;
                return true;
            }
            return false;
        }
        #endregion

        #region Admin room
        public async Task<bool> DeleteRoom(int roomId)
        {
            if (roomId<1)
            {
                return false;
            }
            var res = await client.DeleteAsync($"Rooms/{roomId}");
            if (res.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
        public async Task<bool> UpdateAddDataRoom(AdditionalData data)
        {
            if (data==null)
            {
                return false;
            }
            var response = await client.PostAsJsonAsync("AdditionalDatas", data);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
        public async Task<RoomDto> RegisterRoom(string roomName)
        {
            if (roomName==null||roomName.Length==0||string.IsNullOrWhiteSpace(roomName))
            {
                return null;
            }
            var response = await client.PostAsJsonAsync("/rooms/register", new{roomName=roomName });
            
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsAsync<RoomDto>().Result;
            }
            return null;
        }
        public async Task<List<RoomDto>> AdminGetRooms()
        {
            var res = await client.GetAsync("Rooms/admin_get_rooms");
            if (res.IsSuccessStatusCode)
            {
                return res.Content.ReadAsAsync<List<RoomDto>>().Result;  
            }
            return null;
        }
        public async Task<List<User>> GetUsersList(int roomId, bool get_with_admin = false)
        {
            var res = await client.GetAsync($"Rooms/group_members/{roomId}/{get_with_admin}");
            if (res.IsSuccessStatusCode)
            {
                return res.Content.ReadAsAsync<List<User>>().Result;
            }
            return null;
        }
        public async Task<Dictionary<int, int>> GetTimes(DateTime from,DateTime to,int roomId)
        {
            if (from==null||to==null||roomId<1)
            {
                return null;
            }
            var resp = await client.GetAsync($"TimeTracker/timeroom/{from}/{to}/{roomId}/{0}");
            if (resp.IsSuccessStatusCode)
            {
                return resp.Content.ReadAsAsync<Dictionary<int, int>>().Result; 
            }
            return null;
        }
        public async Task<bool> KickUser(int user,int roomId)
        {
            if (user<1||roomId<1)
            {
                return false;
            }
            var res = await client.PutAsJsonAsync("Rooms/kick_user", new { roomId = roomId, userId = user });
            if (res.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }
        public async Task<AdditionalData> GetUserAddData(int id)
        {
            if (id<1)
            {
                return null;
            }
            var resp = await this.client.GetAsync($"AdditionalDatas/{id}/{true}");
            if (resp.IsSuccessStatusCode)
            {
                return resp.Content.ReadAsAsync<AdditionalData>().Result;
                
            }
            return null;
        }
        public async Task<AdditionalData> GetRoomAddData(int roomId)
        {
            if (roomId<1)
            {
                return null;
            }
            var resp = await this.client.GetAsync($"AdditionalDatas/{roomId}/{false}");
            if (resp.IsSuccessStatusCode)
            {
                return resp.Content.ReadAsAsync<AdditionalData>().Result;                
            }
            return null;
        }

        public async Task<bool> ChangeRoomGuid(RoomDto room)
        {
            var resp = await this.client.GetAsync($"Rooms/change_guid/{room.roomId}");
            if (resp.IsSuccessStatusCode)
            {
                Guid new_guid = resp.Content.ReadAsAsync<Guid>().Result;
                room.guid = new_guid.ToString();
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Friends List

        public async Task<List<UsersListLineViewModel>> GetFriends(int UserId)//Should be Image, Username, id, status.
        {
            List<UsersListLineViewModel> friends_list = new List<UsersListLineViewModel>();
            var resp = await this.client.GetAsync($"Friends/{UserId}");
            if (resp.IsSuccessStatusCode)
            {
                resp.Content.ReadAsAsync<List<Dictionary<string, object>>>().Result.ForEach(x => friends_list.Add(new UsersListLineViewModel(x["photobytes"], null, null, x["username"].ToString(), int.Parse(x["status"].ToString()) == 1 ? Brushes.Green : Brushes.Gray, int.Parse(x["id"].ToString()), null)));
                return friends_list;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<UsersListLineViewModel>> GetFriendRequests(int UserId, string direction_command)
        {
            List<UsersListLineViewModel> requests_list = new List<UsersListLineViewModel>();
            var resp = await this.client.GetAsync($"Friends/requests/{UserId}/{direction_command}");
            if (resp.IsSuccessStatusCode)
            {
                resp.Content.ReadAsAsync<List<Dictionary<string, object>>>().Result.ForEach(x => requests_list.Add(new UsersListLineViewModel(x["photobytes"], null, null, x["username"].ToString(), int.Parse(x["status"].ToString()) == 1 ? Brushes.Green : Brushes.Gray, int.Parse(x["id"].ToString()), null)));
                return requests_list;
            }
            else
            {
                return null;
            }
        }

        public async Task<List<UsersListLineViewModel>> SearchFriends(string search_string)
        {
            List<UsersListLineViewModel> found_list = new List<UsersListLineViewModel>();
            var resp = await this.client.GetAsync($"Friends/search/{this.User.id}/{search_string}");
            if (resp.IsSuccessStatusCode)
            {
                resp.Content.ReadAsAsync<List<Dictionary<string, object>>>().Result.ForEach(x => found_list.Add(new UsersListLineViewModel(x["photobytes"], null, null, x["username"].ToString(), int.Parse(x["status"].ToString()) == 1 ? Brushes.Green : Brushes.Gray, int.Parse(x["id"].ToString()), null)));
                return found_list;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> SendRequest(string command, int other_user_id)
        {
            var resp = await this.client.GetAsync($"Friends/request/{this.User.id}/{other_user_id}/{command}");
            if (resp.IsSuccessStatusCode)
            {
                //resp.Content.ReadAsAsync<List<Dictionary<string, object>>>().Result.ForEach(x => found_list.Add(new UsersListLineViewModel(x["photobytes"], null, null, x["username"].ToString(), int.Parse(x["status"].ToString()) == 1 ? Brushes.Green : Brushes.Gray, int.Parse(x["id"].ToString()), null)));
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Helpers
        public AdditionalData AdditionalData
        {
            get =>additionalData;
        }
        public User User
        {     
            get =>user;
            set
            {
                user = value;
                this.client.DefaultRequestHeaders.Add("Authorization", "Bearer " + value.token);
                Task.Run(() => PingR());
            }
        }
        private async void PingR()
        {            
            try
            {
                while(true)
                {
                    await Task.Delay(10000);
                    var response = await client.GetAsync($"TimeOut/ping/{this.User.id}");
                    if (!response.IsSuccessStatusCode)
                    {
                        Inst.Utils.MainWindow.frame1.NavigationService.Navigate(new Admin());//got kicked
                    }
                }
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.ToString());
                return;
            }
        }
        #endregion
    }
}
