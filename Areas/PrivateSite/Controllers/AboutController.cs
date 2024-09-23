using ComputerDeviceShopping.Models;
using Microsoft.AspNetCore.Mvc;

namespace ComputerDeviceShopping.Areas.PrivateSite.Controllers
{
    [Area("PrivateSite")]
    [CustomAuthentication]
    [CustomAuthorize("quản trị")]
    public class AboutController : Controller
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        public IActionResult Index()
        {
            var about = _context.Abouts.FirstOrDefault();
            return View(about);  
        }
        [HttpPost]
        public IActionResult InsertAndUpdate(About about)
        {
            if (about != null)
            {
                var a = _context.Abouts.FirstOrDefault();
                a.YoutubeLink = about.YoutubeLink;  
                a.FacebookLink = about.FacebookLink;    
                a.TwitterLink = about.TwitterLink;
                a.YoutubeLink = about.YoutubeLink;
                a.Phone = about.Phone;
                a.Email = about.Email;
                a.UrlConnectMessager = about.UrlConnectMessager;
                a.AboutAddress = about.AboutAddress;    
                a.Introduce = about.Introduce;
                _context.SaveChanges();
            }
            return Redirect("Index");
        }
    }
}
