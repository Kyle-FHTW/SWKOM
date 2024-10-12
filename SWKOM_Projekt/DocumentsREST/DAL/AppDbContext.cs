using Microsoft.EntityFrameworkCore;
using DocumentsREST.DAL.Models;

namespace DocumentsREST.DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<Document> Documents { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Additional configurations, if needed
        }
    }
}