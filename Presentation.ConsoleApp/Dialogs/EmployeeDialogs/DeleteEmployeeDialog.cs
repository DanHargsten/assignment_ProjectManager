using Business.Interfaces;
using Business.Models;
using Data.Interfaces;
using Presentation.ConsoleApp.Helpers;


namespace Presentation.ConsoleApp.Dialogs.EmployeeDialogs;

/// <summary>
/// Handles the deletion of employees, including removing them from projects if necessary.
/// </summary>
public class DeleteEmployeeDialog(IEmployeeService employeeService, IProjectService projectService, IProjectEmployeeRepository projectEmployeeRepository)
{
    private readonly IProjectService _projectService = projectService;
    private readonly IEmployeeService _employeeService = employeeService;
    private readonly IProjectEmployeeRepository _projectEmployeeRepository = projectEmployeeRepository;




    // ==================================================
    //                     MAIN EXECUTION
    // ==================================================

    /// <summary>
    /// Executes the employee deletion process.
    /// Lists employees, handles project associations, and confirms removal.
    /// </summary>
    public async Task ExecuteAsync()
    {
        Console.Clear();
        Console.WriteLine("-------------------------------------------");
        Console.WriteLine("              DELETE EMPLOYEE              ");
        Console.WriteLine("-------------------------------------------\n");

        // Hämta alla anställda
        var employees = (await _employeeService.GetEmployeesAsync()).ToList();

        // Om inga anställda finns, informera användaren
        if (employees.Count == 0)
        {
            ConsoleHelper.WriteLineColored("\nNo employees found.", ConsoleColor.Yellow);
            ConsoleHelper.ShowExitPrompt("return to the Employee Menu");
            Console.ReadKey();
            return;
        }

        // Låt användare nvälja en anställd att ta bort
        var employee = SelectEmployee(employees);
        if (employee == null)
            return;
        
        // Hantera om anställda är kopplade till några projekt    
        bool canBeDeleted = await HandleEmployeeInProjects(employee);

        // Bekräfta borttagningen
        bool confirmed = ConfirmEmployeeDeletion(employee, canBeDeleted);
        if (!confirmed) return;

        // Radera antingen från projekten eller helt från systemet
        bool success = canBeDeleted ? await DeleteEmployee(employee) : await RemoveEmployeeFromProjects(employee);

        // Visa resultat
        Console.Clear();
        if (success)
        {
            if (canBeDeleted)
                ConsoleHelper.WriteLineColored("Employee deleted successfully!\n", ConsoleColor.Green);
            else
                ConsoleHelper.WriteLineColored("Employee successfully removed from all projects!\n", ConsoleColor.Green);
        }
        else
        {
            ConsoleHelper.WriteLineColored("Failed to delete emplolyee.\n", ConsoleColor.Red);
        }

        ConsoleHelper.ShowExitPrompt("return to the Employee Menu");
        Console.ReadKey();
    }




    // ==================================================
    //                 SELECT EMPLOYEE
    // ==================================================

    /// <summary>
    /// Displays a list of employees and allows the user to select one for deletion.
    /// </summary>
    /// <param name="employees">The list of available employees.</param>
    /// <returns>The selected employee, or null if selection is invalid.</returns>
    private static Employee? SelectEmployee(List<Employee?> employees)
    {
        for (int i = 0; i < employees.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {employees[i]!.FirstName} {employees[i]!.LastName}");
        }

        Console.Write("\nEnter employee number to delete: ");

        if (int.TryParse(Console.ReadLine(), out int selectedIndex) &&
            selectedIndex >= 1 && selectedIndex <= employees.Count)
        {
            return employees[selectedIndex - 1];
        }

        ConsoleHelper.WriteLineColored("Invalid selection.", ConsoleColor.Red);
        return null;
    }




    // ==================================================
    //             HANDLE EMPLOYEE IN PROJECTS
    // ==================================================

    /// <summary>
    /// Checks if the selected employee is assigned to projects and asks the user how to proceed.
    /// </summary>
    /// <param name="employee">The selected employee.</param>
    /// <returns>True if the employee should be deleted completely, false if only removed from projects.</returns>
    private async Task<bool> HandleEmployeeInProjects(Employee employee)
    {
        //var assignedProjects = await _projectService.GetProjectsByEmployeeIdAsync(employee.Id);

        //// Anställd är inte kopplad till något projekt, kan tas bort direkt
        //if (assignedProjects == null || !assignedProjects.Any())
        //{
        //    return true;
        //}

        //Console.Clear();
        //ConsoleHelper.WriteLineColored($"Employee '{employee.FirstName} {employee.LastName}' is assigned to projects:", ConsoleColor.Yellow);

        //foreach (var project in assignedProjects)
        //{
        //    Console.WriteLine($"- {project?.Title ?? "[Unknown Project]"}");
        //}

        //Console.Write("\nRemove employee from projects only (Y) or delete completely (D)? ");
        //string choice = Console.ReadLine()!.Trim().ToLower();

        //// Returnerar true om användaren vill radera den anställda helt
        //return choice == "d";

        var assignedProjects = await _projectService.GetProjectsByEmployeeIdAsync(employee.Id);

        if (assignedProjects == null || !assignedProjects.Any())
        {
            return true;
        }

        ConsoleHelper.WriteLineColored($"Employee '{employee.FirstName} {employee.LastName}' is assigned to {assignedProjects.Count()} projects.", ConsoleColor.Yellow);

        Console.Write("\nRemove employee from projects only (Y) or delete completely (D)? ");
        string choice = Console.ReadLine()!.Trim().ToLower();

        return choice == "d";
    }




    // ==================================================
    //              CONFIRM EMPLOYEE DELETION
    // ==================================================

    /// <summary>
    /// Confirms the deletion of an employee.
    /// </summary>
    /// <param name="employee">The employee to delete.</param>
    /// <param name="canBeDeleted">Indicates if the employee will be fully deleted.</param>
    /// <returns>True if deletion is confirmed, otherwise false.</returns>
    private static bool ConfirmEmployeeDeletion(Employee employee, bool canBeDeleted)
    {
        Console.Write($"\nAre you sure you want to {(canBeDeleted ? "delete" : "remove from projects")} ");
        ConsoleHelper.WriteColored($"'{employee.FirstName} {employee.LastName}'", ConsoleColor.Red);
        Console.Write("? (yes/no): ");

        return Console.ReadLine()!.Trim().ToLower() == "yes";
    }




    // ==================================================
    //                   DELETE EMPLOYEE
    // ==================================================

    /// <summary>
    /// Completely deletes an employee from the system
    /// </summary>
    /// <param name="employee">The employee to delete</param>
    /// <returns>True if successful, otherwise false</returns>
    private async Task<bool> DeleteEmployee(Employee employee)
    {
        await _projectEmployeeRepository.RemoveAllEmployeesFromProjectAsync(employee.Id);
        return await _employeeService.RemoveEmployeeAsync(employee.Id);
    }




    // ==================================================
    //            REMOVE EMPLOYEE FROM PROJECTS
    // ==================================================

    private async Task<bool> RemoveEmployeeFromProjects(Employee employee)
    {
        var assignedProjects = await _projectEmployeeRepository.GetEmployeesByProjectIdAsync(employee.Id);

        bool success = true;
        foreach (var project in assignedProjects)
        {
            success &= await _projectEmployeeRepository.RemoveEmployeeFromProjectAsync(project.ProjectId, employee.Id);
            
            ConsoleHelper.WriteLineColored("\nEmployee successfully removed from all projects!", ConsoleColor.Green);
            Console.ReadKey();
        }

        return success;

    }
}