using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Helpers;

namespace WebApi.Services
{
    public interface IFriendshipRequestService
    {
        void Create(int sender, int receiver);
        List<int> RequestsOf_Incoming(int id);
        List<int> RequestsOf_Outgoing(int id);
        bool Delete(int sender, int receiver);
    }

    public class FriendshipRequestService : IFriendshipRequestService
    {

        private DataContext _context;

        public FriendshipRequestService(DataContext context)
        {
            _context = context;
        }

        public void Create(int sender, int receiver)
        {
            if (_context.FriendshipRequests.FirstOrDefault(x => (x.Sender == sender && x.Receiver == receiver) || (x.Sender == receiver && x.Receiver == sender)) == null)
            {
                Entities.FriendshipRequest freq = new Entities.FriendshipRequest();
                freq.Sender = sender;
                freq.Receiver = receiver;
                _context.FriendshipRequests.Add(freq);
                _context.SaveChanges();
            }
        }

        public bool Delete(int sender, int receiver)
        {
            Entities.FriendshipRequest freq = _context.FriendshipRequests.FirstOrDefault(x => (x.Sender == sender && x.Receiver == receiver) || (x.Sender == receiver && x.Receiver == sender));
            if (freq == null)
            {
                return false;
            }
            if (freq != null)
            {
                _context.FriendshipRequests.Remove(freq);
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<int> RequestsOf_Incoming(int id)
        {
            //return _context.FriendshipRequests.Where(x => x.Receiver == id || x.Sender == id).Select(y => y.Sender == id ? y.Receiver : y.Sender).ToList();
            return _context.FriendshipRequests.Where(x => x.Receiver == id).Select(y => y.Sender).ToList();
        }

        public List<int> RequestsOf_Outgoing(int id)
        {
            return _context.FriendshipRequests.Where(x => x.Sender == id).Select(y => y.Receiver).ToList();
        }
    }
}
