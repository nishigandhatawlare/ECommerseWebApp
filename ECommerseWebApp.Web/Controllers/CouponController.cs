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
					//TempData["success"] = "Coupons retrieved successfully!";
				}
				else
				{
					TempData["error"] = response?.Message ?? "Error retrieving coupons.";
				}
				Log.Information("Successfully retrieved all coupons.");
				return View(list);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Error occurred while fetching all coupons.");
				TempData["error"] = "An error occurred while fetching the coupons. Please try again later.";
				return View(new List<CouponDto>());
			}
		}

		public IActionResult CouponCreate()
		{
			return View();
		}

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
					TempData["success"] = "Coupon created successfully!";
					return RedirectToAction(nameof(CouponIndex));
				}
				else
				{
					TempData["error"] = response?.Message ?? "Error creating coupon.";
					return View(couponDto);
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Error occurred while creating the coupon.");
				TempData["error"] = "An unexpected error occurred while creating the coupon. Please try again.";
				return View(couponDto);
			}
		}

		public async Task<IActionResult> CouponDelete(int couponId)
		{
			try
			{
				if (couponId <= 0)
				{
					TempData["error"] = "Invalid coupon ID provided.";
					return NotFound();
				}

				ResponseDto? response = await _couponService.GetCouponByIdAsync(couponId);

				if (response != null && response.IsSuccess)
				{
					CouponDto? model = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
					return View(model);
				}

				TempData["error"] = "Coupon not found.";
				return NotFound();
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Error occurred while retrieving coupon for deletion.");
				TempData["error"] = "An error occurred while retrieving the coupon. Please try again later.";
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
					TempData["success"] = "Coupon deleted successfully!";
					Log.Information("Successfully deleted coupon with ID {CouponId}", couponDto.CouponId);
					return RedirectToAction(nameof(CouponIndex));
				}
				else
				{
					TempData["error"] = response?.Message ?? "Error deleting coupon.";
				}

				Log.Warning("Failed to delete coupon with ID {CouponId}. Reason: {Message}", couponDto.CouponId, response?.Message);
				return View(couponDto);
			}
			catch (Exception ex)
			{
				Log.Error(ex, "Error occurred while deleting coupon with ID {CouponId}", couponDto.CouponId);
				TempData["error"] = "An unexpected error occurred while deleting the coupon. Please try again.";
				return View(couponDto);
			}
		}
	}
}
