using Helpers;
using Doris.Filters;
using Doris.Models;
using Doris.ViewModel;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;
using System.Globalization;
using PagedList;
using ImageResizer.Plugins.Basic;
using Microsoft.Ajax.Utilities;
using System.Text.RegularExpressions;
using System.Text;

namespace Doris.Controllers
{
    [MemberLoginFilter, RoutePrefix("dashboard")]
    public class DashboardController : BaseController
    {
        private string MemberEmail => RouteData.Values["MemberEmail"].ToString();
        private new User User => _unitOfWork.UserRepository.GetQuery(a => a.Active && a.Email == MemberEmail).SingleOrDefault();

        [Route("tai-khoan-cua-toi")]
        public ActionResult Account(string result = "")
        {
            ViewBag.Result = result;

            var nextLevel = (int)User.Level + 1;
            var level = (Level)nextLevel;
            decimal totalLevelUp = 0;
            int percent = 0;

            percent = Convert.ToInt32(User.TotalBuy / 250000000 * 100);

            switch (level)
            {
                case Level.Bronze:
                    totalLevelUp = 10000000 - Convert.ToDecimal(User.TotalBuy);
                    break;
                case Level.Silver:
                    totalLevelUp = 50000000 - Convert.ToDecimal(User.TotalBuy);
                    break;
                case Level.Gold:
                    totalLevelUp = 100000000 - Convert.ToDecimal(User.TotalBuy);
                    break;
                case Level.Platinum:
                    totalLevelUp = 150000000 - Convert.ToDecimal(User.TotalBuy);
                    break;
                case Level.Diamond:
                    totalLevelUp = 250000000 - Convert.ToDecimal(User.TotalBuy);
                    break;
            }

            var model = new ProfileViewModel
            {
                Banners = _unitOfWork.BannerRepository.GetQuery(b => b.Active, o => o.OrderBy(b => b.Sort)),
                Username = User.Username,
                FullName = User.FullName,
                Email = User.Email,
                PhoneNumber = User.PhoneNumber,
                Gender = User.Gender,
                Day = User.BirthDay.Day,
                Month = User.BirthDay.Month,
                Year = User.BirthDay.Year,
                Avatar = User.Avatar,
                UserId = User.Id,
                PercentOrder = percent,
                Level = level,
                TotalLevelUp = totalLevelUp,
                Addresses = _unitOfWork.AddressRepository.GetQuery(a => a.UserId == User.Id, o => o.OrderBy(a => a.Id)),
                BankUsers = _unitOfWork.BankUserRepository.GetQuery(a => a.UserId == User.Id, o =>o.OrderBy(a => a.Sort))
            };
            return View(model);
        }
        [ChildActionOnly]
        public PartialViewResult UserNav()
        {
            var user = _unitOfWork.UserRepository.GetQuery(a => a.Id == User.Id).FirstOrDefault();
            return PartialView(user);
        }
        #region UserInfo
        [Route("ho-so-ca-nhan")]
        public ActionResult MyProfile(string result = "")
        {
            ViewBag.Result = result;
            var user = _unitOfWork.UserRepository.GetQuery(a => a.Id == User.Id).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new ProfileViewModel
            {
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Gender = user.Gender,
                Day = user.BirthDay.Day,
                Month = user.BirthDay.Month,
                Year = user.BirthDay.Year,
                Avatar = user.Avatar,
                UserId = user.Id
            };
            return View(model);
        }
        [Route("ho-so-ca-nhan")]
        [HttpPost, ValidateInput(false)]
        public ActionResult MyProfile(ProfileViewModel model, FormCollection fc)
        {
            var user = _unitOfWork.UserRepository.Get(a => a.Id == model.UserId).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                var file = Request.Files["Avatar"];
                if (file != null && file.ContentLength > 0)
                {
                    if (!HtmlHelpers.CheckFileExt(file.FileName, "jpg|jpeg|png"))
                    {
                        ModelState.AddModelError("", @"Chỉ chấp nhận định dạng jpg, png, jpeg");
                    }
                    else
                    {
                        if (file.ContentLength > 1000 * 1024)
                        {
                            ModelState.AddModelError("", @"Dung lượng lớn hơn 1MB. Hãy thử lại");
                        }
                        else
                        {
                            var imgPath = "/images/users/" + DateTime.Now.ToString("yyyy/MM/dd");
                            HtmlHelpers.CreateFolder(Server.MapPath(imgPath));

                            var imgFileName = HtmlHelpers.ConvertToUnSign(null, Path.GetFileNameWithoutExtension(file.FileName)) +
                                "-" + DateTime.Now.Millisecond + Path.GetExtension(file.FileName);

                            user.Avatar = DateTime.Now.ToString("yyyy/MM/dd") + "/" + imgFileName;
                            file.SaveAs(Server.MapPath(Path.Combine(imgPath, imgFileName)));
                        }
                    }
                }

                var birthday = model.Day + "/" + model.Month + "/" + model.Year;
                user.BirthDay = Convert.ToDateTime(birthday);
                user.FullName = model.FullName;
                user.Email = model.Email;
                user.PhoneNumber = model.PhoneNumber;
                user.Gender = model.Gender;
                _unitOfWork.Save();

                var userData = user.Username + "|" + user.Avatar + "|" + user.Id + "|" + user.Email;
                var ticket = new FormsAuthenticationTicket(2, user.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                    userData, FormsAuthentication.FormsCookiePath);

                var encTicket = FormsAuthentication.Encrypt(ticket);
                // Create the cookie.
                Response.Cookies.Add(new HttpCookie(".ASPXAUTHMEMBER", encTicket));

                if (Request.Browser.IsMobileDevice)
                {
                    return RedirectToAction("Account", "Dashboard", new { result = "success" });
                }

                return RedirectToAction("MyProfile", "Dashboard", new { result = "success" });
            }
            return View(model);
        }
        [Route("thong-tin-ca-nhan")]
        public ActionResult UserInfo()
        {
            var user = _unitOfWork.UserRepository.GetQuery(a => a.Id == User.Id).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var nextLevel = (int)user.Level + 1;
            var level = (Level)nextLevel;
            decimal totalMonth = 0, totalLevelUp = 0;
            foreach (var order in user.Orders.Where(a => a.Status == 3 && a.CreateDate.Month == DateTime.Now.AddMonths(-1).Month))
            {
                totalMonth += order.Total();
            }

            switch (level)
            {
                case Level.Bronze:
                    totalLevelUp = 10000000 - Convert.ToDecimal(user.TotalBuy);
                    break;
                case Level.Silver:
                    totalLevelUp = 50000000 - Convert.ToDecimal(user.TotalBuy);
                    break;
                case Level.Gold:
                    totalLevelUp = 100000000 - Convert.ToDecimal(user.TotalBuy);
                    break;
                case Level.Platinum:
                    totalLevelUp = 150000000 - Convert.ToDecimal(user.TotalBuy);
                    break;
                case Level.Diamond:
                    totalLevelUp = 250000000 - Convert.ToDecimal(user.TotalBuy);
                    break;
            }

            var model = new UserInfoViewModel
            {
                User = user,
                Level = level,
                TotalMonth = totalMonth,
                TotalLevelUp = totalLevelUp
            };
            return View(model);
        }
        [Route("doi-mat-khau")]
        public ActionResult UserChangePassword(string result = "")
        {
            ViewBag.Result = result;

            var model = new ChangePasswordModel
            {
                UserId = User.Id,
            };
            return View(model);
        }
        [Route("doi-mat-khau")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UserChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _unitOfWork.UserRepository.GetById(model.UserId);
                if (user == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (HtmlHelpers.VerifyHash(model.OldPassword, "SHA256", user.Password))
                {
                    user.Password = HtmlHelpers.ComputeHash(model.Password, "SHA256", null);
                    _unitOfWork.Save();
                    return RedirectToAction("UserChangePassword", new { result = "success" });
                }
                else
                {
                    ModelState.AddModelError("", @"Mật khẩu hiện tại không đúng!");
                    model.UserId = user.Id;
                    return View(model);
                }
            }
            return View(model);
        }
        #endregion

