using BlazorDashboard.Components;
using BlazorDashboard.Services;
using BlazorDashboard.Shared;

var builder = WebApplication.CreateBuilder(args);

SQLitePCL.Batteries.Init();

// Add services to the container.
builder.Services.AddScoped<UserSession>();
builder.Services.AddScoped(sp =>
{
    var client = new HttpClient { BaseAddress = new Uri("http://localhost:5145") };
    client.DefaultRequestHeaders.Add("X-API-KEY", "super-secret-api-key-12345");
    return client;
});
builder.Services.AddScoped<AuthService>();
builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => { options.DetailedErrors = true; });


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}


app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
