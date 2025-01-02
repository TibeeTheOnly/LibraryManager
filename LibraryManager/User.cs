using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManager
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } = "User";

        public User(string username, string password, string name, string email, string role = "User")
        {
            Username = username;
            Password = password;
            Name = name;
            Email = email;
            Role = role;
        }

        public override string ToString()
        {
            return $"{Username},{Password},{Name},{Email},{Role}";
        }

        public static User FromString(string userData)
        {
            var parts = userData.Split(",");
            if (parts.Length != 5)
                throw new FormatException("A felhasználói adatok formátuma nem megfelelő");
            return new User(parts[0], parts[1], parts[2], parts[3], parts[4]);
        }
    }
}
