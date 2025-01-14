#region

using DocumentsREST.DAL.Models;
using Microsoft.EntityFrameworkCore;

#endregion

namespace DocumentsREST.DAL;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Document> Documents { get; set; }
}