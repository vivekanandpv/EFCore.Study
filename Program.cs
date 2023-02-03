using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Study;

//  Instruction: Please populate the CustomerAddress table with some test data

//  Step 1
[Table("CustomerAddress")]
[PrimaryKey("AddressId")]   //  since the primary key is non-conventional
public class CustomerAddress
{
    public int AddressId { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string AddressLine3 { get; set; }
    public string City { get; set; }
    public string StateCode { get; set; }
    public int Pin { get; set; }

    //  conventional foreign key
    public int CustomerId { get; set; }

    //  Navigation property (good to have)
    //  One address belongs precisely to one customer
    //  Eagerly loaded
    public Customer Customer { get; set; }
}

[Table("Customer")]
public class Customer
{
    public int CustomerId { get; set; }
    public string LegalName { get; set; }
    public string Gstin { get; set; }

    //  Step 2
    //  Navigation property
    //  One customer has multiple addresses
    //  You can also use ICollection<T>, HashSet<T>, List<T> here
    //  Lazily loaded!
    public IList<CustomerAddress> Addresses { get; set; } = new List<CustomerAddress>();
}



public class SalesManagementContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }

    //  Step 3
    //  New DbSet for Addresses
    public DbSet<CustomerAddress> CustomerAddresses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Data Source=(localdb)\MSSQLLocalDb; Initial Catalog=SalesManagement;Integrated Security=true");
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
        var context = new SalesManagementContext();

        await context.Customers.ForEachAsync(c =>
        {
            //  Step 4
            //  The Addresses property of all customers
            //  is null (if you did not initialize in Customer) or empty list
            Console.WriteLine($"{c.CustomerId} -> {c.LegalName}");
        });
    }
}