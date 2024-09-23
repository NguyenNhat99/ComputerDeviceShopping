using ComputerDeviceShopping.Models;
using ComputerDeviceShopping.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ComputerDeviceShopping.Common
{
    public class CommentHub : Hub
    {
        public async Task SendComment(Comment comment)
        {
            await Clients.All.SendAsync("ReceiveComment", comment);
        }
    }
}
