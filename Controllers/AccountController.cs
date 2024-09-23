using Microsoft.AspNetCore.Mvc;
using ComputerDeviceShopping.ViewModel;
using ComputerDeviceShopping.Common;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using ComputerDeviceShopping.Models;
using System.Text.Json;
using ComputerDeviceShopping.Services;

namespace ComputerDeviceShopping.Controllers
{
    public class AccountController : Controller
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        private readonly IUserLoggedService _userLoggedService;
        private readonly IEmailSender _emailSender;
        private readonly IShoppingCartService _shoppingCartService;

        public AccountController(IUserLoggedService userLoggedService, IEmailSender emailSender, IShoppingCartService shoppingCartService)
        {
            _userLoggedService = userLoggedService;
            _emailSender = emailSender;
            _shoppingCartService = shoppingCartService;
        }
        public async Task<IActionResult> Login()
        {
            Account account = _userLoggedService.GetUserLogged();
            if (account != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        /// <summary>
        /// Hàm này dùng để đăng nhập tài khoản
        /// </summary>
        /// <param name="username">Tài khoản</param>
        /// <param name="password">mật khẩu</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            string passHash = HassPass.HassPassSHA512(password);
            Account account = await _context.GetAccountByUsernameAndPassword(username, passHash);
            if (account != null)
            {
                _userLoggedService.SetUserLogged(account);
                if (account.GroupId == 1 || account.GroupId == 2)
                    return RedirectToAction("Index", "Dashboard", new { area = "PrivateSite" });
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ErrorLogin = "Vui lòng kiểm tra lại tài khoản và mật khẩu !";
            return View();
        }
        [HttpGet]
        public IActionResult Register() 
        {
            Account account = _userLoggedService.GetUserLogged();
            if (account != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        /// <summary>
        /// Hàm này dùng để đăng ký tài khoản
        /// </summary>
        /// <param name="avm">Thông tin tài khoản cần đăng ký</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(AccountVM avm)
        {
            Account checkAccount = _context.Accounts.FirstOrDefault(ac=>ac.Email.Equals(avm.email)) ?? new Account();
            if (checkAccount.UserId == null)
            {
                Customer customer = new Customer()
                {
                    CustomerId = CommonTools.RandomNumbers(9),
                    FirstName = avm.firstName,
                    LastName = avm.lastName,
                    Email = avm.email,
                    Phone = avm.phone,
                    DeliverAddress = avm.address??"",
                };
                _context.Customers.Add(customer);

                Account account = new Account()
                {
                    UserId = CommonTools.RandomNumbers(10),
                    Username = avm.username,
                    PasswordHash = HassPass.HassPassSHA512(avm.password),
                    Email = avm.email,
                    Phone = avm.phone,
                    DeliverAddress = avm.address,
                    CreateAt = DateTime.Now,
                    Gender = avm.gender,
                    LastName = avm.lastName,
                    FirstName = avm.firstName,
                    UserStatus = true,
                    CustomerId = customer.CustomerId,
                    GroupId = 3,
                    MemberLevelId = 1
                };
                _context.Accounts.Add(account);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");   
            }
            ViewBag.ErrorRegister = "Email đăng ký này đã tồn tại";
            return View();
        }
        /// <summary>
        /// Hàm này dùng để lấy lại tài khoản
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            Account account = _userLoggedService.GetUserLogged();
            if (account != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email, string code)
        {
            var account = _context.Accounts.Where(d => d.Email.Equals(email)).FirstOrDefault();
            if (account != null) 
            {
                ActivateCode activateCode = _context.ActivateCodes.Where(d=>d.UserId.Equals(account.UserId) && d.Code.Equals(code)).FirstOrDefault() ?? new ActivateCode();
                if (activateCode != null)
                {
                   
                    TimeSpan difference = DateTime.Now - activateCode.CreateAt.GetValueOrDefault();
                    if (difference.TotalMinutes <= 15)
                    {
                        activateCode.ActivateCodeStatus = false;
                        string passRandom = CommonTools.RandomCharacters(10);
                        string hashPassRandom = HassPass.HassPassSHA512(passRandom);
                        account.PasswordHash = hashPassRandom;
                        _context.SaveChanges();
                        await _emailSender.SendEmailAsync(account.Email, "Mật khẩu mới", "Mật khẩu mới của bạn là: " + passRandom);
                        return Redirect("Login");
                    }
                }
            }
            return View();
        }
        /// <summary>
        /// Hàm này dùng để gửi code xác nhận về gmail của tài khoản đăng ký
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/account/forgotpass/sendcodeconfirm/{email}")]
        public async Task<IActionResult> SendCodeConfirm(string email)
        {
            var account = _context.Accounts.Where(d => d.Email.Equals(email)).FirstOrDefault();
            string a = "";
            if (account != null)
            {
                string codeRandom = CommonTools.RandomNumbers(7);
                ActivateCode code = new ActivateCode()
                {
                    CreateAt = DateTime.Now,
                    ActivateCodeStatus = false,
                    UserId = account.UserId,
                    Code = codeRandom
                };
                _context.ActivateCodes.Add(code);
                _context.SaveChanges();
                await _emailSender.SendEmailAsync(account.Email, "Mã xác thực lấy lại mật khẩu", "Mã xác thực ( mã sẽ có hiệu lực trong 15 phút ): " + codeRandom);
                return Json(new { success = true, message = "Mã xác nhận đã được gửi vào Email .Xin vui lòng kiểm nha (Lưu ý: mỗi mã chỉ có hiệu lực trong 15 phút)" });
            }
            return Json(new { success = false, message = "Lấy mã thất bại. Vui lòng kiểm tra lại" });
        }
        public IActionResult ConfirmCode()
        {
            Account account = _userLoggedService.GetUserLogged();
            if (account != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        /// <summary>
        /// Hàm này dùng để xác nhận lấy lại mật khẩu
        /// </summary>
        /// <param name="id">id của tài khoản cần lấy lại mật khẩu</param>
        /// <param name="c">Mã code</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ComfirmCode(string id, string c)
        {
            var acc = _context.Accounts.Where(d => d.UserId.Equals(id)).FirstOrDefault();
            if (acc != null)
            {
                string code = _context.ActivateCodes.Where(d => d.UserId.Equals(acc.UserId)).First().Code;
                if (code.Equals(c))
                {
                    string passRandom = CommonTools.RandomCharacters(10);
                    string hashPassRandom = HassPass.HassPassSHA512(passRandom);
                    acc.PasswordHash = hashPassRandom;
                    await _context.SaveChangesAsync();
                    await _emailSender.SendEmailAsync(acc.Email, "Mật khẩu mới", "Mật khẩu mới của bạn là: " + passRandom);
                    return Json(new { success = true, message = "Lấy lại mật khẩu thành công ! Hãy kiểm tra Email của bạn" });

                }
            }
            return Json(new {success = false, message = "Lấy lại mật khẩu thất bại ! Nếu có thắc mắc gì hãy liên hệ với cửa hàng. Xin cảm ơn"});
        }
        /// <summary>
        /// Hàm này dùng để đăng xuất tài khoản
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            _userLoggedService.Logout();
            _shoppingCartService.RemoveShoppingCartSession();
            return RedirectToAction("Index", "Home");
        }
        public IActionResult TestSendmail()
        {
            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
