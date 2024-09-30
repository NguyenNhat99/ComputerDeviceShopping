using ComputerDeviceShopping.Common;
using ComputerDeviceShopping.Models;
using ComputerDeviceShopping.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComputerDeviceShopping.Controllers
{
    public class Shopping : Controller
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        private readonly IUserLoggedService _userLoggedService; 
        public Shopping(IUserLoggedService userLoggedService)
        {
            _userLoggedService = userLoggedService;
        }

        public async Task<IActionResult> Index(string name = "", int category = 0, int brand = 0, int page = 1)
        {
            await paginition(name, category, brand, page);
            UpdateInterface();
            return View();
        }
        /// <summary>
        /// Chi tiết sản phẩm
        /// </summary>
        /// <param name="id">id sản phẩm cần chi tiết</param>
        /// <returns></returns>
        public IActionResult Detail(string id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                ViewData["AccountSession"] = _userLoggedService.GetUserLogged();
                ViewData["ImageProduct"] = _context.Images.Where(d => d.ProductId.Equals(id)).ToList();
                ViewData["Product"] = product;
                ViewData["AccountList"] = _context.Accounts.ToList();
                ViewData["SpectificationProduct"] = _context.Specifications.Where(d => d.ProductId.Equals(product.ProductId)).ToList();
                ViewBag.CategoryProduct = _context.Categories.Where(d=>d.CategoryId==product.CategoryId).First().CategoryName;
                var comments = _context.Comments.Where(p => p.ProductId == id).ToList() ?? new List<Comment>();
                return View(comments);
            }
            return Redirect("Index");
        }
        /// <summary>
        /// Phân trang
        /// </summary>
        /// <param name="name"></param>
        /// <param name="category"></param>
        /// <param name="brand"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        private async Task paginition(string name = "", int category = 0, int brand = 0, int page = 1 )
        {
            var products = new List<Product>();
            if (category != 0)
            {
                if (brand != 0)
                {
                    products = await _context.Products.Where(d => (String.IsNullOrEmpty(name) || (d.ProductName ?? "").Contains(name)) && (d.BrandId == brand && d.CategoryId == category)).ToListAsync();

                }
                else
                {
                    products = await _context.Products.Where(d => (String.IsNullOrEmpty(name) || (d.ProductName ?? "").Contains(name)) && d.CategoryId == category).ToListAsync();
                }
            }
            else
            {
                if (brand != 0)
                {
                    products = await _context.Products.Where(d => (String.IsNullOrEmpty(name) || (d.ProductName ?? "").Contains(name)) && (d.BrandId == brand )).ToListAsync();

                }
                else
                {
                    products = await _context.Products.Where(d => String.IsNullOrEmpty(name) || (d.ProductName ?? "").Contains(name)).ToListAsync();
                }
            }
            int checkCategory = (category !=0 ? _context.Categories.First(d=>d.CategoryId==category).CategoryId : 0);
            int checkBrand = (brand != 0 ? _context.Brands.First(d => d.BrandId == brand).BrandId : 0);
         
            var pagVM = CommonTools.Paginition(products, page, 9);
            ViewBag.Page = pagVM.page;
            ViewBag.NoOfPages = pagVM.noOfPages;
            ViewBag.DisplayPage = pagVM.displayPage;
            ViewData["Products"] = pagVM.data;
        }
        /// <summary>
        /// Cập nhật lại giao diện
        /// </summary>
        private void UpdateInterface()
        {
            ViewData["Brands"] = _context.Brands.ToList();
            ViewData["Categories"] = _context.Categories.ToList();

        }
        [HttpPost]
        [Route("api/comments/replyComment/{id}")]
        public IActionResult ReplyComment(int id)
        {
            var comment = _context.Comments.FirstOrDefault(c=>c.CommentId == id);
            if (comment!=null)
            {
                var account = _context.Accounts.FirstOrDefault(c => c.UserId.Equals(comment.UserId));
                return Json(new { Success = true, message = "Thành công", name = account.FirstName });
            }
            return Json(new { Success = false, message = "Thất bại" });
        }
    }
}
