using ComputerDeviceShopping.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;

public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _requiredRoles;
    private static ComputerDeviceDataContext _context = new ComputerDeviceDataContext();
    public CustomAuthorizeAttribute(params string[] requiredRoles)
    {
        _requiredRoles = requiredRoles;
    }
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        bool flag = false;
        var userLogged = context.HttpContext.Session.GetString("UserLogged");

        if (string.IsNullOrEmpty(userLogged))
        {
            context.Result = new RedirectToActionResult("Login", "Account", new { area = "" });
            return;
        }
        var account = Newtonsoft.Json.JsonConvert.DeserializeObject<Account>(userLogged) ?? null;
        // Lấy các claim của người dùng hiện tại
        foreach (var role in _requiredRoles)
        {
            if (account.GroupId == getIDAccountGroup(role.Trim()))
            {
                flag = true;
            }
        }
        if (!flag)
        {
            context.Result = new ForbidResult(); 
        }
    }
    private int getIDAccountGroup(string name)
    {
        List<GroupAccount> accountGroupList = _context.GroupAccounts.ToList();
        int result = 0;
        foreach (var accountGroup in accountGroupList)
        {
            if (name.ToLower().Equals(accountGroup.GroupName))
                result = accountGroup.GroupId;
        }
        return result;
    }
}
