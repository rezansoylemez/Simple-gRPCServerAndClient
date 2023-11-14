using gRPCServer.Models;
using Microsoft.EntityFrameworkCore;

public class BaseDbContext : DbContext
{
    public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options)
    {
    }
    public DbSet<User> Users { get; set; }

 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Veritabanı modelini konfigure etme işlemleri buraya eklenebilir.
    }
}
