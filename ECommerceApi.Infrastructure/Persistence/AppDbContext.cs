using ECommerceApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Coupon> Coupons => Set<Coupon>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Global soft-delete filter
        modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Category>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Product>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Order>().HasQueryFilter(e => !e.IsDeleted);

        // User
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Email).HasMaxLength(256);
            e.Property(u => u.FirstName).HasMaxLength(100);
            e.Property(u => u.LastName).HasMaxLength(100);
            e.HasOne(u => u.Cart).WithOne(c => c.User).HasForeignKey<Cart>(c => c.UserId);
        });

        // Category self-reference
        modelBuilder.Entity<Category>(e =>
        {
            e.HasOne(c => c.ParentCategory)
             .WithMany(c => c.SubCategories)
             .HasForeignKey(c => c.ParentCategoryId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Product
        modelBuilder.Entity<Product>(e =>
        {
            e.HasIndex(p => p.SKU).IsUnique();
            e.Property(p => p.Price).HasColumnType("numeric(18,2)");
            e.Property(p => p.DiscountPrice).HasColumnType("numeric(18,2)");
            e.HasOne(p => p.Category).WithMany(c => c.Products).HasForeignKey(p => p.CategoryId);
        });

        // Order
        modelBuilder.Entity<Order>(e =>
        {
            e.Property(o => o.SubTotal).HasColumnType("numeric(18,2)");
            e.Property(o => o.Discount).HasColumnType("numeric(18,2)");
            e.Property(o => o.Total).HasColumnType("numeric(18,2)");
            e.HasOne(o => o.Address).WithMany(a => a.Orders).HasForeignKey(o => o.AddressId).OnDelete(DeleteBehavior.Restrict);
        });

        // OrderItem
        modelBuilder.Entity<OrderItem>(e =>
        {
            e.Property(i => i.UnitPrice).HasColumnType("numeric(18,2)");
            e.HasOne(i => i.ProductVariant).WithMany().HasForeignKey(i => i.ProductVariantId).OnDelete(DeleteBehavior.SetNull);
        });

        // Coupon
        modelBuilder.Entity<Coupon>(e =>
        {
            e.HasIndex(c => c.Code).IsUnique();
            e.Property(c => c.Value).HasColumnType("numeric(18,2)");
            e.Property(c => c.MinOrderAmount).HasColumnType("numeric(18,2)");
        });

        // Review unique per user per product
        modelBuilder.Entity<Review>(e =>
        {
            e.HasIndex(r => new { r.UserId, r.ProductId }).IsUnique();
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is Domain.Entities.BaseEntity entity)
            {
                if (entry.State == EntityState.Modified)
                    entity.UpdatedAt = DateTime.UtcNow;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }
}
