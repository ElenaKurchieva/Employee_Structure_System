using EmployeeStructureSystem.Application.Dashboard;
using EmployeeStructureSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EmployeeStructureSystem.Infrastructure.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly EmployeeStructureDbContext _dbContext;

    public DashboardService(EmployeeStructureDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DashboardStatsDto> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var totalDepartments = await _dbContext.Departments.CountAsync(cancellationToken);
        var totalEmployees = await _dbContext.Employees.CountAsync(cancellationToken);

        return new DashboardStatsDto(totalDepartments, totalEmployees);
    }
}