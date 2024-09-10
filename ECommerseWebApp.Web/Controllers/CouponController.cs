using ECommerseWebApp.Web.Models;
using ECommerseWebApp.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace ECommerseWebApp.Web.Controllers
{
    //stopped 2,05,47
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }
        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDto>? list = new();
            ResponseDto response = await _couponService.GetAllCouponsAsync();
            if (response != null && response.IsSuccess == true)
            {
                list = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result));
            }
            return View(list);
        }
		public async Task<IActionResult> CouponCreate()
		{
            return View();
		}
        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDto couponDto)
        {
            if (ModelState.IsValid) 
            {
                ResponseDto response = await _couponService.CreateCouponAsync(couponDto);
                if (response != null && response.IsSuccess == true)
                {
                    return RedirectToAction(nameof(CouponIndex));
                }
            }
            return View(couponDto);
        }
    }
}
