using EmployeeStructureSystem.Application.Positions;
using EmployeeStructureSystem.Web.Extensions;
using EmployeeStructureSystem.Web.Models.Positions;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeStructureSystem.Web.Controllers;

public sealed class PositionsController : Controller
{
    private readonly IPositionService _positionService;

    public PositionsController(IPositionService positionService)
    {
        _positionService = positionService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var positions = await _positionService.GetAllAsync(cancellationToken);
        var model = positions.Select(x => new PositionListItemViewModel
        {
            Id = x.Id,
            Title = x.Title,
            Description = x.Description,
            EmployeeCount = x.EmployeeCount
        }).ToList();

        return View(model);
    }

    public IActionResult Create()
    {
        return View(new PositionFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PositionFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _positionService.CreateAsync(new PositionUpsertRequest
        {
            Title = model.Title,
            Description = model.Description
        }, cancellationToken);

        if (!result.Succeeded)
        {
            result.ApplyToModelState(ModelState);
            return View(model);
        }

        TempData["StatusMessage"] = "Position created.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var position = await _positionService.GetByIdAsync(id, cancellationToken);
        if (position is null)
        {
            return NotFound();
        }

        return View(new PositionFormViewModel
        {
            Id = position.Id,
            Title = position.Title,
            Description = position.Description
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, PositionFormViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await _positionService.UpdateAsync(id, new PositionUpsertRequest
        {
            Title = model.Title,
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

        TempData["StatusMessage"] = "Position updated.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var position = await _positionService.GetByIdAsync(id, cancellationToken);
        if (position is null)
        {
            return NotFound();
        }

        return View(new PositionDeleteViewModel
        {
            Id = position.Id,
            Title = position.Title,
            Description = position.Description,
            EmployeeCount = position.EmployeeCount
        });
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var position = await _positionService.GetByIdAsync(id, cancellationToken);
        if (position is null)
        {
            return NotFound();
        }

        var result = await _positionService.DeleteAsync(id, cancellationToken);
        if (!result.Succeeded)
        {
            if (result.ErrorCode == "not_found")
            {
                return NotFound();
            }

            ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Unable to delete position.");
            return View(new PositionDeleteViewModel
            {
                Id = position.Id,
                Title = position.Title,
                Description = position.Description,
                EmployeeCount = position.EmployeeCount
            });
        }

        TempData["StatusMessage"] = "Position deleted.";
        return RedirectToAction(nameof(Index));
    }
}