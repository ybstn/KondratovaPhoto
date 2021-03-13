using System;
using System.ComponentModel.DataAnnotations;
namespace Yulya.Models
{
    public class Admin
    {
        [Required(ErrorMessage = "Не указан логин")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Не указан пароль")]
        public string Passw { get; set; }

    }
}
