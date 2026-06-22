using EmployeeStructureSystem.Application.Reports;
using EmployeeStructureSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EmployeeStructureSystem.Infrastructure.Services;

public sealed class SalaryReportService : ISalaryReportService
{
    private readonly EmployeeStructureDbContext _dbContext;

    public SalaryReportService(EmployeeStructureDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SalaryReportDto> GetSalaryReportAsync(CancellationToken cancellationToken = default)
    {
        var departments = await _dbContext.Departments
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new DepartmentSalaryReportItemDto(
                x.Id,
                x.Name,
                x.Employees.Count,
                x.Employees.Sum(employee => (decimal?)employee.Salary) ?? 0m,
                x.Employees.Average(employee => (decimal?)employee.Salary) ?? 0m))
            .ToListAsync(cancellationToken);

        var totalEmployees = departments.Sum(x => x.EmployeeCount);
        var totalSalary = departments.Sum(x => x.TotalSalary);
        var averageSalary = totalEmployees == 0 ? 0m : totalSalary / totalEmployees;

        return new SalaryReportDto(departments, totalEmployees, totalSalary, averageSalary);
    }
}