using ComputerDeviceShopping.Common;
using ComputerDeviceShopping.ViewModel;
using ComputerDeviceShopping.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ComputerDeviceShopping.Services;
using ComputerDeviceShopping.DTOs.Accounts;
using Microsoft.EntityFrameworkCore;
using System.Xml;
using System.Net.WebSockets;

namespace ComputerDeviceShopping.Areas.PrivateSite.Controllers
{
    [Area("PrivateSite")]
    [CustomAuthentication]
    [CustomAuthorize("quản trị","nhân viên")]
    public class AdminProfileController : Controller
    {
        private readonly ComputerDeviceDataContext _context;
        private readonly IUserLoggedService _userLoggedService;
        private const string KEY_ERROR = "Error";
        private const string KEY_TOAST = "Toast";
        public AdminProfileController(ComputerDeviceDataContext context, IUserLoggedService userLoggedService)
        {
            _context = context;
            _userLoggedService = userLoggedService;
        }

        public IActionResult Index()
        {
            UpdateInterface(null);
            return View();
        }
        /// <summary>
        /// Hàm này dùng để đổi mật khẩu tài khoản quản trị, nhân viên
        /// </summary>
        /// <param name="dto">thông tin mật khẩu cần đổi</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO dto)
        {
            ClearAlerts();
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Dữ liệu không hợp lệ.";
                    return RedirectToAction(nameof(Index));
                }
                dto.CurrentPassword = dto.CurrentPassword?.Trim() ?? string.Empty;
                dto.NewPassword = dto.NewPassword?.Trim() ?? string.Empty;
                dto.ConfirmPassword = dto.ConfirmPassword?.Trim() ?? string.Empty;

                var userLogged = _userLoggedService.GetUserLogged();
                var account = await _context.Accounts.FindAsync(userLogged.UserId);
                if (account == null)
                {
                    TempData["Error"] = "Không tìm thấy tài khoản";
                    return RedirectToAction(nameof(Index));
                }
                var currentHash = HassPass.HassPassSHA512(dto.CurrentPassword);
                if (!string.Equals(account.PasswordHash, currentHash, StringComparison.Ordinal))
                {
                    TempData["Error"] = "Mật khẩu hiện tại không đúng.";
                    return RedirectToAction(nameof(Index));
                }
                if (string.Equals(dto.CurrentPassword, dto.NewPassword, StringComparison.Ordinal))
                {
                    TempData["Error"] = "Mật khẩu mới phải khác mật khẩu hiện tại.";
                    return RedirectToAction(nameof(Index));
                }
                account.PasswordHash = HassPass.HassPassSHA512(dto.NewPassword);
                await _context.SaveChangesAsync();

                TempData["Toast"] = "Đổi mật khẩu thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Xin vui lòng thử lại sau !";
                return RedirectToAction(nameof(Index));
            }
        }
        /// <summary>
        /// Hàm này dùng để thay đổi thông tin tài khoản
        /// </summary>
        /// <param name="acc">các thông tin, tài khoản cần thay đổi</param>
        /// <returns>View của index</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>  ChangeInformation(ChangeInfoDTO acc)
        {
            ClearAlerts();
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Dữ liệu không hợp lệ.";
                    return RedirectToAction(nameof(Index));
                }
                var userLogged = _userLoggedService.GetUserLogged();
                var account =await _context.Accounts.FindAsync(userLogged.UserId);
                if (account == null)
                {
                    TempData["Error"] = "Không tìm thấy tài khoản.";
                    return RedirectToAction(nameof(Index));
                }
                account.FirstName = acc.FirstName?.Trim();
                account.LastName = acc.LastName?.Trim();
                account.Phone = acc.Phone?.Trim();
                account.DeliverAddress = acc.DeliverAddress?.Trim();
                account.Gender = acc.Gender;
                await _context.SaveChangesAsync();

                UpdateInterface(account);
                TempData["Toast"] = "Lưu thông tin thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Xin vui lòng thử lại sau !";
                return RedirectToAction(nameof(Index));
            }
        }
        /// <summary>
        /// Hàm này dùng để cập nhật lại giao diện. Nó cập như các thông tin của tài khoản ở trong session (phiên đăng nhập)
        /// </summary>
        /// <param name="changed">là tài khoản cần cập nhật lại giao diện</param>
        private void UpdateInterface(Account? changed)
        {
            if (changed != null)
            {
                _userLoggedService.SetUserLogged(changed);
            }
            var curr = _userLoggedService.GetUserLogged();
            ViewData["AccountInformation"] = curr;
            if (curr != null)
            {
                ViewBag.GroupName = _context.GroupAccounts
                    .Where(g => g.GroupId == curr.GroupId)
                    .Select(g => g.GroupName)
                    .FirstOrDefault();
            }
        }
        private void ClearAlerts()
        {
            TempData.Remove(KEY_ERROR);
            TempData.Remove(KEY_TOAST);
        }
    }
}
