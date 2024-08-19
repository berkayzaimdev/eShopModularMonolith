using Shared.Exceptions;

namespace Catalog.Exceptions;

public class ProductNotFoundException : NotFoundException
{
	public ProductNotFoundException(Guid id) : base("Product", id)
	{
	}
}
