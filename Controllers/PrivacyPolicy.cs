using ComputerDeviceShopping.Models;
using Microsoft.AspNetCore.Mvc;

namespace ComputerDeviceShopping.Controllers
{
    public class PrivacyPolicy : Controller
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        public IActionResult Index()
        {
            return View();
        }
    }
}
