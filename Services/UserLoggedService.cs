using ComputerDeviceShopping.Models;
using ComputerDeviceShopping.ViewModel;
using System.Text.Json;

namespace ComputerDeviceShopping.Services
{
    public interface IUserLoggedService
    {
        Account GetUserLogged();
        void SetUserLogged(Account account);
        void Logout();
    }
    public class UserLoggedService: IUserLoggedService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserLoggedService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public Account GetUserLogged()
        {
            // để khắc phục việc khi vừa lấy lại mật khẩu rồi đăng nhập thì nó sẽ bị lập vô tận giữa activate code với account nên việc dùng ReferenceHandler.Preserve để giải quyết
            var userSession = _httpContextAccessor.HttpContext.Session.GetString("UserLogged");
            if (userSession != null)
            {
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                    PropertyNameCaseInsensitive = true
                };
                Account account = JsonSerializer.Deserialize<Account>(userSession, options);
                return account;
            }

            return null;
        }
        public void SetUserLogged(Account account)
        {
            // để khắc phục việc khi vừa lấy lại mật khẩu rồi đăng nhập thì nó sẽ bị lập vô tận giữa activate code với account nên việc dùng ReferenceHandler.Preserve để giải quyết
            Logout();
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                PropertyNameCaseInsensitive = true
            };

            var userSession = JsonSerializer.Serialize(account, options);
            _httpContextAccessor.HttpContext.Session.SetString("UserLogged", userSession);
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext.Session.Remove("UserLogged");
        }
    }
}
