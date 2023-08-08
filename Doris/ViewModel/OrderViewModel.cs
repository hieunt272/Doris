using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Doris.Models;

namespace Doris.ViewModel
{
    public class OrderViewModel
    {
        public IEnumerable<ListOrder> ListOrders { get; set; }

        public class ListOrder 
        {
            public int MyProperty { get; set; }
        }
        public Order Order { get; set; }

        public List<OrderDetailProduct> OrderDetailProduct { get; set; }
    }
    public class OrderDetailProduct
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? Price { get; set; }
    }
    public class ListOrderViewModel
    {
        public IPagedList<Order> Orders { get; set; }
        public IEnumerable<Order> Orderss { get; set; }
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string MaDonhang { get; set; }
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string CustomerName { get; set; }
        [EmailAddress(ErrorMessage = "Email không hợp lệ"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string CustomerEmail { get; set; }
        [StringLength(20, ErrorMessage = "Tối đa 20 ký tự")]
        public string CustomerMobile { get; set; }
        public int Status { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        [Required]
        public int PageSize { get; set; }
        public int? CityId { get; set; }
        public int UserId { get; set; }
        public SelectList CitySelectList { get; set; }
    }
    public class SearchOrderViewModel
    {
        public IEnumerable<Order> Orders { get; set; }
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string MaDonhang { get; set; }
        [StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string CustomerName { get; set; }
        [EmailAddress(ErrorMessage = "Email không hợp lệ"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string CustomerEmail { get; set; }
        public int Status { get; set; }
        public int Payment { get; set; }
    }

    public class ReportProductViewModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public int? Status { get; set; }
        public IPagedList<ReportProductItem> ReportProductItems { get; set; }
        public int? CityId { get; set; }
        public SelectList CitySelectList { get; set; }

        public class ReportProductItem
        {
            public Product Product { get; set; }
            public int TotalSale { get; set; }
        }
    }
}