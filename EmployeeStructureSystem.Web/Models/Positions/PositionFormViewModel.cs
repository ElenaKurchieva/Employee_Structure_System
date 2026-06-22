using System.ComponentModel.DataAnnotations;

namespace EmployeeStructureSystem.Web.Models.Positions;

public sealed class PositionFormViewModel
{
    public int Id { get; init; }

    [Required]
    [StringLength(200)]
    public string Title { get; init; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; init; }
}