using Business.Interfaces;
using Business.Services;
using Data.Contexts;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Presentation.ConsoleApp.Menus;

var services = new ServiceCollection()
    .AddDbContext<DataContext>(options => options
    .UseSqlServer(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=E:\Projects\Databaser\ProjectManager\Data\Databases\local_database.mdf;Integrated Security=True;Connect Timeout=30"))
    .AddScoped<ICustomerService, CustomerService>()
    .AddScoped<IProjectService, ProjectService>()
    .AddScoped<ICustomerRepository, CustomerRepository>()
    .AddScoped<IProjectRepository, ProjectRepository>()
    .AddScoped<MainMenu>()
    .AddScoped<CustomerMenu>()
    .AddScoped<ProjectMenu>()
    .BuildServiceProvider();

var menu = services.GetRequiredService<MainMenu>();
await menu.ShowMainMenuAsync();