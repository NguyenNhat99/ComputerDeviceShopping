using ComputerDeviceShopping.Models;
using ComputerDeviceShopping.Services;
using ComputerDeviceShopping.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComputerDeviceShopping.ViewComponents
{
    public class HeaderTopViewComponent :ViewComponent
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        private readonly IUserLoggedService _userLoggedService;
        public HeaderTopViewComponent(IUserLoggedService userLoggedService)
        {
            _userLoggedService = userLoggedService;
        }
        public  IViewComponentResult Invoke()
        {
            ViewBag.EmailInformation = _context.Abouts.Select(p=>p.Email).First();
            return View(_userLoggedService.GetUserLogged());
        }
    }
}
