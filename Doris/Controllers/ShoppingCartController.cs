using Helpers;
using Doris.Filters;
using Doris.Models;
using Doris.ViewModel;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Xml.Schema;
using Antlr.Runtime.Tree;
using System.Runtime.InteropServices;

namespace Doris.Controllers
{
    [MemberLoginFilter, RoutePrefix("gio-hang")]
    public class ShoppingCartController : BaseController
    {
        public ConfigSite ConfigSite => (ConfigSite)HttpContext.Application["ConfigSite"];
        private static string Email => WebConfigurationManager.AppSettings["email"];
        private static string Password => WebConfigurationManager.AppSettings["password"];

        private string MemberEmail => RouteData.Values["MemberEmail"].ToString();
        private new User User => _unitOfWork.UserRepository.GetQuery(a => a.Active && a.Email == MemberEmail).SingleOrDefault();

        [Route("thong-tin")]
        public ActionResult Index(string returnUrl)
        {
            var cart = ShoppingCart.GetCart(HttpContext);

            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                CartTotalShipFee = cart.GetTotalShipFee()
            };
            ViewBag.ReturnUrl = returnUrl;

            if (Request.Browser.IsMobileDevice)
            {
                viewModel.TotalFreeShip = 2000000 - viewModel.CartTotal;

                return PartialView("IndexCartMobile", viewModel);
            }

