using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Dtos;
using WebApi.Entities;
using WebApi.Helpers;

namespace WebApi.Services
{
    public interface IUserService
    {
        UserDto Authenticate(string username, string password, bool web);
        IEnumerable<UserDto> GetAll();
        User GetById(int id);
        User Create(User user, string password);
        List<User> GetList(List<int> users);
        void Update(User user, string password = null);
        void Delete(int id);
        List<Dictionary<string, object>> UserViewModel_dict(List<int> ids, bool return_with_status = true);

    }

    public class UserService : IUserService
    {
        private DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }
        

        public UserDto Authenticate(string username, string password, bool web = false)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.Users.SingleOrDefault(x => x.Username == username);
            if (user == null)
            {
                return null;
            }

            //check if user is already logged in.
            if (!web && App.Inst.users.Any(x => x.id == user.Id))
            {
                return null;
            }

            List<int> rooms = ConvertToInts(user.roomsBytes);
            UserDto _user = new UserDto(user.Id,user.FirstName,user.LastName,user.Username,rooms);
            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return _user;
        }

        public IEnumerable<UserDto> GetAll()
        {
            List<UserDto> _users = new List<UserDto>();
            var users = _context.Users;
            foreach (var user in users) 
                _users.Add(new UserDto(user.Id,user.FirstName,user.LastName,user.Username,ConvertToInts(user.roomsBytes)));
            return _users;
        }

        public User GetById(int id)
        {
            return _context.Users.Find(id);
        }
        public List<User> GetList(List<int> users)
        {
            List<User> _users = new List<User>();
            foreach (var item in users)
            {
                User tempUser = GetById(item);
                tempUser.PasswordHash=null;
                tempUser.PasswordSalt=null;
                tempUser.roomsBytes=null;
                _users.Add(tempUser);
            }
            return _users;
        }
        public User Create(User user, string password)
        {
            // validation
            if (string.IsNullOrWhiteSpace(password))
                throw new AppException("Password is required");

            if (_context.Users.Any(x => x.Username == user.Username))
                throw new AppException("Username \"" + user.Username + "\" is already taken");

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            _context.Users.Add(user);
            _context.SaveChanges();

            return _context.Users.First(x=>x.Username==user.Username);
        }

        public void Update(User userParam, string password = null)
        {
            var user = _context.Users.Find(userParam.Id);

            if (user == null)
                throw new AppException("User not found");

            if (userParam.Username != user.Username)
            {
                // username has changed so check if the new username is already taken
                if (_context.Users.Any(x => x.Username == userParam.Username))
                    throw new AppException("Username " + userParam.Username + " is already taken");
            }

            // update user properties
            user.FirstName = userParam.FirstName;
            user.LastName = userParam.LastName;
            user.Username = userParam.Username;

            // update password if it was entered
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);

                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            _context.Users.Update(user);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
        // private helper methods

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }
        
        private static byte[] ConvertToBytes(List<int> users)
        {
            if (users==null)
            {
                return null;
            }
            byte[] _users = new byte[users.Count*4];
            for (int i = 0; i < users.Count; i++)
            {
                BitConverter.GetBytes(users[i]).CopyTo(_users,i*4);
            }
            return _users;
        }
        
        private static List<int> ConvertToInts(byte[] users)
        {
            if (users==null)
            {
                return null;
            }
            List<int> _users = new List<int>();
            for (int i = 0; i < users.Length/4; i++)
            {
                _users.Add(BitConverter.ToInt32(users,i*4));
            }
            return _users;
        }

        public List<Dictionary<string, object>> UserViewModel_dict(List<int> ids, bool return_with_status = true)
        {
            return ids.Select(x => new Dictionary<string, object>() { { "photobytes", _context.AdditionalDatas.FirstOrDefault(z => z.Id == x)?.PhotoBytes }, { "username", _context.Users.FirstOrDefault(z => z.Id == x)?.Username }, { "id", x }, { "status", return_with_status && App.Inst.OnlineStatusUsers.Contains(x) ? 1 : 0 }, { "bio", return_with_status ? _context.AdditionalDatas.FirstOrDefault(z => z.Id == x)?.Biography : null } })?.ToList();
        }
    }
}