using ComputerDeviceShopping.Models;
using Microsoft.AspNetCore.Mvc;

namespace ComputerDeviceShopping.Areas.PrivateSite.Controllers
{
    [Area("PrivateSite")]
    [CustomAuthentication]
    [CustomAuthorize("quản trị")]
    public class Revenue : Controller
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        public IActionResult Index()
        {
            UpdateInterface();
            return View();
        }
        private void UpdateInterface()
        {
            List<Order> orders = _context.Orders.Where(d => d.StatusId == 5 && d.CreateAt.HasValue && d.CreateAt.Value.Year == DateTime.Now.Year).ToList();
            //Revenue Month
            ViewBag.numberOfOrderSuccessMonth = orders.Where(d=>d.CreateAt.HasValue && d.CreateAt.Value.Month == DateTime.Now.Month).ToList().Count();
            ViewBag.revenueMonth = orders.Where(d => d.CreateAt.HasValue && d.CreateAt.Value.Year == DateTime.Now.Year).ToList().Sum(d=>d.TotalPrice);
            //Revenue year
            ViewBag.numberOfOrderSuccessYear = orders.Count();
            ViewBag.revenueYear = orders.Sum(d=>d.TotalPrice);
            ViewData["CustomerList"] = _context.Customers.ToList();
            ViewData["OrderList"] = orders;
        }
        public IActionResult CustomerDetail(string id)
        {
            var customer = _context.Customers.FirstOrDefault(d => d.CustomerId.Equals(id));
            if (customer != null)
            {
                var order = _context.Orders.Where(d => d.CustomerId.Equals(customer.CustomerId)).OrderByDescending(d => d.CreateAt).ToList();
                ViewData["OrderList"] = order;
                ViewData["OrderDetailList"] = _context.OrdersDetails.ToList();
                ViewData["ProductList"] = _context.Products.ToList();
                ViewData["OrderStatusList"] = _context.OrderStatuses.ToList();
                ViewBag.FailedOrder = order.Where(d=>d.StatusId == 4).ToList().Count();
                ViewBag.SuccessOrder = order.Where(d=>d.StatusId==5).ToList().Count();
                ViewBag.PendingOrder = order.Where(d=>d.StatusId==3).ToList().Count();
                ViewBag.ShippingOrdered = order.Where(d=>d.StatusId == 2).ToList().Count();
                ViewBag.CancelOrder = order.Where(d=>d.StatusId == 6).ToList().Count();
                ViewBag.TotalPriceSuccessOrder = order.Where(d => d.StatusId == 5).ToList().Sum(d => d.TotalPrice);
                return View(customer);
            }
            return Redirect("Index");
        }
    }
}
