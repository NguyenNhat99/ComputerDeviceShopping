using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ComputerDeviceShopping.Models;

public partial class ComputerDeviceDataContext : DbContext
{
    public ComputerDeviceDataContext()
    {
    }

    public ComputerDeviceDataContext(DbContextOptions<ComputerDeviceDataContext> options)
        : base(options)
    {
    }

    public virtual DbSet<About> Abouts { get; set; }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<ActivateCode> ActivateCodes { get; set; }

    public virtual DbSet<Article> Articles { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<FavouriteList> FavouriteLists { get; set; }

    public virtual DbSet<GroupAccount> GroupAccounts { get; set; }

    public virtual DbSet<Image> Images { get; set; }


    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<OrdersDetail> OrdersDetails { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Slider> Sliders { get; set; }

    public virtual DbSet<Specification> Specifications { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }
    public virtual DbSet<MemberLevel> MemberLevel{ get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-30I101B\\SQLEXPRESS2012;Initial Catalog=ComputerDeviceData;User ID=sa;Password=nhat123456;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<About>(entity =>
        {
            entity.HasKey(e => e.AboutId).HasName("PK__About__717FC93CEC8203F0");

            entity.ToTable("About");

            entity.Property(e => e.AboutAddress)
                .HasMaxLength(255)
                .HasDefaultValue("");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.FacebookLink)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.InstagramLink)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Introduce)
                .HasMaxLength(1000)
                .HasDefaultValue("");
            entity.Property(e => e.Phone)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.TwitterLink)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.UrlConnectMessager)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.YoutubeLink)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasDefaultValue("");
        });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Account__1788CC4CC47850A2");

            entity.ToTable("Account");

            entity.Property(e => e.UserId)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.DeliverAddress)
                .HasMaxLength(255)
                .HasDefaultValue("");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.Gender).HasDefaultValue(false);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.UserStatus).HasDefaultValue(true);
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Customer).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Account_Customers");

            entity.HasOne(d => d.Group).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Account_GroupAccount");  
            
            entity.HasOne(d => d.Member).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.MemberLevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_account_memberlevel");
        });

        modelBuilder.Entity<MemberLevel>(entity =>
        {
            entity.HasKey(e => e.MemberLevelId).HasName("PK__MemberLe__E2DEB0C866A74DE3");
            entity.Property(e => e.LevelName).HasMaxLength(255);
            entity.Property(e => e.LevelDescription).HasMaxLength(4000);
            entity.Property(e => e.LevelDiscount);
            entity.Property(e => e.Limit);
            entity.Property(e => e.ImageUrl);
        });

        modelBuilder.Entity<ActivateCode>(entity =>
        {
            entity.HasKey(e => e.ActivateCodeId).HasName("PK__Activate__B549DDDAEF3EC87B");

            entity.ToTable("ActivateCode");

            entity.Property(e => e.ActivateCodeStatus).HasDefaultValue(false);
            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserId)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.ActivateCodes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ActivateCode_Account");
        });

        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasKey(e => e.ArticleId).HasName("PK__Articles__9C6270E8C8F416D1");

            entity.Property(e => e.ArticleContent).HasMaxLength(4000);
            entity.Property(e => e.ArticleName).HasMaxLength(255);
            entity.Property(e => e.ArticleView);
            entity.Property(e => e.ArticleStatus).HasDefaultValue(true);
            entity.Property(e => e.Avatar)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.SummaryContent).HasMaxLength(2000);
            entity.Property(e => e.UserId)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.Articles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Articles_Account");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("PK__Brands__DAD4F05E548A4FD5");

            entity.Property(e => e.BrandName)
                .HasMaxLength(255)
                .HasDefaultValue("");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Carts__51BCD7B7BF3C8098");

            entity.Property(e => e.CartId)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.CartStatus).HasDefaultValue(false);
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Totalprice).HasDefaultValue(0.0);
            entity.Property(e => e.UserId)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_carts_account");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("PK__CartItem__488B0B0A643FFC7D");

            entity.Property(e => e.CartId)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.ImageProduct)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.Price).HasDefaultValue(0.0);
            entity.Property(e => e.ProductId)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.ProductName)
                .HasMaxLength(255)
                .HasDefaultValue("");
            entity.Property(e => e.Total).HasDefaultValue(0.0);

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_cartitem_cart");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_cartitem_product");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0BBDA610E9");

            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .HasDefaultValue("");
            entity.Property(e => e.CategorySymbol)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__C3B4DFCA434D4C3E");

            entity.Property(e => e.CommentContent).HasMaxLength(500);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("('')")
                .HasColumnType("datetime");
            entity.Property(e => e.ProductId)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.ReplyComment)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.Product).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Products");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Comments_Account");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64D8160B8B7F");

            entity.Property(e => e.CustomerId)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.DeliverAddress).HasMaxLength(255);
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FavouriteList>(entity =>
        {
            entity.HasKey(e => e.FavouriteId).HasName("PK__Favourit__5944B59A98D1E411");

            entity.ToTable("FavouriteList");

            entity.Property(e => e.ProductId)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.UserId)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.Product).WithMany(p => p.FavouriteLists)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_FavouriteList_Products");

            entity.HasOne(d => d.User).WithMany(p => p.FavouriteLists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_FavouriteList_UserId");
        });

        modelBuilder.Entity<GroupAccount>(entity =>
        {
            entity.HasKey(e => e.GroupId).HasName("PK__GroupAcc__149AF36AEDA8CED5");

            entity.ToTable("GroupAccount");

            entity.Property(e => e.GroupName).HasMaxLength(255);
            entity.Property(e => e.GroupSymbol)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__Images__7516F70C9A26DB80");

            entity.Property(e => e.ImageUrl)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.ProductId)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.Product).WithMany(p => p.Images)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Images_Products");
        });

      

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__C3905BCF380998BF");

            entity.Property(e => e.OrderId)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Note)
                .HasDefaultValue("")
                .HasColumnType("ntext");
            entity.Property(e => e.TotalPrice).HasDefaultValue(0.0);

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Customers");

            entity.HasOne(d => d.Payment).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_PaymentMethod");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("fk_Orders_OrderStatus");

            entity.HasOne(d => d.Voucher).WithMany(p => p.Orders)
                .HasForeignKey(d => d.VoucherId)
                .HasConstraintName("FK_Orders_Voucher");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.OrderStatusId).HasName("PK__OrderSta__BC674CA1B4013261");

            entity.ToTable("OrderStatus");

            entity.Property(e => e.OrderStatusName).HasMaxLength(255);
            entity.Property(e => e.OrderStatusSymbol)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<OrdersDetail>(entity =>
        {
            entity.HasKey(e => e.OrdersDetailId).HasName("PK__OrdersDe__ADE3F6D7E1EED57D");

            entity.ToTable("OrdersDetail");

            entity.Property(e => e.OrderId)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.ProductId)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.Order).WithMany(p => p.OrdersDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ordersdetail_order");

            entity.HasOne(d => d.Product).WithMany(p => p.OrdersDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ordersdetail_products");
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__PaymentM__9B556A38285F01C4");

            entity.ToTable("PaymentMethod");

            entity.Property(e => e.PaymentName)
                .HasMaxLength(255)
                .HasDefaultValue("Thanh toán sau khi nhận hàng");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6CDD971A750");

            entity.Property(e => e.ProductId)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Avatar)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DescriptionSummary)
                .HasMaxLength(1000)
                .HasDefaultValue("");
            entity.Property(e => e.Price).HasDefaultValue(0.0);
            entity.Property(e => e.ProductDescription)
                .HasMaxLength(2000)
                .HasDefaultValue("");
            entity.Property(e => e.ProductName).HasMaxLength(255);
            entity.Property(e => e.Stock).HasDefaultValue(true);
            entity.Property(e => e.UserId)
                .HasMaxLength(15)
                .IsUnicode(false);

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .HasConstraintName("fk_Products_Brands");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Categories");
        });

        modelBuilder.Entity<Slider>(entity =>
        {
            entity.HasKey(e => e.SlideId).HasName("PK__Slider__9E7CB650B19A994A");

            entity.ToTable("Slider");

            entity.Property(e => e.ProductName);
            entity.Property(e => e.SliderTitle);
            entity.Property(e => e.DescriptionSlide);
            
            entity.Property(e => e.ProductLink)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.SlideImage)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasDefaultValue("");
        });

        modelBuilder.Entity<Specification>(entity =>
        {
            entity.HasKey(e => e.SpecificationId).HasName("PK__Specific__A384CDFD4177B8A8");

            entity.Property(e => e.SpecificationDetail)
                .HasMaxLength(500)
                .HasDefaultValue("");
            entity.Property(e => e.SpecificationLabel)
                .HasMaxLength(255)
                .HasDefaultValue("");
            entity.Property(e => e.ProductId)
             .HasMaxLength(15)
             .IsUnicode(false);
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.VoucherId).HasName("PK__Voucher__3AEE79217451F425");

            entity.ToTable("Voucher");

            entity.Property(e => e.CreateAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Discount).HasDefaultValue(0);
            entity.Property(e => e.NumberOfTimesEXE);
            entity.Property(e => e.EndAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.VoucherCode)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.VoucherStatus).HasDefaultValue(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    public async Task<Account> GetAccountByUsernameAndPassword(string username, string passwordHash)
    {
        return Accounts.FirstOrDefault(ac => ac.Username.Equals(username.ToLower()) && ac.PasswordHash.Equals(passwordHash)) ?? null;
    }
}
