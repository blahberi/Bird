namespace Frontend.Services;

public interface IStorageService
{
    public Task SetItem(string key, string value);
    public Task<string> GetItem(string key);
    public Task RemoveItem(string key);
}