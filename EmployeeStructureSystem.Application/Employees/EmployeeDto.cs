namespace EmployeeStructureSystem.Application.Employees;

public sealed record EmployeeDto(
    int Id,
    string FirstName,
    string LastName,
    string? Email,
    decimal Salary,
    int DepartmentId,
    string DepartmentName,
    int PositionId,
    string PositionTitle);