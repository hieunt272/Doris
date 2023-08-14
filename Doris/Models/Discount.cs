using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Doris.Models
{
    public class Discount
    {
        public int Id { get; set; }
        [Display(Name = "Tên ưu đãi"), Required(ErrorMessage = "Hãy nhập tên ưu đãi"), StringLength(200, ErrorMessage = "Tối đa 200 ký tự"), UIHint("TextBox")]
        public string Name { get; set; }
        [Display(Name = "Hiện tên ưu đãi"), StringLength(200, ErrorMessage = "Tối đa 200 ký tự"), UIHint("TextBox")]
        public string ShowName { get; set; }
        [Display(Name = "Mô tả"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Display(Name = "Giảm giá"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public decimal? PriceOff { get; set; }
        [Display(Name = "Tổng tiền đơn"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public decimal? TotalOrder { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        [Display(Name = "Ngày đăng")]
        public DateTime CreateDate { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<DiscountUser> DiscountUsers { get; set; }

        public Discount()
        {
            CreateDate = DateTime.Now;
        }
    }
}