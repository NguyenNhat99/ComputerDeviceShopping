using System;
using System.Collections.Generic;

namespace ComputerDeviceShopping.Models;

public partial class Slider
{
    public int SlideId { get; set; }

    public string? SlideImage { get; set; }

    public string? ProductLink { get; set; }
    public string? ProductName { get; set; }
    public string? SliderTitle { get; set; }
    public string? DescriptionSlide { get; set; }
}
