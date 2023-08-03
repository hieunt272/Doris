using Doris.DAL;
using Doris.Models;
using Doris.ViewModel;
using PagedList;
using System;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
//using OfficeOpenXml;
//using OfficeOpenXml.Style;

namespace Doris.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        // GET: Order
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        public ActionResult ListOrder(int? page, int? cityId, string madonhang, string fromdate, string todate, string customerName, string customerEmail, string customerMobile, int status = 0)
        {
            var pageNumber = page ?? 1;
            var pageSize = 20;
            var orders = _unitOfWork.OrderRepository.GetQuery(orderBy: q => q.OrderByDescending(a => a.Id));

            if (!string.IsNullOrEmpty(madonhang))
            {
                orders = orders.Where(a => a.MaDonHang.Contains(madonhang));
            }
            if (!string.IsNullOrEmpty(customerName))
            {
                orders = orders.Where(a => a.CustomerInfo.Fullname.ToLower().Contains(customerName.ToLower()));
            }
            if (!string.IsNullOrEmpty(customerEmail))
            {
                orders = orders.Where(a => a.CustomerInfo.Email.ToLower().Contains(customerEmail.ToLower()));
            }
            if (!string.IsNullOrEmpty(customerMobile))
            {
                orders = orders.Where(a => a.CustomerInfo.Mobile.Contains(customerMobile));
            }
            if (cityId.HasValue)
            {
                orders = orders.Where(a => a.CityId == cityId);
            }
            if (DateTime.TryParse(fromdate, new CultureInfo("vi-VN"), DateTimeStyles.None, out var fd))
            {
                orders = orders.Where(a => DbFunctions.TruncateTime(a.CreateDate) >= DbFunctions.TruncateTime(fd));
            }
            if (DateTime.TryParse(todate, new CultureInfo("vi-VN"), DateTimeStyles.None, out var td))
            {
                orders = orders.Where(a => DbFunctions.TruncateTime(a.CreateDate) <= DbFunctions.TruncateTime(td));
            }
            if (status >= 1)
            {
                orders = orders.Where(a => a.Status == status);
            }

            var model = new ListOrderViewModel
            {
                Orders = orders.ToPagedList(pageNumber, pageSize),
                MaDonhang = madonhang,
                Status = status,
                CustomerName = customerName,
                CustomerEmail = customerEmail,
                CustomerMobile = customerMobile,
                FromDate = fromdate,
                ToDate = todate,
                PageSize = pageSize,
                CityId = cityId,
                CitySelectList = new SelectList(_unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort)), "Id", "Name")
            };

            return View(model);
        }
        public PartialViewResult LoadOrder(int orderId = 0)
        {
            var order = _unitOfWork.OrderRepository.GetById(orderId);

            var model = new OrderViewModel
            {
                Order = order
            };
            return PartialView(model);
        }

        public ActionResult ReportProduct(int? page, int? cityId, string fromDate, string toDate, int? status = 2)
        {
            var orderDetails = _unitOfWork.OrderDetailRepository.GetQuery();

            if (status.HasValue)
            {
                orderDetails = orderDetails.Where(a => a.Order.Status == status);
            }
            if (cityId.HasValue)
            {
                orderDetails = orderDetails.Where(a => a.Order.CityId == cityId);
            }

            var products = _unitOfWork.ProductRepository.GetQuery();


            // from date
            if (DateTime.TryParse(fromDate, new CultureInfo("vi-VN"), DateTimeStyles.None, out var fd))
            {
                orderDetails = orderDetails.Where(a =>
                    DbFunctions.TruncateTime(a.Order.CreateDate) >= DbFunctions.TruncateTime(fd));
            }
            // to date
            if (DateTime.TryParse(toDate, new CultureInfo("vi-VN"), DateTimeStyles.None, out var td))
            {
                orderDetails = orderDetails.Where(a => DbFunctions.TruncateTime(a.Order.CreateDate) <= DbFunctions.TruncateTime(td));
            }

            var groups = orderDetails.GroupBy(a => new { a.ProductId }).Select(a => new ReportProductViewModel.ReportProductItem
            {
                TotalSale = a.Sum(c => c.Quantity),
                Product = products.FirstOrDefault(s => s.Id == a.Key.ProductId),
            });

            var pageNumber = page ?? 1;

            var model = new ReportProductViewModel
            {
                CityId = cityId,
                CitySelectList = new SelectList(_unitOfWork.CityRepository.Get(a => a.Active, q => q.OrderBy(a => a.Sort)), "Id", "Name"),
                FromDate = fromDate,
                ToDate = toDate,
                Status = status,
                ReportProductItems = groups.OrderBy(a => a.Product.Sort).ToPagedList(pageNumber, 50)
            };

            return View(model);
        }

        [HttpPost]
        public bool UpdateOrder(int status = 0, int orderId = 0)
        {
            var order = _unitOfWork.OrderRepository.GetById(orderId);
            var user = _unitOfWork.UserRepository.GetById(order.UserId);
            decimal total = 0;
            if (order == null)
            {
                return false;
            }
            if (status > 0)
            {
                order.Status = status;
            }

            foreach (var item in user.Orders.Where(a => a.Status == 3))
            {
                total += item.Total();
            }
            user.TotalBuy = total;

            if (total >= 10000000 && total < 50000000)
            {
                user.Level = Level.Bronze;
            }
            else if (total >= 50000000 && total < 100000000)
            {
                user.Level = Level.Silver;
            }
            else if (total >= 100000000 && total < 150000000)
            {
                user.Level = Level.Gold;
            }
            else if (total >= 150000000 && total < 250000000)
            {
                user.Level = Level.Platinum;
            }
            else if (total >= 250000000)
            {
                user.Level = Level.Diamond;
            }
            else
            {
                user.Level = Level.Member;
            }

            _unitOfWork.Save();

            var userData = user.FullName + "|" + user.Level + "|" + user.ShopCode + "|" + user.Avatar;
            var ticket = new FormsAuthenticationTicket(2, user.Email.ToLower(), DateTime.Now, DateTime.Now.AddDays(30), true,
                userData, FormsAuthentication.FormsCookiePath);

            var encTicket = FormsAuthentication.Encrypt(ticket);
            // Create the cookie.
            Response.Cookies.Add(new HttpCookie(".ASPXAUTHMEMBER", encTicket));

            return true;
        }
        [HttpPost]
        public bool UpdateOrderNotice(string notice, int thanhtoantruoc = 0, int ship = 0, int orderId = 0)
        {
            var order = _unitOfWork.OrderRepository.GetById(orderId);
            if (order == null)
            {
                return false;
            }

            order.ShipFee = ship;
            order.ThanhToanTruoc = thanhtoantruoc;
            order.CustomerInfo.Body = notice;
            _unitOfWork.Save();
            return true;
        }
        [HttpPost]
        public bool DeleteOrder(int orderId = 0)
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
        [HttpPost]
        public bool ParmanentDeleteOrder(int orderId = 0)
        {
            var order = _unitOfWork.OrderRepository.GetById(orderId);
            if (order == null || order.Status != 4)
            {
                return false;
            }
            _unitOfWork.OrderRepository.Delete(order);
            _unitOfWork.Save();
            return true;

        }

        //public void ExportOrder(int? cityId, string fromdate, string todate, int status = -1, int payment = 0)
        //{
        //    var orders = _unitOfWork.OrderRepository.GetQuery(orderBy: q => q.OrderByDescending(a => a.Id));

        //    if (DateTime.TryParse(fromdate, new CultureInfo("vi-VN"), DateTimeStyles.None, out var fd))
        //    {
        //        orders = orders.Where(a => DbFunctions.TruncateTime(a.CreateDate) >= DbFunctions.TruncateTime(fd));
        //    }
        //    if (DateTime.TryParse(todate, new CultureInfo("vi-VN"), DateTimeStyles.None, out var td))
        //    {
        //        orders = orders.Where(a => DbFunctions.TruncateTime(a.CreateDate) <= DbFunctions.TruncateTime(td));
        //    }
        //    if (cityId.HasValue)
        //    {
        //        orders = orders.Where(a => a.CityId == cityId);
        //    }

        //    if (status >= 0)
        //    {
        //        orders = orders.Where(a => a.Status == status);
        //    }
        //    else
        //    {
        //        orders = orders.Where(a => a.Status != 3);
        //    }
        //    if (payment > 0)
        //    {
        //        switch (payment)
        //        {
        //            case 1:
        //                orders = orders.Where(a => !a.Payment);
        //                break;
        //            case 2:
        //                orders = orders.Where(a => a.Payment);
        //                break;
        //        }
        //    }

        //    var items = orders.Select(a => new
        //    {
        //        order = a,
        //        wardName = a.Ward.Name,
        //        districtName = a.District.Name,
        //        cityName = a.City.Name,
        //        //typeUser = a.User != null ? a.User.TypeUser.ToString() : "Vãng lai"
        //    });
        //    var dt = new DataTable();
        //    dt.Columns.Add("STT");
        //    dt.Columns.Add("Ngày đặt");
        //    dt.Columns.Add("Mã đơn");
        //    dt.Columns.Add("Phí Ship");
        //    dt.Columns.Add("Tổng tiền");
        //    dt.Columns.Add("Công nợ");
        //    dt.Columns.Add("Trạng thái");
        //    dt.Columns.Add("Họ và tên");
        //    dt.Columns.Add("Email");
        //    dt.Columns.Add("Địện thoại"); 
        //    dt.Columns.Add("Địa chỉ");
        //    dt.Columns.Add("Phường xã");
        //    dt.Columns.Add("Quận huyện");
        //    dt.Columns.Add("Thành phố");
        //    //dt.Columns.Add("Loại KH");

        //    var filename = $"danh-sach-don-hang.xlsx";
        //    var i = 1;
        //    foreach (var item in items)
        //    {
        //        dt.Rows.Add(i, item.order.CreateDate.ToString("dd/MM/yyyy HH:mm"), item.order.MaDonHang, item.order.ShipFee.ToString("N0"), item.order.TotalFee().ToString("N0"), item.order.TotalDebt().ToString("N0"), item.order.Status, item.order.CustomerInfo.Fullname, item.order.CustomerInfo.Email, item.order.CustomerInfo.Mobile, item.order.CustomerInfo.Address, item.wardName, item.districtName, item.cityName);
        //        i++;
        //    }
        //    using (var pck = new ExcelPackage())
        //    {
        //        //Create the worksheet
        //        var ws = pck.Workbook.Worksheets.Add("Danh sách đơn hàng");

        //        //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
        //        ws.Cells["A1"].LoadFromDataTable(dt, true);

        //        //Format the header for column 1-14
        //        using (var rng = ws.Cells["A1:O1"])
        //        {
        //            rng.Style.Font.Bold = true;
        //            rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
        //            rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
        //            rng.Style.Font.Color.SetColor(Color.White);
        //        }

        //        //Example how to Format Column 7 as numeric
        //        //using (var col = ws.Cells[2, 7, 2 + dt.Rows.Count, 7])
        //        //{
        //        //    col.Style.Numberformat.Format = "#,##0";
        //        //    col.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //        //}

        //        //Write it back to the client
        //        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        Response.AddHeader("content-disposition", "attachment;  filename=" + filename + "");
        //        Response.BinaryWrite(pck.GetAsByteArray());
        //    }
        //}

        //public void ExportReportProduct(int? cityId, string fromDate, string toDate, int? status = 2)
        //{
        //    var orderDetails = _unitOfWork.OrderDetailRepository.GetQuery();
        //    if (status.HasValue)
        //    {
        //        orderDetails = orderDetails.Where(a => a.Order.Status == status);
        //    }
        //    if (cityId.HasValue)
        //    {
        //        orderDetails = orderDetails.Where(a => a.Order.CityId == cityId);
        //    }

        //    var products = _unitOfWork.ProductRepository.GetQuery();

        //    if (DateTime.TryParse(fromDate, new CultureInfo("vi-VN"), DateTimeStyles.None, out var fd))
        //    {
        //        orderDetails = orderDetails.Where(a =>
        //            DbFunctions.TruncateTime(a.Order.CreateDate) >= DbFunctions.TruncateTime(fd));
        //    }
        //    if (DateTime.TryParse(toDate, new CultureInfo("vi-VN"), DateTimeStyles.None, out var td))
        //    {
        //        orderDetails = orderDetails.Where(a => DbFunctions.TruncateTime(a.Order.CreateDate) <= DbFunctions.TruncateTime(td));
        //    }

        //    var items = orderDetails.GroupBy(a => new { a.ProductId }).Select(a => new ReportProductViewModel.ReportProductItem
        //    {
        //        TotalSale = a.Sum(c => c.Quantity),
        //        Product = products.FirstOrDefault(s => s.Id == a.Key.ProductId)
        //    });

        //    var dt = new DataTable();
        //    dt.Columns.Add("STT");
        //    dt.Columns.Add("Mã sản phẩm");
        //    dt.Columns.Add("Tên sản phẩm");
        //    dt.Columns.Add("Số lượng");

        //    var filename = $"thong-ke-ban-hang.xlsx";
        //    var i = 1;
        //    foreach (var item in items) 
        //    {
        //        dt.Rows.Add(i, item.Product.Code, item.Product.Name, item.TotalSale);
        //        i++;
        //    }
        //    using (var pck = new ExcelPackage())
        //    {
        //        //Create the worksheet
        //        var ws = pck.Workbook.Worksheets.Add("Thống kê bán hàng");

        //        //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1
        //        ws.Cells["A1"].LoadFromDataTable(dt, true);

        //        //Format the header for column 1-4
        //        using (var rng = ws.Cells["A1:D1"])
        //        {
        //            rng.Style.Font.Bold = true;
        //            rng.Style.Fill.PatternType = ExcelFillStyle.Solid;                      //Set Pattern for the background to Solid
        //            rng.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));  //Set color to dark blue
        //            rng.Style.Font.Color.SetColor(Color.White);
        //        }

        //        //Example how to Format Column 7 as numeric
        //        //using (var col = ws.Cells[2, 7, 2 + dt.Rows.Count, 7])
        //        //{
        //        //    col.Style.Numberformat.Format = "#,##0";
        //        //    col.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
        //        //}

        //        //Write it back to the client
        //        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //        Response.AddHeader("content-disposition", "attachment;  filename=" + filename + "");
        //        Response.BinaryWrite(pck.GetAsByteArray());
        //    }
        //}

        protected override void Dispose(bool disposing)
        {
            _unitOfWork.Dispose();
            base.Dispose(disposing);
        }
    }
}