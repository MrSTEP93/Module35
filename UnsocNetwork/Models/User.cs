using Microsoft.AspNetCore.Identity;
using System;

namespace UnsocNetwork.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string Nickname { get; set; }

        public DateTime BirthDate { get; set; }
    }
}
