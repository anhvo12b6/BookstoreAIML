using Microsoft.AspNetCore.Identity;
namespace BookstoreAIML.Models
{
    public class AppUserModel:IdentityUser
    {
        public string? Ocupation { get; set; }
    }
}
