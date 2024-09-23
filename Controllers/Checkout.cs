using Azure;
using ComputerDeviceShopping.Common;
using ComputerDeviceShopping.Models;
using ComputerDeviceShopping.Payments;
using ComputerDeviceShopping.Services;
using ComputerDeviceShopping.ViewModel;
using MailKit;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace ComputerDeviceShopping.Controllers
{
    public class Checkout : Controller
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        private readonly IUserLoggedService _userLoggedService;
        private readonly IShoppingCartService _shoppingCartService;
        private IEmailSender _emailSender;
        private IVnPayService _vnPayService;
        private static string CodeVoucher = "";
        public Checkout(IUserLoggedService userLoggedService, IShoppingCartService shoppingCartService, IVnPayService vnPayService, IEmailSender emailSender)
        {
            _userLoggedService = userLoggedService;
            _shoppingCartService = shoppingCartService;
            _vnPayService = vnPayService;
            _emailSender = emailSender;
        }
        [HttpGet]
        public IActionResult Index()
        {
            UpdateInterface(CodeVoucher);
            return View();
        }
        /// <summary>
        /// Hàm này dùng để tajo đơn hàng 
        /// </summary>
        /// <param name="cus">thông tin của khách hàng cần mua</param>
        /// <param name="check_method">Phương thức thanh toán</param>
        /// <param name="note">Ghi chú</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Index(Customer cus, int check_method, string note)
        {
            var account = _userLoggedService.GetUserLogged();
            var cart = _shoppingCartService.GetShoppingCartSession();
            if (cart.Count() > 0)
            {
                var sumPrice = (double)cart.Sum(p => p.Price);
                if (account != null)
                {

                    var customer = _context.Customers.FirstOrDefault(p => p.CustomerId.Equals(account.CustomerId));
                    Order order = new Order()
                    {
                        CreateAt = DateTime.Now,
                        CustomerId = customer.CustomerId,
                        OrderId = CommonTools.RandomNumbers(10),
                        Note = note,
                        PaymentId = check_method,
                        StatusId = 3,
                        TotalPrice = sumPrice,
                    };
                    _context.Orders.Add(order);
                    InsertOrderDetail(cart, order.OrderId);
                    _shoppingCartService.SetShoppingCartSession(new List<CartItem>());
                    var cv = (TempData["CodeVoucher"] as string) ?? "";
                    if (cv.Length > 0)
                    {
                        var voucher = _context.Vouchers.FirstOrDefault(p => p.VoucherCode.Equals(cv) && p.VoucherStatus == true);
                        if (voucher != null &&  voucher.NumberOfTimesEXE>0)
                        {
                            voucher.NumberOfTimesEXE -= 1;
                            order.VoucherId = voucher.VoucherId;
                            order.TotalPrice = order.TotalPrice - ((order.TotalPrice * voucher.Discount) / 100);
                            sumPrice = (double)order.TotalPrice;
                            if(voucher.NumberOfTimesEXE<1)
                                voucher.VoucherStatus = false;   
                        }
                    }
                    if (check_method == 2)
                    {
                        var cartUser = _context.Carts.FirstOrDefault(p => p.CartStatus == false && p.UserId.Equals(account.UserId));
                        if (cartUser != null)
                        {
                            DeleteCartAndItem(cartUser.CartId);
                        }
                        _context.SaveChanges();
                        await _emailSender.SendEmailAsync(customer.Email, "Đặt hàng thành công", "Bạn đã đặt hàng tại công tại Techgear với số đơn hàng #" + order.OrderId + ". Bạn có thể tra cứu trạng thái đơn tại website");
                        return RedirectToAction("CheckoutSuccess","Checkout");
                    }
                    else
                    {
                        _context.SaveChanges();
                        var vnpRequest = new VnPaymentRequest()
                        {
                            CreateAt = (DateTime)order.CreateAt,
                            Amount = sumPrice,
                            OrderId= order.OrderId, 
                        };
                        var url = _vnPayService.CreatePaymentUrl(HttpContext, vnpRequest);
                        return Redirect(url);
                    }
                }
                else
                {
                    var customer = _context.Customers.FirstOrDefault(p => p.Email.Equals(cus.Email));
                    var randomOrderId = CommonTools.RandomNumbers(10);
                    var timeCreate = DateTime.Now;
                    if (customer != null)
                    {
                        Order order = new Order()
                        {
                            CreateAt = timeCreate,
                            CustomerId = customer.CustomerId,
                            OrderId = randomOrderId,
                            Note = note,
                            PaymentId = check_method,
                            StatusId = 3,
                            TotalPrice = sumPrice,
                        };
                      
                        _context.Orders.Add(order);
                        InsertOrderDetail(cart, order.OrderId);
                        if (check_method == 2)
                        {
                            _shoppingCartService.SetShoppingCartSession(new List<CartItem>());
                            await _emailSender.SendEmailAsync(cus.Email, "Đặt hàng thành công", "Bạn đã đặt hàng tại công tại Techgear với số đơn hàng #" + order.OrderId + ". Bạn có thể tra cứu trạng thái đơn tại website");
                            return RedirectToAction("CheckoutSuccess", "Checkout");
                        }
                    }
                    else
                    {
                        Customer newCustomer = new Customer()
                        {
                            Email = cus.Email,
                            Phone = cus.Phone,
                            FirstName = cus.FirstName,
                            LastName = cus.LastName,
                            DeliverAddress = cus.DeliverAddress,
                            CustomerId = CommonTools.RandomNumbers(10)
                        };  
                        _context.Customers.Add(newCustomer);

                        Order order = new Order()
                        {
                            
                            CreateAt = timeCreate,
                            CustomerId = newCustomer.CustomerId,
                            OrderId = randomOrderId,
                            Note = note,
                            PaymentId = check_method,
                            StatusId = 3,
                            TotalPrice = sumPrice,
                        };
                        _context.Orders.Add(order);
                        InsertOrderDetail(cart, order.OrderId);
                        _context.SaveChanges();
                        if (check_method == 2)
                        {
                            _shoppingCartService.SetShoppingCartSession(new List<CartItem>());
                            await _emailSender.SendEmailAsync(cus.Email, "Đặt hàng thành công", "Bạn đã đặt hàng tại công tại Techgear với số đơn hàng #" + order.OrderId + ". Bạn có thể tra cứu trạng thái đơn tại website");
                            return RedirectToAction("CheckoutSuccess", "Checkout");
                        }
                    }
                    if(check_method == 1)
                    {
                        var vnpRequest = new VnPaymentRequest()
                        {
                            CreateAt = timeCreate,
                            Amount = sumPrice,
                            OrderId = randomOrderId,
                        };
                        var url = _vnPayService.CreatePaymentUrl(HttpContext, vnpRequest);
                        return Redirect(url);
                    }
                }
            }
            return RedirectToAction("Index", "Shopping");
           
        }
        /// <summary>
        /// Hàm này dùng để xóa giỏ hàng khi mà đơn hàng của giỏ hàng này thanh công
        /// </summary>
        /// <param name="cartId">id giỏ hàng cần xóa</param>
        private void DeleteCartAndItem(string cartId)
        {
            var cart = _context.Carts.FirstOrDefault(p => p.CartId.Equals(cartId) && p.CartStatus == false);
            if (cart != null)
            {
                var items = _context.CartItems.Where(p => p.CartId.Equals(cartId));
                foreach (var item in items) 
                {
                    _context.CartItems.Remove(item);
                }
                _context.Carts.Remove(cart);    
                _context.SaveChanges();
            }
        }
        /// <summary>
        /// Thanh toán thất bại
        /// </summary>
        /// <returns></returns>
        public IActionResult CheckoutError()
        {
            return View();
        }
        private void InsertOrderDetail(List<CartItem> cart, string orderId)
        {
            foreach (var item in cart)
            {
                OrdersDetail detail = new OrdersDetail()
                {
                    OrderId = orderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                };
                _context.OrdersDetails.Add(detail); /// Looix
            }
            _context.SaveChanges();

        }
        /// <summary>
        /// Hàm này dùng để sau khi thanh toán qua vnpay nó sẽ gọi lại hàm và xử lí một số bươc cuối
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> PaymentCallback()
        {
            var response = _vnPayService.PaymentExcute(Request.Query);
            var order = _context.Orders.FirstOrDefault(p => p.OrderId.Equals(response.OrderId));
            //Thất bại
            if (response == null || response.VnpayResponseCode != "00")
            {
                order.StatusId = 4;
                _context.SaveChanges();
                return Redirect("CheckoutError");
            }
            //Thất bại

            //Thành công
            try
            {
                var account = _userLoggedService.GetUserLogged();
                var cartUser = _context.Carts.FirstOrDefault(p => p.CartStatus == false && p.UserId.Equals(account.UserId));
                if (cartUser != null)
                {
                    DeleteCartAndItem(cartUser.CartId);
                }
            }
            catch (Exception ex) { }
            var customer = _context.Customers.FirstOrDefault(d => d.CustomerId.Equals(order.CustomerId));
            await _emailSender.SendEmailAsync(customer.Email, "Đặt hàng thành công", "Bạn đã đặt hàng tại công tại Techgear với số đơn hàng #" + order.OrderId + ". Bạn có thể tra cứu trạng thái đơn tại website" );
            _shoppingCartService.SetShoppingCartSession(new List<CartItem>());
            TempData["Message"] = "Thanh toán vnpay thành công";
            return Redirect("CheckoutSuccess");
        }
        public IActionResult CheckoutSuccess()
        {
            return View();  
        }
        private void UpdateInterface(string voucherCode)
        {
            ViewBag.Discount = "";
            var cartItems = _shoppingCartService.GetShoppingCartSession();
            var account = _userLoggedService.GetUserLogged();
            var sumCarts = cartItems.Sum(d => d.Total);

            //giỏ hàng
            ViewData["ShoppingCart"] = cartItems;
            //Tài khoản
            ViewData["InformationUser"] = account;

            ViewBag.totalCart = sumCarts;
            ViewBag.subTotal = sumCarts;
            if (voucherCode.Length > 0)
            {
                var voucher = _context.Vouchers.Where(d => d.VoucherCode.Equals(voucherCode) && d.EndAt > DateTime.Now).FirstOrDefault();
                if (voucher != null)
                {
                    ViewBag.Discount = voucher.Discount;
                    ViewBag.subTotal = sumCarts - ((sumCarts * voucher.Discount) / 100);
                }
            }
            CodeVoucher = "";
        }
        /// <summary>
        /// Chức năng áp dụng mã giảm giá chỉ dành cho nguwoweif dùng đã đăng ký thành viên tại website
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/checkout/applyvoucher/{code}")]
        public IActionResult ApplyVoucher(string code)
        {
            Account account = _userLoggedService.GetUserLogged();
            if (account != null)
            {
                try
                {
                    var voucher = _context.Vouchers.Where(d => d.VoucherCode.Equals(code) && d.EndAt > DateTime.Now).FirstOrDefault();
                    if (voucher != null && voucher.NumberOfTimesEXE>0)
                    {
                        CodeVoucher = voucher.VoucherCode;
                        TempData["CodeVoucher"] = voucher.VoucherCode;
                        return Json(new { success = true, message = "Áp dụng mã giảm giá thành công" });
                    }
                }
                catch (Exception ex) { }
            }
            else
            {
                return Json(new { success = false, message = "Vui lòng đăng ký tài khoản thành viên để sử dụng chức năng này" });
            }
            return Json(new { success = false, message = "Vui lòng kiểm tra lại" });
        }
    }
}
