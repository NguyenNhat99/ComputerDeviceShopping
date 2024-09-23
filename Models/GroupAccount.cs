using System;
using System.Collections.Generic;

namespace ComputerDeviceShopping.Models;

public partial class GroupAccount
{
    public int GroupId { get; set; }

    public string GroupName { get; set; } = null!;

    public string GroupSymbol { get; set; } = null!;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
