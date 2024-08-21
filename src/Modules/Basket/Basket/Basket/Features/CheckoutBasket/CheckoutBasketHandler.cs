namespace Basket.Basket.Features.CheckoutBasket;
public record CheckoutBasketCommand(BasketCheckoutDto BasketCheckout)
	: ICommand<CheckoutBasketResult>;
public record CheckoutBasketResult();
public class CheckoutBasketCommandValidator : AbstractValidator<CheckoutBasketCommand>
{
	public CheckoutBasketCommandValidator()
	{
	}
}
public class CheckoutBasketHandler
{
}
