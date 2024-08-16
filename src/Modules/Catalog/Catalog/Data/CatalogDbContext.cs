namespace Catalog.Data;

public class CatalogDbContext : DbContext
{
	public CatalogDbContext(DbContextOptions options) : base(options)
	{
	}

	public DbSet<Product> Products => Set<Product>();

	protected override void OnModelCreating(ModelBuilder builder)
	{
		builder.HasDefaultSchema("catalog");
		builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		base.OnModelCreating(builder);
	}
}
