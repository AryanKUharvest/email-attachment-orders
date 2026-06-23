using email_attachment_orders.Services;
using email_attachment_orders.Data;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;

var envPath = Path.Combine(
    AppContext.BaseDirectory,
    "..", "..", "..",
    ".env"
);

Env.Load(envPath);

var builder = WebApplication.CreateBuilder(args);

// Database
var connectionString =
    Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? throw new Exception("CONNECTION_STRING not found");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    );
});

// MVC
builder.Services.AddControllers();
builder.Services.AddRazorPages();

// Graph API
builder.Services.AddSingleton<GraphAuthService>();

builder.Services.AddSingleton<GraphServiceClient>(sp =>
{
    var authService = sp.GetRequiredService<GraphAuthService>();
    return authService.getClient();
});

// Application Services
builder.Services.AddScoped<ExcelParseService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<OutletMappingService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.MapControllers();

app.Run();