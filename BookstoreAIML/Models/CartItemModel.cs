namespace BookstoreAIML.Models
{
    public class CartItemModel
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; } 
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public decimal TotalPrice => Price * Quantity;

        public CartItemModel()
        {

        }

        public CartItemModel(ProductViewModel product)
        {
            ProductId = product.BookID;
            ProductName = product.Title;
            Quantity = 1; // Default quantity
            Price = product.Price;
            ImageUrl = product.CoverImageURL ?? string.Empty; // Default to empty string if null
        }
    }
}
