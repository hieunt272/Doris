using Helpers;
using Doris.DAL;
using Doris.Filters;
using Doris.Models;
using Doris.ViewModel;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Collections.Generic;

namespace Doris.Controllers
{
    [MemberFilter, RoutePrefix("user")]
    public class UserController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        [OverrideActionFilters, Route("dang-ky")]
        public ActionResult Register()
        {
            return View();
        }
        [Route("dang-ky")]
        [HttpPost, ValidateAntiForgeryToken, OverrideActionFilters]
        public ActionResult Register(RegisterViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var phoneVal = model.PhoneNumber.Trim();

                var checkUser = _unitOfWork.UserRepository.GetQuery(a => a.Username.Equals(phoneVal)).SingleOrDefault();
                if (checkUser != null)
                {
                    ModelState.AddModelError("", @"Tên đăng nhập đã tồn tại!! Vui lòng nhập tên đăng nhập khác");
                }
                else
                {
                    var hashPass = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
                    var shopCode = Guid.NewGuid().ToString().ToUpper().Substring(0, 6);
                    var user = new User
                    {
                        FullName = model.FullName,
                        Username = model.PhoneNumber,
                        Password = hashPass,
                        Email = model.Email,
                        PhoneNumber = phoneVal,
                        ShopCode = shopCode,
                        Active = true,
                    };
                    _unitOfWork.UserRepository.Insert(user);
                    _unitOfWork.Save();

                    List<Discount> discounts = _unitOfWork.DiscountRepository.GetQuery().ToList();
                    if (discounts.Any())
                    {
                        foreach (var discount in discounts)
                        {
                            var discountUser = new DiscountUser
                            {
                                DiscountId = discount.Id,
                                UserId = user.Id
                            };
                            _unitOfWork.DiscountUserRepository.Insert(discountUser);
                            _unitOfWork.Save();
                        }
                    }

                    var userData = user.FullName + "|" + user.Level + "|" + user.ShopCode + "|" + user.Avatar;
                    var ticket = new FormsAuthenticationTicket(2, model.PhoneNumber.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                        userData, FormsAuthentication.FormsCookiePath);

                    var encTicket = FormsAuthentication.Encrypt(ticket);
                    //Create the cookie.
                   Response.Cookies.Add(new HttpCookie(".ASPXAUTHMEMBER", encTicket));
                    //if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                    //    && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    //{
                    //    return Redirect(returnUrl);
                    //}
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }
        [OverrideActionFilters, Route("dang-nhap")]
        public ActionResult Login(string result = "")
        {
            ViewBag.Result = result;
            return View();
        }
        [HttpPost, Route("dang-nhap"), OverrideActionFilters]
        public ActionResult Login(UserLoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = _unitOfWork.UserRepository.Get(a => a.Username == model.Username).SingleOrDefault();
                if (user == null || !HtmlHelpers.VerifyHash(model.Password, "SHA256", user.Password))
                {
                    ModelState.AddModelError("", @"Số điện thoại hoặc mật khẩu không chính xác.");
                    return View(model);
                }
                if (!user.Active)
                {
                    ModelState.AddModelError("", @"Tài khoản tạm thời bị khóa. Vui lòng liên hệ với quản trị viên.");
                    return View(model);
                }
                var userData = user.FullName + "|" + user.Level + "|" + user.ShopCode + "|" + user.Avatar;
                var ticket = new FormsAuthenticationTicket(2, user.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                    userData, FormsAuthentication.FormsCookiePath);

                var encTicket = FormsAuthentication.Encrypt(ticket);
                // Create the cookie.
                Response.Cookies.Add(new HttpCookie(".ASPXAUTHMEMBER", encTicket));
                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                    && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }

                if (Request.Browser.IsMobileDevice)
                {
                    return RedirectToAction("Account", "Dashboard");
                }

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
        public RedirectToRouteResult LogOut()
        {
            var cookie = Request.Cookies[".ASPXAUTHMEMBER"];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(cookie);
            }
            return RedirectToAction("Index", "Home");
        }

        [OverrideActionFilters]
        public JsonResult CheckPhoneNumber(string phoneNumber)
        {
            var user = _unitOfWork.UserRepository.GetQuery(a => a.PhoneNumber == phoneNumber).SingleOrDefault();

            if (user == null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

            return Json("Số điện thoại đã được sử dụng, vui lòng thử lại", JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}