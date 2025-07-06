﻿using System.ComponentModel.DataAnnotations;
namespace BTL_Mongodb.Models.ViewModel
{
    public class LoginViewModel
    {
   
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        public string Password { get; set; }
    }
}
