using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Magazzino.Repository
{
    public class MagazzinoDbContext(DbContextOptions<MagazzinoDbContext> dbContextOptions) : DbContext(dbContextOptions)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Prodotto>().HasKey(x => x.Id);
            modelBuilder.Entity<Prodotto>().Property(e => e.Id).ValueGeneratedOnAdd();
        }
        public DbSet<Prodotto> Prodotti { get; set; }
    }
}