        #region UserOrder
        [Route("lich-su-dat-hang")]
        public ActionResult OrderHistory(int? page, int status = 1)
        {
            var pageNumber = page ?? 1;
            var pageSize = 5;
            var orders = _unitOfWork.OrderRepository.GetQuery(a => a.UserId == User.Id, o => o.OrderBy(a => a.Status));
            var model = new OrderHistoryViewModel
            {
                Status = status,
                Orders = orders.ToPagedList(pageNumber, pageSize)
            };
            return View(model);
        }
        public PartialViewResult GetUserOrder(int? page, int status = 0)
        {
            var pageNumber = page ?? 1;
            var pageSize = 5;
            var orders = _unitOfWork.OrderRepository.GetQuery(a => a.UserId == User.Id, o => o.OrderBy(a => a.Status));

            switch (status)
            {
                case 1:
                    orders = orders.Where(a => a.Status == status);
                    break;
                case 2:
                    orders = orders.Where(a => a.Status == status);
                    break;
                case 3:
                    orders = orders.Where(a => a.Status == status);
                    break;
            }

            var model = new OrderHistoryViewModel
            {
                Status = status,
                Orders = orders.ToPagedList(pageNumber, pageSize)
            };

            return PartialView(model);
        }
        [HttpPost]
        public bool CancelOrder(int orderId = 0)
        {
            var order = _unitOfWork.OrderRepository.GetById(orderId);
            if (order == null)
            {
                return false;
            }

            order.Status = 4;
            _unitOfWork.Save();
            return true;
        }
        #endregion

