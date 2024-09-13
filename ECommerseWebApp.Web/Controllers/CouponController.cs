using ECommerseWebApp.Web.Models;
using ECommerseWebApp.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog;

namespace ECommerseWebApp.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

        // Using Serilog directly
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService ?? throw new ArgumentNullException(nameof(couponService));
        }

        public async Task<IActionResult> CouponIndex()
        {
            try
            {
                List<CouponDto>? list = new();
                ResponseDto? response = await _couponService.GetAllCouponsAsync();

                if (response != null && response.IsSuccess)
                {
                    list = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result)) ?? new List<CouponDto>();
                }
                Log.Information("Successfully retrieved all coupon");

                return View(list);
            }
            catch (Exception ex)
            {
                // Log error with Serilog using structured logging
                Log.Error(ex, "Error occurred while fetching all coupons.");
                ViewBag.ErrorMessage = "An error occurred while fetching the coupons. Please try again later.";
                return View(new List<CouponDto>());
            }
        }
        // Display the create coupon form
        public IActionResult CouponCreate()
        {
            return View();
        }

        // Handle the coupon creation form submission
        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDto couponDto)
        {
            if (!ModelState.IsValid)
            {
                return View(couponDto);
            }

            try
            {
                ResponseDto? response = await _couponService.CreateCouponAsync(couponDto);

                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    ViewBag.ErrorMessage = response?.Message ?? "An error occurred while creating the coupon.";
                    return View(couponDto);
                }
            }
            catch (Exception ex)
            {
                // Log the exception and display a user-friendly error message
                // _logger.LogError(ex, "Error occurred while creating the coupon");
                ViewBag.ErrorMessage = "An unexpected error occurred while creating the coupon. Please try again.";
                return View(couponDto);
            }
        }

        public async Task<IActionResult> CouponDelete(int couponId)
        {
            try
            {
                if (couponId <= 0)
                {
                    Log.Warning("Invalid Coupon ID {CouponId} provided for deletion.", couponId);
                    return NotFound();
                }

                ResponseDto? response = await _couponService.GetCouponByIdAsync(couponId);

                if (response != null && response.IsSuccess)
                {
                    CouponDto? model = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));

                    if (model != null)
                    {
                        return View(model);
                    }
                }

                Log.Warning("Coupon with ID {CouponId} not found for deletion.", couponId);
                return NotFound();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while retrieving coupon with ID {CouponId} for deletion.", couponId);
                ViewBag.ErrorMessage = "An error occurred while trying to retrieve the coupon. Please try again later.";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> CouponDelete(CouponDto couponDto)
        {
            try
            {
                ResponseDto? response = await _couponService.DeleteCoupon(couponDto.CouponId);

                if (response != null && response.IsSuccess)
                {
                    // Log successful deletion with Serilog
                    Log.Information("Successfully deleted coupon with ID {CouponId}", couponDto.CouponId);
                    return RedirectToAction(nameof(CouponIndex));
                }

                ViewBag.ErrorMessage = response?.Message ?? "An error occurred while deleting the coupon.";
                Log.Warning("Failed to delete coupon with ID {CouponId}. Reason: {Message}", couponDto.CouponId, response?.Message);
                return View(couponDto);
            }
            catch (Exception ex)
            {
                // Log the error with structured data
                Log.Error(ex, "Error occurred while deleting coupon with ID {CouponId}", couponDto.CouponId);
                ViewBag.ErrorMessage = "An unexpected error occurred while deleting the coupon. Please try again.";
                return View(couponDto);
            }
        }
    }
}
