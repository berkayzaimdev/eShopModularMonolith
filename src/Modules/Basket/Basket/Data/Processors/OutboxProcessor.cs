using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace Basket.Data.Processors;

public class OutboxProcessor 
	(IServiceProvider serviceProvider, IBus bus, ILogger<OutboxProcessor> logger)
	: BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
        while (!stoppingToken.IsCancellationRequested)
        {
			try
			{
				using var scope = serviceProvider.CreateScope();
				var dbContext = scope.ServiceProvider.GetRequiredService<BasketDbContext>();
				var outboxMessages = await dbContext.OutboxMessages
					.Where(m => m.ProcessedOn == null)
					.ToListAsync(stoppingToken);

                foreach (var message in outboxMessages)
                {
                    var eventType = Type.GetType(message.Type);
					if (eventType is null) 
					{
						logger.LogWarning("Could not resolve type: {Type}", message.Type);
						continue;
					}

					var eventMessage = JsonSerializer.Deserialize(message.Content, eventType);
					if (eventMessage is null)
					{
						logger.LogWarning("Could not deserialize message: {Content}", message.Content);
						continue;
					}

					await bus.Publish(eventMessage, stoppingToken);

					message.ProcessedOn = DateTime.UtcNow;

					logger.LogInformation("Successfully processed outbox message with ID: {ID}", message.Id);
				}

				await dbContext.SaveChangesAsync(stoppingToken);
            }

			catch (Exception ex)
			{
				logger.LogError(ex, "Error processing outbox messages");
			}

			await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
