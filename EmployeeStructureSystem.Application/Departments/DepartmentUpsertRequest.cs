namespace EmployeeStructureSystem.Application.Departments;

public sealed class DepartmentUpsertRequest
{
    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }
}