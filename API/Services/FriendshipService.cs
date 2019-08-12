using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Helpers;

namespace WebApi.Services
{
    public interface IFriendshipService
    {
        bool Create(int id1, int id2);
        bool Delete(int id1, int id2);
        List<int> FriendsOf(int id);//Friends list of {id} user.
        List<int> Search(int userId, string search_string);
    }

    public class FriendshipService : IFriendshipService
    {
        private DataContext _context;

        public FriendshipService(DataContext context)
        {
            _context = context;
        }

        public bool Create(int id1, int id2)
        {
            Entities.Friendship f = new Entities.Friendship();
            f.id_first = id1;
            f.id_second = id2;
            _context.Friendships.Add(f);
            _context.SaveChanges();
            return true;
        }

        public bool Delete(int id1, int id2)
        {
            Entities.Friendship f = _context.Friendships.FirstOrDefault(x => (x.id_first == id1 && x.id_second == id2) || (x.id_first == id2 && x.id_second == id1));
            if (f != null)
            {
                _context.Friendships.Remove(f);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<int> FriendsOf(int id)
        {
            var temp = _context.Friendships.Where(x => x.id_first == id || x.id_second == id);
            return temp.Select(y => y.id_first == id ? y.id_second : y.id_first).ToList();
        }

        public List<int> Search(int userId, string search_string)
        {
            //return _context.Users.Where(x => CheckUsername(x.Username, search_string)).Select(y => new Dictionary<string, object>(){ { "photobytes", _context.AdditionalDatas.First(z => z.Id == y.Id).PhotoBytes}, { "username", y.Username}, { "id", y.Id}, { "status", App.Inst.OnlineStatusUsers.Contains(y.Id) ? 1 : 0} }).ToList();
            if (search_string.Last() == '\"' && search_string.First() == '\"')
            {
                string trimmed_string = search_string.Substring(1).Substring(0, search_string.Length - 2);
                return DeductFriends(_context.Users.Where(x => x.Username == trimmed_string).Select(y => y.Id).ToList(), userId);
            }
            else
            {
                return DeductFriends(_context.Users.Where(x => CheckUsername(x.Username, search_string) && x.Id != userId).Select(y => y.Id).ToList(), userId);
            }
        }

        private bool CheckUsername(string username, string search_string)
        {
            if (username == search_string)
            {
                return true;
            }
            
            if (username.Contains(search_string))
            {
                return true;
            }

            //More matching logic.

            return false;
        }

        private List<int> DeductFriends(List<int> found, int userId)
        {
            return found.Where(x => !FriendsOf(userId).Contains(x)).ToList();
        }
    }
}
