using EmployeeStructureSystem.Application.Departments;
using EmployeeStructureSystem.Web.Extensions;
using EmployeeStructureSystem.Web.Models.Departments;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeStructureSystem.Web.Controllers;

public sealed class DepartmentsController : Controller
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var departments = await _departmentService.GetAllAsync(cancellationToken);
        var model = departments.Select(x => new DepartmentListItemViewModel
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            EmployeeCount = x.EmployeeCount
        }).ToList();

        return View(model);
    }

    public IActionResult Create()
    {
        return View(new DepartmentFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DepartmentFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _departmentService.CreateAsync(new DepartmentUpsertRequest
        {
            Name = model.Name,
            Description = model.Description
        }, cancellationToken);

        if (!result.Succeeded)
        {
            result.ApplyToModelState(ModelState);
            return View(model);
        }

        TempData["StatusMessage"] = "Department created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var department = await _departmentService.GetByIdAsync(id, cancellationToken);
        if (department is null)
        {
            return NotFound();
        }

        return View(new DepartmentFormViewModel
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DepartmentFormViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _departmentService.UpdateAsync(id, new DepartmentUpsertRequest
        {
            Name = model.Name,
            Description = model.Description
        }, cancellationToken);

        if (!result.Succeeded)
        {
            if (result.ErrorCode == "not_found")
            {
                return NotFound();
            }

            result.ApplyToModelState(ModelState);
            return View(model);
        }

        TempData["StatusMessage"] = "Department updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var department = await _departmentService.GetByIdAsync(id, cancellationToken);
        if (department is null)
        {
            return NotFound();
        }

        return View(new DepartmentDeleteViewModel
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description,
            EmployeeCount = department.EmployeeCount
        });
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var department = await _departmentService.GetByIdAsync(id, cancellationToken);
        if (department is null)
        {
            return NotFound();
        }

        var result = await _departmentService.DeleteAsync(id, cancellationToken);
        if (!result.Succeeded)
        {
            if (result.ErrorCode == "not_found")
            {
                return NotFound();
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Unable to delete department.");
            return View(new DepartmentDeleteViewModel
            {
                Id = department.Id,
                Name = department.Name,
                Description = department.Description,
                EmployeeCount = department.EmployeeCount
            });
        }

        TempData["StatusMessage"] = "Department deleted.";
        return RedirectToAction(nameof(Index));
    }
}