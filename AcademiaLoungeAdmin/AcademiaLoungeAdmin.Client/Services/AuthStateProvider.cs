//controle de sessão do Blazor)

using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace AcademiaLoungeAdmin.Client.Services;

public class AppAuthStateProvider : AuthenticationStateProvider
{
    private readonly TokenStore _tokenStore;

    public AppAuthStateProvider(TokenStore tokenStore)
    {
        _tokenStore = tokenStore;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _tokenStore.GetAsync();

        if (string.IsNullOrWhiteSpace(token))
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        // Não vamos validar assinatura aqui (quem valida é a API).
        // Vamos apenas criar identidade “logada” pra habilitar rotas.
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "admin") // opcional (pode ficar genérico)
        }, authenticationType: "jwt");

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task SignInAsync(string token)
    {
        await _tokenStore.SetAsync(token);

        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, "admin")
        }, authenticationType: "jwt");

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
    }

    public async Task SignOutAsync()
    {
        await _tokenStore.RemoveAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()))
        ));
    }
}
