namespace BookstoreAIML.Models.ViewModel
{
    public class ProductSearchViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CoverImageURL { get; set; }
        public decimal Price { get; set; }
        public string GenreName { get; set; } // nếu muốn hiển thị thể lo
    }
}
