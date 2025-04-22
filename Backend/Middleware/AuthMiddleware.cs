using Backend.Services;

namespace Backend.Middleware;

public class AuthMiddleware : IMiddleware
{
    private readonly IAuthService authService;
    private readonly ICaptchaService captchaService;

    public AuthMiddleware(IAuthService authService, ICaptchaService captchaService)
    {
        this.authService = authService;
        this.captchaService = captchaService;
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        string? userToken = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
        string? verificationToken = context.Request.Headers["Verification"].FirstOrDefault();
        
        if (userToken != null)
        {
            this.AttachUserToContext(context, userToken);
            this.RefreshUserToken(context, userToken);
        }

        if (verificationToken != null)
        {
            this.AttachVerificationToContext(context, verificationToken);
        }

        await next(context);
    }

    private void AttachUserToContext(HttpContext context, string token)
    {
        if (this.authService.ValidateToken(token, out int? userId))
        {
            context.Items["User"] = userId;
        }
    }
    
    private void RefreshUserToken(HttpContext context, string token)
    {
        string refreshedToken = this.authService.RefreshToken(token);
        context.Response.Headers.Append("Authorization", $"Bearer {refreshedToken}");
    }

    private void AttachVerificationToContext(HttpContext context, string verificationToken)
    {
        context.Items["IsHuman"] = this.captchaService.ValidateVerificationToken(verificationToken);
    }
}