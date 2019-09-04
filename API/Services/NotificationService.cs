using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Helpers;

namespace WebApi.Services
{
    public interface INotificationService
    {
        bool Create(int user_id, int r_inc_c, int r_out_c, int log_c, byte[] notifications_array);
        byte[] FriendListNotificationsOf(int user_id);
        void LeaveNotificationStraightToDb(int user_id, int notification_type);
        //int LogNotificationsCount(int user_id);
        bool CreateRequestInc(int user_id);
        bool CreateRequestOut(int user_id);
        bool CreateLogs(int user_id);
        List<int> GeneralNotificationsCount(int user_id);
    }

    public class NotificationService : INotificationService
    {
        private DataContext _context;

        public NotificationService(DataContext context)
        {
            _context = context;
        }

        public bool Create(int user_id, int r_inc_c, int r_out_c, int log_c, byte[] notifications_array)
        {
            RemoveOldNotification(user_id);
            //Create new
            Entities.Notifications new_n = new Entities.Notifications();
            new_n.User_Id = user_id;
            new_n.RequestsInc_c = r_inc_c;
            new_n.RequestsOut_c = r_out_c;
            new_n.Logs_c = log_c;

            //Total numbers of logs, requests incoming and requests outgoing. Will be used to count how many new were created while the user was offline.
            new_n.TotalLogs_c = _context.Logs.Count(x => x.User_Id == user_id);

            new_n.Notifications_Array = notifications_array;
            _context.Notifications.Add(new_n);
            _context.SaveChanges();
            return true;
        }

        public bool CreateRequestInc(int user_id)
        {
            Entities.Notifications old = RemoveOldNotification(user_id);
            old.RequestsInc_c++;
            _context.Notifications.Add(old);
            _context.SaveChanges();
            return true;
        }

        public bool CreateRequestOut(int user_id)
        {
            Entities.Notifications old = RemoveOldNotification(user_id);
            old.RequestsOut_c++;
            _context.Notifications.Add(old);
            _context.SaveChanges();
            return true;
        }

        public bool CreateLogs(int user_id)
        {
            Entities.Notifications old = RemoveOldNotification(user_id);
            old.Logs_c++;
            _context.Notifications.Add(old);
            _context.SaveChanges();
            return true;
        }

        public byte[] FriendListNotificationsOf(int user_id)
        {
            return _context.Notifications.FirstOrDefault(x => x.User_Id == user_id)?.Notifications_Array;
        }

        public int LogNotificationsCount(int user_id)
        {
            Entities.Notifications n = _context.Notifications.FirstOrDefault(x => x.User_Id == user_id);
            if (n != null)
            {
                return n.Logs_c + _context.Logs.Count(x => x.User_Id == user_id) - n.TotalLogs_c;
            }
            else
            {
                return 0;
            }
        }

        public List<int> GeneralNotificationsCount(int user_id)
        {
            List<int> n_list = new List<int>();

            Entities.Notifications not = _context.Notifications.FirstOrDefault(x => x.User_Id == user_id);
            if (not != null)
            {
                n_list.Add(not.RequestsInc_c);
                n_list.Add(not.RequestsOut_c);
                n_list.Add(LogNotificationsCount(user_id));
            }
            return n_list;
        }

        public void LeaveNotificationStraightToDb(int user_id, int notification_type)
        {
            if (App.Inst.users.FirstOrDefault(x => x.id == user_id) == null)//This means the user {user_id} is offline.
            {
                Entities.Notifications old = RemoveOldNotification(user_id);
                //Entities.Notifications n = new Entities.Notifications();
                //n.User_Id = user_id;
                //n.Notifications_Array = new byte[4 * 8];

                //int n_number = BitConverter.ToInt32(n.Notifications_Array, notification_type * 4);
                //n_number++;
                //byte[] n_bytes = BitConverter.GetBytes(n_number);
                //for (int i = 0; i < 4; i++)
                //{
                //    n.Notifications_Array[notification_type * 4 + i] = n_bytes[i];
                //}
                List<Tuple<int, int>> existing_notifications;
                if (old != null)
                {
                    existing_notifications = App.ByteArrayToObject(old.Notifications_Array) as List<Tuple<int, int>>;
                    existing_notifications.Add(new Tuple<int, int>(user_id, notification_type));

                    old.Notifications_Array = App.ObjectToByteArray(existing_notifications);
                    _context.Notifications.Add(old);
                    _context.SaveChanges();
                }
                else
                {
                    existing_notifications = new List<Tuple<int, int>>();
                    existing_notifications.Add(new Tuple<int, int>(user_id, notification_type));
                    Create(user_id, 0, 0, 0, App.ObjectToByteArray(existing_notifications));
                }
            }
        }

        public void SaveNotifications(int user_id, List<Tuple<int, int>> accumulated_notifications)
        {
            Entities.Notifications old = RemoveOldNotification(user_id);
            List<Tuple<int, int>> existing_notifications = App.ByteArrayToObject(old.Notifications_Array) as List<Tuple<int, int>>;

            foreach (Tuple<int,int> item in accumulated_notifications)
            {
                existing_notifications.Add(new Tuple<int, int>(item.Item1, item.Item2));
            }

            old.Notifications_Array = App.ObjectToByteArray(existing_notifications);
            _context.Notifications.Add(old);
            _context.SaveChangesAsync();
        }

        private Entities.Notifications RemoveOldNotification(int id)
        {
            Entities.Notifications old_n = _context.Notifications.FirstOrDefault(x => x.User_Id == id);
            if (old_n != null)
            {
                Entities.Notifications n = new Entities.Notifications();
                n.Id = old_n.Id;
                n.Logs_c = old_n.Logs_c;
                n.Notifications_Array = old_n.Notifications_Array;
                n.RequestsInc_c = old_n.RequestsInc_c;
                n.RequestsOut_c = old_n.RequestsOut_c;
                n.TotalLogs_c = old_n.TotalLogs_c;
                n.User_Id = old_n.User_Id;
                _context.Notifications.Remove(old_n);
                _context.SaveChanges();
                return n;
            }
            else
            {
                return null;
            }
        }
    }
}
