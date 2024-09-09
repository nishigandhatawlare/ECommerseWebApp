using ECommerseWebApp.Web.Models;

namespace ECommerseWebApp.Web.Service.IService
{
    public interface IBaseService
    {
        Task<ResponseDto?> SendAsync(RequestDto requestDto);
    }
}
