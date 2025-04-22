namespace Shared.DTOs.Captcha;

public class CaptchaAnswer
{
    public string Token { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}