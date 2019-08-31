using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using WebApi.Helpers;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebApi.Services;
using WebApi.Dtos;
using WebApi.Entities;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TimeOutController : ControllerBase
    {
        public TimeOutController()
        {

        }

        //jei useris nepabumpinamas 30sec, jis atjungiamas.
        [Route("ping/{id:int}")]
        public IActionResult Ping(int id)
        {
            UserInfo user = App.Inst.users.FirstOrDefault(x => x.id == id);
            if (user == null)
            {
                return Content("not found online");
            }
            user.time = 0;
            return Ok();
        }

    }

    public class TimeOutControl
    {

        public TimeOutControl()
        {

        }

        public void Start()
        {
            Task.Run(() => CheckerR());
        }

        public void Checker()
        {
            int time = 0;
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            while (true)
            {
                if (time + 10 <= stopWatch.Elapsed.TotalSeconds)
                {
                    IncreaseIndex();
                    CheckForLogOut();
                    time = (int)stopWatch.Elapsed.TotalSeconds;
                }
            }
        }
        public async void CheckerR()
        {
            while (true)
            {
            await Task.Delay(10000);
            IncreaseIndex();
            CheckForLogOut();
            }            
        }
        

        public static void CheckForLogOut()
        {
            foreach(var user in App.Inst.users.Where(x => x.time > 2))
            {
                App.Inst.tempRooms.FirstOrDefault(x => x.usersById.Keys.Contains(user.id))?.usersById.Remove(user.id);
                //If user is trackint time, his time tracking should be stopped (stop mark left).
                App.Inst.User_TimeOut_List.Add(DateTime.Now, user.id);
                App.Inst.OnlineStatusUsers.Remove(user.id);
                //App.Inst.loggedin.Remove(user.id);
            }

            //foreach(UserInfo user in App.Inst.users.Where(x => x.time > 2))
            //{
            //    App.Inst.RaiseFriendschangedEvent("timeout_controller", new FriendsChangeEventArgs() { senderId = user.id, change = 6, receivers = new List<int>(_friendshipService.FriendsOf(user.id)), data = "5" });
            //}
            //Neina prieiti prie DataContext arba service, galima sprendimas - https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.2, tik daug skaityt.
            //----
            //--Sutvarkyta. Client pats klausia ar useriai vis dar online. FriendsListForm.xaml.cs'e
            //----
            App.Inst.users.RemoveAll(x => x.time > 2);
        }

        public static void IncreaseIndex()
        {
            App.Inst.users.ForEach(x => 
            {
                if (x.time < 0)
                {
                    x.time = 1;
                }
                else
                {
                    x.time++;
                }
            });
        }
    }


}