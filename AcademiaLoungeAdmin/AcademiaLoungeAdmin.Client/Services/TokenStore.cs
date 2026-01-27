using Microsoft.JSInterop;

namespace AcademiaLoungeAdmin.Client.Services;

public class TokenStore
{
    private const string Key = "auth_token";
    private readonly IJSRuntime _js;

    public TokenStore(IJSRuntime js) => _js = js;

    public ValueTask SetAsync(string token) =>
        _js.InvokeVoidAsync("localStorage.setItem", Key, token);

    public ValueTask<string?> GetAsync() =>
        _js.InvokeAsync<string?>("localStorage.getItem", Key);

    public ValueTask RemoveAsync() =>
        _js.InvokeVoidAsync("localStorage.removeItem", Key);
}
