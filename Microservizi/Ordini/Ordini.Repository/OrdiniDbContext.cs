using Microsoft.EntityFrameworkCore;
using Ordini.Repository.Models;

namespace Ordini.Repository
{
    public class OrdiniDbContext(DbContextOptions<OrdiniDbContext> dbContextOptions) : DbContext(dbContextOptions)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Chiavi Primarie esplicite
            modelBuilder.Entity<Ordine>().HasKey(x => x.Id);
            modelBuilder.Entity<ProdottoOrdinato>().HasKey(x => x.Id);

            // 2. Autoincremento esplicito
            modelBuilder.Entity<Ordine>().Property(e => e.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<ProdottoOrdinato>().Property(e => e.Id).ValueGeneratedOnAdd();

            // 3. Relazione scritta su una riga compatta (come fa il prof)
            modelBuilder.Entity<Ordine>().HasMany(p => p.Prodotti).WithOne(p => p.Ordine).HasForeignKey(p => p.OrdineId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ordine>().Property(o => o.Totale).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<ProdottoOrdinato>().Property(p => p.PrezzoUnitario).HasColumnType("decimal(18,2)");
        }

        public DbSet<Ordine> Ordini { get; set; }
        public DbSet<ProdottoOrdinato> ProdottiOrdinati { get; set; }
    }
}