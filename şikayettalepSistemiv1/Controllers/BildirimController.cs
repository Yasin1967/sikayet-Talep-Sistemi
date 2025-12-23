using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SikayetProjesi.Data;
using SikayetProjesi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Text; // Excel (CSV) oluşturmak için gerekli

namespace SikayetProjesi.Controllers
{
    // GENEL KURAL: Bu Controller korumalıdır, varsayılan olarak sadece Admin girebilir.
    [Authorize]
    public class BildirimController : Controller
    {
        private readonly UygulamaDbContext _context;

        public BildirimController(UygulamaDbContext context)
        {
            _context = context;
        }

        // 1. YÖNETİM PANELİ / LİSTELEME (Sadece Admin)
        public async Task<IActionResult> Index()
        {
            // Veritabanındaki tüm bildirimleri getir
            var bildirimler = await _context.Bildirimler.ToListAsync();
            return View(bildirimler);
        }

        // 2. ŞİKAYET EKLEME SAYFASI (Herkes Görebilir)
        [AllowAnonymous] // Korumayı kaldırıyoruz, vatandaş giriş yapmadan görsün
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 3. ŞİKAYETİ KAYDETME İŞLEMİ (Herkes Gönderebilir)
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create(Bildirim bildirim)
        {
            // Formdan gelen veriyi hazırla
            bildirim.Durum = "Bekliyor"; // İlk oluştuğunda durumu bu olsun
            bildirim.OlusturulmaTarihi = DateTime.Now;

            // Veritabanına ekle
            _context.Add(bildirim);
            await _context.SaveChangesAsync();

            // İşlem bitince Teşekkür sayfasına yönlendir
            return RedirectToAction("Basarili");
        }

        // 4. BAŞARILI SAYFASI (Herkes Görebilir)
        [AllowAnonymous]
        public IActionResult Basarili()
        {
            return View();
        }

        // --- YENİ EKLENEN ÖZELLİKLER (SADECE ADMİN) ---

        // 5. DETAY İNCELEME SAYFASI
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var bildirim = await _context.Bildirimler.FindAsync(id);

            if (bildirim == null) return NotFound();

            return View(bildirim);
        }

        // 6. DURUMU "TAMAMLANDI" YAPMA
        [HttpPost]
        public async Task<IActionResult> DurumGuncelle(int id)
        {
            var bildirim = await _context.Bildirimler.FindAsync(id);
            if (bildirim != null)
            {
                bildirim.Durum = "Tamamlandı";
                await _context.SaveChangesAsync();
            }
            // İşlem bitince listeye geri dön
            return RedirectToAction(nameof(Index));
        }

        // 7. EXCEL (CSV) OLARAK İNDİRME
        public IActionResult ExportToExcel()
        {
            var bildirimler = _context.Bildirimler.ToList();

            var builder = new StringBuilder();
            // Başlık satırı
            builder.AppendLine("ID;Tarih;Baslik;Aciklama;Durum");

            foreach (var item in bildirimler)
            {
                // Satırları ekle (Noktalı virgül ile ayırıyoruz)
                builder.AppendLine($"{item.Id};{item.OlusturulmaTarihi};{item.Baslik};{item.Aciklama};{item.Durum}");
            }

            // Dosyayı indir (UTF-8 formatında Türkçe karakter destekli)
            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "bildirimler.csv");
        }
    }
}