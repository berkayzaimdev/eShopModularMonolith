using Catalog.Data.Seed;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Data;
using Shared.Data.Interceptors;
using Shared.Data.Seed;

namespace Catalog;

public static class CatalogModule
{
	public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddMediatR(config =>
		{
			config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
		});

		var connectionString = configuration.GetConnectionString("Database");

		services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
		services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

		services.AddDbContext<CatalogDbContext>((sp, opts) =>
		{
			opts.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
			opts.UseNpgsql(connectionString);
		});

		services.AddScoped<IDataSeeder, CatalogDataSeeder>();

		return services;
	}

	public static IApplicationBuilder UseCatalogModule(this IApplicationBuilder app)
	{
		//InitializeDatabaseAsync(app).GetAwaiter().GetResult();
		app.UseMigration<CatalogDbContext>();
		return app;
	}

	//private static async Task InitializeDatabaseAsync(IApplicationBuilder app)
	//{
	//	using var scope = app.ApplicationServices.CreateScope();

	//	var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

	//	await context.Database.MigrateAsync();
	//}
}
