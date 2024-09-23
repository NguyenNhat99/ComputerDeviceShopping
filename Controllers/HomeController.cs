using ComputerDeviceShopping.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ComputerDeviceShopping.Controllers
{
    public class HomeController : Controller
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        public IActionResult Index()
        {
            UpdateInterface();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        private void UpdateInterface()
        {
            ViewData["Slider"] = _context.Sliders.ToList();
            ViewData["Articles"] = _context.Articles.OrderByDescending(d => d.CreateAt).Take(3).ToList();
            ViewData["AccountPost"] = _context.Accounts.ToList();
            ViewData["ProductList"] = _context.Products.OrderByDescending(d => d.CreateAt).Take(9).ToList();
        }
    }
}
