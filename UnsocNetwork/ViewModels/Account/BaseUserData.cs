using System.ComponentModel.DataAnnotations;

namespace UnsocNetwork.ViewModels.Account
{
    public class BaseUserData
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
        public string Email { get; set; }

        [Required(ErrorMessage = "Год должен быть заполнен ")]
        [Display(Name = "Год", Prompt = "Год рождения")]
        [Range(1900, 2100, ErrorMessage = "Введите корректный год (от {1} до {2} ")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Месяц должен быть заполнен ")]
        [Display(Name = "Месяц", Prompt = "Месяц")]
        [Range(1, 12, ErrorMessage = "Невозможно ввести месяц меньше 1 и больше 12 ")]
        public int Month { get; set; }

        [Required(ErrorMessage = "День должен быть заполнен")]
        [Display(Name = "День", Prompt = "Число")]
        [Range(1, 31, ErrorMessage = "Невозможно ввести число меньше 1 и больше 31")]
        public int Date { get; set; }

    }
}
