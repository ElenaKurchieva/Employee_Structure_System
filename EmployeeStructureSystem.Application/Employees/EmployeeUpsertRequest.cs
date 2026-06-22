namespace EmployeeStructureSystem.Application.Employees;

public sealed class EmployeeUpsertRequest
{
    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string? Email { get; init; }

    public decimal Salary { get; init; }

    public int DepartmentId { get; init; }

    public int PositionId { get; init; }
}