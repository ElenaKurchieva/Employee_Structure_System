namespace EmployeeStructureSystem.Web.Models.Reports;

public sealed class DepartmentSalaryReportItemViewModel
{
    public int DepartmentId { get; init; }

    public string DepartmentName { get; init; } = string.Empty;

    public int EmployeeCount { get; init; }

    public decimal TotalSalary { get; init; }

    public decimal AverageSalary { get; init; }
}