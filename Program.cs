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

        //  To add a related entity, there are two approaches
        //  1. create the related entity with primary entity's id
        //  (ensure foreign key), and add the new object as usual.
        //  2. add the related entity to the primary entity's collection
        //  and save as usual

        //  Approach 1
        var address = new CustomerAddress
        {
            CustomerId = 1, //  required
            AddressLine1 = "Line 1",
            AddressLine2 = "Line 2",
            AddressLine3 = "Line 3",
            City = "Bengaluru",
            Pin = 560085,
            StateCode = "Karnataka",
        };

        context.CustomerAddresses.Add(address);

        //  Approach 2
        var address2 = new CustomerAddress
        {
            //  CustomerId is not required
            AddressLine1 = "Line 21",
            AddressLine2 = "Line 22",
            AddressLine3 = "Line 23",
            City = "Bengaluru",
            Pin = 560086,
            StateCode = "Karnataka",
        };

        var customer = await context.Customers.FirstAsync(c => c.CustomerId == 2);
        customer.Addresses.Add(address2);
        
        var changes = context.ChangeTracker.Entries();

        //  Ensuring the addresses are added
        foreach (var entityEntry in changes)
        {
            Console.WriteLine($"The entity {entityEntry.Entity} is: {entityEntry.State}");
        }

        //  If we save, it emits INSERT INTO on the database
        await context.SaveChangesAsync();
    }
}