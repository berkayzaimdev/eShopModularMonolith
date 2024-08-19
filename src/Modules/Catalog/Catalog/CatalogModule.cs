using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Behaviors;
using Shared.Data.Interceptors;

namespace Catalog;

public static class CatalogModule
{
	public static IServiceCollection AddCatalogModule(this IServiceCollection services, IConfiguration configuration)
	{
		Assembly assembly = Assembly.GetExecutingAssembly();

		services.AddValidatorsFromAssembly(assembly);

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
