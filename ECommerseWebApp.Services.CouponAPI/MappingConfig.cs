using AutoMapper;
using ECommerseWebApp.Services.CouponAPI.Models;
using ECommerseWebApp.Services.CouponAPI.Models.Dto;

namespace ECommerseWebApp.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CouponDto, Coupon>();
                config.CreateMap<Coupon, CouponDto>();

            });
            return mappingConfig;
        }
    }
}
