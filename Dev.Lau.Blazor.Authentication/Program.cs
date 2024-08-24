using Azure.Core;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using Azure.Security.KeyVault.Secrets;
using Dev.Lau.Blazor.Authentication.Components;
using Dev.Lau.Blazor.Authentication.HostedServices;
using Dev.Lau.Blazor.Authentication.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Teqit.Extensions.Seq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSeq(useDefaultDockerContainerUrl: true);
builder.Services.AddHealthChecks();

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services
    .AddControllersWithViews()
    .AddMicrosoftIdentityUI();

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(x =>
    {
        builder.Configuration.Bind("AzureAd", x);
        x.Prompt = "select_account";
    });

builder.Services.AddAuthorization(options =>
    {
        options.FallbackPolicy = options.DefaultPolicy;
        options.AddPolicy("Weather.Read", policy =>
        {
            policy.RequireRole("Weather.Read");
        });
        options.AddPolicy("Counter.Read", policy =>
        {
            policy.RequireRole("Counter.Read");
        });
    });

builder.Services.AddScoped<ServiceBusClient>(x =>
{
    var clientOptions = new ServiceBusClientOptions
    {
        TransportType = ServiceBusTransportType.AmqpWebSockets
    };

    var client = x.GetRequiredService<SecretClient>();

    KeyVaultSecret secret = client.GetSecret("Azb");

    string secretValue = secret.Value;

    return new ServiceBusClient(secretValue, clientOptions);
});
builder.Services.AddScoped<ServiceBusService>();

builder.Services.AddHostedService<ServiceBusListener>();

builder.Services.AddScoped<SecretClient>(x =>
{
    SecretClientOptions options = new()
    {
        Retry =
        {
            Delay= TimeSpan.FromSeconds(2),
            MaxDelay = TimeSpan.FromSeconds(16),
            MaxRetries = 5,
            Mode = RetryMode.Exponential
         }
    };
    return new SecretClient(new Uri("https://teqit-playground-dev.vault.azure.net/"), new DefaultAzureCredential(), options);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();
app.UseHealthChecks("/health");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();