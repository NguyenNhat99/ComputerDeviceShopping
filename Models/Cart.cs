using System;
using System.Collections.Generic;

namespace ComputerDeviceShopping.Models;

public partial class Cart
{
    public string CartId { get; set; } = null!;

    public double? Totalprice { get; set; }

    public string UserId { get; set; } = null!;

    public bool? CartStatus { get; set; }

    public DateTime? CreateAt { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Account User { get; set; } = null!;
}