        #region UserAddress
        [Route("so-dia-chi")]
        public ActionResult Address(string result = "")
        {
            ViewBag.Result = result;
            var addresses = _unitOfWork.AddressRepository.GetQuery(a => a.UserId == User.Id, o => o.OrderBy(a => a.Id));

            var model = new AddressViewModel
            {
                Addresses = addresses,
                CitySelectList = CitySelectList,
                Address = new Address { UserId = User.Id },
            };
            return View(model);
        }
        [Route("so-dia-chi")]
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Address(AddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Address.CityId = model.CityId;
                model.Address.DistrictId = model.DistrictId;
                model.Address.WardId = model.WardId;
                _unitOfWork.AddressRepository.Insert(model.Address);
                _unitOfWork.Save();

                var count = _unitOfWork.AddressRepository.GetQuery(a => a.UserId == User.Id).Count();
                if (count > 1)
                {
                    model.Address.Default = false;
                }
                _unitOfWork.Save();
                return RedirectToAction("Address", new { result = "success" });
            }
            return View(model);
        }
        [Route("cap-nhat-dia-chi/{addressId}")]
        public ActionResult UpdateAddress(int addressId = 0)
        {
            var address = _unitOfWork.AddressRepository.GetById(addressId);
            if (address == null)
            {
                return RedirectToAction("UserAddress");
            }
            var model = new AddressViewModel
            {
                Address = address,
                CitySelectList = CitySelectList,
                DistrictSelectList = DistrictSelectList(address.CityId),
                WardSelectList = WardSelectList(address.DistrictId),
                CityId = address.CityId,
                DistrictId = address.DistrictId,
                WardId = address.WardId,
            };
            return View(model);
        }
        [Route("cap-nhat-dia-chi/{addressId}")]
        [HttpPost]
        public ActionResult UpdateAddress(AddressViewModel model)
        {
            var address = _unitOfWork.AddressRepository.GetById(model.Address.Id);
            if (address == null)
            {
                return RedirectToAction("Address");
            }
            if (ModelState.IsValid)
            {
                address.Fullname = model.Address.Fullname;
                address.Mobile = model.Address.Mobile;
                address.CityId = model.CityId;
                address.DistrictId = model.DistrictId;
                address.WardId = model.WardId;
                address.SpecificAddress = model.Address.SpecificAddress;

                _unitOfWork.Save();
                return RedirectToAction("Address", new { result = "update" });
            }
            model.CitySelectList = CitySelectList;
            return View(model);
        }
        public bool DefaultAddress(bool addressDefault = false, int addressId = 0)
        {
            var address = _unitOfWork.AddressRepository.GetById(addressId);
            if (address == null)
            {
                return false;
            }
            var addresses = _unitOfWork.AddressRepository.GetQuery(a => a.UserId == User.Id && a.Id != address.Id);
            foreach (var item in addresses)
            {
                item.Default = false;
            }
            address.Default = addressDefault;

            _unitOfWork.Save();
            return true;
        }
        [HttpPost]
        public bool DeleteAddress(int addressId = 0)
        {
            var address = _unitOfWork.AddressRepository.GetById(addressId);
            var fa = _unitOfWork.AddressRepository.GetQuery(a => a.Id != address.Id && a.UserId == User.Id).FirstOrDefault();
            if (address == null)
            {
                return false;
            }
            if (address.Default == true)
            {
                if (fa != null)
                {
                    fa.Default = true;
                }
            }

            _unitOfWork.AddressRepository.Delete(address);
            _unitOfWork.Save();
            return true;
        }
        #endregion

        public PartialViewResult AddOrUpdateAddressInfo(int? addressId)
        {
            var model = new AddressViewModel
            {
                CitySelectList = CitySelectList,
                Address = new Address { UserId = User.Id },
            };
            if (addressId.HasValue)
            {
                model.Address = _unitOfWork.AddressRepository.GetQuery(a => a.Id == addressId && a.UserId == User.Id).FirstOrDefault();
                model.CityId = model.Address.CityId;
                model.DistrictId = model.Address.DistrictId;
                model.WardId = model.Address.WardId;
                model.DistrictSelectList = DistrictSelectList(model.CityId);
                model.WardSelectList = WardSelectList(model.DistrictId);
            }
            return PartialView(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult AddOrUpdateAddressInfo(AddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Address.CityId = model.CityId;
                model.Address.DistrictId = model.DistrictId;
                model.Address.WardId = model.WardId;
                model.Address.UserId = User.Id;

                var count = _unitOfWork.AddressRepository.GetQuery(a => a.UserId == User.Id && a.Default && a.Id != model.Address.Id).Count();

                if (count > 0)
                {
                    model.Address.Default = false;
                }

                _unitOfWork.AddressRepository.AddOrUpdate(model.Address);
                _unitOfWork.Save();
                return Json(new { status = 0, msg = "Cập nhật thành công" });
            }
            return Json(new { status = 1, msg = "Quá trình thực hiện không thành công." });
        }

        public PartialViewResult AddOrUpdateBankInfo(int? bankId)
        {
            var model = new BankInfoViewModel
            {
                BankUser = new BankUser { Sort = 1 },
                BankSelectList = new SelectList(_unitOfWork.BankRepository.Get(a => a.Active, o => o.OrderBy(a => a.Sort)), "Id", "Name")
            };
            if (bankId.HasValue)
            {
                model.BankUser = _unitOfWork.BankUserRepository.GetQuery(a => a.BankId == bankId && a.UserId == User.Id).FirstOrDefault();
                model.BankId = model.BankUser.BankId;
            }
            return PartialView(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult AddOrUpdateBankInfo(BankInfoViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.BankUser.BankId = model.BankId;
                model.BankUser.UserId = User.Id;

                Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
                string temp = model.BankUser.AccountName.Normalize(NormalizationForm.FormD);
                model.BankUser.AccountName = regex.Replace(temp, String.Empty).Replace('\u0111', 'd').Replace('\u0110', 'D').ToUpper();

                var count = _unitOfWork.BankUserRepository.GetQuery(a => a.UserId == User.Id && a.Default && a.BankId != model.BankUser.BankId).Count();

                if (count > 0)
                {
                    model.BankUser.Default = false;
                }

                _unitOfWork.BankUserRepository.AddOrUpdate(model.BankUser);
                _unitOfWork.Save();
                return Json(new { status = 0, msg = "Cập nhật thành công" });
            }
            return Json(new { status = 1, msg = "Quá trình thực hiện không thành công." });
        }
        public bool DefaultBankInfo(bool bankDefault = false, int bankId = 0)
        {
            var bankUser = _unitOfWork.BankUserRepository.GetQuery(a => a.BankId == bankId && a.UserId == User.Id).FirstOrDefault();
            if (bankUser == null)
            {
                return false;
            }
            var bankUsers = _unitOfWork.BankUserRepository.GetQuery(a => a.UserId == User.Id && a.BankId != bankUser.BankId);
            foreach (var item in bankUsers)
            {
                item.Default = false;
            }
            bankUser.Default = bankDefault;

            _unitOfWork.Save();
            return true;
        }
        [HttpPost]
        public bool DeleteBankInfo(int bankId = 0)
        {
            var bankUser = _unitOfWork.BankUserRepository.GetQuery(a => a.BankId == bankId && a.UserId == User.Id).FirstOrDefault();
            var fb = _unitOfWork.BankUserRepository.GetQuery(a => a.BankId != bankUser.BankId && a.UserId == User.Id).FirstOrDefault();
            if (bankUser == null)
            {
                return false;
            }
            if (bankUser.Default == true)
            {
                if (fb != null)
                {
                    fb.Default = true;
                }
            }

            _unitOfWork.BankUserRepository.Delete(bankUser);
            _unitOfWork.Save();
            return true;
        }

        [Route("cap-bac-thanh-vien")]
        public PartialViewResult Membership()
        {
            var nextLevel = (int)User.Level + 1;
            var level = (Level)nextLevel;
            int percent = 0;

            percent = Convert.ToInt32(User.TotalBuy / 250000000 * 100);

            var model = new ProfileViewModel
            {
                Level = level,
                PercentOrder = percent,
            };
            return PartialView(model);
        }

        [Route("danh-muc-cua-ban")]
        public ActionResult UserCategory()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}