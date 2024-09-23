using System;
using System.Collections.Generic;

namespace ComputerDeviceShopping.Models;

public partial class ActivateCode
{
    public int ActivateCodeId { get; set; }

    public string Code { get; set; } = null!;

    public DateTime? CreateAt { get; set; }

    public bool? ActivateCodeStatus { get; set; }

    public string UserId { get; set; } = null!;

    public virtual Account User { get; set; } = null!;
}
