namespace EmployeeStructureSystem.Application.Positions;

public sealed record PositionDto(int Id, string Title, string? Description, int EmployeeCount);