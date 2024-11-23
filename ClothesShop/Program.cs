var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // ����� ����� ������
    options.Cookie.HttpOnly = true; // ������ �� XSS
    options.Cookie.IsEssential = true; // ������ ����� ��� ������ ����������

});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
       name: "women_index",
       pattern: "Women",
       defaults: new { controller = "Women", action = "Index" });

app.MapControllerRoute(
       name: "men_index",
       pattern: "Men",
       defaults: new { controller = "Men", action = "Index" });

app.MapControllerRoute(
       name: "cart_index",
       pattern: "Cart",
       defaults: new { controller = "Cart", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
