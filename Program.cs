using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Study;

//  Instruction: Please populate the customer table with some test data

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

//  For client specific requirements, we expose the projection of entity through a view-model
//  (aka data transfer object - DTO)
public class CustomerViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }

    public override string ToString()
    {
        return $"Id = {Id}; Name = {Name}";
    }
}

internal class Program
{
    static async Task Main(string[] args)
    {
        using var context = new SalesManagementContext();

        //  IQueryable<T> is a subtype of IEnumerable<T>
        //  https://learn.microsoft.com/en-us/dotnet/api/system.linq.iqueryable-1?view=net-7.0
        //  Also read this: https://stackoverflow.com/a/252857/3969961

        CustomerViewModel[] viewModels = await context.Customers
            .Select(c => 
                new CustomerViewModel
                {
                    Id = c.CustomerId, 
                    Name = c.LegalName
                })
            .ToArrayAsync();

        foreach (var viewModel in viewModels)
        {
            Console.WriteLine(viewModel);
        }
    }
}