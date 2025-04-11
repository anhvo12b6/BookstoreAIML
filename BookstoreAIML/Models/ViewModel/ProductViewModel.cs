namespace BookstoreAIML.Models.ViewModel
{
    public class ProductViewModel
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public string CoverImageURL { get; set; }
        public decimal Price { get; set; }
        public float Discount { get; set; }
        public double AverageRating { get; set; }
    }
}
