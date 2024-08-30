using System.ComponentModel.DataAnnotations;
using UnsocNetwork.ViewModels.Account;

namespace UnsocNetwork.ViewModels.Validators
{
    public static class LoginViewModelValidator
    {
        public static bool Check(LoginViewModel model)
        {
            if (string.IsNullOrEmpty(model.Password) && string.IsNullOrEmpty(model.Email))
            {
                return false;
            }
            return true;
        }

    }
}
