using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [Route("post/{userId:int}/{r_inc_c:int}/{r_out_c:int}/{log_c:int}")]
        [HttpPost]
        public IActionResult Create(int userId, int r_inc_c, int r_out_c, int log_c, [FromBody] byte[] notifications_array)
        {
            //($"Notifications/post/{this.User.id}/{n.RequestsIncoming_count}/{n.RequestsOutgoing_count}/{n.Log_count}", Inst.ObjectToByteArray(n));
            if (_notificationService.Create(userId, r_inc_c, r_out_c, log_c, notifications_array))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [Route("{userId:int}")]
        [HttpGet]
        public IActionResult GetNotifications(int userId)
        {
            return Ok(_notificationService.FriendListNotificationsOf(userId) ?? new byte[0]);
        }

        [Route("notif_count/{userId:int}")]
        [HttpGet]
        public IActionResult GetLogNotificationsCount(int userId)
        {
            return Ok(_notificationService.GeneralNotificationsCount(userId));
        }
    }

    //public class NotificationControl
    //{
    //    //[FromServices]
    //    //public INotificationService CustomService { get; set; }

    //    //Tuple<{Notification belongs to id - }int, {Causer id - }int,{What had happened code - }int>
    //    private List<Tuple<int, int, int>> temp_notifications;
    //    public List<Tuple<int, int, int>> Notifications
    //    {
    //        get
    //        {
    //            return temp_notifications;
    //        }
    //        set
    //        {
    //            temp_notifications = value;
    //        }
    //    }

    //    public NotificationControl()
    //    {
    //        temp_notifications = new List<Tuple<int, int, int>>();
    //        Task.Run(() => WriteToDb(300));
    //    }

    //    public void Notify(int receiver, int causer, int message_code)
    //    {
    //        temp_notifications.Add(new Tuple<int, int, int>(receiver, causer, message_code));
    //    }

    //    private async Task WriteToDb(int everyXsec)
    //    {
    //        while (true)
    //        {
    //            Task.Delay(everyXsec * 1000);
    //            this.temp_notifications.RemoveAll(x => App.Inst.users.Any(y => y.id == x.Item1));//User x is online and received all the notifications, therefore his notifications from this temp list can be deleted.
    //            //this.temp_notifications.ForEach(x => )
    //        }
    //    }
    //}
}