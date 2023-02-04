using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Study;

//  EF Core provides LINQ operators as extension methods on IQueryable<T>
//  These operators form LINQ-to-entities approach
//  These operators are extension methods defined in Queryable static class
//  https://learn.microsoft.com/en-us/dotnet/api/system.linq.queryable?view=net-7.0

//  Most of these data-access methods come in async variants
//  It is recommended to use async methods

[Table("Customer")]
public class Customer
{
    public int CustomerId { get; set; }
    public string LegalName { get; set; }
    public string Gstin { get; set; }
}

public class SalesManagementContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Data Source=(localdb)\MSSQLLocalDb; Initial Catalog=SalesManagement;Integrated Security=true");
    }
}

internal class Program
{
    //  Since C# 7.1, we can declare the Main method as async Task
    //  https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-version-history#c-version-71
    //  https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/program-structure/main-command-line#async-main-return-values
    static async Task Main(string[] args)
    {
        using var context = new SalesManagementContext();

        Console.WriteLine($"Currently we have {await context.Customers.CountAsync()} customers");
    }
}