using ComputerDeviceShopping.Services;
using ComputerDeviceShopping.ViewModel;
using Microsoft.AspNetCore.Authentication.Cookies;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
       .AddCookie(options =>
       {
           options.LoginPath = "/Account/Login"; // Path to login
           options.AccessDeniedPath = "/Account/AccessDenied"; // Path to access denied page
       });
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor(); // Thêm dịch vụ HttpContextAccessor
//Thêm dịch vụ cho getcartsession để tái sử dụng code
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<IUserLoggedService, UserLoggedService>();
builder.Services.AddSession();

builder.Services.AddSingleton<IVnPayService, VnPayService>();

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<IEmailSender, EmailSender>();


builder.Services.AddDistributedMemoryCache(); // Sử dụng bộ nhớ trong để lưu trữ session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thời gian tồn tại của session
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSignalR();


builder.Services.AddControllers()
     .AddJsonOptions(options =>
     {
         options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
     });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller}/{action}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
//Map hub để dùng cho việc xử lí bình luận thời gian thực
app.MapHub<ComputerDeviceShopping.Common.CommentHub>("/CommentHub");

app.Run();
