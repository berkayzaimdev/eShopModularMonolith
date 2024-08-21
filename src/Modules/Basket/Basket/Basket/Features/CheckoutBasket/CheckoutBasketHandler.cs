
namespace Basket.Basket.Features.CheckoutBasket;
public record CheckoutBasketCommand(BasketCheckoutDto BasketCheckout)
	: ICommand<CheckoutBasketResult>;
public record CheckoutBasketResult(bool IsSuccess);
public class CheckoutBasketCommandValidator : AbstractValidator<CheckoutBasketCommand>
{
	public CheckoutBasketCommandValidator()
	{
		RuleFor(x => x.BasketCheckout).NotNull().WithMessage("BasketCheckoutDto can't be null");
		RuleFor(x => x.BasketCheckout.UserName).NotEmpty().WithMessage("UserName is required");
	}
}
internal class CheckoutBasketHandler
	(IBasketRepository repository, IBus bus)
	: ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
{
	public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand request, CancellationToken cancellationToken)
	{
		var basket = await repository.GetBasket(request.BasketCheckout.UserName, true, cancellationToken);

		var eventMessage = request.BasketCheckout.Adapt<BasketCheckoutIntegrationEvent>();

		eventMessage.TotalPrice = basket.TotalPrice;

		await bus.Publish(eventMessage, cancellationToken);

		await repository.DeleteBasket(request.BasketCheckout.UserName, cancellationToken);

		return new CheckoutBasketResult(true);
	}
}
