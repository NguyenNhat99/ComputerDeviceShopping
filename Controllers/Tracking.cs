using ComputerDeviceShopping.Models;
using Microsoft.AspNetCore.Mvc;

namespace ComputerDeviceShopping.Controllers
{
    public class Tracking : Controller
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        public IActionResult Index()
        {
            ViewBag.NotificationTracking = "";
            return View();
        }
        [HttpPost]
        public IActionResult Index(string id)
        {
            ViewBag.NotificationTracking = "";
            var order = _context.Orders.FirstOrDefault(d=>d.OrderId.Equals(id.Trim()) && d.StatusId != 4);
            if (order != null)
            {
                ViewBag.NotificationTracking = "";
                return View(order);
            }
            else
            {
                ViewBag.NotificationTracking = "Đơn hàng lỗi";
                return View();
            }
        }
    }
}
