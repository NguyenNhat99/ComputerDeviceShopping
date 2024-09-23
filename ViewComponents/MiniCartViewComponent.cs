using Microsoft.AspNetCore.Mvc;
using ComputerDeviceShopping.ViewModel;
using ComputerDeviceShopping.Common;
using Microsoft.EntityFrameworkCore;
using ComputerDeviceShopping.Models;
using ComputerDeviceShopping.Services;

public class MiniCartViewComponent : ViewComponent
{
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IUserLoggedService _userLoggedService;
    private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();

    public MiniCartViewComponent(IShoppingCartService shoppingCartService,IUserLoggedService userLoggedService)
    {
        _shoppingCartService = shoppingCartService;
        _userLoggedService = userLoggedService; 
    }

    public IViewComponentResult Invoke()
    {
        var products = _shoppingCartService.GetShoppingCartSession();
        return View(products);
    }

}
