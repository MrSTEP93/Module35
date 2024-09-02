using Microsoft.AspNetCore.Identity;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace UnsocNetwork.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }

        public string SecondName { get; set; }

        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }

        public string PathToPhoto { get; set; }

        public string Status { get; set; }

        public string About { get; set; }

        public DateTime RegDate { get; set; }

        public User()
        {
            PathToPhoto = "/static/img/NoPhoto.jpg";
            Status = "Дратути! Я с вами, с усами!";
            About = "Пришёл, покушал и поспал";
            RegDate = DateTime.Now;
        }
    }
}
