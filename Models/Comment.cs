using System;
using System.Collections.Generic;

namespace ComputerDeviceShopping.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public string CommentContent { get; set; } = null!;

    public DateTime? CreateAt { get; set; }

    public string UserId { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public int? ReplyComment { get; set; }
    public string? FirstName { set; get; } 

    public virtual Product Product { get; set; } = null!;

    public virtual Account User { get; set; } = null!;
}
