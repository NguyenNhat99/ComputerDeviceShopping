using System;
using System.Collections.Generic;

namespace ComputerDeviceShopping.Models;

public partial class OrdersDetail
{
    public int OrdersDetailId { get; set; }

    public string ProductId { get; set; } = null!;

    public string OrderId { get; set; } = null!;

    public int Quantity { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
