using Doris.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Doris.ViewModel
{
    public class ProfileViewModel
    {
        [Display(Name = "Họ và tên"), Required(ErrorMessage = "Vui lòng nhập họ tên"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string FullName { get; set; }
        [Display(Name = "Số điện thoại"), RegularExpression(@"^\(?(09|03|07|08|05)\)?[-. ]?([0-9]{8})$", ErrorMessage = "Số điện thoại không đúng định dạng!"),
         Required(ErrorMessage = "Hãy nhập số điện thoại"), StringLength(10, ErrorMessage = "Tối đa 20 ký tự"), UIHint("TextBox")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Địa chỉ email"), Required(ErrorMessage = "Hãy nhập Email"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), EmailAddress(ErrorMessage = "Email không chính xác")]
        public string Email { get; set; }
        public string Username { get; set; }
        public int Gender { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string Avatar { get; set; }
        public int UserId { get; set; }
        public int PercentOrder { get; set; }
        public Level Level { get; set; }
        public decimal TotalLevelUp { get; set; }
        public IEnumerable<Banner> Banners { get; set; }
        public IEnumerable<Address> Addresses { get; set; }
        public IEnumerable<BankUser> BankUsers { get; set; }
    }
    public class UserInfoViewModel
    {
        public User User { get; set; }
        public Level Level { get; set; }
        public decimal TotalMonth { get; set; }
        public decimal TotalLevelUp { get; set; }
    }

    public class OrderHistoryViewModel
    {
        public int Status { get; set; }
        public IPagedList<Order> Orders { get; set; }
    }

    public class AddressViewModel
    {
        public IEnumerable<Address> Addresses { get; set; }
        public Address Address { get; set; }
        [Display(Name = "Tỉnh/Thành phố"), Required(ErrorMessage = "Bạn hãy chọn Tỉnh/Thành phố")]
        public int? CityId { get; set; }
        [Display(Name = "Quận/Huyện"), Required(ErrorMessage = "Bạn hãy chọn Quận/Huyện")]
        public int? DistrictId { get; set; }
        [Display(Name = "Phường/Xã"), Required(ErrorMessage = "Bạn hãy chọn Phường/Xã")]
        public int? WardId { get; set; }
        public SelectList CitySelectList { get; set; }
        public SelectList DistrictSelectList { get; set; }
        public SelectList WardSelectList { get; set; }
        public AddressViewModel()
        {
            DistrictSelectList = new SelectList(new List<District>(), "Id", "Name");
            WardSelectList = new SelectList(new List<Ward>(), "Id", "Name");
        }
    }

    public class BankInfoViewModel
    {
        public BankUser BankUser { get; set; }
        [Display(Name = "Ngân hàng")]
        public int BankId { get; set; }
        public SelectList BankSelectList { get; set; }
    }

    public class ListOrderMobileViewModel
    {
        public IPagedList<Order> Orders { get; set; }
        public int Status { get; set; }
        public string Keywords { get; set; }
    }
}