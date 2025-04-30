using Backend.Core;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Extensions;

namespace Backend.Extensions;

public static class ControllerExtensions
{
    public static int AuthorizeUser(this ControllerBase controller) 
    {
        if (!controller.HttpContext.Items.ContainsKey("User"))
        {
            throw new UnauthorizedAccessException();
        }
        return (int)controller.HttpContext.Items["User"]!;
    }
}