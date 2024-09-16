using System.Collections.Generic;

namespace UnsocNetwork.ViewModels.UserVM
{
    public class PersonListWithFriendFlags
    {
        public List<UserWithFriendExt> List { get; set; }

        public PersonListWithFriendFlags()
        {
            List = new List<UserWithFriendExt>();
        }
    }
}
