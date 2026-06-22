namespace EmployeeStructureSystem.Web.Models.Positions;

public sealed class PositionListItemViewModel
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public int EmployeeCount { get; init; }
}