using ComputerDeviceShopping.Common;
using ComputerDeviceShopping.Models;
using Microsoft.AspNetCore.Mvc;

namespace ComputerDeviceShopping.Areas.PrivateSite.Controllers
{

    [Area("PrivateSite")]
    //[CustomAuthentication]
    //[CustomAuthorize("quản trị")]


    public class ManagementAccountController : Controller
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        private static IEmailSender _emailSender;
        private static List<string> gmailList = new List<string>();
        public ManagementAccountController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }
        [HttpPost]
        [Route("api/managementaccount/sendnotification/{checkedAccounts}")]
        public IActionResult SendNotifycation(string checkedAccounts)
        {
            var emailList = new List<string>();
            var checkedAccounts1 = checkedAccounts.Split(',');

            foreach (var idAccount in checkedAccounts1)
            {
                var account = _context.Accounts.Find(idAccount);
                if (account != null)
                    emailList.Add(account.Email);
            }
            if (emailList.Count > 0)
            {
                gmailList = emailList;
                return Json(new { success = true, data = emailList });
            }
            return Json(new { success = false });
        }
        [HttpPost]
        public async Task<IActionResult> SendMail(string subject, string content)
        {
            try
            {
                foreach (var mail in gmailList)
                {
                    await _emailSender.SendEmailAsync(mail, subject, content);
                }
            }
            catch (Exception ex) { }
            return RedirectToAction("Index", "ManagementAccount");
        }
        public IActionResult Index(int page = 1, string name = "")
        {
            paginition(page, name);
            return View();
        }
        /// <summary>
        /// Hàm này dùng để khóa hoặc mở khóa tài khoản
        /// </summary>
        /// <param name="id">id tài khoản cần khóa hoặc mở khóa</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/managementaccount/active/{id}")]
        public IActionResult Active(string id)
        {
            var account = _context.Accounts.Find(id);
            if (account != null)
            {
                account.UserStatus = !account.UserStatus;
                _context.SaveChanges();
                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            return Json(new { success = false, message = "Cập nhật thất bại" });
        }
        /// <summary>
        /// Hàm này dùng để mở khóa nhiều tài khoản cùng lúc
        /// </summary>
        /// <param name="idUserList">danh sách id tài khoản cần mở khóa</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/managementaccount/multiactive/{idUserList}")]
        public IActionResult MultiActive(string idUserList)
        {
            string[] idList = idUserList.Split(',');
            if (idList.Length > 0) 
            {
                foreach(var id in idList)
                {
                    var account = _context.Accounts.Find(id);
                    if (account != null)
                        if (account.UserStatus == false)
                            account.UserStatus = true;
                }
                _context.SaveChanges(); 
                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            return Json(new { success = false, message = "Cập nhật thất bại" });
        }
        /// <summary>
        /// Hàm này dùng để khóa nhiều tài khoản cùng lúc
        /// </summary>
        /// <param name="idUserList">danh sách id tài khoản cần khóa cùng lúc</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/managementaccount/multiblock/{idUserList}")]
        public IActionResult MultiBlock(string idUserList)
        {
            string[] idList = idUserList.Split(',');
            if (idList.Length > 0)
            {
                foreach (var id in idList)
                {
                    var account = _context.Accounts.Find(id);
                    if (account != null)
                        if (account.UserStatus == true)
                            account.UserStatus = false;
                }
                _context.SaveChanges();
                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            return Json(new { success = false, message = "Cập nhật thất bại" });
        }
        [HttpPost]
        [Route("api/managementaccount/changepermisstion/{id}&{newPermisstion}")]
        public IActionResult ChangePermisstion(string id, int newPermisstion) 
        {
            var account = _context.Accounts.Find(id);
            if (account != null)
            {
                account.GroupId = newPermisstion;
                _context.SaveChanges();
                return Json(new { success = true, message = "Cập nhật thành công" });
            }
          
            return Json(new { success = false, message = "Cập nhật thất bại" });
        }
        /// <summary>
        /// Hàm này dùng để xem chi tiết tài khoản
        /// </summary>
        /// <param name="id">id tài khoản cần xem chi tiết</param>
        /// <returns></returns>
        public IActionResult Detail(string id)
        {
            var account = _context.Accounts.Find(id);
            if (account != null)
            {
                ViewBag.GroupName = _context.GroupAccounts.Where(d => d.GroupId.Equals(account.GroupId)).Select(d => d.GroupName).FirstOrDefault();
                return View(account);
            }
            return NotFound();

        }
        [HttpPost]
        [Route("api/managementaccount/resetpassword/{id}")]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var account = _context.Accounts.Find(id);
            if (account != null)
            {
                string randomPass = CommonTools.RandomCharacters(8);
                string newPassHash = HassPass.HassPassSHA512(randomPass);
                account.PasswordHash = newPassHash;
                await _emailSender.SendEmailAsync(account.Email, "Mật khẩu mới", "Mật khẩu của bạn là: " + randomPass);
                _context.SaveChanges();
                return Json(new { success = true, message = "Reset mật khẩu thành công" });
            }
            return Json(new { success = false, message = "Reset mật khẩu thất bại" });
        }
        /// <summary>
        /// Hàm này dùng để xóa một tài khoản
        /// </summary>
        /// <param name="id">id tài khoản cần xóa </param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/accounts/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Xóa thành công" });
            }
            return Json(new { success = false, message = "Xóa thất bại" });
        }
        /// <summary>
        /// Hàm này dùng để phân trang và tìm kiếm
        /// </summary>
        /// <param name="page">trang</param>
        /// <param name="name">tìm kiếm bằng mã voucher</param>
        private void paginition(int page = 1, string name = "")
        {
            List<Account> accounts = _context.Accounts.Where(d => (String.IsNullOrEmpty(name) || d.Username.Contains(name) || d.UserId.Equals(name))).ToList();
            var pagVM = CommonTools.Paginition(accounts, page, 10);
            ViewBag.Page = pagVM.page;
            ViewBag.NoOfPages = pagVM.noOfPages;
            ViewBag.DisplayPage = pagVM.displayPage;
            ViewData["Accounts"] = pagVM.data;
        }
        /// <summary>
        /// Hàm này thể hiện thông tin của phiên đang đăng nhập
        /// </summary>
        /// <returns>thông tin của tài khoản trong phiên</returns>
        private Account UserLogged()
        {
            var userSession = HttpContext.Session.GetString("UserLogged");
            var account = Newtonsoft.Json.JsonConvert.DeserializeObject<Account>(userSession);
            return account;
        }

    }
}
