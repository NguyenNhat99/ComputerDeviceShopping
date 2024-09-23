using System;
using System.Collections.Generic;

namespace ComputerDeviceShopping.Models;

public partial class Product
{
    public string ProductId { get; set; } = null!;

    public string ProductName { get; set; } = null!;

    public string? ProductDescription { get; set; }

    public string? DescriptionSummary { get; set; }

    public DateTime? CreateAt { get; set; }

    public double? Price { get; set; }

    public bool? Stock { get; set; }

    public int CategoryId { get; set; }

    public string? Avatar { get; set; }

    public string? UserId { get; set; }

    public int? BrandId { get; set; }

    public int? SpectificationId { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<FavouriteList> FavouriteLists { get; set; } = new List<FavouriteList>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<OrdersDetail> OrdersDetails { get; set; } = new List<OrdersDetail>();
}
