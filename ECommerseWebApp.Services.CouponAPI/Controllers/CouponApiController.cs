using AutoMapper;
using ECommerseWebApp.Services.CouponAPI.Data;
using ECommerseWebApp.Services.CouponAPI.Models;
using ECommerseWebApp.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerseWebApp.Services.CouponAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponApiController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;

        public CouponApiController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _response = new ResponseDto();
            _mapper = mapper;

        }
        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Coupon> objList = _db.Coupons.ToList();
                _response.Result = _mapper.Map<IEnumerable<CouponDto>>(objList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
        [HttpGet("{id}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Coupon objList = _db.Coupons.First(u => u.CouponId == id);
                if (objList == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Coupon not found";
                }
                _response.Result = _mapper.Map<CouponDto>(objList);
                _response.IsSuccess = true;
                _response.Message = $"Coupon retrieved successfully for Id : {id}!";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDto GetByCode(string code)
        {
            try
            {
                Coupon objList = _db.Coupons.FirstOrDefault(u => u.CouponCode.ToLower() == code.ToLower());
                if (objList == null)
                {
                    _response.IsSuccess = false;
                }
                _response.Result = _mapper.Map<CouponDto>(objList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
        [HttpPost]
        public ResponseDto Post([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon obj = _mapper.Map<Coupon>(couponDto);
                _db.Coupons.Add(obj);
                _db.SaveChanges();
                _response.Result = _mapper.Map<CouponDto>(obj);
                _response.IsSuccess = true;
                _response.Message = "Coupon added successfully!";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
        [HttpPut]
        public ResponseDto Put(int id, [FromBody] CouponDto couponDto)
        {
            try
            {
                // Validate incoming data
                if (couponDto == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Invalid coupon data.";
                    return _response;
                }

                // Fetch the existing coupon by id
                var couponFromDb = _db.Coupons.FirstOrDefault(c => c.CouponId == id);

                // Check if coupon exists
                if (couponFromDb == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Coupon not found.";
                    return _response;
                }

                // Map changes from CouponDto to the existing coupon object, excluding CouponId
                _mapper.Map(couponDto, couponFromDb);

                // No need to call Update here since couponFromDb is already being tracked
                _db.SaveChanges();

                // Prepare the response
                _response.Result = _mapper.Map<CouponDto>(couponFromDb);
                _response.IsSuccess = true;
                _response.Message = "Coupon updated successfully!";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpDelete("{id}")]
        public ResponseDto Delete(int id)
        {
            try
            {

                Coupon obj = _db.Coupons.First(u => u.CouponId == id);
                _db.Coupons.Remove(obj);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
