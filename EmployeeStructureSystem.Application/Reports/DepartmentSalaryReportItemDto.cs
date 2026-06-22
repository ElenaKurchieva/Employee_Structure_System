namespace EmployeeStructureSystem.Application.Reports;

public sealed record DepartmentSalaryReportItemDto(
    int DepartmentId,
    string DepartmentName,
    int EmployeeCount,
    decimal TotalSalary,
    decimal AverageSalary);