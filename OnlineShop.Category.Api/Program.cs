using OnlineShop.Shared.Common.Bootstraps;

var builder = WebApplication.CreateBuilder(args);
AppBootstrap.AddFeaturesApp(builder, "");
// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
