using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Doris.Models;
using System.Drawing.Drawing2D;
using System.Web.UI.WebControls;
using System.Security.Permissions;

namespace Doris.ViewModel
{
    public class HomeViewModel
    {
        public IEnumerable<Banner> Banners { get; set; }
        public IEnumerable<Product> NewProducts { get; set; }
        public IEnumerable<Product> ProductHots { get; set; }
        public IEnumerable<Product> ProductWholesales { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Brand> Brands { get; set; }
        public IEnumerable<ProductCategory> Categories { get; set; }
    }

    public class HeaderViewModel
    {
        public IEnumerable<ProductCategory> ProductCategories { get; set; }
        public IEnumerable<Brand> Brands { get; set; }
        public User User { get; set; }
    }
    public class FooterViewModel
    {
        public IEnumerable<CategoryItem> CategoryItems { get; set; }
        public class CategoryItem
        {
            public ArticleCategory Category { get; set; }
            public IEnumerable<Article> Articles { get; set; }
        }
    }
    public class AllArticleViewModel
    {
        public IPagedList<Article> Articles { get; set; }
        public IEnumerable<ArticleCategory> Categories { get; set; }
        public string Sort { set; get; }
        public int BeginCount { get; set; }
        public int EndCount { get; set; }
    }
    public class ArticleCategoryViewModel
    {
        public ArticleCategory Category { get; set; }
        public IPagedList<Article> Articles { get; set; }
        public IEnumerable<ArticleCategory> Categories { get; set; }
        public string Sort { get; set; }
        public int BeginCount { get; set; }
        public int EndCount { get; set; }
    }
    public class ArticleDetailViewModel
    {
        public Article Article { get; set; }
        public IEnumerable<Article> Articles { get; set; }
    }
    public class ArticleSearchViewModel
    {
        public string Keywords { get; set; }
        public IPagedList<Article> Articles { get; set; }
        public IEnumerable<ArticleCategory> Categories { get; set; }
        public string Sort { get; set; }
        public int BeginCount { get; set; }
        public int EndCount { get; set; }
    }
    public class MenuArticleViewModel
    {
        public IEnumerable<Article> Articles { get; set; }
        public IEnumerable<ArticleCategory> ArticleCategories { get; set; }
    }
    public class ProductDetailViewModel
    {
        public Product Product { get; set; }
        public decimal Comission { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
    public class CategoryProductViewModel
    {
        public ProductCategory Category { get; set; }
        public IPagedList<Product> Products { get; set; }
        public IEnumerable<ProductCategory> Categories { get; set; }
        public IEnumerable<Brand> Brands { get; set; }
        public int? CatId { get; set; }
        public int? BrandId { get; set; }
        public string Url { get; set; }
        public string Sort { get; set; }
    }
    public class SearchProductViewModel
    {
        public string Keywords { get; set; }
        public IPagedList<Product> Products { get; set; }
        public IEnumerable<ProductCategory> Categories { get; set; }
        public IEnumerable<Brand> Brands { get; set; }
        public int? CatId { get; set; }
        public int? BrandId { get; set; }
        public string Sort { get; set; }
    }

    public class PromotionViewModel
    {
        public IEnumerable<Banner> Banners { get; set; }
        public IEnumerable<Article> Articles { get; set; }
        public IPagedList<Product> Products { get; set; }
        public IEnumerable<ProductCategory> Categories { get; set; }
        public IEnumerable<Brand> Brands { get; set; }
        public int? CatId { get; set; }
        public int? BrandId { get; set; }
        public string Sort { get; set; }
    }

    public class AllBrandViewModel
    {
        public IEnumerable<Brand> BrandHots { get; set; }
        public IEnumerable<Brand> Brands { get; set; }
        public IEnumerable<ProductCategory> Categories { get; set; }
    }

    public class BrandViewModel
    {
        public Brand Brand { get; set; }
        public IPagedList<Product> Products { get; set; }
        public IEnumerable<ProductCategory> Categories { get; set; }
        public int? CatId { get; set; }
        public string Url { get; set; }
        public string Sort { get; set; }
    }

    public class GetBrandViewModel
    {
        public IEnumerable<Brand> Brands { get; set; }
        public string Char { get; set; }
    }

    public class ProductWholesaleViewModel
    {
        public IPagedList<Product> Products { get; set; }
    }
}