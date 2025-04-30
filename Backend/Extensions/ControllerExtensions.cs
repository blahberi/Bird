using Backend.Core;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Extensions;

namespace Backend.Extensions;

public static class ControllerExtensions
{
    public static async Task<Result<int, Error>> AuthorizeUser(this ControllerBase controller)
    {
        return controller.HttpContext.Items
                .ContainsKey("User")
                .OkIf(contains => contains, new Error(
                    "User not authorized", 
                    new Dictionary<string, string>(),
                    ErrorType.Unauthorized))
                .AndThen(_ => ((int)controller.HttpContext.Items["User"]!).ToOkResult());
    }
}