using System.ComponentModel.DataAnnotations;

namespace BookstoreAIML.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập tên tài khoản")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập email")]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Nhập password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
