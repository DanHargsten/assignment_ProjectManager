using Business.Interfaces;
using Business.Services;
using Data.Contexts;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Presentation.ConsoleApp.Dialogs;
using Presentation.ConsoleApp.Menus;

var services = new ServiceCollection()
    .AddDbContext<DataContext>(options => options
    .UseSqlServer(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=E:\Projects\Databaser\ProjectManager\Data\Databases\pm_local_database.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=True"))
    .AddScoped<ICustomerService, CustomerService>()
    .AddScoped<IProjectService, ProjectService>()
    .AddScoped<ICustomerRepository, CustomerRepository>()
    .AddScoped<IProjectRepository, ProjectRepository>()
    .AddScoped<MainMenu>()
    .AddScoped<CustomerMenu>()
    .AddScoped<ProjectMenu>()
    .AddScoped<CustomerDialog>()
    .AddScoped<CreateProjectDialog>()
    .BuildServiceProvider();

var menu = services.GetRequiredService<MainMenu>();
await menu.ShowMainMenuAsync();