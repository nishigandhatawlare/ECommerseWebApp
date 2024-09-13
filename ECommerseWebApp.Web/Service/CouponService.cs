using ECommerseWebApp.Web.Models;
using ECommerseWebApp.Web.Service.IService;
using ECommerseWebApp.Web.Utility;
using static ECommerseWebApp.Web.Utility.SD;

namespace ECommerseWebApp.Web.Service
{
   
    public class CouponService : ICouponService
    {
        private readonly IBaseService _baseService;
        public CouponService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto>? CreateCouponAsync(CouponDto couponDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.Post,
                Data = couponDto,
                Url = CouponApiBase + "/api/couponapi/"
            });
        }

        public async Task<ResponseDto>? DeleteCoupon(int couponId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.Delete,
                Url = $"{CouponApiBase}/api/couponapi/{couponId}"
            });
        }

        public async Task<ResponseDto>? GetAllCouponsAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
            ApiType = ApiType.Get,
            Url = CouponApiBase + "/api/couponapi"
            });
        }

        public async Task<ResponseDto>? GetCouponAsync(string couponCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.Get,
                Url = CouponApiBase + "/api/couponapi/GetByCode/" + couponCode
            });
        }

        public async Task<ResponseDto>? GetCouponByIdAsync(int couponId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.Get,
                Url = $"{CouponApiBase}/api/couponapi/{couponId}"
            });
        }

        public async Task<ResponseDto>? UpdateCouponAsync(int couponId, CouponDto couponDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = ApiType.Put,
                Data = couponDto,
                Url = CouponApiBase + "/api/couponapi/" + couponId
            });
        }
    }
}
