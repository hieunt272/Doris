using Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Doris.Models
{
    public class User
    {
        public int Id { get; set; }
        [Display(Name = "Họ và tên"), Required(ErrorMessage = "Vui lòng nhập họ tên"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string FullName { get; set; }
        [Display(Name = "Tên đăng nhập", Description = "Tên đăng nhập"), Required(ErrorMessage = "Hãy điền tên đăng nhập"), RegularExpression(@"[a-z0-9_.]{4,10}", ErrorMessage = "Chỉ nhập chữ thường và số 0-9, từ 4-10 ký tự"), UIHint("TextBox")]
        public string Username { get; set; }
        [Display(Name = "Ảnh đại diện"), StringLength(500)]
        public string Avatar { get; set; }
        [DisplayName("Mật khẩu"), Required(ErrorMessage = "Hãy nhập mật khẩu"), StringLength(60, ErrorMessage = "Tối đa 60 ký tự"), UIHint("Password")]
        public string Password { get; set; }
        [Display(Name = "Số điện thoại"), RegularExpression(@"^\(?(09|03|07|08|05)\)?[-. ]?([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng!"),
         Required(ErrorMessage = "Hãy nhập số điện thoại"), StringLength(10, ErrorMessage = "Tối đa 20 ký tự"), UIHint("TextBox")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Địa chỉ email"), Required(ErrorMessage = "Hãy nhập Email"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), EmailAddress(ErrorMessage = "Email không chính xác")]
        public string Email { get; set; }
        [Display(Name = "Giới tính")]
        public int Gender { get; set; }
        [Display(Name = "Ngày sinh"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime BirthDay { get; set; }
        [Display(Name = "Mã cửa hàng"), StringLength(20)]
        public string ShopCode { get; set; }
        [Display(Name = "Hạng thành viên")]
        public Level Level { get; set; }
        [Display(Name = "Hoạt động", Description = "Hoạt động")]
        [StringLength(50)]
        public string Token { get; set; }
        [Display(Name = "Tổng đơn mua"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public decimal? TotalBuy { get; set; }
        public bool Active { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<BankUser> BankUsers { get; set; }
        public virtual ICollection<ProductUser> ProductUsers { get; set; }
        public virtual ICollection<DiscountUser> DiscountUsers { get; set; }
        public User()
        {
            CreateDate = DateTime.Now;
            BirthDay = DateTime.Now;
            Active = true;
            Token = HtmlHelpers.RandomCode(50);
            TotalBuy = 0;
        }
    }

    public enum Level
    {
        [Display(Name = "Thành viên")]
        Member,
        [Display(Name = "Hạng Đồng")]
        Bronze,
        [Display(Name = "Hạng Bạc")]
        Silver,
        [Display(Name = "Hạng Vàng")]
        Gold,
        [Display(Name = "Hạng Bạch Kim")]
        Platinum,
        [Display(Name = "Hạng Kim Cương")]
        Diamond
    }
}