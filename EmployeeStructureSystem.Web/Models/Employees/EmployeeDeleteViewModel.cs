namespace EmployeeStructureSystem.Web.Models.Employees;

public sealed class EmployeeDeleteViewModel
{
    public int Id { get; init; }

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string? Email { get; init; }

    public decimal Salary { get; init; }

    public string DepartmentName { get; init; } = string.Empty;

    public string PositionTitle { get; init; } = string.Empty;
}