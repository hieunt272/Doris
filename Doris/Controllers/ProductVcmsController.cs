using Doris.DAL;
using Doris.Models;
using Doris.ViewModel;
using Helpers;
using Microsoft.Ajax.Utilities;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Doris.Controllers
{
    [Authorize]
    public class ProductVcmsController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private IEnumerable<ProductCategory> ProductCategories =>
            _unitOfWork.ProductCategoryRepository.Get(a => a.CategoryActive, q => q.OrderBy(a => a.CategorySort));
        private IEnumerable<Brand> Brands =>
            _unitOfWork.BrandRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort));
        private SelectList ParentProductCategoryList => new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName");

        #region ProductCategory
        [ChildActionOnly]
        public ActionResult ListCategory()
        {
            var allcats = _unitOfWork.ProductCategoryRepository.Get(orderBy: q => q.OrderBy(a => a.CategorySort));
            return PartialView(allcats);
        }
        public ActionResult ProductCategory(string result = "")
        {
            ViewBag.ArticleCat = result;

            var model = new InsertProductCatViewModel
            {
                RootCats = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName"),
                ChildCategoryList = new SelectList(new List<ProductCategory>(), "Id", "Name"),
                ProductCategory = new ProductCategory { CategorySort = 1 }
            };

            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult ProductCategory(InsertProductCatViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i] == null || Request.Files[i].ContentLength <= 0) continue;
                    if (!HtmlHelpers.CheckFileExt(Request.Files[i].FileName, "jpg|jpeg|png|gif")) continue;
                    if (Request.Files[i].ContentLength > 1024 * 1024 * 4) continue;

                    var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(Request.Files[i].FileName)) +
                        "-" + DateTime.Now.Millisecond + Path.GetExtension(Request.Files[i].FileName);
                    var imgPath = "/images/productCategory/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                    var imgFile = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                    var newImage = Image.FromStream(Request.Files[i].InputStream);
                    var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1000, 1000, false);
                    HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);

                    if (Request.Files.Keys[i] == "ProductCategory.Image")
                    {
                        model.ProductCategory.Image = imgFile;
                    }
                    else if (Request.Files.Keys[i] == "ProductCategory.CoverImage")
                    {
                        model.ProductCategory.CoverImage = imgFile;
                    }
                }

                model.ProductCategory.Url = HtmlHelpers.ConvertToUnSign(null, model.ProductCategory.Url ?? model.ProductCategory.CategoryName);
                model.ProductCategory.ParentId = model.CategoryId ?? model.ParentId;
                _unitOfWork.ProductCategoryRepository.Insert(model.ProductCategory);
                _unitOfWork.Save();

                var count = _unitOfWork.ProductCategoryRepository.GetQuery(a => a.Url == model.ProductCategory.Url).Count();
                if (count > 1)
                {
                    model.ProductCategory.Url += "-" + model.ProductCategory.Id;
                    _unitOfWork.Save();
                }

                return RedirectToAction("ProductCategory", new { result = "success" });
            }
            model.RootCats = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName");
            return View(model);
        }
        public ActionResult UpdateCategory(int catId = 0)
        {
            var category = _unitOfWork.ProductCategoryRepository.GetById(catId);
            var parentCat = ProductCategories.FirstOrDefault(a => a.Id == category.ParentId);
            if (category == null)
            {
                return RedirectToAction("ProductCategory");
            }

            var model = new InsertProductCatViewModel
            {
                ParentId = 0,
                CategoryId = 0,
                RootCats = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName"),
                ChildCategoryList = new SelectList(new List<ProductCategory>(), "Id", "Name"),
                ProductCategory = category
            };
            if (parentCat != null)
            {
                model.ParentId = Convert.ToInt32(parentCat.Id);
                if (parentCat.ParentId != null)
                {
                    model.ParentId = Convert.ToInt32(parentCat.ParentId);
                    model.CategoryId = parentCat.Id;
                }
            }

            if (model.ParentId > 0)
            {
                model.ChildCategoryList = new SelectList(ProductCategories.Where(a => a.ParentId == model.ParentId), "Id", "CategoryName");
            }
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateCategory(InsertProductCatViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i] == null || Request.Files[i].ContentLength <= 0) continue;
                    if (!HtmlHelpers.CheckFileExt(Request.Files[i].FileName, "jpg|jpeg|png|gif")) continue;
                    if (Request.Files[i].ContentLength > 1024 * 1024 * 4) continue;

                    var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(Request.Files[i].FileName)) +
                        "-" + DateTime.Now.Millisecond + Path.GetExtension(Request.Files[i].FileName);
                    var imgPath = "/images/productCategory/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                    var imgFile = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                    var newImage = Image.FromStream(Request.Files[i].InputStream);
                    var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1000, 1000, false);
                    HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);

                    if (Request.Files.Keys[i] == "ProductCategory.Image")
                    {
                        model.ProductCategory.Image = imgFile;
                    }
                    else if (Request.Files.Keys[i] == "ProductCategory.CoverImage")
                    {
                        model.ProductCategory.CoverImage = imgFile;
                    }
                }

                var file = Request.Files["ProductCategory.Image"];
                var file2 = Request.Files["ProductCategory.CoverImage"];

                if (file != null && file.ContentLength == 0)
                {
                    model.ProductCategory.Image = fc["CurrentFile"] == "" ? null : fc["CurrentFile"];
                }
                if (file2 != null && file2.ContentLength == 0)
                {
                    model.ProductCategory.CoverImage = fc["CurrentFile2"] == "" ? null : fc["CurrentFile2"];
                }

                model.ProductCategory.Url = HtmlHelpers.ConvertToUnSign(null, model.ProductCategory.Url ?? model.ProductCategory.CategoryName);
                model.ProductCategory.ParentId = model.CategoryId ?? model.ParentId;
                _unitOfWork.ProductCategoryRepository.Update(model.ProductCategory);
                _unitOfWork.Save();

                var count = _unitOfWork.ProductCategoryRepository.GetQuery(a => a.Url == model.ProductCategory.Url).Count();
                if (count > 1)
                {
                    model.ProductCategory.Url += "-" + model.ProductCategory.Id;
                    _unitOfWork.Save();
                }

                return RedirectToAction("ProductCategory", new { result = "update" });
            }
            model.RootCats = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName");
            if (model.ParentId > 0)
            {
                model.ChildCategoryList = new SelectList(ProductCategories.Where(a => a.ParentId == model.ParentId), "Id", "CategoryName");
            }
            return View(model);
        }
        [HttpPost]
        public bool DeleteCategory(int catId = 0)
        {
            var category = _unitOfWork.ProductCategoryRepository.GetById(catId);
            if (category == null)
            {
                return false;
            }
            _unitOfWork.ProductCategoryRepository.Delete(category);
            _unitOfWork.Save();
            return true;
        }
        public bool UpdateProductCat(int sort = 1, bool active = false, bool menu = false, int productCatId = 0)
        {
            var productCat = _unitOfWork.ProductCategoryRepository.GetById(productCatId);
            if (productCat == null)
            {
                return false;
            }
            productCat.CategorySort = sort;
            productCat.CategoryActive = active;
            productCat.ShowMenu = menu;

            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region Product  
        public ActionResult ListProduct(int? page, string name, int? catId, int? parentId, int? brandId, string sort = "sort-asc", string result = "")
        {
            ViewBag.Result = result;
            var pageNumber = page ?? 1;
            const int pageSize = 15;
            var products = _unitOfWork.ProductRepository.GetQuery().AsNoTracking();

            if (catId > 0)
            {
                products = products.Where(a => a.ProductCategoryId == catId);
            }
            else if (parentId > 0)
            {
                products = products.Where(a => a.ProductCategoryId == parentId);
            }
            if (brandId > 0)
            {
                products = products.Where(a => a.BrandId == brandId);
            }
            if (!string.IsNullOrEmpty(name))
            {
                products = products.Where(l => l.Name.Contains(name));
            }

            switch (sort)
            {
                case "date-desc":
                    products = products.OrderByDescending(a => a.CreateDate);
                    break;
                case "sort-desc":
                    products = products.OrderByDescending(a => a.Sort);
                    break;
                case "hot":
                    products = products.OrderByDescending(a => a.Sort);
                    break;
                case "date-asc":
                    products = products.OrderBy(a => a.CreateDate);
                    break;
                default:
                    products = products.OrderBy(a => a.Sort);
                    break;
            }
            var model = new ListProductViewModel
            {
                SelectCategories = new SelectList(ProductCategories.Where(a => a.ParentId == null), "Id", "CategoryName"),
                SelectBrands = new SelectList(Brands, "Id", "BrandName"),
                Products = products.ToPagedList(pageNumber, pageSize),
                CatId = catId,
                ParentId = parentId,
                BrandId = brandId,
                Name = name,
                Sort = sort
            };
            if (parentId.HasValue)
            {
                model.SelectCategories = new SelectList(ProductCategories.Where(a => a.ParentId == parentId), "Id", "CategoryName");
            }
            if (catId.HasValue)
            {
                model.ChildCategoryList = new SelectList(ProductCategories.Where(a => a.ParentId == catId), "Id", "CategoryName");
            }
            if (brandId.HasValue)
            {
                model.SelectBrands = new SelectList(Brands, "Id", "BrandName");
            }
            return View(model);
        }
        public ActionResult Product()
        {
            var model = new InsertProductViewModel
            {
                Product = new Product { Sort = 1, Active = true },
                Categories = ProductCategories,
                Brands = Brands
            };
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Product(InsertProductViewModel model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                if (model.Price != null)
                {
                    model.Product.Price = Convert.ToDecimal(model.Price.Replace(",", ""));
                }
                if (model.PriceBronze != null)
                {
                    model.Product.PriceBronze = Convert.ToDecimal(model.PriceBronze.Replace(",", ""));
                }
                if (model.PriceSilver != null)
                {
                    model.Product.PriceSilver = Convert.ToDecimal(model.PriceSilver.Replace(",", ""));
                }
                if (model.PriceGold != null)
                {
                    model.Product.PriceGold = Convert.ToDecimal(model.PriceGold.Replace(",", ""));
                }
                if (model.PricePlatinum != null)
                {
                    model.Product.PricePlatinum = Convert.ToDecimal(model.PricePlatinum.Replace(",", ""));
                }
                if (model.PriceDiamond != null)
                {
                    model.Product.PriceDiamond = Convert.ToDecimal(model.PriceDiamond.Replace(",", ""));
                }
                if (model.WholesalePrice != null)
                {
                    model.Product.WholesalePrice = Convert.ToDecimal(model.WholesalePrice.Replace(",", ""));
                }

                model.Product.ListImage = fc["Pictures"];
                model.Product.BrandId = model.BrandId;
                model.Product.ProductCategoryId = model.CategoryId;
                model.Product.Url = HtmlHelpers.ConvertToUnSign(null, model.Product.Url ?? model.Product.Name);
                _unitOfWork.ProductRepository.Insert(model.Product);
                _unitOfWork.Save();

                var count = _unitOfWork.ProductRepository.GetQuery(a => a.Url == model.Product.Url).Count();
                if (count > 1)
                {
                    model.Product.Url += "-" + DateTime.Now.Millisecond;
                    _unitOfWork.Save();
                }

                return RedirectToAction("ListProduct", new { result = "success" });
            }

            model.Categories = ProductCategories;
            model.Brands = Brands;
            return View(model);
        }
        public ActionResult UpdateProduct(int proId = 0)
        {
            var product = _unitOfWork.ProductRepository.GetById(proId);
            if (product == null)
            {
                return RedirectToAction("ListProduct");
            }

            var model = new InsertProductViewModel
            {
                Product = product,
                CategoryId = product.ProductCategoryId,
                Categories = ProductCategories,
                Brands = Brands,
                Price = product.Price?.ToString("N0"),
                PriceBronze = product.PriceBronze?.ToString("N0"),
                PriceSilver = product.PriceSilver?.ToString("N0"),
                PriceGold = product.PriceGold?.ToString("N0"),
                PricePlatinum = product.PricePlatinum?.ToString("N0"),
                PriceDiamond = product.PriceDiamond?.ToString("N0"),
                WholesalePrice = product.WholesalePrice?.ToString("N0")
            };
            return View(model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateProduct(InsertProductViewModel model, FormCollection fc)
        {
            var product = _unitOfWork.ProductRepository.GetById(model.Product.Id);
            if (product == null)
            {
                return RedirectToAction("ListProduct");
            }

            if (ModelState.IsValid)
            {
                if (model.Price != null)
                {
                    product.Price = Convert.ToDecimal(model.Price.Replace(",", ""));
                }
                else
                {
                    product.Price = null;
                }
                if (model.PriceBronze != null)
                {
                    product.PriceBronze = Convert.ToDecimal(model.PriceBronze.Replace(",", ""));
                }
                else
                {
                    product.PriceBronze = null;
                }
                if (model.PriceSilver != null)
                {
                    product.PriceSilver = Convert.ToDecimal(model.PriceSilver.Replace(",", ""));
                }
                else
                {
                    product.PriceSilver = null;
                }
                if (model.PriceGold != null)
                {
                    product.PriceGold = Convert.ToDecimal(model.PriceGold.Replace(",", ""));
                }
                else
                {
                    product.PriceGold = null;
                }
                if (model.PricePlatinum != null)
                {
                    product.PricePlatinum = Convert.ToDecimal(model.PricePlatinum.Replace(",", ""));
                }
                else
                {
                    product.PricePlatinum = null;
                }
                if (model.PriceDiamond != null)
                {
                    product.PriceDiamond = Convert.ToDecimal(model.PriceDiamond.Replace(",", ""));
                }
                else
                {
                    product.PriceDiamond = null;
                }
                if (model.WholesalePrice != null)
                {
                    product.WholesalePrice = Convert.ToDecimal(model.WholesalePrice.Replace(",", ""));
                }
                else
                {
                    product.WholesalePrice = null;
                }

                product.ListImage = fc["Pictures"] == "" ? null : fc["Pictures"];
                product.Url = HtmlHelpers.ConvertToUnSign(null, model.Product.Url ?? model.Product.Name);
                product.BrandId = model.BrandId;
                product.ProductCategoryId = model.CategoryId;
                product.Name = model.Product.Name;
                product.Body = model.Product.Body;
                product.Active = model.Product.Active;
                product.Home = model.Product.Home;
                product.Hot = model.Product.Hot;
                product.New = model.Product.New;
                product.BestSeller = model.Product.BestSeller;
                product.Wholesale = model.Product.Wholesale;
                product.TitleMeta = model.Product.TitleMeta;
                product.DescriptionMeta = model.Product.DescriptionMeta;
                product.Sort = model.Product.Sort;
                product.Description = model.Product.Description;
                _unitOfWork.Save();

                var count = _unitOfWork.ProductRepository.GetQuery(a => a.Url == product.Url).Count();
                if (count > 1)
                {
                    product.Url += "-" + DateTime.Now.Millisecond;
                    _unitOfWork.Save();
                }

                return RedirectToAction("ListProduct", new { result = "update" });
            }

            model.Categories = ProductCategories;
            model.Brands = Brands;
            return View(model);
        }
        [HttpPost]
        public bool DeleteProduct(int proId = 0)
        {
            var product = _unitOfWork.ProductRepository.GetById(proId);
            if (product == null)
            {
                return false;
            }
            _unitOfWork.ProductRepository.Delete(product);
            _unitOfWork.Save();
            return true;
        }
        public bool QuickUpdate(int sort = 1, int proId = 0)
        {
            var product = _unitOfWork.ProductRepository.GetById(proId);
            if (product == null)
            {
                return false;
            }
            product.Sort = sort;

            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region Brand
        public ActionResult ListBrand(int? page, string name, string result = "")
        {
            ViewBag.Result = result;
            var pageNumber = page ?? 1;
            const int pageSize = 15;
            var brands = _unitOfWork.BrandRepository.GetQuery(orderBy: o => o.OrderBy(a => a.Sort));

            if (!string.IsNullOrEmpty(name))
            {
                brands = brands.Where(l => l.BrandName.Contains(name));
            }

            var model = new ListBrandViewModel
            {
                Brands = brands.ToPagedList(pageNumber, pageSize),
                Name = name
            };
            return View(model);
        }
        public ActionResult Brand()
        {
            var brand = new Brand { Sort = 1, Active = true };

            return View(brand);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult Brand(Brand model, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i] == null || Request.Files[i].ContentLength <= 0) continue;
                    if (!HtmlHelpers.CheckFileExt(Request.Files[i].FileName, "jpg|jpeg|png|gif")) continue;
                    if (Request.Files[i].ContentLength > 1024 * 1024 * 4) continue;

                    var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(Request.Files[i].FileName)) +
                        "-" + DateTime.Now.Millisecond + Path.GetExtension(Request.Files[i].FileName);
                    var imgPath = "/images/brands/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                    var imgFile = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                    var newImage = Image.FromStream(Request.Files[i].InputStream);
                    var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1000, 1000, false);
                    HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);

                    if (Request.Files.Keys[i] == "Image")
                    {
                        model.Image = imgFile;
                    }
                    else if (Request.Files.Keys[i] == "CoverImage")
                    {
                        model.CoverImage = imgFile;
                    }
                }

                model.Url = HtmlHelpers.ConvertToUnSign(null, model.Url ?? model.BrandName);

                _unitOfWork.BrandRepository.Insert(model);
                _unitOfWork.Save();
                return RedirectToAction("ListBrand", new { result = "success" });
            }
            return View(model);
        }
        public ActionResult UpdateBrand(int brandId = 0)
        {
            var brand = _unitOfWork.BrandRepository.GetById(brandId);
            if (brand == null)
            {
                return RedirectToAction("Brand");
            }

            return View(brand);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult UpdateBrand(Brand brand, FormCollection fc)
        {
            if (ModelState.IsValid)
            {
                for (var i = 0; i < Request.Files.Count; i++)
                {
                    if (Request.Files[i] == null || Request.Files[i].ContentLength <= 0) continue;
                    if (!HtmlHelpers.CheckFileExt(Request.Files[i].FileName, "jpg|jpeg|png|gif")) continue;
                    if (Request.Files[i].ContentLength > 1024 * 1024 * 4) continue;

                    var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(Request.Files[i].FileName)) +
                        "-" + DateTime.Now.Millisecond + Path.GetExtension(Request.Files[i].FileName);
                    var imgPath = "/images/brands/" + DateTime.Now.ToString("yyyy/MM/dd");
                    HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                    var imgFile = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;

                    var newImage = Image.FromStream(Request.Files[i].InputStream);
                    var fixSizeImage = HtmlHelpers.FixedSize(newImage, 1000, 1000, false);
                    HtmlHelpers.SaveJpeg(Server.MapPath(Path.Combine(imgPath, imgFileName)), fixSizeImage, 90);

                    if (Request.Files.Keys[i] == "Image")
                    {
                        brand.Image = imgFile;
                    }
                    else if (Request.Files.Keys[i] == "CoverImage")
                    {
                        brand.CoverImage = imgFile;
                    }
                }

                var file = Request.Files["Image"];
                var file2 = Request.Files["CoverImage"];

                if (file != null && file.ContentLength == 0)
                {
                    brand.Image = fc["CurrentFile"] == "" ? null : fc["CurrentFile"];
                }
                if (file2 != null && file2.ContentLength == 0)
                {
                    brand.CoverImage = fc["CurrentFile2"] == "" ? null : fc["CurrentFile2"];
                }

                brand.Url = HtmlHelpers.ConvertToUnSign(null, brand.Url ?? brand.BrandName);
                _unitOfWork.BrandRepository.Update(brand);
                _unitOfWork.Save();
                return RedirectToAction("ListBrand", new { result = "update" });
            }
            return View(brand);
        }
        [HttpPost]
        public bool DeleteBrand(int brandId = 0)
        {
            var brand = _unitOfWork.BrandRepository.GetById(brandId);
            if (brand == null)
            {
                return false;
            }
            _unitOfWork.BrandRepository.Delete(brand);
            _unitOfWork.Save();
            return true;
        }
        public bool QuickUpdateBrand(int sort = 1, bool active = false, bool home = false, bool hot = false, int brandId = 0)
        {
            var brand = _unitOfWork.BrandRepository.GetById(brandId);
            if (brand == null)
            {
                return false;
            }
            brand.Sort = sort;
            brand.Active = active;
            brand.Home = home;
            brand.Hot = hot;

            _unitOfWork.Save();
            return true;
        }
        #endregion

        public JsonResult GetProductCategory(int? parentId)
        {
            var categories = ProductCategories.Where(a => a.ParentId == parentId);
            return Json(categories.Select(a => new { a.Id, Name = a.CategoryName }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetChildCategory(int parentId = 0)
        {
            var childCategories = ProductCategories.Where(a => a.ParentId == parentId);
            return Json(childCategories.Select(a => new { a.Id, a.CategoryName }), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}