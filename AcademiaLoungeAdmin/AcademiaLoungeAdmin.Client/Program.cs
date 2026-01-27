using AcademiaLoungeAdmin.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(_ => new HttpClient
{
    BaseAddress = new Uri("http://localhost:5267")
});

builder.Services.AddScoped<TokenStore>();
builder.Services.AddScoped<ApiClient>();
builder.Services.AddMudServices();

await builder.Build().RunAsync();
