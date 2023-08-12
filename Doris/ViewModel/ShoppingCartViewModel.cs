using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Permissions;
using System.Web.Mvc;
using Doris.Models;

namespace Doris.ViewModel
{
    public class ShoppingCartViewModel
    {
        public List<Cart> CartItems { get; set; }
        public decimal CartTotal { get; set; }
        public int CartCount { get; set; }
        public decimal CartTotalShipFee { get; set; }
        public decimal TotalFreeShip { get; set; }
    }
    public class ShoppingCartRemoveViewModel
    {
        public string Message { get; set; }
        public decimal CartTotal { get; set; }
        public int CartCount { get; set; }
        public int Status { get; set; }
        public int DeleteId { get; set; }
    }

    public class CheckOutViewModel
    {
        public Order Order { get; set; }
        public User User { get; set; }
        public Address Address { get; set; }
        public int UserId { get; set; }
        public int? AddressId { get; set; }
        public decimal CartTotal { get; set; }
        public int? Transport { get; set; }

        [Display(Name = "Tỉnh/Thành phố")]
        public int? CityId { get; set; }
        [Display(Name = "Quận/Huyện")]
        public int? DistrictId { get; set; }
        [Display(Name = "Phường/Xã")]
        public int? WardId { get; set; }
        [Display(Name = "Số nhà, Tên đường")]
        public string SpecialAddress { get; set; }
        [Display(Name = "Tên")]
        public string Fullname { get; set; }
        [Display(Name = "Số tài khoản")]
        public string BankNumber { get; set; }
        [Display(Name = "Ngân hàng thụ hưởng")]
        public int? BankId { get; set; }

        public SelectList CitySelectList { get; set; }
        public SelectList DistrictSelectList { get; set; }
        public SelectList WardSelectList { get; set; }
        public SelectList BankSelectList { get; set; }

        public List<Cart> Carts { get; set; }
        public BankUser BankUser { get; set; }
        public IEnumerable<Address> Addresses { get; set; }
        public IEnumerable<BankUser> BankUsers { get; set; }
        public CheckOutViewModel()
        {
            DistrictSelectList = new SelectList(new List<District>(), "Id", "Name");
            WardSelectList = new SelectList(new List<Ward>(), "Id", "Name");
        }
    }

    public class CartSummaryViewModel
    {
        public List<Cart> Carts { get; set; }
        public decimal TotalMoney { get; set; }
        public int Count { get; set; }
    }
}