using ComputerDeviceShopping.Common;
using ComputerDeviceShopping.Models;
using ComputerDeviceShopping.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace ComputerDeviceShopping.Controllers
{
    public class CommentController : Controller
    {
        private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
        private readonly IUserLoggedService _userLoggedService;
        private readonly IHubContext<CommentHub> _hubContext;
        public CommentController(IUserLoggedService userLoggedService, IHubContext<CommentHub> hubContext)
        {
            _userLoggedService = userLoggedService;
            _hubContext = hubContext;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Route("api/comments/addcomment/{productid}&{commentContent}&{replyComment}")]
        public async Task<IActionResult> AddComment(string productId, string commentContent, int replyComment)
        {
            var account = _userLoggedService.GetUserLogged();
            if (account == null) return Unauthorized();

            var comment = new Comment
            {
                ProductId = productId,
                CommentContent = commentContent,
                UserId = account.UserId,
                CreateAt = DateTime.Now,
                FirstName = account.FirstName
            };
            if (replyComment > 0)
            {
                var cm = _context.Comments.FirstOrDefault(p => p.CommentId.Equals(replyComment));
                if (cm != null) {
                    comment.ReplyComment = replyComment;
                }
            }
            _context.Comments.Add(comment); 
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveComment", comment);
            return Ok(comment);
        }
        [HttpPost]
        [Route("api/comments/deletecomment/{idComment}")]
        public async Task<IActionResult> DeleteComment(int idComment)
        {
            var account  =_userLoggedService.GetUserLogged();
            if (account != null)
            {
                var comment = _context.Comments.FirstOrDefault(d=>d.CommentId.Equals(idComment));
                if (comment != null && account.UserId.Equals(comment.UserId)) 
                {
                    var replyCMList = _context.Comments.Where(d=>d.ReplyComment!=null && d.ReplyComment.Equals(comment.CommentId));
                    _context.Comments.Remove(comment);  
                    foreach(var rl in replyCMList)
                    {
                        _context.Comments.Remove(rl);
                    }
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Thành công" });
                }
            }
            return Json(new { success = false, message = "Thất bại" });

        }
        [HttpPost]
        [Route("api/comments/replycomment/{idComment}&{idProduct}&{commentContent}")]
        public async Task<IActionResult> ReplyComment(int idComment, string idProduct, string commentContent)
        {
            var account = _userLoggedService.GetUserLogged();
            if(account==null) return Unauthorized();

            var comment = new Comment()
            {
                CommentContent = commentContent,
                ReplyComment = idComment,
                CreateAt= DateTime.Now,
                ProductId = idProduct,
                
            };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveComment", comment);
            return Ok(comment);
        }

    }
}
