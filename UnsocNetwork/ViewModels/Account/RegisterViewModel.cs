using System.ComponentModel.DataAnnotations;

namespace UnsocNetwork.ViewModels.Account
{
    public class RegisterViewModel
    {

        [Required(ErrorMessage = "Имя обязательно для заполнения")]
        [DataType(DataType.Text)]
        [Display(Name = "Имя", Prompt = "Введите имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Фамилия обязательна для заполнения")]
        [DataType(DataType.Text)]
        [Display(Name = "Фамилия", Prompt = "Введите фамилию")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Требуется указать адрес электронной почты")]
        [EmailAddress]
        [Display(Name = "Email", Prompt = "Введите ваш email")]
        public string EmailReg { get; set; }

        [Required]
        [Display(Name = "Год", Prompt = "Год рождения")]
        [Range(1900, 2100, ErrorMessage = "Кажется, вы нас обманываете")]
        public int Year { get; set; }

        [Required]
        [Display(Name = "Месяц", Prompt = "Месяц")]
        [Range(1, 12, ErrorMessage = "Невозможно ввести месяц меньше 1 и больше 12")]
        public int Month { get; set; }

        [Required]
        [Display(Name = "День", Prompt = "Число")]
        [Range(1, 31, ErrorMessage = "Невозможно ввести число меньше 1 и больше 31")]
        public int Date { get; set; }

        [Required(ErrorMessage = "Поле Пароль обязательно для заполнения")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль", Prompt = "Введите пароль")]
        [StringLength(100, ErrorMessage = "{0} должен содержать от {2} до {1} символов.", MinimumLength = 3)]
        public string PasswordReg { get; set; }

        [Required(ErrorMessage = "Обязательно подтвердите пароль")]
        [Compare("PasswordReg", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтвердить пароль", Prompt = "Введите пароль еще раз")]
        public string PasswordConfirm { get; set; }

        public string Login => EmailReg;
    }
}
