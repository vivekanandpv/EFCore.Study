using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EFCore.Study;

//  Step 1
//  An entity class is a POCO class with simple public auto properties
//  Since the conventional name of the table is a pluralized form,
//  we use [Table] annotation to provide custom name
[Table("Customer")] //  we are using singular name
public class Customer
{
    //  properties are conventionally named
    public int CustomerId { get; set; }
    public string LegalName { get; set; }
    public string Gstin { get; set; }
}

//  Step 2
//  Data context is the representative of the database in the code
//  data context class inherits from DbContext class
public class SalesManagementContext : DbContext
{
    //  Step 3
    //  Every table is represented by DbSet<T> where T is the entity class
    public DbSet<Customer> Customers { get; set; }

    //  Step 4
    //  DbContext needs to know the database driver and connection string to connect
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        //  Step 5
        optionsBuilder.UseSqlServer(
            @"Data Source=(localdb)\MSSQLLocalDb; Initial Catalog=SalesManagement;Integrated Security=true");
    }
}

internal class Program
{
    static void Main(string[] args)
    {
        //  Step 6
        //  Creating the instance of data context
        var context = new SalesManagementContext();
        int countOfCustomers = context.Customers.Count();

        Console.WriteLine($"Currently we have {countOfCustomers} customers");
    }
}