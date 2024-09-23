using ComputerDeviceShopping.Common;
using ComputerDeviceShopping.Models;
using ComputerDeviceShopping.Services;
using ComputerDeviceShopping.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit.Tnef;
using System.Net.WebSockets;

namespace ComputerDeviceShopping.Areas.PrivateSite.Controllers
{
    [Area("PrivateSite")]
    [CustomAuthentication]
    [CustomAuthorize("quản trị", "nhân viên")]

    public class ArticlesController : Controller
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        private static bool check = false;
        private readonly IUserLoggedService _userLoggedService;
        public ArticlesController(IUserLoggedService userLoggedService)
        {
            _userLoggedService = userLoggedService;
        }

        public IActionResult Index(int page = 1, string name = "")
        {
            check = false;
            paginition(page, name);
            return View();
        }
        public IActionResult Insert()
        {
            ViewBag.UserName = _userLoggedService.GetUserLogged().Username;
            return View();
        }
        /// <summary>
        /// Hàm này dùng để thêm hoặc cập nhật một bài viết
        /// </summary>
        /// <param name="avm">đây là một bài viết cần được thêm vào</param>
        /// <param name="avatar">Hình ảnh đại diện của bài viết</param>
        /// <returns>View của index</returns>
        [HttpPost]
        public async Task<IActionResult> Insert(Article ar, IFormFile avatar)
        {
            ViewBag.UserName = _userLoggedService.GetUserLogged().Username;
            if (ar.ArticleId == 0)
            {
                var insertArticle = new Article()
                {
                    ArticleName = ar.ArticleName,
                    ArticleContent = ar.ArticleContent,
                    ArticleStatus = true,
                    CreateAt = DateTime.Now,
                    UserId = _userLoggedService.GetUserLogged().UserId,
                    SummaryContent = ar.SummaryContent,
                    ArticleView = 0
                };
                insertArticle.Avatar = await CommonTools.SaveImage(avatar, "images", "articles");
                _context.Articles.Add(insertArticle);
            }
            else
            {
                var article = _context.Articles.Find(ar.ArticleId);
                article.ArticleContent = ar.ArticleContent;
                article.SummaryContent = ar.SummaryContent;
                article.ArticleName = ar.ArticleName;
                if (avatar != null)
                    article.Avatar = await CommonTools.SaveImage(avatar, "images", "articles");
            }
            _context.SaveChanges();
           if(check) return Redirect("ManagementArticles"); else return Redirect("Index");
        }
        public IActionResult Detail(int id)
        {
            var article = _context.Articles.Find(id);
            if (article != null)
            {
                if (!check && article.UserId.Equals(_userLoggedService.GetUserLogged().UserId))
                {
                    ViewBag.ArticleAvatar = article.Avatar;
                    ViewBag.UserName = _userLoggedService.GetUserLogged().Username;
                }
                else
                {
                    ViewBag.ArticleAvatar = article.Avatar;
                    ViewBag.UserName = _context.Accounts.FirstOrDefault(d => d.UserId.Equals(article.UserId)).Username;
                }
                return View(article);
            }
            if (check)
                return Redirect("ManagementArticles"); 
            else 
                return Redirect("Index");

        }
        /// <summary>
        /// Hàm này dùng để xóa bài viết
        /// </summary>
        /// <param name="id">id của bài viết cần xóa</param>
        /// <returns>Trả về json</returns>
        [HttpDelete]
        [Route("api/articles/delete/{id}")]
        public async Task<IActionResult> Delete(int id) 
        {
            var article = await _context.Articles.FindAsync(id);
            if (article != null)
            {
                _context.Articles.Remove(article);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Xóa thành công" });
            }
            return Json(new { success = false, message = "Xóa xóa thất bại" });
        }
        [HttpPost]
        [Route("api/articles/active/{id}")]
        public IActionResult Active(int id)
        {
            var article = _context.Articles.Find(id);
            if (article != null)
            {
                article.ArticleStatus = !article.ArticleStatus;
                _context.SaveChanges();
                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            return Json(new { success = false, message = "Cập nhật thất bại" });
        }
        private void paginition(int page = 1, string name = "")
        {
            List<Article> articles = new List<Article>();
            if (check)
            {
                articles = _context.Articles.Where(d => (String.IsNullOrEmpty(name) || d.ArticleName.Contains(name))).ToList();
            }
            else
            {
                articles = _context.Articles.Where(d => d.UserId.Equals(_userLoggedService.GetUserLogged().UserId) && (String.IsNullOrEmpty(name) || d.ArticleName.Contains(name))).ToList();
                ViewBag.UserName = _userLoggedService.GetUserLogged().Username;
            }
            var pagVM = CommonTools.Paginition(articles, page, 10);
            ViewBag.Page = pagVM.page;
            ViewBag.NoOfPages = pagVM.noOfPages;
            ViewBag.DisplayPage = pagVM.displayPage;
            ViewData["Articles"] = pagVM.data;
        }
        public IActionResult ManagementArticles(int page = 1, string name = "")
        {
            check = true;
            paginition(page, name);
            return View();
        }

    }
}