            return View(viewModel);
        }
        [HttpPost, Route("thong-tin")]
        public ActionResult Index(int? transport, FormCollection fc)
        {
            var records = fc.GetValues("RecordId");
            var quantities = fc.GetValues("Quantity");

            if (transport > 0)
            {
                return RedirectToAction("Checkout", new { transport = transport });
            }

            if (records == null || quantities == null)
            {
                return RedirectToActionPermanent("Index");
            }
            for (var i = 0; i < records.Length; i++)
            {
                var recordId = Convert.ToInt32(records[i]);
                var quantity = Convert.ToInt32(quantities[i]);

                var cartItem = _unitOfWork.CartRepository.GetById(recordId);
                if (cartItem == null || cartItem.Count == quantity || quantity < 1) continue;

                cartItem.Count = quantity;
                _unitOfWork.Save();
            }

            return RedirectToActionPermanent("Index");
        }
        public PartialViewResult IndexCartMobile()
        {
            return PartialView();
        }
        [Route("thanh-toan")]
        public ActionResult CheckOut(int? transport)
        {
            var cart = ShoppingCart.GetCart(HttpContext);
            if (!cart.GetCartItems().Any())
            {
                return RedirectToAction("Index");
            }

            var carts = cart.GetCartItems();

            if (cart.GetTotal() >= 2000000)
            {
                foreach (var item in carts)
                {
                    var cartItem = _unitOfWork.CartRepository.GetById(item.RecordId);
                    if (item.Product.WholesalePrice > 0 && cartItem != null)
                    {
                        item.Price = item.Product.WholesalePrice;
                        cartItem.Price = item.Product.WholesalePrice;
                        _unitOfWork.Save();
                    }
                }
            }

            if (transport == 2)
            {
                foreach (var item in carts)
                {
                    var cartItem = _unitOfWork.CartRepository.GetById(item.RecordId);
                    var productUser = _unitOfWork.ProductUserRepository.GetQuery(a => a.ProductId == item.ProductId && a.UserId == User.Id).FirstOrDefault();
                    if (productUser != null && cartItem != null)
                    {
                        item.PriceUser = productUser.UserPrice;
                        cartItem.PriceUser = productUser.UserPrice;
                        _unitOfWork.Save();
                    }
                }
            }

            var addresses = _unitOfWork.AddressRepository.GetQuery(a => a.UserId == User.Id, o => o.OrderBy(a => a.Id));

            var model = new CheckOutViewModel
            {
                Order = new Order
                {
                    TypePay = 1,
                    ShipFee = 30000,

                },
                CartTotal = cart.GetTotalShipFee(),
                Addresses = addresses,
                Address = addresses.Where(a => a.Default).FirstOrDefault(),
                User = User,
                UserId = User.Id,
                CitySelectList = CitySelectList,
                Carts = carts
            };

            if (Request.Browser.IsMobileDevice)
            {
                model.BankSelectList = new SelectList(_unitOfWork.BankRepository.Get(a => a.Active, o => o.OrderBy(a => a.Sort)), "Id", "Name");
                model.Transport = transport;
                if (transport == 2)
                {
                    var bankUsers = _unitOfWork.BankUserRepository.GetQuery(a => a.UserId == User.Id, o => o.OrderBy(a => a.BankId));
                    model.CartTotal = cart.GetTotalUserShipFee();
                    model.BankUser = bankUsers.FirstOrDefault(a => a.Default);
                    model.BankUsers = bankUsers;
                }
                return PartialView("CheckoutMobile", model);
            }

            return View(model);
        }
        [Route("thanh-toan")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CheckOut(CheckOutViewModel model)
        {
            if (ModelState.IsValid)
            {
                var carts = ShoppingCart.GetCart(HttpContext);
                var item = carts.GetCartItems();

                model.Order.ShipFee = 30000;
                model.Order.UserId = model.UserId;
                model.Order.CityId = model.CityId;
                model.Order.DistrictId = model.DistrictId;
                model.Order.WardId = model.WardId;
                model.Order.Status = 1;

                if (model.AddressId != null)
                {
                    var address = _unitOfWork.AddressRepository.GetById(model.AddressId);
                    model.Order.CustomerInfo = new CustomerInfo
                    {
                        Fullname = address.Fullname,
                        Mobile = address.Mobile,
                        Email = MemberEmail,
                        Address = address.SpecificAddress
                    };
                }

                _unitOfWork.OrderRepository.Insert(model.Order);
                _unitOfWork.Save();

                model.Order.MaDonHang = DateTime.Now.ToString("yyyyMMddHHmm") + "C" + model.Order.Id;
                if (model.Transport == 2)
                {
                    foreach (var odetails in from cart1 in item
                                             let product = _unitOfWork.ProductRepository.GetById(cart1.ProductId)
                                             select new OrderDetail
                                             {
                                                 OrderId = model.Order.Id,
                                                 ProductId = cart1.ProductId,
                                                 Quantity = cart1.Count,
                                                 Price = cart1.PriceUser,
                                             })
                    {
                        _unitOfWork.OrderDetailRepository.Insert(odetails);
                    }
                }
                else
                {
                    foreach (var odetails in from cart1 in item
                                             let product = _unitOfWork.ProductRepository.GetById(cart1.ProductId)
                                             select new OrderDetail
                                             {
                                                 OrderId = model.Order.Id,
                                                 ProductId = cart1.ProductId,
                                                 Quantity = cart1.Count,
                                                 Price = cart1.Price,
                                             })
                    {
                        _unitOfWork.OrderDetailRepository.Insert(odetails);
                    }
                }
                _unitOfWork.Save();

                //Thanh toán CK
                var ward = _unitOfWork.WardRepository.GetById(model.WardId);

                var typepay = "Thanh toán khi nhận hàng";
                switch (model.Order.TypePay)
                {
                    case 1:
                        typepay = "Tiền mặt";
                        break;
                    case 2:
                        typepay = "Chuyển khoản";
                        break;
                }
                var tamtinh = carts.GetTotal().ToString("N0");
                var addressCustomer = model.Order.CustomerInfo.Address + ", " + ward?.Name + ", " + ward?.District.Name + ", " + ward?.District.City.Name;
                if (model.Transport == 2)
                {
                    tamtinh = carts.GetTotalUser().ToString("N0");
                    addressCustomer = model.Order.CustomerInfo.Address;
                }
                var sb = "<p style='font-size:16px'>Thông tin đơn hàng gửi từ website " + Request.Url?.Host + "</p>";
                sb += "<p>Mã đơn hàng: <strong>" + model.Order.MaDonHang + "</strong></p>";
                sb += "<p>Họ và tên: <strong>" + model.Order.CustomerInfo.Fullname + "</strong></p>";
                sb += "<p>Địa chỉ: <strong>" + addressCustomer + "</strong></p>";
                sb += "<p>Email: <strong>" + model.Order.CustomerInfo.Email + "</strong></p>";
                sb += "<p>Điện thoại: <strong>" + model.Order.CustomerInfo.Mobile + "</strong></p>";
                sb += "<p>Yêu cầu thêm: <strong>" + model.Order.CustomerInfo.Body + "</strong></p>";
                sb += "<p>Ngày đặt hàng: <strong>" + model.Order.CreateDate.ToString("dd-MM-yyyy HH:ss") + "</strong></p>";
                sb += "<p>Hình thức thanh toán: <strong>" + typepay + "</strong></p>";
                sb += "<p>Thông tin đơn hàng</p>";
                sb += "<table border='1' cellpadding='10' style='border:1px #ccc solid;border-collapse: collapse'>" +
                      "<tr>" +
                      "<th>Ảnh sản phẩm</th>" +
                      "<th>Tên sản phẩm</th>" +
                      "<th>Số lượng</th>" +
                      "<th>Giá tiền</th>" +
                      "<th>Thành tiền</th>" +
                      "</tr>";
                foreach (var odetails in model.Order.OrderDetails)
                {
                    var thanhtien = Convert.ToDecimal(odetails.Price * odetails.Quantity);
                    var img = "NO PICTURE";
                    if (odetails.Product.ListImage != null)
                    {
                        img = "<img src='" + Request.Url?.GetLeftPart(UriPartial.Authority) + "/images/products/" + odetails.Product.ListImage.Split(',')[0] + "?w=100' />";
                    }
                    sb += "<tr>" +
                          "<td>" + img + "</td>" +
                          "<td>" + odetails.Product.Name;

                    sb += "</td>" +
                          "<td style='text-align:center'>" + odetails.Quantity + "</td>" +
                          "<td style='text-align:center'>" + Convert.ToDecimal(odetails.Price).ToString("N0") + " đ</td>" +
                          "<td style='text-align:center'>" + thanhtien.ToString("N0") + " đ</td>" +
                          "</tr>";
                }

                sb += "<tr><td colspan='6' style='text-align:right'><strong>Tạm tính: " + tamtinh + " đ</strong></td></tr>";
                sb += "<tr><td colspan='6' style='text-align:right'><strong>Giao hàng: " + model.Order.ShipFee.ToString("N0") + " đ</strong></td></tr>";
                sb += "<tr><td colspan='6' style='text-align:right'><strong>Tổng tiền: " + model.Order.TotalFee().ToString("N0") + " đ</strong></td></tr>";
                sb += "</table>";
                sb += "<p>Cảm ơn bạn đã tin tưởng và mua hàng của chúng tôi.</p>";

                Task.Run(() => HtmlHelpers.SendEmail("gmail", "[" + model.Order.MaDonHang + "] Đơn đặt hàng từ website Doris", sb, model.Order.CustomerInfo.Email, Email, Email, Password, "Doris", model.Order.CustomerInfo.Email, ConfigSite.Email));

                return RedirectToAction("CheckOutComplete", new { orderId = model.Order.MaDonHang });
            }
            var cart = ShoppingCart.GetCart(HttpContext);
            model.CartTotal = cart.GetTotal();
            model.CitySelectList = model.CitySelectList;
            model.Carts = cart.GetCartItems();
            model.Addresses = _unitOfWork.AddressRepository.GetQuery(a => a.UserId == User.Id, o => o.OrderBy(a => a.Id));
            model.User = User;
            if (model.CityId > 0)
            {
                model.DistrictSelectList = DistrictSelectList(model.CityId);
            }
            if (model.DistrictId > 0)
            {
                model.WardSelectList = WardSelectList(model.DistrictId);
            }
            return View(model);
        }
        public PartialViewResult CheckoutMobile()
        {
            return PartialView();
        }

        [Route("thanh-toan-thanh-cong")]
        public ActionResult CheckOutComplete(string orderId)
        {
            EmptyCart();
            ViewBag.OrderId = orderId;
            return View();
        }

        public ActionResult EmptyCart()
        {
            var cart = ShoppingCart.GetCart(HttpContext);
            cart.EmptyCart();
            return RedirectToAction("Index");
        }

        [Route("them-vao-gio-hang")]
        public JsonResult AddToCart(int productId, int quantity = 1)
        {
            var cart = ShoppingCart.GetCart(HttpContext);
            decimal? price = null;

            var addedProduct = _unitOfWork.ProductRepository.GetQuery(a => a.Id == productId).SingleOrDefault();

            if (addedProduct.PriceDiamond > 0 && User.Level == Level.Diamond)
            {
                price = addedProduct.PriceDiamond;
            }
            else if (addedProduct.PricePlatinum > 0 && User.Level == Level.Platinum)
            {
                price = addedProduct.PricePlatinum;
            }
            else if (addedProduct.PriceGold > 0 && User.Level == Level.Gold)
            {
                price = addedProduct.PriceGold;
            }
            else if (addedProduct.PriceSilver > 0 && User.Level == Level.Silver)
            {
                price = addedProduct.PriceSilver;
            }
            else if (addedProduct.PriceBronze > 0 && User.Level == Level.Bronze)
            {
                price = addedProduct.PriceBronze;
            }
            else if (addedProduct.Price > 0)
            {
                price = addedProduct.Price;
            }
            try
            {
                cart.AddToCart(productId, price, quantity);
                var data = new
                {
                    result = 1,
                    count = cart.GetCount()
                };
                return Json(data);
            }
            catch
            {
                var data = new
                {
                    result = 0,
                    count = cart.GetCount()
                };
                return Json(data);
            }
        }
        [HttpPost]
        public JsonResult UpdateCart(int cartId, int quantity)
        {
            var cart = ShoppingCart.GetCart(HttpContext);

            var cartItem = _unitOfWork.CartRepository.GetQuery(a => a.RecordId == cartId).FirstOrDefault();
            if (cartItem == null)
            {
                return Json(new { result = 0 });
            }

            cartItem.Count = quantity;
            _unitOfWork.Save();
            return Json(new { result = 1 });
        }

        [HttpPost]
        public void AddProduct(int sid = 0, int pid = 0, int quantity = 0)
        {
            var product = _unitOfWork.ProductRepository.GetById(pid);
            if (product == null) return;
            var cart = _unitOfWork.CartRepository.GetById(sid);
            if (cart == null) return;
            cart.Count = quantity;
            _unitOfWork.Save();
        }
        [HttpPost]
        public JsonResult RemoveFromCart(int id)
        {
            // Remove the item from the cart
            var cart = ShoppingCart.GetCart(HttpContext);

            // Get the name of the album to display confirmation
            var productName = _unitOfWork.CartRepository.GetById(id).Product.Name;

            // Remove from cart
            var itemCount = cart.RemoveFromCart(id);

            // Display the confirmation message
            var results = new ShoppingCartRemoveViewModel
            {
                Message = productName + " đã được xóa khỏi giỏ hàng của bạn.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                Status = itemCount,
                DeleteId = id
            };
            return Json(results);
        }
        public PartialViewResult CartSummary()
        {
            var cart = ShoppingCart.GetCart(HttpContext);
            var model = new CartSummaryViewModel
            {
                Carts = cart.GetCartItems(),
                Count = cart.GetCount(),
                TotalMoney = cart.GetTotal()
            };
            return PartialView("CartSummary", model);
        }

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}