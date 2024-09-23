using ComputerDeviceShopping.Models;
using ComputerDeviceShopping.Services;
using Microsoft.AspNetCore.Mvc;

namespace ComputerDeviceShopping.Controllers
{
    public class WishListController : Controller
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        private readonly IUserLoggedService _userLoggedService;
        public WishListController(IUserLoggedService userLoggedService)
        {
            _userLoggedService = userLoggedService;
        }
        [Route("api/wishlist/insertfavouriteproduct/{id}")]

        public IActionResult Insert(string id)
        {
            var account = _userLoggedService.GetUserLogged();
            if (account != null)
            {
                var product = _context.Products.FirstOrDefault(p => p.ProductId.Equals(id));
                if (product != null)
                {
                    var wishList = _context.FavouriteLists.Where(d => d.UserId.Equals(account.UserId) && d.ProductId.Equals(product.ProductId)).ToList();
                    string a = "";
                    if (wishList.Any())
                    {
                        return Json(new { success = false, message = "Sản phẩm này đã có trong danh sách" });
                    }
                    else
                    {
                        var favouriteProduct = new FavouriteList()
                        {
                            ProductId = id,
                            UserId = account.UserId,
                        };
                        _context.FavouriteLists.Add(favouriteProduct);
                        _context.SaveChanges();
                        return Json(new { success = true, message = "Thêm thành công" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Lỗi" });
                }
            }
            return Json(new { success = false, message = "Vui lòng đăng nhập để sử dụng chức năng" });
        }
        [HttpDelete]
        [Route("api/wishlist/delete/{id}")]
        public IActionResult DeleteItem(int id)
        {
            var item = _context.FavouriteLists.FirstOrDefault(d => d.FavouriteId == id);
            if (item != null)
            {
                _context.FavouriteLists.Remove(item);
                _context.SaveChanges();
                return Json(new { success = true, message = "Xóa thành công sản phẩm yêu thích" });
            }
            return Json(new { success = false, message = "Xóa thất bại sản phẩm yêu thích" });
        }
    }
}
