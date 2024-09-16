using Microsoft.AspNetCore.Identity;

namespace ECommerseWebApp.Services.AuthAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name {  get; set; }
    }
}
