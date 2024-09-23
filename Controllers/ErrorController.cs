using Microsoft.AspNetCore.Mvc;

namespace ComputerDeviceShopping.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult DenielAccess()
        {
            return View();
        }
    }
}
