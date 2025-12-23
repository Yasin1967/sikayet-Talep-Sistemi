namespace SikayetProjesi.Models
{
    public class Bildirim
    {
        public int Id { get; set; }
        public string Baslik { get; set; } = string.Empty;
        public string Aciklama { get; set; } = string.Empty;
        public string Durum { get; set; } = "Bekliyor";
        public DateTime OlusturulmaTarihi { get; set; } = DateTime.Now;
    }
}