using Microsoft.AspNetCore.Identity;
using System;

namespace UnsocNetwork.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        //public string Login { get; set; }

        public DateTime BirthDate { get; set; }
    }
}
