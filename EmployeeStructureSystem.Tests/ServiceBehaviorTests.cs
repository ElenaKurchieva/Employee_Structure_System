using EmployeeStructureSystem.Application.Departments;
using EmployeeStructureSystem.Application.Positions;
using EmployeeStructureSystem.Domain.Entities;
using EmployeeStructureSystem.Infrastructure.Persistence;
using EmployeeStructureSystem.Infrastructure.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EmployeeStructureSystem.Tests;

public sealed class ServiceBehaviorTests : IAsyncLifetime
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private EmployeeStructureDbContext _dbContext = null!;

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();
        var options = new DbContextOptionsBuilder<EmployeeStructureDbContext>()
            .UseSqlite(_connection)
            .Options;

        _dbContext = new EmployeeStructureDbContext(options);
        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        await _connection.DisposeAsync();
    }

    [Fact]
    public async Task DepartmentService_Rejects_Duplicate_Name()
    {
        var service = new DepartmentService(_dbContext);
        await service.CreateAsync(new DepartmentUpsertRequest { Name = "Finance" });

        var result = await service.CreateAsync(new DepartmentUpsertRequest { Name = "Finance" });

        Assert.False(result.Succeeded);
        Assert.Contains(nameof(DepartmentUpsertRequest.Name), result.ValidationErrors.Keys);
    }

    [Fact]
    public async Task PositionService_Rejects_Delete_When_Employees_Exist()
    {
        var department = new Department("Operations");
        var position = new Position("Analyst");
        _dbContext.Departments.Add(department);
        _dbContext.Positions.Add(position);
        await _dbContext.SaveChangesAsync();

        _dbContext.Employees.Add(new Employee("Grace", "Hopper", 1000m, department.Id, position.Id));
        await _dbContext.SaveChangesAsync();

        var service = new PositionService(_dbContext);
        var result = await service.DeleteAsync(position.Id);

        Assert.False(result.Succeeded);
        Assert.Equal("delete_restricted", result.ErrorCode);
    }

    [Fact]
    public async Task DepartmentService_Reports_Employee_Count_In_List()
    {
        var department = new Department("Engineering");
        var position = new Position("Developer");
        _dbContext.Departments.Add(department);
        _dbContext.Positions.Add(position);
        await _dbContext.SaveChangesAsync();

        _dbContext.Employees.Add(new Employee("Linus", "Torvalds", 2500m, department.Id, position.Id));
        await _dbContext.SaveChangesAsync();

        var service = new DepartmentService(_dbContext);
        var departments = await service.GetAllAsync();

        Assert.Single(departments);
        Assert.Equal(1, departments[0].EmployeeCount);
    }
}