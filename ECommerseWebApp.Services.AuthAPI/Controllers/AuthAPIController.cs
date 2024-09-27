using ECommerseWebApp.Services.AuthAPI.Models.Dto;
using ECommerseWebApp.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerseWebApp.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDto _response;
        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
            _response = new();
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto registrationRequestDto) 
        {
            var errorMessage = await _authService.Register(registrationRequestDto);
            if (!string.IsNullOrEmpty(errorMessage)) 
            {
                _response.IsSuccess = false;
                _response.Message = errorMessage;
                return BadRequest(_response);
            }
            _response.IsSuccess = true;
            _response.Message = errorMessage;
            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            try
            {
                // Call the login service
                var loginResponse = await _authService.Login(loginRequestDto);

                // Check if the login was unsuccessful
                if (loginResponse?.User == null)
                {
                    return BadRequest(new ResponseDto
                    {
                        IsSuccess = false,
                        Message = "Username or Password is incorrect!",
                        Result = null
                    });
                }

                // If successful, return the login response with the token and user details
                return Ok(new ResponseDto
                {
                    IsSuccess = true,
                    Message = "Login successful!",
                    Result = new
                    {
                        User = loginResponse.User,   // Send user details
                        Token = loginResponse.Token  // Send JWT token
                    }
                });
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return StatusCode(500, new ResponseDto
                {
                    IsSuccess = false,
                    Message = "An error occurred while processing your login request.",
                    Result = null
                });
            }
        }


        [HttpPost("assignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {
            var assignRoleSuccessfull = await _authService.AssignRole(model.Email,model.Role.ToUpper());
            if (!assignRoleSuccessfull)
            {
                _response.IsSuccess = false;
                _response.Message = "Error Encountered!";
                return BadRequest(_response);
            }
            _response.IsSuccess = true;
            return Ok(_response);
        }
    }
}
