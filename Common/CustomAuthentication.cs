using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

public class CustomAuthentication : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var userLogged = context.HttpContext.Session.GetString("UserLogged");

        if (string.IsNullOrEmpty(userLogged))
        {
            // Người dùng không được xác thực
            context.Result = new RedirectToActionResult("Login", "Account", new {area=""});
        }
        else
        {
            // Người dùng đã được xác thực, có thể thêm thông tin vào context.User nếu cần
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userLogged)
            // Thêm các claim khác nếu cần
        };

            var claimsIdentity = new ClaimsIdentity(claims, "Custom");
            context.HttpContext.User = new ClaimsPrincipal(claimsIdentity);
        }
    }
}

