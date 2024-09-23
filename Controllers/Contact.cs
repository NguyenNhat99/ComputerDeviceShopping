using ComputerDeviceShopping.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace ComputerDeviceShopping.Controllers
{
    public class Contact : Controller
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        private readonly IEmailSender _emailSender;
        public Contact(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(string email, string content, string name)
        {
            ViewBag.Notify = "";
            if (email.Length > 0 && content.Length > 0 && name.Length > 0)
            {
                string mail = email.Trim();
                await _emailSender.SendEmailAsync("techgear.hotro@gmail.com", "Yêu cầu hỗ trợ, email: " + mail, "Họ tên: " + name + ", vấn đề: " + content);
                ViewBag.Notify = "Chúng tôi sẽ phản hồi sớm nhất. Cảm ơn bạn đã liên hệ.";
                return View();
            }
            else
            {
                ViewBag.Notify = "Gửi thất bại.";
            }
            return Redirect("Index");
        }

    }
}
