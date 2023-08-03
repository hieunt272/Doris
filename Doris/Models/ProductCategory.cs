using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Doris.Models
{
    public class ProductCategory
    {
        public int Id { get; set; }
        [Display(Name = "Tên danh mục"), Required(ErrorMessage = "Hãy nhập tên danh mục"),
            StringLength(80, ErrorMessage = "Tối đa 80 ký tự"), UIHint("TextBox")]
        public string CategoryName { get; set; }

        [Display(Name = "Đường dẫn"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextBox")]
        public string Url { get; set; }

        [Display(Name = "Thứ tự"), Required(ErrorMessage = "Hãy nhập số thứ tự"),
            RegularExpression(@"\d+", ErrorMessage = "Chỉ nhập số nguyên dương"), UIHint("NumberBox")]
        public int CategorySort { get; set; }
        [Display(Name = "Giới thiệu ngắn"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string Description { get; set; }

        [Display(Name = "Nội dung"), UIHint("EditorBox")]
        public string Body { get; set; }

        [Display(Name = "Hoạt động")]
        public bool CategoryActive { get; set; }
        [Display(Name = "Danh mục cha")]
        public int? ParentId { get; set; }
        [Display(Name = "Hiển thị menu")]
        public bool ShowMenu { get; set; }
        [Display(Name = "Thẻ tiêu đề"), StringLength(100, ErrorMessage = "Tối đa 100 ký tự"), UIHint("TextBox")]
        public string TitleMeta { get; set; }
        [Display(Name = "Thẻ mô tả"), StringLength(500, ErrorMessage = "Tối đa 500 ký tự"), UIHint("TextArea")]
        public string DescriptionMeta { get; set; }
        [Display(Name = "Ảnh đại diện"), StringLength(500), UIHint("ImageProCat")]
        public string Image { get; set; }
        [Display(Name = "Ảnh bìa"), StringLength(500), UIHint("ImageProCat")]
        public string CoverImage { get; set; }

        public virtual ICollection<Product> Products { get; set; }
        [ForeignKey("ParentId")]
        public virtual ProductCategory ParentCategory { get; set; }
        public virtual ICollection<ProductCategory> Categories { get; set; }
        public ProductCategory()
        {
            CategoryActive = true;
        }
    }
}