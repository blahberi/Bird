using Backend.Core;
using Backend.Extensions;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.DTOs;
using Shared.DTOs.Captcha;
using Shared.Extensions;

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
        byte[] result = await this.captchaService.GenerateCaptchaImage(code);

        string image = Convert.ToBase64String(result);
        string token = this.captchaService.GenerateCaptchaToken(code);
        return this.Ok(new CaptchaDto{ Image = image, Token = token });
    }
    
    [HttpPost]
    public IActionResult ValidateAnswer(CaptchaAnswer request)
    {
        string token = request.Token;
        string answer = request.Answer;
        
        return this.captchaService.ValidateAnswer(token, answer, out string? verificationToken)
            .Map(result => new VerificationTokenDto{Token = result})
            .ToActionResult();
    }
}