using ECommerseWebApp.Web.Models;
using ECommerseWebApp.Web.Service.IService;
using static ECommerseWebApp.Web.Utility.SD;

namespace ECommerseWebApp.Web.Service
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;
        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto>? AssignRoleAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.Post,
                Data = registrationRequestDto,
                Url = AuthApiBase + "/api/auth/assignRole"
            });
        }

        public async Task<ResponseDto>? LoginAsync(LoginRequestDto loginRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.Post,
                Data = loginRequestDto,
                Url = AuthApiBase + "/api/auth/login"
            }, withBearer: false);
        }

        public async  Task<ResponseDto>? RegisterAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.Post,
                Data = registrationRequestDto,
                Url = AuthApiBase + "/api/auth/register"
            }, withBearer: false);
        }
    }
}
