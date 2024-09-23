using ComputerDeviceShopping.Models;
using ComputerDeviceShopping.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace ComputerDeviceShopping.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        public IViewComponentResult Invoke()
        {
            About a = _context.Abouts.First();
            return View(a);
        }
    }
}
