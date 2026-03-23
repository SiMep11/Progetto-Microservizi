using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Ordini.Repository.Models;

namespace Ordini.Repository
{
    public class OrdiniDbContext(DbContextOptions<OrdiniDbContext> dbContextOptions) : DbContext(dbContextOptions)
    {
        // Queste diventeranno le tue tabelle nel database!
        public DbSet<Ordine> Ordini { get; set; }
        public DbSet<ProdottoOrdinato> ProdottiOrdinati { get; set; }

        // Configurazione aggiuntiva (Opzionale ma consigliata per relazioni chiare)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Diciamo esplicitamente a EF Core come sono collegati Ordine e ProdottoOrdinato
            modelBuilder.Entity<ProdottoOrdinato>()
                .HasOne(p => p.Ordine)
                .WithMany(o => o.Prodotti)
                .HasForeignKey(p => p.OrdineId)
                .OnDelete(DeleteBehavior.Cascade); // Se elimino un ordine, elimino anche le sue righe

            // Diciamo a EF Core che il totale e il prezzo hanno i decimali (evita warning)
            modelBuilder.Entity<Ordine>()
                .Property(o => o.Totale)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ProdottoOrdinato>()
                .Property(p => p.PrezzoUnitario)
                .HasColumnType("decimal(18,2)");
        }
    }
}
