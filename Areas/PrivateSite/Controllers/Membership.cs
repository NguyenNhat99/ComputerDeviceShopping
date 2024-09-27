using ComputerDeviceShopping.Common;
using ComputerDeviceShopping.Models;
using Microsoft.AspNetCore.Mvc;

namespace ComputerDeviceShopping.Areas.PrivateSite.Controllers
{
    [Area("PrivateSite")]
    [CustomAuthentication]
    [CustomAuthorize("quản trị")]
    public class Membership : Controller
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        public IActionResult Index(int page = 1)
        {
            paginition(page);
            return View();
        }
        [HttpPost]
        public IActionResult Insert(MemberLevel ml)
        {
            if(ml.MemberLevelId == 0)
            {
                var memberLevel = new MemberLevel()
                {
                    LevelDiscount = ml.LevelDiscount,
                    LevelName = ml.LevelName,   
                    Limit = ml.Limit,   
                    LevelDescription = ml.LevelDescription, 
                };
                _context.MemberLevel.Add(memberLevel);
                _context.SaveChanges(); 
            }
            else
            {
                var memberLevel = _context.MemberLevel.Find(ml.MemberLevelId);
                if (memberLevel != null) 
                {
                    memberLevel.LevelName = ml.LevelName;
                    memberLevel.Limit = ml.Limit;
                    memberLevel.LevelDescription = ml.LevelDescription; 
                    memberLevel.LevelDiscount = ml.LevelDiscount;
                    _context.SaveChanges();
                }
            }
            return Redirect("Index"); 
        }
        [HttpPost]
        //[Route("api/membership/update/${id}")]
        public IActionResult Update(int id)
        {
            var memberLevel = _context.MemberLevel.Find(id);
            if (memberLevel != null)
            {
                paginition(1);
                return View("Index", memberLevel);
            }
            return Redirect("Index");

        }
        private void paginition(int page)
        {
            var memberLevel = _context.MemberLevel.ToList();
            var pagVM = CommonTools.Paginition(memberLevel, page, 5);
            ViewBag.Page = pagVM.page;
            ViewBag.NoOfPages = pagVM.noOfPages;
            ViewBag.DisplayPage = pagVM.displayPage;
            ViewData["MemberLevelList"] = pagVM.data;
        }

    }
}
