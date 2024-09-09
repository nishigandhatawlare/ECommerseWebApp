using ECommerseWebApp.Web.Models;

namespace ECommerseWebApp.Web.Service.IService
{
    public interface ICouponService
    {
        public Task<ResponseDto>? GetCouponAsync(string couponCode);
        public Task<ResponseDto>? GetCouponByIdAsync(int couponId);
        public Task<ResponseDto>? GetAllCouponsAsync();
        public Task<ResponseDto>? CreateCouponAsync(CouponDto couponDto);
        public Task<ResponseDto>? UpdateCouponAsync(int couponId,CouponDto couponDto);
        public Task<ResponseDto>? DeleteCoupon(int couponId);

    }
}
