using System;
using System.Collections.Generic;

namespace ComputerDeviceShopping.Models;

public partial class Account
{
    public string UserId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public DateTime? CreateAt { get; set; }

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? DeliverAddress { get; set; }

    public bool? Gender { get; set; }

    public bool? UserStatus { get; set; }

    public string CustomerId { get; set; } = null!;

    public int GroupId { get; set; }
    public int MemberLevelId { get; set; }

    public virtual ICollection<ActivateCode> ActivateCodes { get; set; } = new List<ActivateCode>();

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<FavouriteList> FavouriteLists { get; set; } = new List<FavouriteList>();

    public virtual GroupAccount Group { get; set; } = null!;

    public virtual MemberLevel Member { get; set; } = null!;
}
