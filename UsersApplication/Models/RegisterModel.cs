﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UsersApplication.Models
{
    public class RegisterModel
    {
            [Required]
            [DataType(DataType.EmailAddress)]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "Логин")]
            public string Login { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Пароль")]
            public string Password { get; set; }

            [Required]
            [Compare("Password", ErrorMessage = "Пароли не совпадают")]
            [DataType(DataType.Password)]
            [Display(Name = "Повторите пароль")]
            public string PasswordConfirm { get; set; }
    }
}