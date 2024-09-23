using System;
using System.Collections.Generic;

namespace ComputerDeviceShopping.Models;

public partial class Customer
{
    public string CustomerId { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string DeliverAddress { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
