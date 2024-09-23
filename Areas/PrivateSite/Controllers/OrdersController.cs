using ComputerDeviceShopping.Common;
using ComputerDeviceShopping.Models;
using ComputerDeviceShopping.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Net.WebSockets;

namespace ComputerDeviceShopping.Areas.PrivateSite.Controllers
{
    [Area("PrivateSite")]
    [CustomAuthentication]
    [CustomAuthorize("quản trị", "nhân viên")]

    public class OrdersController : Controller
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        private readonly IUserLoggedService _userLoggedService;
        public OrdersController(IUserLoggedService userLoggedService)
        {
            _userLoggedService = userLoggedService;
        }
        //public IActionResult Index(int page = 1, string name = "", string status = "")
        //{
        //    paginition(page, name, status);
        //    return View();
        //}
        public IActionResult Index()
        {
            ViewData["OrderStatus"] = _context.OrderStatuses.ToList();
            return View();
        }
        
        [HttpDelete]
        public IActionResult DeleteOrder(string id)
        {
            var order = _context.Orders.FirstOrDefault(d => d.OrderId.Equals(id));
            if (order != null)
            {

                return Json(new { success = true, message = "Xóa thành công đơn hàng" });
            }
            return Json(new { success = false, message = "Xóa thất bại đơn hàng" });
        }
        [Route("api/privatesite/order/loaddata/{page}&{name}&{status}")]
        public JsonResult LoadData(int page = 1, string name = "", int status = 0)
        {
            name = name.Trim();
            if (status == 0)
            {
                var orders1 = _context.Orders.OrderByDescending(d => d.StatusId == 3).Select(d => new
                {
                    d.OrderId,
                    StatusName = (status == 0 ? _context.OrderStatuses.First(p => p.OrderStatusId == d.StatusId).OrderStatusName : _context.OrderStatuses.First(p => p.OrderStatusId == status).OrderStatusName),
                    d.CustomerId,
                    d.CreateAt,
                    d.VoucherId,
                }).ToList();
                if (!name.Equals("none"))
                {
                    orders1 = _context.Orders.OrderByDescending(d=>d.StatusId == 3).Where(d => d.CustomerId.Contains(name) || d.OrderId.Contains(name)).Select(d => new
                    {
                        d.OrderId,
                        StatusName = (status == 0 ? _context.OrderStatuses.First(p => p.OrderStatusId == d.StatusId).OrderStatusName : _context.OrderStatuses.First(p => p.OrderStatusId == status).OrderStatusName),
                        d.CustomerId,
                        d.CreateAt,
                        d.VoucherId,
                    }).ToList();
                }
                var pagVM = CommonTools.Paginition(orders1, page, 10);
                ViewBag.Page = pagVM.page;
                ViewBag.NoOfPages = pagVM.noOfPages;
                ViewBag.DisplayPage = pagVM.displayPage;
                ViewData["Orders1"] = pagVM.data;
                return Json(new { data = pagVM.data, Page = pagVM.page, NoOfPages = pagVM.noOfPages, pagVM.displayPage });
            }
            else
            {
                var orders1 = _context.Orders.OrderByDescending(d => d.StatusId == 3).Where(d => d.StatusId == status).Select(d => new
                {
                    d.OrderId,
                    StatusName = (status == 0 ? _context.OrderStatuses.First(p => p.OrderStatusId == d.StatusId).OrderStatusName : _context.OrderStatuses.First(p => p.OrderStatusId == status).OrderStatusName),
                    d.CustomerId,
                    d.CreateAt,
                    d.VoucherId,
                }).ToList();
                if (!name.Equals("none"))
                {
                    orders1 = _context.Orders.OrderByDescending(d => d.StatusId == 3).Where(d => (d.CustomerId.Contains(name) || d.OrderId.Contains(name)) && d.StatusId == status).Select(d => new
                    {
                        d.OrderId,
                        StatusName = (status == 0 ? _context.OrderStatuses.First(p => p.OrderStatusId == d.StatusId).OrderStatusName : _context.OrderStatuses.First(p => p.OrderStatusId == status).OrderStatusName),
                        d.CustomerId,
                        d.CreateAt,
                        d.VoucherId,
                    }).ToList();
                }
                var pagVM = CommonTools.Paginition(orders1, page, 10);
                ViewBag.Page = pagVM.page;
                ViewBag.NoOfPages = pagVM.noOfPages;
                ViewBag.DisplayPage = pagVM.displayPage;
                ViewData["Orders1"] = pagVM.data;
                return Json(new { data = pagVM.data, Page = pagVM.page, NoOfPages = pagVM.noOfPages, pagVM.displayPage });
            }
        }
        [HttpPost]
        [Route("api/privatesite/order/changestatus/{id}&{status}")]
        public IActionResult ChangeStatus(string id, int status)
        {
            var order = _context.Orders.FirstOrDefault(d => d.OrderId.Equals(id));
            if (order != null)
            {
                order.StatusId = status;
                if (status == 5)
                {
                    var account = _context.Accounts.Where(d=>d.CustomerId.Equals(order.CustomerId)).FirstOrDefault();
                    if (account != null)
                    {
                        double sum = 0;
                        var orderList = _context.Orders.Where(d => d.CustomerId.Equals(account.CustomerId) && (d.CreateAt.HasValue && d.CreateAt.Value.Year == DateTime.Now.Year)).ToList();
                        sum = orderList.Sum(d => d.TotalPrice??0);

                        var memberLevelList = _context.MemberLevel.ToList();

                        List<int> idLevelList = new List<int>();    

                        for(int i = 1; i < memberLevelList.Count(); i++)
                        {
                            if(sum > memberLevelList[i].Limit)
                            {
                                idLevelList.Add(memberLevelList[i].MemberLevelId);
                            }
                        };
                        account.MemberLevelId = idLevelList[idLevelList.Count-1];
                    }
                }
                _context.SaveChanges();
                return Json(new { success = true, message = "Thành công" });
            }
            return Json(new { success = false, message = "Thất bại" });
        }
        public IActionResult Detail(string id)
        {
            var order = _context.Orders.Find(id);
            if (order != null)
            {
                ViewData["ProductOrdered"] = _context.OrdersDetails.Where(d => d.OrderId.Equals(order.OrderId)).ToList(); ;
                ViewData["ProductList"] = _context.Products.ToList();
                ViewData["CustomerOrder"] = _context.Customers.FirstOrDefault(d => d.CustomerId.Equals(order.CustomerId));
                if (order.VoucherId != null)
                {
                    var applyVoucher = _context.Vouchers.Where(d => d.VoucherId.Equals(order.VoucherId)).First();
                    ViewBag.DiscountVoucherOrder = applyVoucher.Discount;
                    ViewBag.CodeVoucherOrder = applyVoucher.VoucherCode;

                }
                return View(order);
            }
            return Redirect("Index");
        }
        //public IActionResult Detail(string id)
        //{
        //    var order = _context.Orders.Find(id);
        //    if (order != null)
        //    {
        //        return View(order);
        //    }
        //    return Redirect("Index");
        //}
        private void paginition(int page = 1, string name = "", string status = "")
        {
            List<Order> orders = _context.Orders.Where(d => String.IsNullOrEmpty(name) || d.OrderId.Equals(name)).ToList();
            ViewData["OrderStatus"] = _context.OrderStatuses.ToList();
            var pagVM = CommonTools.Paginition(orders, page, 5);
            ViewBag.Page = pagVM.page;
            ViewBag.NoOfPages = pagVM.noOfPages;
            ViewBag.DisplayPage = pagVM.displayPage;
            ViewData["Orders"] = pagVM.data;
        }
        private void FindStatusOrder(string name, int status)
        {
            List<Order> orders = _context.Orders.Where(d => String.IsNullOrEmpty(name) || d.OrderId.Equals(name) && d.StatusId == status).ToList();

        }
        private void UpdateInterface()
        {

        }
    }
}
