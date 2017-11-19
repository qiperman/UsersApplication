using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UsersApplication.Models
{
    public class EditModel
    {
        [Required]
        public string UserId { get; set; }
        [Display(Name = "Логин")]
        public string Login { get; set; }

        
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        [Display(Name = "Имя")]
        public string FirstName { get; set; }
        [Display(Name = "Фамилия")]
        public string SecondName { get; set; }
        [Display(Name = "Отчетсво")]
        public string Patronymic { get; set; }
    }
}