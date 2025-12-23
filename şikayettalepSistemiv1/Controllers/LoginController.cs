using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SikayetProjesi.Data; // Proje adın farklıysa burayı düzelt
using SikayetProjesi.Models;

namespace SikayetProjesi.Controllers
{
    public class LoginController : Controller
    {
        private readonly UygulamaDbContext _context;

        public LoginController(UygulamaDbContext context)
        {
            _context = context;
        }

        // Giriş Sayfasını Açan Kod
        [HttpGet]
        public IActionResult GirisYap()
        {
            return View();
        }

        // Giriş Butonuna Basınca Çalışan Kod
        [HttpPost]
        public async Task<IActionResult> GirisYap(string kAdi, string sifre)
        {
            var admin = _context.Admins.FirstOrDefault(x => x.KullaniciAdi == kAdi && x.Sifre == sifre);

            if (admin != null)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, kAdi) };
                var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(userIdentity));

                // Başarılıysa Ana Sayfaya git
                return RedirectToAction("Index", "Bildirim");
            }

            ViewBag.Hata = "Kullanıcı adı veya şifre hatalı!";
            return View();
        }

        // Çıkış Yapma Kodu
        public async Task<IActionResult> CikisYap()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("GirisYap");
        }
    }
}