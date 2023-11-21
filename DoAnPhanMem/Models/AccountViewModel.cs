using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DoAnPhanMem.Models
{
    public class LoginViewModels
    {
        [DisplayName("Email")]
        [Required(ErrorMessage = "Nhập Email")]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        [DisplayName("Mật khẩu")]
        [Required(ErrorMessage = "Nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string acc_password { get; set; }
    }
    public class ChangePasswordViewModels
    {
        public int AccountID { get; set; }

        [DisplayName("Mật khẩu cũ")]
        [Required(ErrorMessage = "Nhập Mật khẩu cũ")]
        public string OldPassword { get; set; }

        [DisplayName("Mật khẩu mới")]
        [Required(ErrorMessage = "Nhập Mật khẩu mới")]
        [MaxLength(30, ErrorMessage = "Mật khẩu tối đa 30 ký tự")]
        [RegularExpression("^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9]).{8,}$", ErrorMessage = "Mật khẩu tổi thiếu 8 ký tự bao gồm: chữ số, chữ thường và chữ hoa")]

        public string NewPassword { get; set; }

        [DisplayName("Mật khẩu xác nhận")]
        [Required(ErrorMessage = "Nhập Mật khẩu xác nhận")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không trùng với mật khẩu mới")]
        public string PasswordConfirm { get; set; }

    }
}