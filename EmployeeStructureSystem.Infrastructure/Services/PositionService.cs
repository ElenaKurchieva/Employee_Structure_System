using EmployeeStructureSystem.Application.Common;
using EmployeeStructureSystem.Application.Positions;
using EmployeeStructureSystem.Domain.Entities;
using EmployeeStructureSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EmployeeStructureSystem.Infrastructure.Services;

public sealed class PositionService : IPositionService
{
    private readonly EmployeeStructureDbContext _dbContext;

    public PositionService(EmployeeStructureDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<PositionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Positions
            .AsNoTracking()
            .OrderBy(x => x.Title)
            .Select(x => new PositionDto(x.Id, x.Title, x.Description, x.Employees.Count))
            .ToListAsync(cancellationToken);
    }

    public async Task<PositionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Positions
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new PositionDto(x.Id, x.Title, x.Description, x.Employees.Count))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<OperationResult<int>> CreateAsync(PositionUpsertRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = Validate(request);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var normalizedTitle = request.Title.Trim();
        var exists = await _dbContext.Positions.AnyAsync(x => x.Title == normalizedTitle, cancellationToken);
        if (exists)
        {
            return OperationResult<int>.ValidationFailure(new Dictionary<string, string[]>
            {
                [nameof(request.Title)] = ["A position with this title already exists."]
            });
        }

        var position = new Position(normalizedTitle, request.Description);
        _dbContext.Positions.Add(position);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return OperationResult<int>.Success(position.Id);
    }

    public async Task<OperationResult> UpdateAsync(int id, PositionUpsertRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = Validate(request);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var position = await _dbContext.Positions.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (position is null)
        {
            return OperationResult.Failure("Position was not found.", "not_found");
        }

        var normalizedTitle = request.Title.Trim();
        var duplicateExists = await _dbContext.Positions.AnyAsync(x => x.Id != id && x.Title == normalizedTitle, cancellationToken);
        if (duplicateExists)
        {
            return OperationResult.ValidationFailure(new Dictionary<string, string[]>
            {
                [nameof(request.Title)] = ["A position with this title already exists."]
            });
        }

        position.UpdateDetails(normalizedTitle, request.Description);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return OperationResult.Success();
    }

    public async Task<OperationResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var position = await _dbContext.Positions
            .Include(x => x.Employees)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (position is null)
        {
            return OperationResult.Failure("Position was not found.", "not_found");
        }

        if (position.Employees.Count > 0)
        {
            return OperationResult.Failure("This position cannot be deleted because employees are assigned to it.", "delete_restricted");
        }

        _dbContext.Positions.Remove(position);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return OperationResult.Success();
    }

    private static OperationResult<int>? Validate(PositionUpsertRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            errors[nameof(request.Title)] = ["Position title is required."];
        }

        return errors.Count == 0 ? null : OperationResult<int>.ValidationFailure(errors);
    }
}