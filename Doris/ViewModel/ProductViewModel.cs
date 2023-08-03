using Doris.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace Doris.ViewModel
{
    public class InsertProductCatViewModel
    {
        [Display(Name = "Danh mục sản phẩm")]
        public int? ParentId { get; set; }
        [Display(Name = "Danh mục sản phẩm")]
        public int? CategoryId { get; set; }
        public SelectList RootCats { get; set; }
        public SelectList ChildCategoryList { get; set; }
        public ProductCategory ProductCategory { get; set; }
    }

    public class ListProductViewModel
    {
        public PagedList.IPagedList<Product> Products { get; set; }
        public SelectList SelectCategories { get; set; }
        public SelectList ChildCategoryList { get; set; }
        public SelectList SelectBrands { get; set; }
        public int? ParentId { get; set; }
        public int? CatId { get; set; }
        public int? BrandId { get; set; }
        public string Name { get; set; }
        public string Sort { get; set; }

        public ListProductViewModel()
        {
            ChildCategoryList = new SelectList(new List<ProductCategory>(), "Id", "CategoryName");
        }
    }

    public class InsertProductViewModel
    {
        public Product Product { get; set; }
        [Display(Name = "Danh mục sản phẩm"), Required(ErrorMessage = "Hãy chọn danh mục sản phẩm")]
        public int CategoryId { get; set; }
        public IEnumerable<ProductCategory> Categories { get; set; }
        [Display(Name = "Thương hiệu"), Required(ErrorMessage = "Hãy chọn thương hiệu")]
        public int BrandId { get; set; }
        public IEnumerable<Brand> Brands { get; set; }
        [Display(Name = "Giá niêm yết"), UIHint("MoneyBox"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public string Price { get; set; }
        [Display(Name = "Giá Hạng Đồng"), UIHint("MoneyBox"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public string PriceBronze { get; set; }
        [Display(Name = "Giá Hạng Bạc"), UIHint("MoneyBox"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public string PriceSilver { get; set; }
        [Display(Name = "Giá Hạng Vàng"), UIHint("MoneyBox"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public string PriceGold { get; set; }
        [Display(Name = "Giá Hạng Bạch Kim"), UIHint("MoneyBox"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public string PricePlatinum { get; set; }
        [Display(Name = "Giá Hạng Kim Cương"), UIHint("MoneyBox"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public string PriceDiamond { get; set; }
        [Display(Name = "Giá sỉ"), UIHint("MoneyBox"), DisplayFormat(DataFormatString = "{0:N0}đ")]
        public string WholesalePrice { get; set; }
    }

    public class ListBrandViewModel
    {
        public PagedList.IPagedList<Brand> Brands { get; set; }
        public string Name { get; set; }
    }
}