using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeStructureSystem.Web.Models.Employees;

public sealed class EmployeeIndexViewModel
{
    public int? DepartmentId { get; init; }

    public int? PositionId { get; init; }

    public IReadOnlyList<SelectListItem> DepartmentOptions { get; init; } = [];

    public IReadOnlyList<SelectListItem> PositionOptions { get; init; } = [];

    public IReadOnlyList<EmployeeListItemViewModel> Employees { get; init; } = [];
}