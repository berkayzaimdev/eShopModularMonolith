var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();

//builder.Services.AddCarter(configurator: config =>
//{
//	var catalogModules = typeof(CatalogModule).Assembly.GetTypes()
//		.Where(t => t.IsAssignableTo(typeof(ICarterModule))).ToArray();

//	config.WithModules(catalogModules);
//});

// common services; carter, mediatr, fluent validation
var catalogAssembly = typeof(CatalogModule).Assembly;
var basketAssembly = typeof(BasketModule).Assembly;

builder.Services.AddCarterWithAssemblies(catalogAssembly, basketAssembly);

builder.Services.AddMediatR(config =>
{
	config.RegisterServicesFromAssemblies(catalogAssembly, basketAssembly);
	config.AddOpenBehavior(typeof(ValidationBehavior<,>));
	config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});

builder.Services.AddValidatorsFromAssemblies([catalogAssembly, basketAssembly]);

builder.Services
	.AddCatalogModule(builder.Configuration)
	.AddBasketModule(builder.Configuration)
	.AddOrderingModule(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var app = builder.Build();

app.MapCarter();
app.UseSerilogRequestLogging();
app.UseExceptionHandler(opts => { });

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
