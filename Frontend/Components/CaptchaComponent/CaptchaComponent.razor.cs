using Frontend.Models;
using Frontend.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shared;
using Shared.DTOs.Captcha;

namespace Frontend.Components.CaptchaComponent;

public partial class CaptchaComponent : ComponentBase
{
    private bool isCaptchaPopupVisible;
    private bool isCaptchaCompleted;

    private string captchaImageUrl = string.Empty;
    private CaptchaDto captcha = new();
    private CaptchaAnswerModel captchaAnswerModel = new();

    private string errorMessage = string.Empty;

    [Inject]
    private ICaptchaService CaptchaService { get; set; } = default!;

    public bool IsHuman => CaptchaService.IsHuman;

    protected override async Task OnInitializedAsync()
    {
        CaptchaService.OnCaptchaExpiration += OnCaptchaExpired;
        CaptchaService.OnVerificationExpiration += OnVerificationExpired;
    }

    private async Task GetCaptcha()
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

    private async Task ValidateCaptcha(EditContext arg)
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

    private async Task ShowCaptchaPopup()
    {
        if (!isCaptchaCompleted)
        {
            isCaptchaPopupVisible = true;
            await GetCaptcha();
        }
    }

    private void CloseCaptchaPopup()
    {
        isCaptchaPopupVisible = false;
    }

    private async void OnCaptchaExpired(object? sender, EventArgs e)
    {
        await GetCaptcha();
        StateHasChanged();
    }

    private void OnVerificationExpired(object? sender, EventArgs e)
    {
        isCaptchaCompleted = false;
        StateHasChanged();
    }
}