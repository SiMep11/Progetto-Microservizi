using Approvvigionamenti.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace Approvvigionamenti.Repository
{
    public class ApprovvigionamentiDbContext(DbContextOptions<ApprovvigionamentiDbContext> dbContextOptions) : DbContext(dbContextOptions)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fornitore>()
                .HasMany(f => f.ProdottiForniti)
                .WithOne(p => p.Fornitore!)
                .HasForeignKey(p => p.FornitoreId)
                .OnDelete(DeleteBehavior.Cascade);

            // Un fornitore con ordini a suo carico non deve sparire per sbaglio
            modelBuilder.Entity<OrdineFornitore>()
                .HasOne(o => o.Fornitore)
                .WithMany()
                .HasForeignKey(o => o.FornitoreId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ipotesi di progetto: un solo fornitore per tipo di prodotto.
            // L'indice unico fa rispettare l'ipotesi al database invece di lasciarla come convenzione non scritta.
            modelBuilder.Entity<ProdottoFornito>()
                .HasIndex(p => p.ProdottoId)
                .IsUnique();

            // Senza precisione esplicita EF avvisa che i decimal verranno troncati
            modelBuilder.Entity<ProdottoFornito>().Property(p => p.PrezzoAcquisto).HasPrecision(18, 2);
            modelBuilder.Entity<OrdineFornitore>().Property(o => o.PrezzoUnitario).HasPrecision(18, 2);
            modelBuilder.Entity<TransactionalOutbox>().HasKey(e => e.Id);
            modelBuilder.Entity<TransactionalOutbox>().Property(e => e.Id).ValueGeneratedOnAdd();
        }

        public DbSet<Fornitore> Fornitori { get; set; }
        public DbSet<ProdottoFornito> ProdottiForniti { get; set; }
        public DbSet<OrdineFornitore> OrdiniFornitore { get; set; }
        public DbSet<TransactionalOutbox> TransactionalOutboxList { get; set; }
    }
}