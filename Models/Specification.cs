using System;
using System.Collections.Generic;

namespace ComputerDeviceShopping.Models;

public partial class Specification
{
    public int SpecificationId { get; set; }

    public string? SpecificationLabel { get; set; }

    public string? SpecificationDetail { get; set; }
    public string? ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;

}
