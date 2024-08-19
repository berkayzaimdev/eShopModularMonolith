using Basket;
using Carter;
using Catalog;
using Ordering;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddCarter(configurator: config =>
{
	var catalogModules = typeof(CatalogModule).Assembly.GetTypes()
		.Where(t => t.IsAssignableTo(typeof(ICarterModule))).ToArray();

	config.WithModules(catalogModules);
});

builder.Services
	.AddCatalogModule(builder.Configuration)
	.AddBasketModule(builder.Configuration)
	.AddOrderingModule(builder.Configuration);

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var app = builder.Build();

app
	.UseCatalogModule()
	.UseBasketModule()
	.UseOrderingModule();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();
