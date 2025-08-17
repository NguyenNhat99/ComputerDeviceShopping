using ComputerDeviceShopping.Common;
using ComputerDeviceShopping.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComputerDeviceShopping.Areas.PrivateSite.Controllers
{

    [Area("PrivateSite")]
    [CustomAuthentication]
    [CustomAuthorize("quản trị")]
    public class ManagementAccountController : Controller
    {
        private readonly ComputerDeviceDataContext _context;
        private readonly IEmailSender _emailSender;
        private static List<string> gmailList = new List<string>();
        public ManagementAccountController(ComputerDeviceDataContext context,IEmailSender emailSender)
        {
            _emailSender = emailSender;
            _context = context;
        }
        [HttpPost]
        [Route("api/managementaccount/sendnotification/{checkedAccounts}")]
        public async Task<IActionResult> SendNotifycation(string checkedAccounts)
        {
            var emailList = new List<string>();
            var checkedAccounts1 = checkedAccounts.Split(',');

            foreach (var idAccount in checkedAccounts1)
            {
                var account =await _context.Accounts.FindAsync(idAccount);
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
        public async Task<IActionResult> Index(int page = 1, string name = "")
        {
            await paginition(page, name);
            return View();
        }
        /// <summary>
        /// Hàm này dùng để khóa hoặc mở khóa tài khoản
        /// </summary>
        /// <param name="id">id tài khoản cần khóa hoặc mở khóa</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/managementaccount/active/{id}")]
        public async Task<IActionResult> Active(string id)
        {
            var account =await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                account.UserStatus = !account.UserStatus;
                await _context.SaveChangesAsync();
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
        public async Task<IActionResult> MultiActive(string idUserList)
        {
            string[] idList = idUserList.Split(',');
            if (idList.Length > 0) 
            {
                foreach(var id in idList)
                {
                    var account =await _context.Accounts.FindAsync(id);
                    if (account != null)
                        if (account.UserStatus == false)
                            account.UserStatus = true;
                }
                await _context.SaveChangesAsync();
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
        public async Task<IActionResult> MultiBlock(string idUserList)
        {
            string[] idList = idUserList.Split(',');
            if (idList.Length > 0)
            {
                foreach (var id in idList)
                {
                    var account =await _context.Accounts.FindAsync(id);
                    if (account != null)
                        if (account.UserStatus == true)
                            account.UserStatus = false;
                }
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Cập nhật thành công" });
            }
            return Json(new { success = false, message = "Cập nhật thất bại" });
        }
        [HttpPost]
        [Route("api/managementaccount/changepermisstion/{id}&{newPermisstion}")]
        public async Task<IActionResult> ChangePermisstion(string id, int newPermisstion) 
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                account.GroupId = newPermisstion;
                await _context.SaveChangesAsync();
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
            var account =await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                string randomPass = CommonTools.RandomCharacters(8);
                string newPassHash = HassPass.HassPassSHA512(randomPass);
                account.PasswordHash = newPassHash;
                await _emailSender.SendEmailAsync(account.Email, "Mật khẩu mới", "Mật khẩu của bạn là: " + randomPass);
                 await _context.SaveChangesAsync();
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
        [Route("api/managementaccount/delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(c => c.UserId.Equals(id));
            if (account != null)
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId.Equals(account.CustomerId));
                if (customer != null)
                {
                    var orders = await _context.Orders.Where(c => c.CustomerId.Equals(customer.CustomerId)).ToListAsync();
                    foreach (var order in orders)
                    {
                        var ordersDetail = await _context.OrdersDetails.Where(c => c.OrderId.Equals(order.OrderId)).ToListAsync();
                        _context.OrdersDetails.RemoveRange(ordersDetail);
                        _context.Orders.Remove(order);
                    }
                    _context.Customers.Remove(customer);
                }

                var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId.Equals(account.UserId));
                if (cart != null)
                {
                    var cartItems = await _context.CartItems.Where(c => c.CartId.Equals(cart.CartId)).ToListAsync();
                    _context.CartItems.RemoveRange(cartItems);
                    _context.Carts.Remove(cart);
                }

                var comments = await _context.Comments.Where(c => c.UserId.Equals(account.UserId)).ToListAsync();
                _context.Comments.RemoveRange(comments);

                var favouriteList = await _context.FavouriteLists.Where(c => c.UserId.Equals(account.UserId)).ToListAsync();
                _context.FavouriteLists.RemoveRange(favouriteList);

                var articles = await _context.Articles.Where(c => c.UserId.Equals(account.UserId)).ToListAsync();
                _context.Articles.RemoveRange(articles);

                var activateCodes = await _context.ActivateCodes.Where(c => c.UserId.Equals(account.UserId)).ToListAsync();
                _context.ActivateCodes.RemoveRange(activateCodes);

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
        private async Task paginition(int page = 1, string name = "")
        {
            List<Account> accounts = await _context.Accounts.Include(d => d.Group).Where(d => (String.IsNullOrEmpty(name) || d.Username.Contains(name) || d.UserId.Equals(name)) && !(d.Group.GroupName.Equals("quản trị"))).ToListAsync();
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
