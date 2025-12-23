using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using SikayetProjesi.Data;
using SikayetProjesi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabaný Servisi
builder.Services.AddDbContext<UygulamaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. MVC ve Login Servisleri
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/GirisYap";
    });

var app = builder.Build();

// 3. Veritabaný Oluþturma ve Admin Ekleme (Otomatik)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<UygulamaDbContext>();
    context.Database.EnsureCreated(); // Veritabanýný kurar

    if (!context.Admins.Any()) // Admin yoksa ekler
    {
        context.Admins.Add(new Admin { KullaniciAdi = "admin", Sifre = "Yasinefe61" });
        context.SaveChanges();
    }
}

// 4. Standart Ayarlar
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); // Kimlik Doðrulama
app.UseAuthorization();  // Yetkilendirme

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();