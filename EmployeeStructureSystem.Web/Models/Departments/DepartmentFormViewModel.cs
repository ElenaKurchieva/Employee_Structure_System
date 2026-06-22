using System.ComponentModel.DataAnnotations;

namespace EmployeeStructureSystem.Web.Models.Departments;

public sealed class DepartmentFormViewModel
{
    public int Id { get; init; }

    [Required]
    [StringLength(200)]
    public string Name { get; init; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; init; }
}