using UnsocNetwork.Models;

namespace UnsocNetwork.ViewModels.Account
{
    public class UserViewModel
    {
        public User User { get; private set; }
        
        public UserViewModel(User user)
        {
            User = user;
        }
    }
}
