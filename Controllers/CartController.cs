using ComputerDeviceShopping.Common;
using ComputerDeviceShopping.Models;
using ComputerDeviceShopping.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.DataClassification;
using Microsoft.EntityFrameworkCore;
using MimeKit.Cryptography;

namespace ComputerDeviceShopping.Controllers
{
    public class CartController : Controller
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        private readonly IUserLoggedService _userLoggedService;
        private readonly IShoppingCartService _shoppingCartService;
        public CartController(IUserLoggedService userLoggedService, IShoppingCartService shoppingCartService)
        {
            _userLoggedService = userLoggedService;
            _shoppingCartService = shoppingCartService;
        }
        public IActionResult Index()
        {
            var ci = _shoppingCartService.GetShoppingCartSession();
            ViewData["ShoppingCart"] = ci;
            UpdateInterface(ci);
            return View();
        }
        /// <summary>
        /// Dùng để thêm một sản phẩm vào giỏ hàng 
        /// </summary>
        /// <param name="id">id sản phẩm cần thêm vào giỏ hàng</param>
        /// <param name="quantity">số lượng</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/shoppingcart/insert/{id}&{quantity}")]
        public IActionResult Insert(string id, int quantity = 1)
        {
            var product = _context.Products.FirstOrDefault(d => d.ProductId.Equals(id));
            if (product != null)
            {
                List<CartItem> cartItems = _shoppingCartService.GetShoppingCartSession();
                var productSelected = cartItems.FirstOrDefault(d => d.ProductId.Equals(product.ProductId));

                if(_userLoggedService.GetUserLogged() != null)
                {
                    var myCart = _context.Carts.Where(d => d.UserId.Equals(_userLoggedService.GetUserLogged().UserId) && d.CartStatus == false).FirstOrDefault();

                    if (myCart == null)
                    {
                         myCart = new Cart()
                        {
                            CreateAt = DateTime.Now,
                            Totalprice = 0,
                            UserId = _userLoggedService.GetUserLogged().UserId,
                            CartStatus = false,
                            CartId = CommonTools.RandomNumbers(7)
                        };
                        _context.Carts.Add(myCart);
                    }
                    if(productSelected == null)
                    {
                        CartItem cartItem = new CartItem()
                        {
                            ImageProduct = product.Avatar,
                            Price = product.Price,
                            Quantity = quantity,
                            Total = product.Price * quantity,
                            ProductName = product.ProductName,
                            ProductId = product.ProductId,
                            CartId = myCart.CartId,
                        };
                        myCart.Totalprice += cartItem.Total;
                        _context.CartItems.Add(cartItem);
                        cartItems.Add(cartItem);
                        //_shoppingCartService.SetShoppingCartSession(products);
                        _context.SaveChanges();
                    }
                    else
                    {
                        var item1 = _context.CartItems.Where(d=>d.CartItemId == productSelected.CartItemId).First();
                        item1.Quantity += quantity;
                        item1.Total = item1.Quantity * item1.Price;
                        productSelected.Quantity += quantity;
                        productSelected.Total = item1.Quantity * item1.Price;
                        myCart.Totalprice += item1.Total;
                        _context.SaveChanges();
                    }
                }
                else
                {
                    if (productSelected == null)
                    {
                        CartItem item = new CartItem()
                        {
                            ImageProduct = product.Avatar,
                            ProductName = product.ProductName,
                            Price = product.Price,
                            ProductId = product.ProductId,
                        };
                        item.Quantity += quantity;
                        item.Total = item.Price * item.Quantity;
                        cartItems.Add(item);
                    }
                    else
                    {
                        productSelected.Quantity += quantity;
                        productSelected.Total = productSelected.Price * productSelected.Quantity;
                        //_context.CartItems.Update(productSelected);
                    }
                }
                _shoppingCartService.SetShoppingCartSession(cartItems);
            }
            return Json(new { success = true, message = "Thêm giỏ hàng thành công" });
        }
        /// <summary>
        /// Dùng để cập nhật lại giỏ hàng trong session
        /// </summary>
        /// <param name="listItem">Danh sách vật phẩm giỏ hàng</param>
        /// <param name="quantity">Số lượng</param>
        /// <param name="id">id sản phẩm</param>
        private void UpdateShoppingCartSession(List<CartItem> listItem, int quantity, string id)
        {
            var item = listItem.Where(d => d.ProductId.Equals(id)).FirstOrDefault();
            if (item != null)
            {
                item.Quantity = quantity;
                item.Total = item.Quantity * item.Price;
                _shoppingCartService.SetShoppingCartSession(listItem);
            }
        }
        /// <summary>
        /// Hàm này đùng để cập nhật giỏ hàng
        /// </summary>
        /// <param name="id">id sản phẩm canaf cập nhật</param>
        /// <param name="quantity">Số lượng</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/cart/updatecart/{id}&{quantity}")]
        public async Task<IActionResult> UpdateCart(string id,int quantity)
        {
            var account = _userLoggedService.GetUserLogged();
            var products = _shoppingCartService.GetShoppingCartSession();
            if (account != null)
            {
                var myCart = await _context.Carts.Where(d => d.CartStatus == false && d.UserId.Equals(account.UserId)).FirstOrDefaultAsync();
                if (myCart != null)
                {
                    var cartItem =await _context.CartItems.Where(d => d.CartId.Equals(myCart.CartId) && d.ProductId.Equals(id)).FirstOrDefaultAsync();
                    myCart.Totalprice -= cartItem.Total;
                    cartItem.Quantity = quantity;
                    cartItem.Total = cartItem.Quantity * cartItem.Price;
                    myCart.Totalprice += cartItem.Total;
                    await _context.SaveChangesAsync();
                    UpdateShoppingCartSession(products,quantity,id);
                    return Json(new { success = true, message = "Cập nhật giỏ hàng thành công" });
                }
                return Json(new { success = false, message = "Cập nhật giỏ hàng thất bại" });
            }
            else
            {
                UpdateShoppingCartSession(products, quantity, id);
                return Json(new { success = true, message = "Cập nhật giỏ hàng thành công" });
            }
        }
        /// <summary>
        /// Hàm này dùng để xóa sản phẩm ra khỏi giỏ hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("cart/deleteitemincart/{id}")]
        public IActionResult Delete(string id)
        {
            var cartItems = _shoppingCartService.GetShoppingCartSession();
            var product = _context.Products.FirstOrDefault(d => d.ProductId.Equals(id));
            if (_userLoggedService.GetUserLogged() != null)
            {
                //item trong giỏ
                var myCart = _context.Carts.Where(d => d.UserId.Equals(_userLoggedService.GetUserLogged().UserId) && d.CartStatus == false).FirstOrDefault();
                var cartItem = _context.CartItems.FirstOrDefault(d => d.ProductId.Equals(id) & d.CartId.Equals(myCart.CartId));
                if (myCart != null) 
                {
                    if (cartItem != null)
                    {
                        myCart.Totalprice = myCart.Totalprice - cartItem.Total;
                        _context.CartItems.Remove(cartItem);
                    }
                    _context.SaveChanges();
                    _shoppingCartService.SetShoppingCartSession(cartItems);
                    return Json(new { success = true, messsge = "Xóa thành công" });
                }
                return Json(new { success = false, messsge = "Xóa thất bại" });
            }
            else 
            {
                var ci = cartItems.Where(d => d.ProductId.Equals(id)).FirstOrDefault();
                if (ci != null)
                {
                    cartItems.Remove(ci);
                    _shoppingCartService.SetShoppingCartSession(cartItems);
                    return Json(new { success = true, message = "Xóa thành công" });
                }
                return Json(new { success = false, message = "Xóa thất bại" });
            }
        }
       
        /// <summary>
        /// Hàm này dùng để cập nhật giao diện giỏ hàng
        /// </summary>
        /// <param name="carts"></param>
        private void UpdateInterface(List<CartItem> carts)
        {
            Account account = _userLoggedService.GetUserLogged();
            float totalPrice = 0;
            if (carts != null)
            {
                foreach (var cart in carts) {
                    totalPrice += (float)(cart.Total??0);
                }
                ViewBag.TotalPriceCart = totalPrice;

                ViewBag.FeeShipping = "Miễn phí"; // xử lí
                ViewBag.ApplyVc = "0%"; // xử lí
            }
        }
        private void SaveVoucher(string code)
        {
            ViewData["VoucherCode"] = code;
        }
    }
}
