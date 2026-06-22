namespace EmployeeStructureSystem.Web.Models.Reports;

public sealed class SalaryReportViewModel
{
    public IReadOnlyList<DepartmentSalaryReportItemViewModel> Departments { get; init; } = [];

    public int TotalEmployees { get; init; }

    public decimal TotalSalary { get; init; }

    public decimal AverageSalary { get; init; }
}