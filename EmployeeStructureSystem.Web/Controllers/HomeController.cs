using System.Diagnostics;
using EmployeeStructureSystem.Application.Dashboard;
using Microsoft.AspNetCore.Mvc;
using EmployeeStructureSystem.Web.Models;

namespace EmployeeStructureSystem.Web.Controllers;

public class HomeController : Controller
{
    private readonly IDashboardService _dashboardService;

    public HomeController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var stats = await _dashboardService.GetStatsAsync(cancellationToken);
        var model = new DashboardViewModel
        {
            TotalDepartments = stats.TotalDepartments,
            TotalEmployees = stats.TotalEmployees
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
