namespace EmployeeStructureSystem.Web.Models.Departments;

public sealed class DepartmentListItemViewModel
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public int EmployeeCount { get; init; }
}