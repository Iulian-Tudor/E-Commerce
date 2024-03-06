using Commerce.Domain;
using Commerce.Business;
using Microsoft.EntityFrameworkCore;

namespace Commerce.Infrastructure;

public class EfDbContext : DbContext
{
    public DbSet<UserReadModel> Users { get; set; }

    public DbSet<UserGateReadModel> UserGates { get; set; }

    public DbSet<ProductReadModel> Products { get; set; }

    public DbSet<OrderedProduct> OrderedProducts { get; set; }

    public DbSet<FavoriteProductSnapshotReadModel> FavoriteProductSnapshots { get; set; }

    public DbSet<CategoryReadModel> Categories { get; set; }

    public DbSet<OrderReadModel> Orders { get; set; }

    public DbSet<RefreshTokenReadModel> RefreshTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var isIntegrationTestEnvironment = Environment.GetEnvironmentVariable("IS_INTEGRATION_TEST_ENVIRONMENT");

        if (isIntegrationTestEnvironment == "true")
        {
            optionsBuilder.UseInMemoryDatabase("Commerce");
            return;
        }

        var connectionString = CommerceEnvironment.IsDevDockerEnvironment
            ? "Host=postgres;Database=Commerce;Username=commerce;Password=Pass4Commerce1!" 
            : "Host=localhost;Database=Commerce;Username=commerce;Password=Pass4Commerce1!";
        optionsBuilder.UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<UserReadModel>()
            .HasKey(u => u.Id);

        modelBuilder 
            .Entity<UserGateReadModel>() 
            .HasKey(ug => ug.Id);

        modelBuilder
            .Entity<CategoryReadModel>()
            .HasKey(c => c.Id);

        modelBuilder
            .Entity<ProductReadModel>()
            .HasKey(p => p.Id);

        modelBuilder
            .Entity<ProductReadModel>()
            .OwnsOne(p => p.MediaAsset, nb =>
            {
                nb.ToJson();
            });

        modelBuilder
            .Entity<OrderedProduct>()
            .HasKey(ps => ps.Id);

        modelBuilder
            .Entity<FavoriteProductSnapshotReadModel>()
            .HasKey(fps => fps.Id);

        modelBuilder
            .Entity<OrderReadModel>()
            .HasMany(e => e.Products)
            .WithOne()
            .HasForeignKey($"{nameof(OrderReadModel)}Id")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<RefreshTokenReadModel>()
            .HasKey(rt => rt.Id);
    }
}