using Microsoft.AspNetCore.Mvc;

namespace ComputerDeviceShopping.Areas.PrivateSite.Controllers
{

    [Area("PrivateSite")]
    [CustomAuthentication]
    [CustomAuthorize("quản trị", "nhân viên")]


    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
