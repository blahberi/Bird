using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.DTOs;
using Shared.DTOs.Captcha;

namespace Backend.Controllers;

[ApiController]
[Route("api/captcha")]
public class CaptchaController : ControllerBase
{
    private readonly ICaptchaService captchaService;

    public CaptchaController(ICaptchaService captchaService)
    {
        this.captchaService = captchaService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCaptcha()
    {
        string code = this.captchaService.GenerateCaptchaCode();
        Result<byte[]> result = await this.captchaService.GenerateCaptchaImage(code);
        if (!result.Success || result.Value == null)
        {
            return this.BadRequest(new ErrorResponse{ Error = result.Error });
        }
        
        string image = Convert.ToBase64String(result.Value);
        string token = this.captchaService.GenerateCaptchaToken(code);
        return this.Ok(new CaptchaDto{ Image = image, Token = token });
    }
    
    [HttpPost]
    public IActionResult ValidateAnswer(CaptchaAnswer request)
    {
        string token = request.Token;
        string answer = request.Answer;
        
        if (!this.captchaService.ValidateAnswer(token, answer, out string? verificationToken) || 
            verificationToken == null)
        {
            return this.BadRequest(new ErrorResponse{ Error = "Incorrect answer" });
        }
        
        return this.Ok(new VerificationTokenDto{Token = verificationToken});
    }
}