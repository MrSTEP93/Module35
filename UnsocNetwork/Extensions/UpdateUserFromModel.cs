using UnsocNetwork.Models;
using UnsocNetwork.ViewModels.UserVM;

namespace UnsocNetwork.Extensions
{
    public static class UserFromModel
    {
        public static User Convert(this User user, UserEditViewModel model)
        {
            //user.PathToPhoto = usereditvm.PathToPhoto;
            user.LastName = model.LastName;
            user.SecondName = model.SecondName;
            user.FirstName = model.FirstName;
            user.Email = model.Email;
            user.BirthDate = new System.DateTime(model.Year, model.Month, model.Day);
            user.UserName = model.Email;
            user.Status = model.Status;
            user.About = model.About;

            return user;
        }
    }
}
