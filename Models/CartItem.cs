using System;
using System.Collections.Generic;

namespace ComputerDeviceShopping.Models;

public partial class CartItem
{
    public int CartItemId { get; set; }

    public string CartId { get; set; } = null!;

    public string ProductId { get; set; } = null!;

    public int Quantity { get; set; }

    public double? Price { get; set; }

    public double? Total { get; set; }

    public string? ImageProduct { get; set; }

    public string? ProductName { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
