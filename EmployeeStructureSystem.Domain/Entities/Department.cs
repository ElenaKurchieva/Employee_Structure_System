namespace EmployeeStructureSystem.Domain.Entities;

public sealed class Department
{
    private string _name = string.Empty;

    private Department()
    {
    }

    public Department(string name, string? description = null)
    {
        UpdateDetails(name, description);
    }

    public int Id { get; private set; }

    public string Name
    {
        get => _name;
        private set => _name = ValidateName(value);
    }

    public string? Description { get; private set; }

    public ICollection<Employee> Employees { get; } = new List<Employee>();

    public void UpdateDetails(string name, string? description)
    {
        Name = name;
        Description = NormalizeOptionalText(description);
    }

    private static string ValidateName(string value)
    {
        var normalized = value?.Trim();

        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("Department name is required.", nameof(value));
        }

        return normalized;
    }

    private static string? NormalizeOptionalText(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
    }
}