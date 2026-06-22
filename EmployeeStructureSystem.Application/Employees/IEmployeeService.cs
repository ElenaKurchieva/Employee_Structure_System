using EmployeeStructureSystem.Application.Common;

namespace EmployeeStructureSystem.Application.Employees;

public interface IEmployeeService
{
    Task<IReadOnlyList<EmployeeDto>> GetAllAsync(int? departmentId = null, int? positionId = null, CancellationToken cancellationToken = default);

    Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<OperationResult<int>> CreateAsync(EmployeeUpsertRequest request, CancellationToken cancellationToken = default);

    Task<OperationResult> UpdateAsync(int id, EmployeeUpsertRequest request, CancellationToken cancellationToken = default);

    Task<OperationResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}