using EmployeeStructureSystem.Application.Common;

namespace EmployeeStructureSystem.Application.Positions;

public interface IPositionService
{
    Task<IReadOnlyList<PositionDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<PositionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<OperationResult<int>> CreateAsync(PositionUpsertRequest request, CancellationToken cancellationToken = default);

    Task<OperationResult> UpdateAsync(int id, PositionUpsertRequest request, CancellationToken cancellationToken = default);

    Task<OperationResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
}