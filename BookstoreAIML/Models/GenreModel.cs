using System.ComponentModel.DataAnnotations;

namespace BookstoreAIML.Models
{
    public class GenreModel
    {
        [Key]
        public int GenreID { get; set; }
        [Required,MinLength(4,ErrorMessage ="Yêu cầu nhập tên danh mục sách")]
        public string? GenreName { get; set; }
        [Required,MinLength(4,ErrorMessage ="Yêu cầu nhập mô tả danh mục")]
        public string? Description { get; set; }
        public int status { get; set; }
        public virtual ICollection<ProductViewModel> Products { get; set; }

    }
}
