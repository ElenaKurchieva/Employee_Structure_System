using EmployeeStructureSystem.Domain.Entities;

namespace EmployeeStructureSystem.Tests;

public sealed class DomainValidationTests
{
    [Fact]
    public void Department_Rejects_Empty_Name()
    {
        var action = () => new Department("   ");

        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Equal("value", exception.ParamName);
    }

    [Fact]
    public void Position_Rejects_Empty_Title()
    {
        var action = () => new Position(" ");

        var exception = Assert.Throws<ArgumentException>(action);
        Assert.Equal("value", exception.ParamName);
    }

    [Fact]
    public void Employee_Rejects_Negative_Salary()
    {
        var action = () => new Employee("Ada", "Lovelace", -1m, 1, 1);

        Assert.Throws<ArgumentOutOfRangeException>(action);
    }
}