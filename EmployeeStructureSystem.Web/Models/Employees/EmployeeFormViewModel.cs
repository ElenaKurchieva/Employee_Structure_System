using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeStructureSystem.Web.Models.Employees;

public sealed class EmployeeFormViewModel
{
    public int Id { get; init; }

    [Required]
    [StringLength(100)]
    [Display(Name = "First name")]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    [StringLength(100)]
    [Display(Name = "Last name")]
    public string LastName { get; init; } = string.Empty;

    [EmailAddress]
    [StringLength(320)]
    public string? Email { get; init; }

    [Range(0, 999999999)]
    public decimal Salary { get; init; }

    [Display(Name = "Department")]
    [Range(1, int.MaxValue, ErrorMessage = "Department is required.")]
    public int DepartmentId { get; init; }

    [Display(Name = "Position")]
    [Range(1, int.MaxValue, ErrorMessage = "Position is required.")]
    public int PositionId { get; init; }

    public IReadOnlyList<SelectListItem> DepartmentOptions { get; init; } = [];

    public IReadOnlyList<SelectListItem> PositionOptions { get; init; } = [];
}