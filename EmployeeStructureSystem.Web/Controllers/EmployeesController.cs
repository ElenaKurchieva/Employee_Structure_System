using EmployeeStructureSystem.Application.Departments;
using EmployeeStructureSystem.Application.Employees;
using EmployeeStructureSystem.Application.Positions;
using EmployeeStructureSystem.Web.Extensions;
using EmployeeStructureSystem.Web.Models.Employees;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeStructureSystem.Web.Controllers;

public sealed class EmployeesController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentService _departmentService;
    private readonly IPositionService _positionService;

    public EmployeesController(
        IEmployeeService employeeService,
        IDepartmentService departmentService,
        IPositionService positionService)
    {
        _employeeService = employeeService;
        _departmentService = departmentService;
        _positionService = positionService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var employees = await _employeeService.GetAllAsync(cancellationToken);
        var model = employees.Select(x => new EmployeeListItemViewModel
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Email = x.Email,
            Salary = x.Salary,
            DepartmentName = x.DepartmentName,
            PositionTitle = x.PositionTitle
        }).ToList();

        return View(model);
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        return View(await BuildFormModelAsync(new EmployeeFormViewModel(), cancellationToken));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(await BuildFormModelAsync(model, cancellationToken));
        }

        var result = await _employeeService.CreateAsync(new EmployeeUpsertRequest
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Salary = model.Salary,
            DepartmentId = model.DepartmentId,
            PositionId = model.PositionId
        }, cancellationToken);

        if (!result.Succeeded)
        {
            result.ApplyToModelState(ModelState);
            return View(await BuildFormModelAsync(model, cancellationToken));
        }

        TempData["StatusMessage"] = "Employee created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var employee = await _employeeService.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        var model = new EmployeeFormViewModel
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Salary = employee.Salary,
            DepartmentId = employee.DepartmentId,
            PositionId = employee.PositionId
        };

        return View(await BuildFormModelAsync(model, cancellationToken));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EmployeeFormViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(await BuildFormModelAsync(model, cancellationToken));
        }

        var result = await _employeeService.UpdateAsync(id, new EmployeeUpsertRequest
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Salary = model.Salary,
            DepartmentId = model.DepartmentId,
            PositionId = model.PositionId
        }, cancellationToken);

        if (!result.Succeeded)
        {
            if (result.ErrorCode == "not_found")
            {
                return NotFound();
            }

            result.ApplyToModelState(ModelState);
            return View(await BuildFormModelAsync(model, cancellationToken));
        }

        TempData["StatusMessage"] = "Employee updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var employee = await _employeeService.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        return View(new EmployeeDeleteViewModel
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Salary = employee.Salary,
            DepartmentName = employee.DepartmentName,
            PositionTitle = employee.PositionTitle
        });
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var employee = await _employeeService.GetByIdAsync(id, cancellationToken);
        if (employee is null)
        {
            return NotFound();
        }

        var result = await _employeeService.DeleteAsync(id, cancellationToken);
        if (!result.Succeeded)
        {
            if (result.ErrorCode == "not_found")
            {
                return NotFound();
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Unable to delete employee.");
            return View(new EmployeeDeleteViewModel
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Salary = employee.Salary,
                DepartmentName = employee.DepartmentName,
                PositionTitle = employee.PositionTitle
            });
        }

        TempData["StatusMessage"] = "Employee deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<EmployeeFormViewModel> BuildFormModelAsync(EmployeeFormViewModel model, CancellationToken cancellationToken)
    {
        var departments = await _departmentService.GetAllAsync(cancellationToken);
        var positions = await _positionService.GetAllAsync(cancellationToken);

        return new EmployeeFormViewModel
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Salary = model.Salary,
            DepartmentId = model.DepartmentId,
            PositionId = model.PositionId,
            DepartmentOptions = departments
                .Select(x => new SelectListItem(x.Name, x.Id.ToString()))
                .ToList(),
            PositionOptions = positions
                .Select(x => new SelectListItem(x.Title, x.Id.ToString()))
                .ToList()
        };
    }
}