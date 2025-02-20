using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Contexts;

public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {        
        base.OnConfiguring(optionsBuilder);               
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Primärnyckel för många-till-många-tabellen
        modelBuilder.Entity<ProjectEmployee>()
            .HasKey(pe => new { pe.ProjectId, pe.EmployeeId });


        // Koppling mellan ProjectEmployee och Project
        modelBuilder.Entity<ProjectEmployee>()
            .HasOne(pe => pe.Project)
            .WithMany(p => p.ProjectEmployees)
            .HasForeignKey(pe => pe.ProjectId);


        // Koppling mellan ProjectEmployee och Employee
        modelBuilder.Entity<ProjectEmployee>()
            .HasOne(pe => pe.Employee)
            .WithMany(e => e.ProjectEmployees)
            .HasForeignKey(pe => pe.EmployeeId);
    }


    public virtual DbSet<CustomerEntity> Customers { get; set; }
    public virtual DbSet<ProjectEntity> Projects { get; set; }
    public virtual DbSet<EmployeeEntity> Employees { get; set; }
    public virtual DbSet<ProjectEmployee> ProjectEmployees { get; set; }
}