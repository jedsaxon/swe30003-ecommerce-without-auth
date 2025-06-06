using DataAccess.Repositories;
using DataAccess.Repositories.Sqlite;
using Services;
using WebApplication1;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<SqliteDataAccess>(s =>
{
    var c = s.GetService<IConfiguration>();
    if (c is null) throw new NullReferenceException("IConfiguration is null, cannot get connection string");
    return new SqliteDataAccess(c.GetConnectionString("Sqlite") ?? string.Empty);
});
builder.Services.AddTransient<IProductRepository, SqliteProductsRepository>();
builder.Services.AddTransient<IUserRepository, SqliteUserRepository>();
builder.Services.AddTransient<IOrderRepository, SqliteOrderRepository>();
builder.Services.AddTransient<ProductsService>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<ShippingService>();
builder.Services.AddScoped<OrderService>();

var app = builder.Build();

await app.InitTables();
await app.InitAdminAccount();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseExceptionHandler("/Error");

    await app.SeedDatabase();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/");

app.Run();