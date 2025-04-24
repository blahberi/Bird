using Frontend.Models;
using Frontend.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared;
using Shared.DTOs.Captcha;

namespace Frontend.Components.CaptchaComponent;

public partial class CaptchaComponentBase : ComponentBase
{
    protected bool isCaptchaPopupVisible;
    protected bool isCaptchaCompleted;

    protected string captchaImageUrl = string.Empty;
    protected CaptchaDto captcha = new();
    protected CaptchaAnswerModel captchaAnswerModel = new();

    protected string errorMessage = string.Empty;

    [Inject]
    protected ICaptchaService CaptchaService { get; set; } = default!;

    public bool IsHuman => CaptchaService.IsHuman;

    protected override async Task OnInitializedAsync()
    {
        CaptchaService.OnCaptchaExpiration += OnCaptchaExpired;
        CaptchaService.OnVerificationExpiration += OnVerificationExpired;
    }

    protected async Task GetCaptcha()
    {
        Result<CaptchaDto> result = await CaptchaService.GetCaptchaAsync();
        if (!result.Success || result.Value == null)
        {
            return;
        }

        captcha = result.Value;
        captchaImageUrl = $"data:image/png;base64,{captcha.Image}";
        CaptchaService.SetCaptchaToken(captcha.Token);
    }

    protected async Task ValidateCaptcha(EditContext arg)
    {
        Result<VerificationTokenDto> result = await CaptchaService.VerifyCaptchaAsync(captcha.Token, captchaAnswerModel.Answer);
        if (!result.Success || result.Value == null)
        {
            errorMessage = "Invalid answer...";
            return;
        }
        string verificationToken = result.Value.Token;
        CaptchaService.SetVerificationToken(verificationToken);
        isCaptchaCompleted = true;
        CloseCaptchaPopup();
    }

    protected async Task ShowCaptchaPopup()
    {
        if (!isCaptchaCompleted)
        {
            isCaptchaPopupVisible = true;
            await GetCaptcha();
        }
    }

    protected void CloseCaptchaPopup()
    {
        isCaptchaPopupVisible = false;
    }

    protected async void OnCaptchaExpired(object? sender, EventArgs e)
    {
        await GetCaptcha();
        StateHasChanged();
    }

    protected void OnVerificationExpired(object? sender, EventArgs e)
    {
        isCaptchaCompleted = false;
        StateHasChanged();
    }
}
