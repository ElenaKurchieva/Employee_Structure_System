namespace EmployeeStructureSystem.Application.Positions;

public sealed class PositionUpsertRequest
{
    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }
}