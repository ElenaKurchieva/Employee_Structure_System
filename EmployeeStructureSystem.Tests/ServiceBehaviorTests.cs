using EmployeeStructureSystem.Application.Departments;
using EmployeeStructureSystem.Application.Employees;
using EmployeeStructureSystem.Application.Positions;
using EmployeeStructureSystem.Application.Reports;
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

    [Fact]
    public async Task EmployeeService_Creates_Employee_With_Assignments()
    {
        var department = new Department("Engineering");
        var position = new Position("Developer");
        _dbContext.Departments.Add(department);
        _dbContext.Positions.Add(position);
        await _dbContext.SaveChangesAsync();

        var service = new EmployeeService(_dbContext);
        var result = await service.CreateAsync(new EmployeeUpsertRequest
        {
            FirstName = "Ada",
            LastName = "Lovelace",
            Email = "ada@example.com",
            Salary = 4200m,
            DepartmentId = department.Id,
            PositionId = position.Id
        });

        Assert.True(result.Succeeded);

        var employees = await service.GetAllAsync();
        Assert.Single(employees);
        Assert.Equal("Engineering", employees[0].DepartmentName);
        Assert.Equal("Developer", employees[0].PositionTitle);
    }

    [Fact]
    public async Task EmployeeService_Filters_By_Department_And_Position()
    {
        var engineering = new Department("Engineering");
        var hr = new Department("Human Resources");
        var developer = new Position("Developer");
        var recruiter = new Position("Recruiter");

        _dbContext.Departments.AddRange(engineering, hr);
        _dbContext.Positions.AddRange(developer, recruiter);
        await _dbContext.SaveChangesAsync();

        _dbContext.Employees.AddRange(
            new Employee("Ada", "Lovelace", 4000m, engineering.Id, developer.Id),
            new Employee("Grace", "Hopper", 5000m, engineering.Id, recruiter.Id),
            new Employee("Mina", "Lee", 3000m, hr.Id, recruiter.Id));
        await _dbContext.SaveChangesAsync();

        var service = new EmployeeService(_dbContext);

        var byDepartment = await service.GetAllAsync(departmentId: engineering.Id);
        var byPosition = await service.GetAllAsync(positionId: recruiter.Id);
        var byDepartmentAndPosition = await service.GetAllAsync(engineering.Id, recruiter.Id);

        Assert.Equal(2, byDepartment.Count);
        Assert.Equal(2, byPosition.Count);
        Assert.Single(byDepartmentAndPosition);
        Assert.Equal("Grace", byDepartmentAndPosition[0].FirstName);
        Assert.Equal("Engineering", byDepartmentAndPosition[0].DepartmentName);
        Assert.Equal("Recruiter", byDepartmentAndPosition[0].PositionTitle);
    }

    [Fact]
    public async Task EmployeeService_Rejects_Invalid_Assignment()
    {
        var service = new EmployeeService(_dbContext);

        var result = await service.CreateAsync(new EmployeeUpsertRequest
        {
            FirstName = "Alan",
            LastName = "Turing",
            Salary = 3000m,
            DepartmentId = 999,
            PositionId = 999
        });

        Assert.False(result.Succeeded);
        Assert.Contains(nameof(EmployeeUpsertRequest.DepartmentId), result.ValidationErrors.Keys);
        Assert.Contains(nameof(EmployeeUpsertRequest.PositionId), result.ValidationErrors.Keys);
    }

    [Fact]
    public async Task SalaryReportService_Returns_Department_And_Company_Totals()
    {
        var engineering = new Department("Engineering");
        var hr = new Department("Human Resources");
        var developer = new Position("Developer");
        var recruiter = new Position("Recruiter");

        _dbContext.Departments.AddRange(engineering, hr);
        _dbContext.Positions.AddRange(developer, recruiter);
        await _dbContext.SaveChangesAsync();

        _dbContext.Employees.AddRange(
            new Employee("Ada", "Lovelace", 4000m, engineering.Id, developer.Id),
            new Employee("Grace", "Hopper", 5000m, engineering.Id, developer.Id),
            new Employee("Mina", "Lee", 3000m, hr.Id, recruiter.Id));
        await _dbContext.SaveChangesAsync();

        var service = new SalaryReportService(_dbContext);
        var report = await service.GetSalaryReportAsync();

        Assert.Equal(3, report.TotalEmployees);
        Assert.Equal(12000m, report.TotalSalary);
        Assert.Equal(4000m, report.AverageSalary);

        Assert.Equal(2, report.Departments.Count);
        Assert.Equal("Engineering", report.Departments[0].DepartmentName);
        Assert.Equal(2, report.Departments[0].EmployeeCount);
        Assert.Equal(9000m, report.Departments[0].TotalSalary);
        Assert.Equal(4500m, report.Departments[0].AverageSalary);

        Assert.Equal("Human Resources", report.Departments[1].DepartmentName);
        Assert.Equal(1, report.Departments[1].EmployeeCount);
        Assert.Equal(3000m, report.Departments[1].TotalSalary);
        Assert.Equal(3000m, report.Departments[1].AverageSalary);
    }
}