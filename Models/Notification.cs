using System;
using System.Collections.Generic;

namespace ComputerDeviceShopping.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public string? NotificationTitle { get; set; }

    public string? NotificationContent { get; set; }

    public DateTime? CreateAt { get; set; }

    public string UserId { get; set; } = null!;

    public virtual Account User { get; set; } = null!;
}
