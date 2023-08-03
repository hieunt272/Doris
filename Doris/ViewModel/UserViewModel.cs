using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace Doris.ViewModel
{
    public class RegisterViewModel
    {
        [Display(Name = "Họ và tên"), Required(ErrorMessage = "Hãy nhập họ tên"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự")]
        public string FullName { get; set; }
        [Display(Name = "Email"), Required(ErrorMessage = "Hãy nhập Email"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), EmailAddress(ErrorMessage = "Email không chính xác")]
        public string Email { get; set; }
        [Display(Name = "Số điện thoại"), Required(ErrorMessage = "Hãy nhập số điện thoại"),
         RegularExpression(@"^\(?(09|03|07|08|05)\)?[-. ]?([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng!"), Remote("CheckPhoneNumber", "User")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Link shop của bạn (Dùng xác thực tài khoản)"), StringLength(500)]
        public string LinkShop { get; set; }
        [Display(Name = "Mã giới thiệu"), StringLength(10)]
        public string Code { get; set; }
        [DisplayName("Mật khẩu"), Required(ErrorMessage = "Bạn chưa nhập mật khẩu"), StringLength(20, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6 - 20 ký tự")]
        public string Password { get; set; }
        [DisplayName("Nhập lại mật khẩu"), Required(ErrorMessage = "Bạn chưa nhập lại mật khẩu")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Nhập lại mật khẩu không chính xác")]
        public string ConfirmPassword { get; set; }
    }
    public class UserLoginModel
    {
        [Display(Name = "Số điện thoại"), Required(ErrorMessage = "Hãy nhập tên đăng nhập"), RegularExpression(@"[a-z0-9_.]{4,20}", ErrorMessage = "Từ 4 đến 20 ký tự, chỉ nhập chữ thường, số 0-9, dấu . và dấu _")]
        public string Username { get; set; }
        [Display(Name = "Mật khẩu"), Required(ErrorMessage = "Hãy nhập mật khẩu")]
        public string Password { get; set; }
    }
}