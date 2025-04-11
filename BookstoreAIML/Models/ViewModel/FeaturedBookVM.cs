namespace BookstoreAIML.Models.ViewModel
{
    public class FeaturedBookVM
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public string CoverImageURL { get; set; }
        public decimal Price { get; set; }
        public double AverageRating { get; set; }
        public Decimal Discount { get; set; } 
    }
}
