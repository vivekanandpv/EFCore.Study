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
    public IList<Invoice> Invoices { get; set; } = new List<Invoice>();
}

public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public double Price { get; set; }
    public double TaxRate { get; set; }
    public IList<LineItem> LineItems { get; set; } = new List<LineItem>();
}

public class Invoice
{
    public int InvoiceId { get; set; }
    public DateTime InvoiceDate { get; set; }
    public int AddressId { get; set; }
    public string Narration { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
    public IList<LineItem> LineItems { get; set; } = new List<LineItem>();
}

public class LineItem
{
    public int InvoiceId { get; set; }
    public Invoice Invoice { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public double BasePrice { get; set; }
    public double TaxRate { get; set; }
    public double Quantity { get; set; }
    public double TaxableAmount { get; set; }
    public double TaxAmount { get; set; }
    public double LineAmount { get; set; }
}



public class SalesManagementContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerAddress> CustomerAddresses { get; set; }

    //  New entities added
    public DbSet<Product> Products { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<LineItem> LineItems { get; set; }

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

        modelBuilder.Entity<Product>()
            .ToTable("Product");

        modelBuilder.Entity<Invoice>()
            .ToTable("Invoice");

        modelBuilder.Entity<LineItem>()
            .ToTable("LineItem")
            //  Defining a composite key
            .HasKey(li => new { InvoiceId = li.InvoiceId, ProductId = li.ProductId });
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

        Console.WriteLine($"We have {context.Invoices.Count()} invoices");
        Console.WriteLine($"We have {context.Products.Count()} products");
        Console.WriteLine($"We have {context.LineItems.Count()} line items");
    }
}