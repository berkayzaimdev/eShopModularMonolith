using FluentValidation;

namespace Catalog.Products.Features.CreateProduct;

public record CreateProductCommand(ProductDto Product)
	: ICommand<CreateProductResult>;

public record CreateProductResult(Guid Id);

public class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator()
    {
		RuleFor(x => x.Product.Name).NotEmpty().WithMessage("Name is required");
		RuleFor(x => x.Product.Category).NotEmpty().WithMessage("Category are required");
		RuleFor(x => x.Product.ImageFile).NotEmpty().WithMessage("Image is required");
		RuleFor(x => x.Product.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
    }
}


internal class CreateProductHandler 
	(CatalogDbContext dbContext,
	 IValidator<CreateProductCommand> validator)
	: ICommandHandler<CreateProductCommand, CreateProductResult>
{
	public async Task<CreateProductResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
	{
		var result = await validator.ValidateAsync(request, cancellationToken);
		var errors = result.Errors.Select(x => x.ErrorMessage).ToList();
		if (errors.Any())
		{
			throw new ValidationException(errors.FirstOrDefault());
		}

		var product = CreateNewProduct(request.Product);
		dbContext.Products.Add(product);
		await dbContext.SaveChangesAsync(cancellationToken);
		
		return new CreateProductResult(product.Id);
	}

	private Product CreateNewProduct(ProductDto productDto)
	{
		var product = Product.Create(
			Guid.NewGuid(),
			productDto.Name,
			productDto.Category,
			productDto.Description,
			productDto.ImageFile,
			productDto.Price
		);

		return product;
	}
}
