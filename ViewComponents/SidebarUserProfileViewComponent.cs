using Microsoft.AspNetCore.Mvc;
using ComputerDeviceShopping.ViewModel;
using ComputerDeviceShopping.Models;
using ComputerDeviceShopping.Services;
namespace ComputerDeviceShopping.ViewComponents
{
    public class SidebarUserProfileViewComponent : ViewComponent
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        private readonly IUserLoggedService _userLoggedService;
        public SidebarUserProfileViewComponent(IUserLoggedService userLogged)
        {
            _userLoggedService = userLogged;
        }
        public IViewComponentResult Invoke()
        {
            var account = _userLoggedService.GetUserLogged();
            ViewBag.GroupAcc = _context.GroupAccounts.Where(d => d.GroupId.Equals(account.GroupId)).First().GroupName;
            return View(account);
        }
    }
}
