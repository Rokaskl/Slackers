using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Helpers;

namespace WebApi.Services
{
    public interface INotificationService
    {
        bool Create(int user_id, byte[] notifications_array);
        byte[] Notifications_of(int user_id);
        void LeaveNotification(int user_id, int notification_type);
    }

    public class NotificationService : INotificationService
    {
        private DataContext _context;

        public NotificationService(DataContext context)
        {
            _context = context;
        }

        public bool Create(int user_id, byte[] notifications_array)
        {
            //Delete old
            Entities.Notifications old_n = _context.Notifications.FirstOrDefault(x => x.User_Id == user_id);
            if (old_n != null)
            {
               _context.Notifications.Remove(old_n);
            }
           
            //Create new
            Entities.Notifications new_n = new Entities.Notifications();
            new_n.User_Id = user_id;
            new_n.Notifications_Array = notifications_array;
            _context.Notifications.Add(new_n);
            _context.SaveChanges();
            return true;
        }

        public byte[] Notifications_of(int user_id)
        {
            return _context.Notifications.FirstOrDefault(x => x.User_Id == user_id)?.Notifications_Array;
        }

        public void LeaveNotification(int user_id, int notification_type)
        {
            Entities.Notifications notification = _context.Notifications.FirstOrDefault(x => x.User_Id == user_id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
            }

            Entities.Notifications n = new Entities.Notifications();
            n.User_Id = user_id;
            n.Notifications_Array = new byte[4 * 8];

            int n_number = BitConverter.ToInt32(n.Notifications_Array, notification_type * 4);
            n_number++;
            byte[] n_bytes = BitConverter.GetBytes(n_number);
            for (int i = 0; i < 4; i++)
            {
                n.Notifications_Array[notification_type * 4 + i] = n_bytes[i];
            }
            _context.Notifications.Add(n);
            _context.SaveChanges();
        }
    }
}
