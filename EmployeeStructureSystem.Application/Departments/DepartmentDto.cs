namespace EmployeeStructureSystem.Application.Departments;

public sealed record DepartmentDto(int Id, string Name, string? Description, int EmployeeCount);