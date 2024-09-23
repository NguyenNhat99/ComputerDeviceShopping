using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ComputerDeviceShopping.Models;

public partial class Article
{
    public int ArticleId { get; set; }

    public string ArticleName { get; set; } = null!;

    public string ArticleContent { get; set; } = null!;
    public string SummaryContent { get; set; } = null!;
    public int ArticleView { set; get; }

    public string? Avatar { get; set; }

    public DateTime? CreateAt { get; set; }

    public string UserId { get; set; } = null!;

    public bool? ArticleStatus { get; set; }

    public virtual Account User { get; set; } = null!;
}
