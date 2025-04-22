using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Frontend;
using Frontend.Services;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

string? apiBaseAddress = builder.Configuration.GetValue<string>("ApiSettings:BaseUrl");
if (apiBaseAddress is null)
{
    throw new InvalidOperationException("ApiSettings:BaseUrl is not set in appsettings.json");
}

builder.Services
    .AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseAddress) });
builder.Services
    .AddScoped<IApiService, ApiService>()
    .AddScoped<IStorageService, LocalStorageService>()
    .AddScoped<IAuthService, AuthService>()
    .AddScoped<IUserService, UserService>()
    .AddScoped<IPostsService, PostsService>()
    .AddScoped<ICommentsService, CommentsService>()
    .AddScoped<ILikesService, LikesService>()
    .AddTransient<ICaptchaService, CaptchaService>();

await builder.Build().RunAsync();