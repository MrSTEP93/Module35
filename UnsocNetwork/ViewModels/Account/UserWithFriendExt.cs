using UnsocNetwork.Models;

namespace UnsocNetwork.ViewModels.Account
{
    public class UserWithFriendExt : User
    {
        public bool IsFriendWithCurrent { get; set; }

        public bool IsCurrentUser {  get; set; }
    }
}
