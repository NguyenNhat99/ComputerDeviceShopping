using ComputerDeviceShopping.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComputerDeviceShopping.ViewComponents
{
    public class SidebarBlogViewComponent : ViewComponent
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        public IViewComponentResult Invoke()
        {
            var articleTop = _context.Articles.OrderByDescending(d => d.ArticleView).Take(6).ToList();
            return View(articleTop);
        }
    }
}
