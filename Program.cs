using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EFCore.Study;

public class CustomerAddress
{
    public int AddressId { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string AddressLine3 { get; set; }
    public string City { get; set; }
    public string StateCode { get; set; }
    public int Pin { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
}

public class Customer
{
    public int CustomerId { get; set; }
    public string LegalName { get; set; }
    public string Gstin { get; set; }
    public IList<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();
}



public class SalesManagementContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerAddress> CustomerAddresses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .UseSqlServer(
            @"Data Source=(localdb)\MSSQLLocalDb; Initial Catalog=SalesManagement;Integrated Security=true");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .ToTable("Customer");   //  This string value can be dynamically set,
                                    //  which is not the case with attributes

        modelBuilder.Entity<CustomerAddress>()
            .ToTable("CustomerAddress")
            .HasKey(ca => ca.AddressId);   //  Defining a non-conventional key.
    }
}

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

        //  To add to a primary entity, we create an object
        //  Then add it to the DbSet

        //  This object is standalone. Context doesn't know anything about it.
        //  If the primary key is an identity, no arbitrary values for it are allowed
        //  Default value of zero will be overridden by the database
        var customer = new Customer { LegalName = "Philips India Pvt Ltd", Gstin = "PHLIPS7845" };

        //  Adding to the DbSet
        context.Customers.Add(customer);

        var changes = context.ChangeTracker.Entries();

        //  Ensuring the Customer is added
        foreach (var entityEntry in changes)
        {
            Console.WriteLine($"The entity {entityEntry.Entity} is: {entityEntry.State}");
        }

        //  If we save, it emits INSERT INTO on the database
        await context.SaveChangesAsync();
    }
}