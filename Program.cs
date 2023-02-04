using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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
        optionsBuilder.UseSqlServer(
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

        await context.Customers.ForEachAsync(c =>
        {
            Console.WriteLine($"{c.CustomerId} -> {c.LegalName}");
        });
    }
}