using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace EmployeeStructureSystem.Infrastructure.Persistence;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<EmployeeStructureDbContext>
{
    public EmployeeStructureDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<EmployeeStructureDbContext>();
        builder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=EmployeeStructureSystemDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

        return new EmployeeStructureDbContext(builder.Options);
    }
}