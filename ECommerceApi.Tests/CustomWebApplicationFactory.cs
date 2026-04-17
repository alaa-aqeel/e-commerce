using ECommerceApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerceApi.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove all Entity Framework Core related services
            var descriptors = services.Where(
                d => d.ServiceType.FullName != null && 
                     d.ServiceType.FullName.Contains("EntityFrameworkCore") ||
                     d.ServiceType == typeof(System.Data.Common.DbConnection)).ToList();

            foreach (var d in descriptors)
            {
                services.Remove(d);
            }

            // Add AppDbContext using an in-memory database for testing
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });
            
            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context (AppDbContext)
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<AppDbContext>();

            // Ensure the database is created
            db.Database.EnsureCreated();
            
            // You can seed data here if needed
        });
        
        // Optional: override configuration for JWT, Logging, etc.
        builder.UseEnvironment("Development");
    }
}
