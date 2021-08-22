using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TourismMallMS.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password), ErrorMessage = "密码输入不一致")]
        public string ConfirmPassword { get; set; }
    }
}
