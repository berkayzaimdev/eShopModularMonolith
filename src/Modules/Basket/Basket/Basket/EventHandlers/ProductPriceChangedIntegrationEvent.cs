namespace Basket.Basket.EventHandlers;

public class ProductPriceChangedIntegrationEventHandler
	(ISender sender, ILogger<ProductPriceChangedIntegrationEventHandler> logger)
	: IConsumer<ProductPriceChangedIntegrationEvent>
{
	public Task Consume(ConsumeContext<ProductPriceChangedIntegrationEvent> context)
	{
		logger.LogInformation("Integration Event handled: {IntegrationEvent}", context.Message.GetType().Name);

		return Task.CompletedTask;
	}
}