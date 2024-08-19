using Shared.DDD;

namespace Basket.Basket.Models;

public class ShoppingCart : Aggregate<Guid>
{
	public string UserName { get; private set; } = default!;
	private readonly List<ShoppingCartItem> _items = new();
	public IReadOnlyList<ShoppingCartItem> Items => _items.AsReadOnly();
	public decimal TotalPrice => Items.Sum(x => x.Price * x.Quantity);
}

public class ShoppingCartItem : Entity<Guid>
{
	public Guid ShoppingCartId { get; private set; } = default!;
	public Guid ProductId { get; private set; } = default!;
	public int Quantity { get; internal set; } = default!;
	public string Color { get; private set; } = default!;

	// will comes from Catalog module
	public decimal Price { get; private set; } = default!;
	public string ProductName { get; private set; } = default!;

	public ShoppingCartItem(Guid shoppingCartId, Guid productId, int quantity, string color, decimal price, string productName)
	{
		ShoppingCartId = shoppingCartId;
		ProductId = productId;
		Quantity = quantity;
		Color = color;
		Price = price;
		ProductName = productName;
	}
}
