using Microsoft.EntityFrameworkCore;
using SikayetProjesi.Models;

namespace SikayetProjesi.Data
{
    public class UygulamaDbContext : DbContext
    {
        public UygulamaDbContext(DbContextOptions<UygulamaDbContext> options) : base(options) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Bildirim> Bildirimler { get; set; }
    }
}