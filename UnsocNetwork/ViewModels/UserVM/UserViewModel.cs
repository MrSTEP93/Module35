using System.Collections.Generic;
using UnsocNetwork.Models;

namespace UnsocNetwork.ViewModels.UserVM
{
    public class UserViewModel
    {
        public User User { get; private set; }

        public List<User> Friends { get; set; }

        public bool IsCurrentUser { get; set; }

        public string NotifySuccess { get; set; }
        public string NotifyDanger { get; set; }

        public UserViewModel(User user)
        {
            User = user;
        }
    }
}
