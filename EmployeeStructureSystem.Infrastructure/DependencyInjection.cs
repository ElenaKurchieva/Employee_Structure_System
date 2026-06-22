using EmployeeStructureSystem.Application.Dashboard;
using EmployeeStructureSystem.Application.Departments;
using EmployeeStructureSystem.Application.Employees;
using EmployeeStructureSystem.Application.Positions;
using EmployeeStructureSystem.Infrastructure.Persistence;
using EmployeeStructureSystem.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeStructureSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("EmployeeStructureDatabase")
            ?? throw new InvalidOperationException("Connection string 'EmployeeStructureDatabase' was not found.");

        services.AddDbContext<EmployeeStructureDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IPositionService, PositionService>();

        return services;
    }
}