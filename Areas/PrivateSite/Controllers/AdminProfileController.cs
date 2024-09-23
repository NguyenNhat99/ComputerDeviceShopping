using ComputerDeviceShopping.Common;
using ComputerDeviceShopping.ViewModel;
using ComputerDeviceShopping.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ComputerDeviceShopping.Services;

namespace ComputerDeviceShopping.Areas.PrivateSite.Controllers
{
    [Area("PrivateSite")]
    [CustomAuthentication]
    [CustomAuthorize("quản trị","nhân viên")]
    public class AdminProfileController : Controller
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        private readonly IUserLoggedService _userLoggedService;
        public AdminProfileController(IUserLoggedService userLoggedService)
        {
            _userLoggedService = userLoggedService;
        }

        public IActionResult Index()
        {
            UpdateInterface(null);
            return View();
        }
        /// <summary>
        ///Hàm này dùng để đổi mật khẩu tài khoản quản trị
        /// </summary>
        /// <param name="id">id tài khoản cần đổi</param>
        /// <param name="currentPassword">mật khẩu hiện tại</param>
        /// <param name="newPassword">mật khẩu mới</param>
        /// <param name="confirmNewPassword">xác nhận mật khẩu mới</param>
        /// <returns>giao diện trang index</returns>
        [HttpPost]
        [Route("api/adminprofile/changepassword/{id}&{currentPassword}&{newPassword}")]
        public IActionResult ChangePassword(string id, string currentPassword, string newPassword)
        {
            string passhass = HassPass.HassPassSHA512(currentPassword);
            var account = _context.Accounts.FirstOrDefault(d => d.UserId.Equals(id) && d.PasswordHash.Equals(passhass));
            if (account != null)
            {
                account.PasswordHash = HassPass.HassPassSHA512(newPassword);
                _context.SaveChanges();
                return Json(new { success = true, message = "Đổi mật khẩu thành công" });
            }
            return Json(new { success = false, message = "Đổi mật khẩu thất bại" });
        }
        /// <summary>
        /// Hàm này dùng để thay đổi thông tin tài khoản
        /// </summary>
        /// <param name="acc">các thông tin, tài khoản cần thay đổi</param>
        /// <returns>View của index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeInformation(Account acc)
        {
            var account = _context.Accounts.Find(acc.UserId);
            if (account != null)
            {
                account.FirstName = acc.FirstName;
                account.LastName = acc.LastName;
                account.DeliverAddress = acc.DeliverAddress;
                account.Phone = acc.Phone;
                account.Gender = acc.Gender;    
                _context.SaveChanges();
                UpdateInterface(account);
            }
            return Redirect("Index");
        }
        /// <summary>
        /// Hàm này dùng để cập nhật lại giao diện. Nó cập như các thông tin của tài khoản ở trong session (phiên đăng nhập)
        /// </summary>
        /// <param name="acc">là tài khoản cần cập nhật lại giao diện</param>
        private void UpdateInterface(Account acc)
        {
            if (acc != null)
            {
                _userLoggedService.SetUserLogged(acc);
            }
            ViewData["AccountInformation"] = _userLoggedService.GetUserLogged(); 
            ViewBag.GroupName = _context.GroupAccounts.Where(d => d.GroupId.Equals(_userLoggedService.GetUserLogged().GroupId)).Select(d=>d.GroupName).FirstOrDefault();
        }
    }
}
