using EmployeeStructureSystem.Application.Common;
using EmployeeStructureSystem.Application.Employees;
using EmployeeStructureSystem.Domain.Entities;
using EmployeeStructureSystem.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EmployeeStructureSystem.Infrastructure.Services;

public sealed class EmployeeService : IEmployeeService
{
    private readonly EmployeeStructureDbContext _dbContext;

    public EmployeeService(EmployeeStructureDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<EmployeeDto>> GetAllAsync(int? departmentId = null, int? positionId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Employees
            .AsNoTracking()
            .AsQueryable();

        if (departmentId.HasValue)
        {
            query = query.Where(x => x.DepartmentId == departmentId.Value);
        }

        if (positionId.HasValue)
        {
            query = query.Where(x => x.PositionId == positionId.Value);
        }

        return await query
            .OrderBy(x => x.LastName)
            .ThenBy(x => x.FirstName)
            .Select(x => new EmployeeDto(
                x.Id,
                x.FirstName,
                x.LastName,
                x.Email,
                x.Salary,
                x.DepartmentId,
                x.Department!.Name,
                x.PositionId,
                x.Position!.Title))
            .ToListAsync(cancellationToken);
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Employees
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new EmployeeDto(
                x.Id,
                x.FirstName,
                x.LastName,
                x.Email,
                x.Salary,
                x.DepartmentId,
                x.Department!.Name,
                x.PositionId,
                x.Position!.Title))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<OperationResult<int>> CreateAsync(EmployeeUpsertRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await ValidateAsync(request, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var employee = new Employee(
            request.FirstName.Trim(),
            request.LastName.Trim(),
            request.Salary,
            request.DepartmentId,
            request.PositionId,
            request.Email);

        _dbContext.Employees.Add(employee);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return OperationResult<int>.Success(employee.Id);
    }

    public async Task<OperationResult> UpdateAsync(int id, EmployeeUpsertRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await ValidateAsync(request, cancellationToken);
        if (validationResult is not null)
        {
            return validationResult;
        }

        var employee = await _dbContext.Employees.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (employee is null)
        {
            return OperationResult.Failure("Employee was not found.", "not_found");
        }

        employee.UpdateIdentity(request.FirstName.Trim(), request.LastName.Trim(), request.Email);
        employee.UpdateSalary(request.Salary);
        employee.UpdateAssignment(request.DepartmentId, request.PositionId);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return OperationResult.Success();
    }

    public async Task<OperationResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var employee = await _dbContext.Employees.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (employee is null)
        {
            return OperationResult.Failure("Employee was not found.", "not_found");
        }

        _dbContext.Employees.Remove(employee);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return OperationResult.Success();
    }

    private async Task<OperationResult<int>?> ValidateAsync(EmployeeUpsertRequest request, CancellationToken cancellationToken)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.FirstName))
        {
            errors[nameof(request.FirstName)] = ["First name is required."];
        }

        if (string.IsNullOrWhiteSpace(request.LastName))
        {
            errors[nameof(request.LastName)] = ["Last name is required."];
        }

        if (request.Salary < 0)
        {
            errors[nameof(request.Salary)] = ["Salary cannot be negative."];
        }

        if (!await _dbContext.Departments.AnyAsync(x => x.Id == request.DepartmentId, cancellationToken))
        {
            errors[nameof(request.DepartmentId)] = ["A valid department is required."];
        }

        if (!await _dbContext.Positions.AnyAsync(x => x.Id == request.PositionId, cancellationToken))
        {
            errors[nameof(request.PositionId)] = ["A valid position is required."];
        }

        return errors.Count == 0 ? null : OperationResult<int>.ValidationFailure(errors);
    }
}