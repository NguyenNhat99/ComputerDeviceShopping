using ComputerDeviceShopping.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComputerDeviceShopping.Areas.PrivateSite.Controllers
{

    [Area("PrivateSite")]
    [CustomAuthentication]
    [CustomAuthorize("quản trị", "nhân viên")]
    public class DashboardController : Controller
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            DateTime dateTime = DateTime.Now;
            ViewBag.OrderConfirm = await _context.Orders.Include(o => o.Status).Where(o => o.Status.OrderStatusName.Equals("Chờ xác nhận")).CountAsync();
            var revenueOfDay = await _context.Orders.Include(o => o.Status).Where(o =>o.Status.OrderStatusName.Equals("Thành công") && o.CreateAt.HasValue && o.CreateAt.Value.Month == dateTime.Month && o.CreateAt.Value.Day == dateTime.Day).ToListAsync();
            ViewBag.RevenueOfDay = revenueOfDay.Sum(o=> o.TotalPrice);
            TempData["Orders"] = await _context.Orders.OrderByDescending(o => o.CreateAt).Include(o => o.Customer).Include(o => o.Status).Take(10).ToListAsync(); ;
            return View();
        }
    }
}
