using EmployeeStructureSystem.Application.Common;

namespace EmployeeStructureSystem.Application.Departments;

public interface IDepartmentService
{
    Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<DepartmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<OperationResult<int>> CreateAsync(DepartmentUpsertRequest request, CancellationToken cancellationToken = default);

    Task<OperationResult> UpdateAsync(int id, DepartmentUpsertRequest request, CancellationToken cancellationToken = default);

    Task<OperationResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}