using ComputerDeviceShopping.Common;
using ComputerDeviceShopping.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace ComputerDeviceShopping.Controllers
{
    public class Blog : Controller
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        public IActionResult Index(int page = 1, string name = "")
        {
            paginition(page, name);
            return View();
        }
        public IActionResult Detail(int id)
        {
            var article = _context.Articles.Find(id);
            if (article != null)
            {
                var accountPost = _context.Accounts.FirstOrDefault(d => d.UserId.Equals(article.UserId));
                ViewBag.Writer = accountPost.LastName + accountPost.FirstName;
                article.ArticleView += 1;
                _context.SaveChanges();
                return View(article);
            }
            return Redirect("Index");
        }
        private void paginition(int page = 1, string name = "")
        {
            List<Article> articles = _context.Articles.Where(d => (String.IsNullOrEmpty(name) || d.ArticleName.Contains(name)) && d.ArticleStatus == true).ToList();
            var pagVM = CommonTools.Paginition(articles, page, 4);
            ViewBag.Page = pagVM.page;
            ViewBag.NoOfPages = pagVM.noOfPages;
            ViewBag.DisplayPage = pagVM.displayPage;
            ViewData["Articles"] = pagVM.data;
            ViewData["AccountPost"] = _context.Accounts.ToList();
        }
    }
}
