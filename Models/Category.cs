using System;
using System.Collections.Generic;

namespace ComputerDeviceShopping.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string CategorySymbol { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
