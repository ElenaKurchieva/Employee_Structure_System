namespace EmployeeStructureSystem.Application.Reports;

public interface ISalaryReportService
{
    Task<SalaryReportDto> GetSalaryReportAsync(CancellationToken cancellationToken = default);
}