using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // DEBUG
        optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
        //
        base.OnConfiguring(optionsBuilder);
    }


    public virtual DbSet<CustomerEntity> Customers { get; set; }
    public virtual DbSet<ProjectEntity> Projects { get; set; }

}