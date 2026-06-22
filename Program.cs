using email_attachment_orders.Services;
using DotNetEnv;
using Microsoft.Graph;

var envPath = Path.Combine(
    AppContext.BaseDirectory,
    "..", "..", "..",
    ".env"
);

Env.Load(envPath);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddSingleton<GraphAuthService>();

builder.Services.AddSingleton<GraphServiceClient>(sp =>
{
    var authService = sp.GetRequiredService<GraphAuthService>();
    return authService.getClient();
});

builder.Services.AddScoped<ExcelParseService>();
builder.Services.AddScoped<EmailService>();

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