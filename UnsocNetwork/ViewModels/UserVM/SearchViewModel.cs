using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using UnsocNetwork.Models;

namespace UnsocNetwork.ViewModels.UserVM
{
    public class SearchViewModel
    {
        [Required(ErrorMessage = "Введите поисковый запрос")]
        [Display(Name = "Найти...")]
        [StringLength(100, ErrorMessage = "Запрос должен содержать от {2} до {1} символов.", MinimumLength = 3)]
        public string SearchString { get; set; }

        public List<UserWithFriendExt> UserList { get; set; }
    }
}
