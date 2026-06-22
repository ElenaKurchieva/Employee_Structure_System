namespace EmployeeStructureSystem.Domain.Entities;

public sealed class Employee
{
    private string _firstName = string.Empty;
    private string _lastName = string.Empty;

    private Employee()
    {
    }

    public Employee(string firstName, string lastName, decimal salary, int departmentId, int positionId, string? email = null)
    {
        UpdateIdentity(firstName, lastName, email);
        UpdateAssignment(departmentId, positionId);
        UpdateSalary(salary);
    }

    public int Id { get; private set; }

    public string FirstName
    {
        get => _firstName;
        private set => _firstName = ValidateRequired(value, "First name is required.");
    }

    public string LastName
    {
        get => _lastName;
        private set => _lastName = ValidateRequired(value, "Last name is required.");
    }

    public string? Email { get; private set; }

    public decimal Salary { get; private set; }

    public int DepartmentId { get; private set; }

    public Department? Department { get; private set; }

    public int PositionId { get; private set; }

    public Position? Position { get; private set; }

    public void UpdateIdentity(string firstName, string lastName, string? email)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = NormalizeOptionalText(email);
    }

    public void UpdateAssignment(int departmentId, int positionId)
    {
        if (departmentId <= 0)
        {
            throw new ArgumentException("Department is required.", nameof(departmentId));
        }

        if (positionId <= 0)
        {
            throw new ArgumentException("Position is required.", nameof(positionId));
        }

        DepartmentId = departmentId;
        PositionId = positionId;
    }

    public void UpdateSalary(decimal salary)
    {
        if (salary < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(salary), "Salary cannot be negative.");
        }

        Salary = salary;
    }

    private static string ValidateRequired(string value, string errorMessage)
    {
        var normalized = value?.Trim();

        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException(errorMessage, nameof(value));
        }

        return normalized;
    }

    private static string? NormalizeOptionalText(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
    }
}