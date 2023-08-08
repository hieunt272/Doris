using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Doris.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Display(Name = "Tên sản phẩm", Description = "Tên sản phẩm dài tối đa 200 ký tự")
            , Required(ErrorMessage = "Hãy nhập Tên sản phẩm"), StringLength(200, ErrorMessage = "Tối đa 200 ký tự"), UIHint("TextBox")]
        public string Name { get; set; }
        [Display(Name = "Mô tả"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Display(Name = "Thông tin sản phẩm"), UIHint("EditorBox")]
        public string Body { get; set; }
        [Display(Name = "Danh sách ảnh"), UIHint("UploadMultiFile")]
        public string ListImage { get; set; }
        [Display(Name = "Giá niêm yết"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public decimal? Price { get; set; }
        [Display(Name = "Giá Hạng Đồng"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public decimal? PriceBronze { get; set; }
        [Display(Name = "Giá Hạng Bạc"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public decimal? PriceSilver { get; set; }
        [Display(Name = "Giá Hạng Vàng"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public decimal? PriceGold { get; set; }
        [Display(Name = "Giá Hạng Bạch Kim"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public decimal? PricePlatinum { get; set; }
        [Display(Name = "Giá Hạng Kim Cương"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public decimal? PriceDiamond { get; set; }
        [Display(Name = "Giá sỉ"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public decimal? WholesalePrice { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        [Display(Name = "Ngày đăng")]
        public DateTime CreateDate { get; set; }
        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập số thứ tự"), RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int Sort { get; set; }
        [Display(Name = "Hoạt động")]
        public bool Active { get; set; }
        [Display(Name = "Hiện trang chủ")]
        public bool Home { get; set; }
        [Display(Name = "Sản phẩm mới")]
        public bool New { get; set; }
        [Display(Name = "Khuyến mãi hot")]
        public bool Hot { get; set; }
        [Display(Name = "Bán chạy nhất")]
        public bool BestSeller { get; set; }
        [Display(Name = "Giá sỉ tốt nhất")]
        public bool Wholesale { get; set; }
        [Display(Name = "Mã sản phẩm"), StringLength(50, ErrorMessage = "Tối đa 50 ký tự"), UIHint("TextBox")]
        public string Code { get; set; }

        [StringLength(300)]
        public string Url { get; set; }
        [Display(Name = "Thẻ tiêu đề"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string TitleMeta { get; set; }
        [Display(Name = "Thẻ mô tả"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string DescriptionMeta { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}đ")]
        public decimal? FinalPrice => PriceDiamond ?? PricePlatinum ?? PriceGold ?? PriceSilver ?? PriceBronze ?? Price;
        [Display(Name = "Thương hiệu"), Required(ErrorMessage = "Hãy chọn thương hiệu")]
        public int BrandId { get; set; }
        [Display(Name = "Danh mục sản phẩm"), Required(ErrorMessage = "Hãy chọn danh mục sản phẩm")]
        public int ProductCategoryId { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
        public virtual ICollection<ProductUser> ProductUsers { get; set; }

        public Product()
        {
            CreateDate = DateTime.Now;
            Active = true;
        }
    }
}