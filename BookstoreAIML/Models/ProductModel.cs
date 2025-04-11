using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BookstoreAIML.Models
{
    public class ProductViewModel
    {
        [Key]
        public long BookID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề sách")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên tác giả")]
        public string Author { get; set; }

        public string? ISBN { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn nhà xuất bản")]
        public int? PublisherID { get; set; }

        public DateTime? PublicationDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn thể loại")]
        public int? GenreID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá bán")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }

        [Range(0, 100, ErrorMessage = "Chiết khấu phải từ 0 đến 100")]
        public decimal Discount { get; set; } = 0;

        [Required(ErrorMessage = "Vui lòng nhập số lượng tồn kho")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải lớn hơn hoặc bằng 0")]
        public int StockQuantity { get; set; }

        public string? Description { get; set; }

        public string? CoverImageURL { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số trang phải lớn hơn 0")]
        public int? Pages { get; set; }

        public string? Language { get; set; }

        [Range(0, 5)]
        public decimal AverageRating { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public string? Image { get; set; }

        public virtual GenreModel? Genre { get; set; }
    }
}
