using ComputerDeviceShopping.Common;
using ComputerDeviceShopping.Models;
using ComputerDeviceShopping.Services;
using ComputerDeviceShopping.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;

namespace ComputerDeviceShopping.Controllers
{
	[CustomAuthentication]
	public class UserProfile : Controller
	{
		private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
		private readonly IUserLoggedService _userLoggedService;
		public UserProfile(IUserLoggedService userLogged)
		{
			_userLoggedService = userLogged;	
		}
		public IActionResult Index()
		{
			return View(_userLoggedService.GetUserLogged());
		}
		/// <summary>
		/// Để thay đổi các thông tin của người dùng
		/// </summary>
		/// <param name="acc">tài khoản</param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Index(Account acc)
		{
			if(acc.UserId != null && acc.UserId.Equals(_userLoggedService.GetUserLogged().UserId))
			{
				var account = _context.Accounts.Where(d => d.UserId.Equals(acc.UserId)).First();
				if (account != null)
				{
					account.LastName = acc.LastName;
					account.FirstName = acc.FirstName;
					account.Gender = acc.Gender;
					account.Phone = acc.Phone;
					account.DeliverAddress = acc.DeliverAddress;
					var comments = _context.Comments.Where(p => p.UserId.Equals(account.UserId)).ToList();
					if (comments != null) 
					{
						foreach (var comment in comments) 
						{
							comment.FirstName = account.FirstName;
						}
					}
					_context.SaveChanges();
					_userLoggedService.SetUserLogged(account);
					return RedirectToAction("Index", "UserProfile");
				}
			}
			return View();
		}
		/// <summary>
		/// Để thay đổi mật khẩu của nguoiwf dùng
		/// </summary>
		/// <param name="id">id tài khoản </param>
		/// <param name="currentPassword">mật khauaur hiện tại</param>
		/// <param name="newPassword">mật khẩu mới</param>
		/// <returns></returns>
        [HttpPost]
        [Route("api/userprofile/changepassword/{id}&{currentPassword}&{newPassword}")]
        public IActionResult ChangePassword(string id, string currentPassword, string newPassword)
        {
			if (_userLoggedService.GetUserLogged().UserId.Equals(id))
			{
                string passhass = HassPass.HassPassSHA512(currentPassword);
                var account = _context.Accounts.FirstOrDefault(d => d.UserId.Equals(id) && d.PasswordHash.Equals(passhass));
                if (account != null)
                {
                    account.PasswordHash = HassPass.HassPassSHA512(newPassword);
                    _context.SaveChanges();
                    return Json(new { success = true, message = "Đổi mật khẩu thành công" });
                }
            }
            return Json(new { success = false, message = "Đổi mật khẩu thất bại" });
        }
        public IActionResult MemberRank()
		{
			var account = _userLoggedService.GetUserLogged();

			var memberLevel = _context.MemberLevel.Where(d => d.MemberLevelId != 1).ToList();
			ViewData["MemberRankList"] = memberLevel;

            var orderList = _context.Orders.Where(d => d.CustomerId.Equals(account.CustomerId) && d.StatusId == 5 && (d.CreateAt.HasValue && d.CreateAt.Value.Year == DateTime.Now.Year)).ToList();
			double sum = orderList.Sum(d => d.TotalPrice??0);
			ViewBag.TotalOrderAmount = sum;

			ViewData["MemberLevelCurrent"] = _context.MemberLevel.Where(d => d.MemberLevelId == account.MemberLevelId).First();
            return View();
		}
		[Route("api/userprofile/getcontionalmemberrank/{id}")]
		public IActionResult GetConditionalMemberRank(int id)
		{
			var memberLevel = _context.MemberLevel.FirstOrDefault(d => d.MemberLevelId == id);
			return Json(new {success = true, message = "Thành công", data = memberLevel});
		}

		//QUẢN LÝ ĐƠN HÀNG CHỨC NĂNG (NGƯỜI DÙNG)
		public IActionResult Order(int page = 1, int status = 0)
		{
			var account = _userLoggedService.GetUserLogged();
			PaginitionOrder(page,status , account.CustomerId);
            return View();
		}
		public IActionResult OrderDetail(string idOrder)
		{
			var order = _context.Orders.Find(idOrder);
			if (order != null)
			{
				ViewData["InformationOrder"] = _context.Customers.FirstOrDefault(d => d.CustomerId.Equals(order.CustomerId));
				ViewData["ItemOrderList"] = _context.OrdersDetails.Where(d => d.OrderId.Equals(order.OrderId)).ToList();
				ViewData["ProductList"] = _context.Products.ToList();
                return View(order);
            }
			return Redirect("Order");
        }
        private void PaginitionOrder(int page, int status, string customerid)
        {
            List<Order> orders = new List<Order>();
			if(status == 0)
				orders = _context.Orders.Where(p => p.CustomerId.Equals(customerid)).ToList();
			else
                orders = _context.Orders.Where(p => p.CustomerId.Equals(customerid) && p.StatusId == status).ToList();
            var pagVM = CommonTools.Paginition(orders, page, 5);
            ViewBag.Page = pagVM.page;
            ViewBag.NoOfPages = pagVM.noOfPages;
            ViewBag.DisplayPage = pagVM.displayPage;
            ViewData["OrderList"] = pagVM.data;

            ViewData["ProductList"] = _context.Products.ToList();
            ViewData["OrderDetailList"] = _context.OrdersDetails.ToList(); ;
            ViewData["StatusOrderList"] = _context.OrderStatuses.ToList();

			var od = _context.Orders.Where(d => d.CustomerId.Equals(customerid) && d.StatusId == 5).ToList();
            ViewBag.TotalOrder = od.Count();
            ViewBag.SumPriceOrder = od.Sum(p => p.TotalPrice);
        }
		[HttpPost]
		[Route("api/userprofile/orderdetail/cancel/{id}")]
		public IActionResult CancelOrder(string id)
		{
			var order = _context.Orders.FirstOrDefault(d => d.OrderId.Equals(id));
			if (order != null && order.StatusId == 3)
			{
				order.StatusId = 6;
				_context.SaveChanges();
                return Json(new { success = true, message = "Hủy đơn hàng thành công" });
            }
            return Json(new { success = false, message = "Hủy đơn hàng thất bại" });
        }
        //WISHLIST CHỨC NĂNG
        public IActionResult WishList(int page = 1)
		{
			var account = _userLoggedService.GetUserLogged();
			var wishlist = _context.FavouriteLists.Where(d => d.UserId.Equals(account.UserId)).ToList();
			PaginitionWishList(page,wishlist);
            return View();
		}
        private void PaginitionWishList(int page, List<FavouriteList> wishlist)
        {
            var pagVM = CommonTools.Paginition(wishlist, page, 5);
            ViewBag.Page = pagVM.page;
            ViewBag.NoOfPages = pagVM.noOfPages;
            ViewBag.DisplayPage = pagVM.displayPage;
            ViewData["WishList"] = pagVM.data;
			ViewData["ProductList"] = _context.Products.ToList();
        }
      
    }
}
