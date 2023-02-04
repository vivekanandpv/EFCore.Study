using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EFCore.Study;

//  Step 1
//  Sometimes, you may wish to enable lazy-loading
//  You want to load the related data, only on the first access
//  To enable this, please install: Microsoft.EntityFrameworkCore.Proxies package
//  and follow the below instructions

//  This is not recommended for web applications

//  Lazy loading can cause unneeded extra database roundtrips to occur
//  (the so-called N+1 problem), and care should be taken to avoid this.
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

    //  Step 2
    //  related entities (one side or many side) should be marked as virtual
    public virtual Customer Customer { get; set; }
}

public class Customer
{
    public int CustomerId { get; set; }
    public string LegalName { get; set; }
    public string Gstin { get; set; }

    //  Step 3
    //  related entities (one side or many side) should be marked as virtual
    public virtual IList<CustomerAddress> Addresses { get; set; }
}



public class SalesManagementContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerAddress> CustomerAddresses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            //  Step 4
            //  We enable lazy loading using this call
            .UseLazyLoadingProxies()

            //  Step 5
            //  For logging the SQL, we use these methods
            //  Logging the queries is only for development purposes
            //  It's a severe vulnerability in production grade code
            //  Use these methods with caution
            .LogTo(Console.WriteLine, LogLevel.Information)

            //  Step 6
            //  Be extra careful using this
            .EnableSensitiveDataLogging()

            .UseSqlServer(
            @"Data Source=(localdb)\MSSQLLocalDb; Initial Catalog=SalesManagement;Integrated Security=true");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .ToTable("Customer");

        modelBuilder.Entity<CustomerAddress>()
            .ToTable("CustomerAddress")
            .HasKey(ca => ca.AddressId);
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
        Console.WriteLine("--------------------Customer First-----------------------------");

        using (var context = new SalesManagementContext())
        {
            //  Step 7
            //  Only the customer is queried
            var customer = await context.Customers.FirstAsync(c => c.CustomerId == 1);
        
            //  Step 8
            //  Now the addresses are queried
            var addresses = customer.Addresses;
        }

        Console.WriteLine("--------------------Address First-----------------------------");

        using (var context = new SalesManagementContext())
        {
            //  Step 9
            //  Only the address is queried
            var address = await context.CustomerAddresses.FirstAsync(a => a.AddressId == 1);

            //  Step 10
            //  Now the customer is queried
            var customer = address.Customer;
        }
    }
}