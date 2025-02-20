using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Data.Contexts;

public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        optionsBuilder.UseSqlServer(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=E:\Projects\Databaser\ProjectManager\Data\Databases\pm_database.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=True");
        //optionsBuilder.UseSqlServer(@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=pm_local_database;Integrated Security=True;Connect Timeout=30;Encrypt=True");


        return new DataContext(optionsBuilder.Options);
    }
}