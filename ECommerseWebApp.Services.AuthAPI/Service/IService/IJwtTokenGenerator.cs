using ECommerseWebApp.Services.AuthAPI.Models;

namespace ECommerseWebApp.Services.AuthAPI.Service.IService
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser applicationUser,IEnumerable<string> roles);
    }
}
