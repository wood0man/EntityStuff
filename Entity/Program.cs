using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Entity.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<EntityContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EntityContext") ?? throw new InvalidOperationException("Connection string 'EntityContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(1); });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseSession();


app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=login}/{id?}");

app.Run();
