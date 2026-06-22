using EmployeeStructureSystem.Application.Common;
using EmployeeStructureSystem.Application.Departments;
using EmployeeStructureSystem.Domain.Entities;
using EmployeeStructureSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EmployeeStructureSystem.Infrastructure.Services;

public sealed class DepartmentService : IDepartmentService
{
    private readonly EmployeeStructureDbContext _dbContext;

    public DepartmentService(EmployeeStructureDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Departments
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new DepartmentDto(x.Id, x.Name, x.Description, x.Employees.Count))
            .ToListAsync(cancellationToken);
    }

    public async Task<DepartmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Departments
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new DepartmentDto(x.Id, x.Name, x.Description, x.Employees.Count))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<OperationResult<int>> CreateAsync(DepartmentUpsertRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = Validate(request);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var normalizedName = request.Name.Trim();
        var exists = await _dbContext.Departments.AnyAsync(x => x.Name == normalizedName, cancellationToken);
        if (exists)
        {
            return OperationResult<int>.ValidationFailure(new Dictionary<string, string[]>
            {
                [nameof(request.Name)] = ["A department with this name already exists."]
            });
        }

        var department = new Department(normalizedName, request.Description);
        _dbContext.Departments.Add(department);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return OperationResult<int>.Success(department.Id);
    }

    public async Task<OperationResult> UpdateAsync(int id, DepartmentUpsertRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = Validate(request);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var department = await _dbContext.Departments.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (department is null)
        {
            return OperationResult.Failure("Department was not found.", "not_found");
        }

        var normalizedName = request.Name.Trim();
        var duplicateExists = await _dbContext.Departments.AnyAsync(x => x.Id != id && x.Name == normalizedName, cancellationToken);
        if (duplicateExists)
        {
            return OperationResult.ValidationFailure(new Dictionary<string, string[]>
            {
                [nameof(request.Name)] = ["A department with this name already exists."]
            });
        }

        department.UpdateDetails(normalizedName, request.Description);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return OperationResult.Success();
    }

    public async Task<OperationResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var department = await _dbContext.Departments
            .Include(x => x.Employees)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (department is null)
        {
            return OperationResult.Failure("Department was not found.", "not_found");
        }

        if (department.Employees.Count > 0)
        {
            return OperationResult.Failure("This department cannot be deleted because employees are assigned to it.", "delete_restricted");
        }

        _dbContext.Departments.Remove(department);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return OperationResult.Success();
    }

    private static OperationResult<int>? Validate(DepartmentUpsertRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors[nameof(request.Name)] = ["Department name is required."];
        }

        return errors.Count == 0 ? null : OperationResult<int>.ValidationFailure(errors);
    }
}