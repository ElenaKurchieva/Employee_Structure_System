namespace EmployeeStructureSystem.Domain.Entities;

public sealed class Position
{
    private string _title = string.Empty;

    private Position()
    {
    }

    public Position(string title, string? description = null)
    {
        UpdateDetails(title, description);
    }

    public int Id { get; private set; }

    public string Title
    {
        get => _title;
        private set => _title = ValidateTitle(value);
    }

    public string? Description { get; private set; }

    public ICollection<Employee> Employees { get; } = new List<Employee>();

    public void UpdateDetails(string title, string? description)
    {
        Title = title;
        Description = NormalizeOptionalText(description);
    }

    private static string ValidateTitle(string value)
    {
        var normalized = value?.Trim();

        if (string.IsNullOrWhiteSpace(normalized))
        {
            throw new ArgumentException("Position title is required.", nameof(value));
        }

        return normalized;
    }

    private static string? NormalizeOptionalText(string? value)
    {
        var normalized = value?.Trim();
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
    }
}