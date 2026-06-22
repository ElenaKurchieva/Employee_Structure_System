namespace EmployeeStructureSystem.Application.Reports;

public sealed record SalaryReportDto(
    IReadOnlyList<DepartmentSalaryReportItemDto> Departments,
    int TotalEmployees,
    decimal TotalSalary,
    decimal AverageSalary);