using Business.Interfaces;
using Business.Services;
using Data.Contexts;
using Data.Interfaces;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Presentation.ConsoleApp.Dialogs.CustomerDialogs;
using Presentation.ConsoleApp.Dialogs.EmployeeDialogs;
using Presentation.ConsoleApp.Dialogs.ProjectDialogs;
using Presentation.ConsoleApp.Menus;

var services = new ServiceCollection()
    .AddDbContext<DataContext>(options => options
    .UseSqlServer(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=E:\Projects\Databaser\ProjectManager\Data\Databases\pm_database.mdf;Integrated Security=True;Connect Timeout=30;Encrypt=True"))
    .AddScoped<ICustomerService, CustomerService>()
    .AddScoped<IProjectService, ProjectService>()
    .AddScoped<IEmployeeService, EmployeeService>()
    .AddScoped<ICustomerRepository, CustomerRepository>()
    .AddScoped<IProjectRepository, ProjectRepository>()
    .AddScoped<IEmployeeRepository, EmployeeRepository>()
    .AddScoped<IProjectEmployeeRepository, ProjectEmployeeRepository>()
    .AddScoped<MainMenu>()
    .AddScoped<CustomerMenu>()
    .AddScoped<ProjectMenu>()
    .AddScoped<EmployeeMenu>()
    .AddScoped<CreateCustomerDialog>()
    .AddScoped<CreateProjectDialog>()
    .AddScoped<CreateEmployeeDialog>()
    .AddScoped<ViewCustomersDialog>()
    .AddScoped<ViewProjectsDialog>()
    .AddScoped<ViewEmployeesDialog>()
    .AddScoped<UpdateCustomerDialog>()
    .AddScoped<UpdateProjectDialog>()
    .AddScoped<UpdateEmployeeDialog>()
    .AddScoped<DeleteCustomerDialog>()
    .AddScoped<DeleteProjectDialog>()
    .AddScoped<DeleteEmployeeDialog>()
    .BuildServiceProvider();

var menu = services.GetRequiredService<MainMenu>();
await menu.ShowMainMenuAsync();