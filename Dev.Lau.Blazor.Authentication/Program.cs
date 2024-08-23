using Dev.Lau.Blazor.Authentication.Components;
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