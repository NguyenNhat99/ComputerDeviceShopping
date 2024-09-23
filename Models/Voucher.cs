using System;
using System.Collections.Generic;

namespace ComputerDeviceShopping.Models;

public partial class Voucher
{
    public int VoucherId { get; set; }

    public string VoucherCode { get; set; } = null!;

    public int? Discount { get; set; }

    public bool? VoucherStatus { get; set; }
    public int NumberOfTimesEXE { set; get; }

    public DateTime? CreateAt { get; set; }

    public DateTime? EndAt { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
