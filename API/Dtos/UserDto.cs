using System.Collections.Generic;

namespace WebApi.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<int> rooms {get;set;}

        public UserDto(string vardas, string pavarde, string username, string pass)
        {
            FirstName = vardas;
            LastName = pavarde;
            Username = username;
            Password = pass;
        }

        public UserDto()
        {
        }

        public UserDto(int id, string firstName, string lastName, string username, List<int> rooms)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            this.rooms = rooms;
        }
    }

}