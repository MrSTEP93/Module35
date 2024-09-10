﻿using System.Collections.Generic;
using UnsocNetwork.Models;

namespace UnsocNetwork.ViewModels.Account
{
    public class UserViewModel
    {
        public User User { get; private set; }

        public List<User> Friends { get; set; }

        public string NotifySuccess { get; set; }
        public string NotifyDanger { get; set; }
        
        public UserViewModel(User user)
        {
            User = user;
        }
    }
}
