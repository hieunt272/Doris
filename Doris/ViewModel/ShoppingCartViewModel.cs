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

        [Display(Name = "Tỉnh/Thành phố"), Required(ErrorMessage = "Bạn hãy chọn Tỉnh/Thành phố")]
        public int? CityId { get; set; }
        [Display(Name = "Quận/Huyện"), Required(ErrorMessage = "Bạn hãy chọn Quận/Huyện")]
        public int? DistrictId { get; set; }
        [Display(Name = "Phường/Xã"), Required(ErrorMessage = "Bạn hãy chọn Phường/Xã")]
        public int? WardId { get; set; }

        public SelectList CitySelectList { get; set; }
        public SelectList DistrictSelectList { get; set; }
        public SelectList WardSelectList { get; set; }

        public SelectList SelectTransport { get; set; }
        public SelectList SelectTypePay { get; set; }

        public IEnumerable<Cart> CartItems { get; set; }
        public IEnumerable<Address> Addresses { get; set; }
        public CheckOutViewModel()
        {
            var selectTransport = new Dictionary<int, string> { { 1, "Đến địa chỉ người nhận" }, { 2, "Khách đến nhận hàng" }, { 3, "Qua bưu điện" }, { 4, "Hình thức khác" } };
            var typePay = new Dictionary<int, string> { { 1, "Tiền mặt" }, { 2, "Chuyển khoản" } };
            SelectTransport = new SelectList(selectTransport, "Key", "Value");
            SelectTypePay = new SelectList(typePay, "Key", "Value");

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