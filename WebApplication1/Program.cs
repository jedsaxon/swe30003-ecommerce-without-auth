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
builder.Services.AddTransient<ProductsService>();

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

await app.InitTables();

app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/");

app.Run();