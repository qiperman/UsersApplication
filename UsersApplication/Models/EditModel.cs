using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UsersApplication.Models
{
    public class EditModel
    {
        public string UserId { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public string Patronymic { get; set; }
    }
}