using System.ComponentModel.DataAnnotations;
using UnsocNetwork.ViewModels.Validators;

namespace UnsocNetwork.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Мы должны узнать тебя по емэйлу")]
        [Display(Name = "Email", Prompt = "Ваш логин (email)")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Без пароля никак")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль", Prompt = "Ваш пароль")]
        [StringLength(100, ErrorMessage = "{0} должен быть от {2} до {1} символов.", MinimumLength = 3)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }

        private bool IsValid { get
            {
                return LoginViewModelValidator.Check(this);
            } }

    }
}
