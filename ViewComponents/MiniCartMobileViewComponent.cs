using Microsoft.AspNetCore.Mvc;
using ComputerDeviceShopping.ViewModel;
using ComputerDeviceShopping.Services;
namespace ComputerDeviceShopping.ViewComponents
{
    public class MiniCartMobileViewComponent : ViewComponent
    {
        private readonly IShoppingCartService _shoppingCartService;

        public MiniCartMobileViewComponent(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }

        public IViewComponentResult Invoke()
        {
            return View(_shoppingCartService.GetShoppingCartSession());
        }
    }
}
