using System;
using System.Collections.Generic;

namespace ComputerDeviceShopping.Models;

public partial class PaymentMethod
{
    public int PaymentId { get; set; }

    public string? PaymentName { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
