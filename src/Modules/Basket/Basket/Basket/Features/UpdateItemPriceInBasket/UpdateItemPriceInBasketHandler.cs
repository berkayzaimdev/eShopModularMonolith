﻿
namespace Basket.Basket.Features.UpdateItemPriceInBasket;

public record UpdateItemPriceInBasketCommand(Guid ProductId, decimal Price)
	: ICommand<UpdateItemPriceInBasketResult>;
public record UpdateItemPriceInBasketResult(bool IsSuccess);
public class UpdateItemPriceInBasketCommandValidator : AbstractValidator<UpdateItemPriceInBasketCommand>
{
	public UpdateItemPriceInBasketCommandValidator()
	{
		RuleFor(x => x.ProductId).NotEmpty().WithMessage("ProductId is required");
		RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
	}
}

internal class UpdateItemPriceInBasketHandler(BasketDbContext dbContext)
	: ICommandHandler<UpdateItemPriceInBasketCommand, UpdateItemPriceInBasketResult>
{
	public Task<UpdateItemPriceInBasketResult> Handle(UpdateItemPriceInBasketCommand request, CancellationToken cancellationToken)
	{
		throw new NotImplementedException();
	}
}