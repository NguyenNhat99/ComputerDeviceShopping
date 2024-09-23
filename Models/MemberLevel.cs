using System;
using System.Collections.Generic;

namespace ComputerDeviceShopping.Models;

public partial class MemberLevel
{
    public int MemberLevelId { get; set; }

    public string LevelDescription { set; get; } = null!;
    public string LevelName { set; get; } = null!;
    public int LevelDiscount { set; get; }
    public double? Limit { set; get; }
    public string? ImageUrl { set; get; }
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
    

}
