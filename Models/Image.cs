using System;
using System.Collections.Generic;

namespace ComputerDeviceShopping.Models;

public partial class Image
{
    public int ImageId { get; set; }

    public string? ImageUrl { get; set; }

    public string ProductId { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
