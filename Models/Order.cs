using System;
using System.Collections.Generic;

namespace ComputerDeviceShopping.Models;

public partial class Order
{
    public string OrderId { get; set; } = null!;

    public DateTime? CreateAt { get; set; }

    public double? TotalPrice { get; set; }

    public string? Note { get; set; }

    public int PaymentId { get; set; }

    public string CustomerId { get; set; } = null!;

    public int? VoucherId { get; set; }

    public int? StatusId { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrdersDetail> OrdersDetails { get; set; } = new List<OrdersDetail>();

    public virtual PaymentMethod Payment { get; set; } = null!;

    public virtual OrderStatus? Status { get; set; }

    public virtual Voucher? Voucher { get; set; }
}
