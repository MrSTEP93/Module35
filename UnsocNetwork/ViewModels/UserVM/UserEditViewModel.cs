﻿using System;
using System.ComponentModel.DataAnnotations;
using UnsocNetwork.Models;
using UnsocNetwork.ViewModels.Account;

namespace UnsocNetwork.ViewModels.UserVM
{
    public class UserEditViewModel : BaseUserData
    {
        public string Id { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Отчество", Prompt = "Отчество / псевдоним / кличка / погоняло")]
        public string SecondName { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Статус", Prompt = "Ваш статус")]
        public string Status { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "О себе", Prompt = "Введите данные о себе")]
        public string About { get; set; }

        public bool IsAttempted { get; set; }

        public string Result { get; set; }

        public string NotifyDanger { get; set; }

        public string NotifySuccess { get; set; }

    }
}
