using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EFCore.Study;

//  Instruction: Please populate the CustomerAddress table with some test data

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

    //  To customize the entity structure, we use OnModelCreating
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //base.OnModelCreating(modelBuilder); //  This is a no-op
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

        //  The change tracker in the data context keeps track of all the property changes we make
        //  To facilitate this, by default, all the entities are tracked
        //  This obviously has a small overhead. If you think there is no need to track the entities
        //  you can disable tracking by attaching AsNoTracking()
        var customer = await context.Customers.FirstAsync(c => c.CustomerId == 1);

        //  Because this entity is tracked, any change we make, is tracked
        customer.Gstin = "HELLO1234";

        //  getting all the changes the context has detected
        var changes = context.ChangeTracker.Entries();


        //  enumerating the changes
        foreach (var entityEntry in changes)
        {
            Console.WriteLine($"Original Value: {entityEntry.OriginalValues[nameof(Customer.Gstin)]}");
            Console.WriteLine($"Current Value: {entityEntry.CurrentValues[nameof(Customer.Gstin)]}");
        }

        //  Now that this change is tracked, we can save the changes if we wish
        //  this will emit UPDATE statements on the database
        int result = await context.SaveChangesAsync();  //  returns the number of records updated

        Console.WriteLine($"We have updated {result} records");

        //  To delete a record, first you need to fetch it
        var address = await context.CustomerAddresses.FirstAsync();

        //  Then, it has to be removed from the DbSet
        context.CustomerAddresses.Remove(address);

        //  enumerating the changes
        var newChanges = context.ChangeTracker.Entries();

        //  Ensuring the Customer is unchanged and address is deleted
        foreach (var entityEntry in newChanges)
        {
            Console.WriteLine($"The entity {entityEntry.Entity} is: {entityEntry.State}");
        }

        //  If we save, it emits DELETE on the database
        await context.SaveChangesAsync();
    }
}