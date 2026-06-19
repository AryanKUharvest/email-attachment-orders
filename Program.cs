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

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddSingleton<GraphAuthService>();
builder.Services.AddSingleton<GraphServiceClient>(sp =>
{
    var authService = sp.GetRequiredService<GraphAuthService>();
    return authService.getClient();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
