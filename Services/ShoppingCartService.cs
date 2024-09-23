using ComputerDeviceShopping.Models;
using ComputerDeviceShopping.ViewModel;
using System.Text.Json;

namespace ComputerDeviceShopping.Services
{
    public interface IShoppingCartService
    {
        List<CartItem> GetShoppingCartSession();
        void SetShoppingCartSession(List<CartItem> cart);
        void RemoveShoppingCartSession();

    }
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        private readonly IUserLoggedService _userLoggedService;
        public ShoppingCartService(IHttpContextAccessor httpContextAccessor, IUserLoggedService userLoggedService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userLoggedService = userLoggedService;
        }

        public List<CartItem> GetShoppingCartSession()
        {
            if (_userLoggedService.GetUserLogged() != null)
            {
                var cart = _context.Carts.Where(d => d.UserId.Equals(_userLoggedService.GetUserLogged().UserId) && d.CartStatus == false).FirstOrDefault();
                if (cart != null)
                {
                    var cartItem = _context.CartItems.Where(d => d.CartId == cart.CartId).ToList();
                    return cartItem;
                }
            }
            else
            {
                var productsSession = _httpContextAccessor.HttpContext.Session.GetString("CartSession");
                if (productsSession != null)
                {
                    var options = new JsonSerializerOptions
                    {
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                        PropertyNameCaseInsensitive = true
                    };
                    List<CartItem> products = JsonSerializer.Deserialize<List<CartItem>>(productsSession, options);
                    return products;
                }
            }

            return new List<CartItem>();
        }
        public void SetShoppingCartSession(List<CartItem> cart)
        {
            RemoveShoppingCartSession();
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
                PropertyNameCaseInsensitive = true
            };

            var shoppingCartSession = JsonSerializer.Serialize(cart, options);
            _httpContextAccessor.HttpContext.Session.SetString("CartSession", shoppingCartSession);
        }

        public void RemoveShoppingCartSession()
        {
            _httpContextAccessor.HttpContext.Session.Remove("CartSession");
        }
    }
}
