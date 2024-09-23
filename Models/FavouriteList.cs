using System;
using System.Collections.Generic;

namespace ComputerDeviceShopping.Models;

public partial class FavouriteList
{
    public int FavouriteId { get; set; }

    public string UserId { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual Account User { get; set; } = null!;
}
