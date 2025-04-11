using System.ComponentModel.DataAnnotations;

namespace BookstoreAIML.Models.ViewModel
{
    public class LoginViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập tên tài khoản")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Nhập password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
