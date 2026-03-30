using Microsoft.EntityFrameworkCore;
using Pagamenti.Repository.Model;

namespace Pagamenti.Repository 
{
    public class PagamentiDbContext(DbContextOptions<PagamentiDbContext> dbContextOptions) : DbContext(dbContextOptions)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Versamento>().HasKey(x => x.Id);
            modelBuilder.Entity<Versamento>().Property(e => e.Id).ValueGeneratedOnAdd();

            modelBuilder.Entity<TransactionalOutbox>().HasKey(e => new { e.Id });
            modelBuilder.Entity<TransactionalOutbox>().Property(e => e.Id).ValueGeneratedOnAdd();
        }

        public DbSet<Versamento> Versamenti { get; set; }
        public DbSet<TransactionalOutbox> TransactionalOutboxList { get; set; }
    }
}