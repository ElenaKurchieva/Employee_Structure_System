using EmployeeStructureSystem.Application.Reports;
using EmployeeStructureSystem.Web.Models.Reports;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeStructureSystem.Web.Controllers;

public sealed class ReportsController : Controller
{
    private readonly ISalaryReportService _salaryReportService;

    public ReportsController(ISalaryReportService salaryReportService)
    {
        _salaryReportService = salaryReportService;
    }

    public async Task<IActionResult> Salary(CancellationToken cancellationToken)
    {
        var report = await _salaryReportService.GetSalaryReportAsync(cancellationToken);
        var model = new SalaryReportViewModel
        {
            Departments = report.Departments.Select(x => new DepartmentSalaryReportItemViewModel
            {
                DepartmentId = x.DepartmentId,
                DepartmentName = x.DepartmentName,
                EmployeeCount = x.EmployeeCount,
                TotalSalary = x.TotalSalary,
                AverageSalary = x.AverageSalary
            }).ToList(),
            TotalEmployees = report.TotalEmployees,
            TotalSalary = report.TotalSalary,
            AverageSalary = report.AverageSalary
        };

        return View(model);
    }
}