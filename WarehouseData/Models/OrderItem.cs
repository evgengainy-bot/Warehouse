namespace WarehouseData.Models
{
    public class OrderItem
    {
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal PriceAtMoment { get; set; } 

        public OrderItem() { }

        public OrderItem(Product product, int quantity)
        {
            Product = product;
            Quantity = quantity;
            PriceAtMoment = product.Price;
        }

        public string DisplayLine =>
            $"{Product.Name} — {Quantity} {Product.Unit} × {PriceAtMoment:N2} руб.";
    }
}