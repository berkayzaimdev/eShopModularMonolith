

using Keycloak.AuthServices.Authentication;

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
var orderingAssembly = typeof(OrderingModule).Assembly;

Assembly[] assemblies =
{
	catalogAssembly,
	basketAssembly,
	orderingAssembly
};

builder.Services.AddCarterWithAssemblies(assemblies);

builder.Services.AddMediatRWithAssemblies(assemblies);

builder.Services.AddStackExchangeRedisCache(opts =>
{
	opts.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.AddMassTransitWithAssemblies(builder.Configuration, assemblies);

builder.Services.AddKeycloakWebApiAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

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
app.UseAuthentication();
app.UseAuthorization();

app
	.UseCatalogModule()
	.UseBasketModule()
	.UseOrderingModule();

await app.RunAsync();
