using Microsoft.JSInterop;

namespace Frontend.Services;

public class LocalStorageService : IStorageService
{
    private readonly IJSRuntime jsRuntime;

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }
    public async Task SetItem(string key, string value)
    {
        try
        {
            await this.jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
        }
        catch (Exception)
        {
            // ignore
        }
    }

    public async Task<string> GetItem(string key)
    {
        try
        {
            return await this.jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
        }
        catch (Exception)
        {
            return string.Empty;
        }

    }

    public async Task RemoveItem(string key)
    {
        try
        {
            await this.jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }
        catch
        {
            // ignore
        }
    }
}