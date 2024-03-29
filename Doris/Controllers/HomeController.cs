﻿using Helpers;
using Doris.DAL;
using Doris.Filters;
using Doris.Models;
using Doris.ViewModel;
using PagedList;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using System;
using System.Web.Services.Description;
using System.ComponentModel;

namespace Doris.Controllers
{
    [MemberLoginFilter]
    public class HomeController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private static string Email => WebConfigurationManager.AppSettings["email"];
        private static string Password => WebConfigurationManager.AppSettings["password"];
        private string MemberEmail => RouteData.Values["MemberEmail"].ToString();
        private new User User => _unitOfWork.UserRepository.GetQuery(a => a.Active && a.Email == MemberEmail).SingleOrDefault();
        public ConfigSite ConfigSite => (ConfigSite)HttpContext.Application["ConfigSite"];

        private IEnumerable<ArticleCategory> ArticleCategories =>
            _unitOfWork.ArticleCategoryRepository.Get(a => a.CategoryActive, q => q.OrderBy(a => a.CategorySort));
        private IEnumerable<ProductCategory> ProductCategories =>
            _unitOfWork.ProductCategoryRepository.Get(a => a.CategoryActive, q => q.OrderBy(a => a.CategorySort));
        private IEnumerable<Brand> Brands =>
            _unitOfWork.BrandRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));

        #region Home
        public ActionResult Index()
        {
            var products = _unitOfWork.ProductRepository.GetQuery(a => a.Active, o => o.OrderBy(a => a.Sort));
            var model = new HomeViewModel
            {
                Banners = _unitOfWork.BannerRepository.GetQuery(b => b.Active, o => o.OrderBy(b => b.Sort)),
                NewProducts = products.Where(a => a.New).Take(10),
                ProductHots = products.Where(a => a.Hot).Take(10),
                Products = products.Where(a => a.Home).Take(20),
                Brands = Brands.Where(a => a.Home).Take(20)
            };

            if (Request.Browser.IsMobileDevice)
            {
                model.Brands = Brands.Where(a => a.Hot).Take(20);
                model.Categories = ProductCategories;
                model.ProductHots.Take(20);
                model.ProductWholesales = products.Where(a => a.Wholesale).Take(20);

                return PartialView("IndexMobile", model);
            }

            return View(model);
        }
        public PartialViewResult IndexMobile()
        {
            return PartialView();
        }
        [ChildActionOnly]
        public PartialViewResult Header()
        {
            var model = new HeaderViewModel
            {
                ProductCategories = ProductCategories.Where(a => a.ShowMenu),
                Brands = Brands,
                User = User
            };
            return PartialView(model);
        }
        public PartialViewResult HeaderMobile()
        {
            return PartialView();
        }
        [ChildActionOnly]
        public PartialViewResult Footer()
        {
            var categories = ArticleCategories.Where(p => p.ShowFooter);
            var categoryItems = categories.Select(a => new FooterViewModel.CategoryItem
            {
                Category = a,
                Articles = _unitOfWork.ArticleRepository.GetQuery(p => p.Active && (p.ArticleCategoryId == a.Id || p.ArticleCategory.ParentId == a.Id),
                    c => c.OrderByDescending(l => l.CreateDate), 10),
            });
            var model = new FooterViewModel
            {
                CategoryItems = categoryItems,
            };
            return PartialView(model);
        }
        public PartialViewResult FooterMobile()
        {
            return PartialView();
        }
        #endregion

        [Route("lien-he")]
        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult ContactForm(Contact model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { status = false, msg = "Hãy điền đúng định dạng." });
            }
            _unitOfWork.ContactRepository.Insert(model);
            _unitOfWork.Save();
            var subject = "Email liên hệ từ website: " + Request.Url?.Host;
            var body = $"<p>Tên người liên hệ: {model.FullName},</p>" +
                       $"<p>Khu vực: {model.Region},</p>" +
                       $"<p>Số điện thoại: {model.Mobile},</p>" +
                       $"<p>Email: {model.Email},</p>" +
                       $"<p>Code: {model.Code},</p>" +
                       $"<p>Nội dung: {model.Body}</p>" +
                       $"<p>Đây là hệ thống gửi email tự động, vui lòng không phản hồi lại email này.</p>";
            Task.Run(() => HtmlHelpers.SendEmail("gmail", subject, body, ConfigSite.Email, Email, Email, Password, "Doris"));

            return Json(new { status = true, msg = "Gửi liên hệ thành công.\nChúng tôi sẽ liên lạc với bạn sớm nhất có thể." });
        }
        public PartialViewResult SubcribeForm()
        {
            return PartialView();
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult SubcribeForm(string keywords)
        {
            Subcribe model = new Subcribe { Email = keywords };

            _unitOfWork.SubcribeRepository.Insert(model);
            _unitOfWork.Save();
            return Json(new { status = true, msg = "Đăng ký thành viên thành công!" });
        }

        #region Article 
        [Route("blogs/{url}.html")]
        public ActionResult ArticleDetail(string url)
        {
            var article = _unitOfWork.ArticleRepository.GetQuery(a => a.Url == url && a.Active).FirstOrDefault();
            if (article == null)
            {
                return RedirectToAction("Index");
            }

            var model = new ArticleDetailViewModel
            {
                Article = article,
                Articles = _unitOfWork.ArticleRepository.GetQuery(p =>
                p.Active && (p.ArticleCategoryId == article.ArticleCategoryId && p.Id != article.Id), q => q.OrderByDescending(a => a.CreateDate), 6),
            };

            if (Request.Browser.IsMobileDevice)
            {
                return PartialView("ArticleDetailMobile", model);
            }

            return View(model);
        }
        public PartialViewResult ArticleDetailMobile()
        {
            return PartialView();
        }
        #endregion

        #region Product
        [Route("san-pham")]
        public ActionResult AllProduct(int? page, int? catId, int? brandId, string sort = "all")
        {
            var pageNumber = page ?? 1;
            var pageSize = 30;
            var products = _unitOfWork.ProductRepository.GetQuery(p => p.Active, o => o.OrderByDescending(a => a.CreateDate));

            if (catId > 0)
            {
                products = products.Where(a => a.ProductCategoryId == catId);
            }
            if (brandId > 0)
            {
                products = products.Where(a => a.BrandId == brandId);
            }

            switch (sort)
            {
                case "new":
                    products = products.Where(a => a.New);
                    break;
                case "hot":
                    products = products.Where(a => a.Hot);
                    break;
            }

            var model = new CategoryProductViewModel
            {
                Products = products.ToPagedList(pageNumber, pageSize),
                Categories = ProductCategories,
                Brands = Brands,
                CatId = catId,
                BrandId = brandId,
                Sort = sort
            };

            if (Request.Browser.IsMobileDevice)
            {
                return PartialView("AllProductMobile", model);
            }

            return View(model);
        }
        public PartialViewResult GetProduct(int? page, int? catId, int? brandId, string sort = "all")
        {
            var pageNumber = page ?? 1;
            var pageSize = 30;
            var products = _unitOfWork.ProductRepository.GetQuery(p => p.Active, o => o.OrderByDescending(a => a.CreateDate));

            if (catId > 0)
            {
                products = products.Where(a => a.ProductCategoryId == catId);
            }
            if (brandId > 0)
            {
                products = products.Where(a => a.BrandId == brandId);
            }

            switch (sort)
            {
                case "new":
                    products = products.Where(a => a.New);
                    break;
                case "hot":
                    products = products.Where(a => a.Hot);
                    break;
            }

            var model = new CategoryProductViewModel
            {
                Products = products.ToPagedList(pageNumber, pageSize),
                Categories = ProductCategories,
                Brands = Brands,
                CatId = catId,
                BrandId = brandId,
                Sort = sort
            };
            return PartialView(model);
        }
        public PartialViewResult AllProductMobile()
        {
            return PartialView();
        }
        [Route("{url:regex(^(?!.*(vcms|uploader|article|banner|contact|productvcms|user)).*$)}", Order = 2)]
        public ActionResult ProductCategory(int? page, int? brandId, string url, string sort = "all")
        {
            var pageNumber = page ?? 1;
            var pageSize = 30;
            var category = ProductCategories.FirstOrDefault(a => a.Url == url);
            if (category == null)
            {
                return RedirectToAction("Index");
            }
            var products = _unitOfWork.ProductRepository.GetQuery(p => p.Active && p.ProductCategoryId == category.Id, o => o.OrderByDescending(a => a.CreateDate));

            if (brandId > 0)
            {
                products = products.Where(a => a.BrandId == brandId);
            }

            switch (sort)
            {
                case "new":
                    products = products.Where(a => a.New);
                    break;
                case "hot":
                    products = products.Where(a => a.Hot);
                    break;
            }

            var model = new CategoryProductViewModel
            {
                Category = category,
                Products = products.ToPagedList(pageNumber, pageSize),
                Brands = Brands,
                Url = url,
                Sort = sort,
                BrandId = brandId
            };

            if (Request.Browser.IsMobileDevice)
            {
                return PartialView("ProductCategoryMobile", model);
            }

            return View(model);
        }
        public PartialViewResult GetProductCategory(int? page, int? brandId, string url, string sort = "all")
        {
            var pageNumber = page ?? 1;
            var pageSize = 30;
            var category = ProductCategories.FirstOrDefault(a => a.Url == url);

            var products = _unitOfWork.ProductRepository.GetQuery(p => p.Active && p.ProductCategoryId == category.Id, o => o.OrderByDescending(a => a.CreateDate));

            if (brandId > 0)
            {
                products = products.Where(a => a.BrandId == brandId);
            }

            switch (sort)
            {
                case "new":
                    products = products.Where(a => a.New);
                    break;
                case "hot":
                    products = products.Where(a => a.Hot);
                    break;
            }

            var model = new CategoryProductViewModel
            {
                Products = products.ToPagedList(pageNumber, pageSize),
                Url = url,
                Sort = sort,
                BrandId = brandId
            };
            return PartialView(model);
        }
        public PartialViewResult ProductCategoryMobile()
        {
            return PartialView();
        }
        [Route("tim-san-pham")]
        public ActionResult SearchProduct(int? page, int? catId, int? brandId, string keywords, string sort = "all")
        {
            var pageNumber = page ?? 1;
            var pageSize = 30;

            if (string.IsNullOrEmpty(keywords))
            {
                return RedirectToAction("Index");
            }

            var products = _unitOfWork.ProductRepository.GetQuery(p => p.Active && p.Name.Contains(keywords), o => o.OrderByDescending(a => a.CreateDate));

            if (catId > 0)
            {
                products = products.Where(a => a.ProductCategoryId == catId);
            }
            if (brandId > 0)
            {
                products = products.Where(a => a.BrandId == brandId);
            }

            switch (sort)
            {
                case "new":
                    products = products.Where(a => a.New);
                    break;
                case "hot":
                    products = products.Where(a => a.Hot);
                    break;
            }

            var model = new SearchProductViewModel
            {
                Categories = ProductCategories,
                Brands = Brands,
                Products = products.ToPagedList(pageNumber, pageSize),
                Keywords = keywords,
                CatId = catId,
                BrandId = brandId
            };

            if (Request.Browser.IsMobileDevice)
            {
                return PartialView("SearchProductMobile", model);
            }

            return View(model);
        }
        public PartialViewResult GetSearchProduct(int? page, int? catId, int? brandId, string keywords, string sort = "all")
        {
            var pageNumber = page ?? 1;
            var pageSize = 30;

            var products = _unitOfWork.ProductRepository.GetQuery(p => p.Active && p.Name.Contains(keywords), o => o.OrderByDescending(a => a.CreateDate));

            if (catId > 0)
            {
                products = products.Where(a => a.ProductCategoryId == catId);
            }
            if (brandId > 0)
            {
                products = products.Where(a => a.BrandId == brandId);
            }

            switch (sort)
            {
                case "new":
                    products = products.Where(a => a.New);
                    break;
                case "hot":
                    products = products.Where(a => a.Hot);
                    break;
            }

            var model = new SearchProductViewModel
            {
                Categories = ProductCategories,
                Brands = Brands,
                Products = products.ToPagedList(pageNumber, pageSize),
                Keywords = keywords,
                CatId = catId,
                BrandId = brandId
            };
            return PartialView(model);
        }
        public PartialViewResult SearchProductMobile()
        {
            return PartialView();
        }
        [Route("{url}.html", Order = 1)]
        public ActionResult ProductDetail(string url)
        {
            var product = _unitOfWork.ProductRepository.GetQuery(p => p.Url == url).FirstOrDefault();

            if (product == null)
            {
                return RedirectToAction("Index");
            }

            var model = new ProductDetailViewModel
            {
                Product = product
            };

            if (Request.Browser.IsMobileDevice)
            {
                model.Comission = Convert.ToDecimal(model.Product.Price - model.Product.WholesalePrice);
                model.Products = _unitOfWork.ProductRepository.GetQuery(a => a.Active && a.ProductCategoryId == product.ProductCategoryId && a.Id != product.Id, o => o.OrderByDescending(a => a.CreateDate), 10);
                return PartialView("ProductDetailMobile", model);
            }

            return View(model);
        }
        public PartialViewResult ProductDetailMobile()
        {
            return PartialView();
        }
        [Route("khuyen-mai")]
        public ActionResult Promotion(int? page, int? catId, int? brandId, string sort = "all")
        {
            var pageSize = page ?? 1;
            var pageNumber = 20;
            var products = _unitOfWork.ProductRepository.GetQuery(a => a.Active && a.Hot, o => o.OrderByDescending(a => a.CreateDate));

            if (catId > 0)
            {
                products = products.Where(a => a.ProductCategoryId == catId);
            }
            if (brandId > 0)
            {
                products = products.Where(a => a.BrandId == brandId);
            }

            switch (sort)
            {
                case "new":
                    products = products.Where(a => a.New);
                    break;
                case "hot":
                    products = products.Where(a => a.Hot);
                    break;
            }

            var model = new PromotionViewModel
            {
                Products = products.ToPagedList(pageSize, pageNumber),
                Categories = ProductCategories,
                Brands = Brands,
                CatId = catId,
                BrandId = brandId,
                Sort = sort
            };

            if (Request.Browser.IsMobileDevice)
            {
                return PartialView("PromotionMobile", model);
            }

            return View(model);
        }
        public PartialViewResult GetPromotion(int? page, int? catId, int? brandId, string sort = "all")
        {
            var pageNumber = page ?? 1;
            var pageSize = 20;
            var products = _unitOfWork.ProductRepository.GetQuery(p => p.Active, o => o.OrderByDescending(a => a.CreateDate));

            if (catId > 0)
            {
                products = products.Where(a => a.ProductCategoryId == catId);
            }
            if (brandId > 0)
            {
                products = products.Where(a => a.BrandId == brandId);
            }

            switch (sort)
            {
                case "new":
                    products = products.Where(a => a.New);
                    break;
                case "hot":
                    products = products.Where(a => a.Hot);
                    break;
            }

            var model = new PromotionViewModel
            {
                Products = products.ToPagedList(pageNumber, pageSize),
                CatId = catId,
                BrandId = brandId,
                Sort = sort
            };
            return PartialView(model);
        }
        public PartialViewResult PromotionMobile()
        {
            return PartialView();
        }
        [Route("thuong-hieu/{url}")]
        public ActionResult BrandDetail(int? page, int? catId, string url, string sort = "all")
        {
            var pageNumber = page ?? 1;
            var pageSize = 30;
            var brand = _unitOfWork.BrandRepository.GetQuery(a => a.Active && a.Url == url).FirstOrDefault();
            if (brand == null)
            {
                return RedirectToAction("Index");
            }

            var products = _unitOfWork.ProductRepository.GetQuery(p => p.Active && p.BrandId == brand.Id, o => o.OrderByDescending(a => a.CreateDate));

            if (catId > 0)
            {
                products = products.Where(a => a.ProductCategoryId == catId);
            }

            switch (sort)
            {
                case "new":
                    products = products.Where(a => a.New);
                    break;
                case "hot":
                    products = products.Where(a => a.Hot);
                    break;
            }

            var model = new BrandViewModel
            {
                Products = products.ToPagedList(pageNumber, pageSize),
                Brand = brand,
                Categories = ProductCategories,
                CatId = catId,
                Sort = sort,
                Url = url
            };

            if (Request.Browser.IsMobileDevice)
            {
                return PartialView("BrandDetailMobile", model);
            }

            return View(model);
        }
        public PartialViewResult GetProductBrand(int? page, int? catId, string url, string sort = "all")
        {
            var pageNumber = page ?? 1;
            var pageSize = 30;
            var brand = Brands.FirstOrDefault(a => a.Url == url);

            var products = _unitOfWork.ProductRepository.GetQuery(p => p.Active && p.BrandId == brand.Id, o => o.OrderByDescending(a => a.CreateDate));

            if (catId > 0)
            {
                products = products.Where(a => a.ProductCategoryId == catId);
            }

            switch (sort)
            {
                case "new":
                    products = products.Where(a => a.New);
                    break;
                case "hot":
                    products = products.Where(a => a.Hot);
                    break;
            }

            var model = new BrandViewModel
            {
                Products = products.ToPagedList(pageNumber, pageSize),
                Url = url,
                Sort = sort,
                CatId = catId
            };
            return PartialView(model);
        }
        public PartialViewResult BrandDetailMobile()
        {
            return PartialView();
        }
        [Route("ban-chay-nhat")]
        public ActionResult BestSeller(int? page, int? catId, int? brandId, string sort = "all")
        {
            var pageNumber = page ?? 1;
            var pageSize = 35;
            var products = _unitOfWork.ProductRepository.GetQuery(p => p.Active && p.BestSeller, o => o.OrderByDescending(a => a.CreateDate));

            if (catId > 0)
            {
                products = products.Where(a => a.ProductCategoryId == catId);
            }
            if (brandId > 0)
            {
                products = products.Where(a => a.BrandId == brandId);
            }

            switch (sort)
            {
                case "new":
                    products = products.Where(a => a.New);
                    break;
                case "hot":
                    products = products.Where(a => a.Hot);
                    break;
            }

            var model = new CategoryProductViewModel
            {
                Products = products.ToPagedList(pageNumber, pageSize),
                Categories = ProductCategories,
                Brands = Brands,
                CatId = catId,
                BrandId = brandId,
                Sort = sort
            };
            return View(model);
        }
        public PartialViewResult GetBestSeller(int? page, int? catId, int? brandId, string sort = "all")
        {
            var pageNumber = page ?? 1;
            var pageSize = 35;
            var products = _unitOfWork.ProductRepository.GetQuery(p => p.Active && p.BestSeller, o => o.OrderByDescending(a => a.CreateDate));

            if (catId > 0)
            {
                products = products.Where(a => a.ProductCategoryId == catId);
            }
            if (brandId > 0)
            {
                products = products.Where(a => a.BrandId == brandId);
            }

            switch (sort)
            {
                case "new":
                    products = products.Where(a => a.New);
                    break;
                case "hot":
                    products = products.Where(a => a.Hot);
                    break;
            }

            var model = new CategoryProductViewModel
            {
                Products = products.ToPagedList(pageNumber, pageSize),
                Categories = ProductCategories,
                Brands = Brands,
                CatId = catId,
                BrandId = brandId,
                Sort = sort
            };
            return PartialView(model);
        }
        [Route("hang-moi-ve")]
        public ActionResult NewProduct(int? page, int? catId, int? brandId, string sort = "all")
        {
            var pageNumber = page ?? 1;
            var pageSize = 35;
            var products = _unitOfWork.ProductRepository.GetQuery(p => p.Active && p.New, o => o.OrderByDescending(a => a.CreateDate));

            if (catId > 0)
            {
                products = products.Where(a => a.ProductCategoryId == catId);
            }
            if (brandId > 0)
            {
                products = products.Where(a => a.BrandId == brandId);
            }

            switch (sort)
            {
                case "new":
                    products = products.Where(a => a.New);
                    break;
                case "hot":
                    products = products.Where(a => a.Hot);
                    break;
            }

            var model = new CategoryProductViewModel
            {
                Products = products.ToPagedList(pageNumber, pageSize),
                Categories = ProductCategories,
                Brands = Brands,
                CatId = catId,
                BrandId = brandId,
                Sort = sort
            };
            return View(model);
        }
        public PartialViewResult GetNewProduct(int? page, int? catId, int? brandId, string sort = "all")
        {
            var pageNumber = page ?? 1;
            var pageSize = 35;
            var products = _unitOfWork.ProductRepository.GetQuery(p => p.Active && p.New, o => o.OrderByDescending(a => a.CreateDate));

            if (catId > 0)
            {
                products = products.Where(a => a.ProductCategoryId == catId);
            }
            if (brandId > 0)
            {
                products = products.Where(a => a.BrandId == brandId);
            }

            switch (sort)
            {
                case "new":
                    products = products.Where(a => a.New);
                    break;
                case "hot":
                    products = products.Where(a => a.Hot);
                    break;
            }

            var model = new CategoryProductViewModel
            {
                Products = products.ToPagedList(pageNumber, pageSize),
                Categories = ProductCategories,
                Brands = Brands,
                CatId = catId,
                BrandId = brandId,
                Sort = sort
            };
            return PartialView(model);
        }
        [Route("gia-si-tot-nhat")]
        public ActionResult ProductWholesale(int? page)
        {
            var pageSize = page ?? 1;
            var products = _unitOfWork.ProductRepository.GetQuery(a => a.Active && a.Wholesale, o => o.OrderBy(a => a.Sort));

            var model = new ProductWholesaleViewModel
            {
                Products = products.ToPagedList(pageSize, 30),
            };
            return View(model);
        }
        #endregion

        [Route("chinh-sach-gia-{productId}")]
        public ActionResult CollabPolicy(int productId)
        {
            var product = _unitOfWork.ProductRepository.GetById(productId);
            if (product == null)
            {
                return RedirectToAction("Index");
            }

            var productUser = _unitOfWork.ProductUserRepository.GetQuery(a => a.ProductId == productId && a.UserId == User.Id).FirstOrDefault();

            decimal price = 0;
            if (product.PriceDiamond > 0 && User.Level == Level.Diamond)
            {
                price = Convert.ToDecimal(product.PriceDiamond);
            }
            else if (product.PricePlatinum > 0 && User.Level == Level.Platinum)
            {
                price = Convert.ToDecimal(product.PricePlatinum);
            }
            else if (product.PriceGold > 0 && User.Level == Level.Gold)
            {
                price = Convert.ToDecimal(product.PriceGold);
            }
            else if (product.PriceSilver > 0 && User.Level == Level.Silver)
            {
                price = Convert.ToDecimal(product.PriceSilver);
            }
            else if (product.PriceBronze > 0 && User.Level == Level.Bronze)
            {
                price = Convert.ToDecimal(product.PriceBronze);
            }
            else if (product.Price > 0)
            {
                price = Convert.ToDecimal(product.Price);
            }

            var model = new CollabPolicyViewModel
            {
                MyPrice = Convert.ToDecimal(product.Price),
                Price = Convert.ToDecimal(product.Price),
                LevelPrice = price,
                ProductId = productId
            };

            if (productUser != null)
            {
                model.MyPrice = Convert.ToDecimal(productUser.UserPrice);
            }

            model.Comission = model.MyPrice - price;

            return View(model);
        }
        public JsonResult ChangePrice(int productId, string price)
        {
            if (ModelState.IsValid)
            {
                var productUser = _unitOfWork.ProductUserRepository.GetQuery(a => a.ProductId == productId && a.UserId == User.Id).FirstOrDefault();

                if (productUser == null)
                {
                    productUser = new ProductUser
                    {
                        ProductId = productId,
                        UserId = User.Id,
                        UserPrice = Convert.ToDecimal(price.Replace(",", ""))
                    };
                }
                else
                {
                    productUser.UserPrice = Convert.ToDecimal(price.Replace(",", ""));
                }

                _unitOfWork.ProductUserRepository.AddOrUpdate(productUser);
                _unitOfWork.Save();
                return Json(new { status = 0 });
            }
            return Json(new { status = 1 });
        }

        [Route("deal-hot")]
        public ActionResult DealHot()
        {
            var model = new PromotionViewModel
            {
                Banners = _unitOfWork.BannerRepository.GetQuery(a => a.Active),
                Articles = _unitOfWork.ArticleRepository.GetQuery(a => a.Active && a.ArticleCategory.TypePost == TypePost.Promotion, o => o.OrderByDescending(a => a.CreateDate))
            };
            return View(model);
        }
        [Route("thuong-hieu")]
        public ActionResult AllBrand()
        {
            var model = new AllBrandViewModel
            {
                BrandHots = Brands.Where(a => a.Hot).Take(20),
                Brands = Brands,
                Categories = ProductCategories
            };
            return View(model);
        }
        [Route("ho-tro")]
        public ActionResult Support()
        {
            var articles = _unitOfWork.ArticleRepository.GetQuery(a => a.Active && a.ArticleCategory.TypePost == TypePost.Support, o => o.OrderByDescending(a => a.CreateDate));
            return View(articles);
        }

        [ChildActionOnly]
        public PartialViewResult FormLogin()
        {
            return PartialView();
        }
        public PartialViewResult GetBrand(string c)
        {
            var model = new GetBrandViewModel
            {
                Brands = Brands,
                Char = c
            };

            var newChar = c.ToLower();

            if (c != "ALL")
            {
                model.Brands = Brands.Where(a => a.BrandName.ToLower().StartsWith(newChar));
            }

            return PartialView(model);
        }
        public PartialViewResult GetBrandMobile(string c)
        {
            var model = new GetBrandViewModel
            {
                Brands = Brands,
                Char = c
            };

            var newChar = c.ToLower();

            if (c != "ALL")
            {
                model.Brands = Brands.Where(a => a.BrandName.ToLower().StartsWith(newChar));
            }

            return PartialView(model);
        }

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}