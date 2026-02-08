using System.Net.Http.Headers;

namespace AcademiaLoungeAdmin.Client.Services;

//Classe bara (injeta Bearer automaticamente)
public class ApiAuthHandler : DelegatingHandler
{
    private readonly TokenStore _tokenStore;

    public ApiAuthHandler(TokenStore tokenStore)
    {
        _tokenStore = tokenStore;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenStore.GetAsync();

        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}
