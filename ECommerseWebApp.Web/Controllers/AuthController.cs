using ECommerseWebApp.Web.Models;
using ECommerseWebApp.Web.Service.IService;
using ECommerseWebApp.Web.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace ECommerseWebApp.Web.Controllers
{
    //3,58,54
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;


        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _tokenProvider = tokenProvider;
        }

        [HttpGet]
        public IActionResult Login()
        {
            try
            {
                LoginRequestDto loginRequestDto = new LoginRequestDto();
                return View(loginRequestDto);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while loading the Login view.");
                TempData["error"] = "An error occurred while loading the page.";
                return RedirectToAction("Error", "Home");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            if (!ModelState.IsValid)
            {
                Log.Warning("Invalid login model state for user {Username}.", loginRequestDto.UserName);
                return View(loginRequestDto);
            }

            try
            {
                // Call the login service
                ResponseDto responseDto = await _authService.LoginAsync(loginRequestDto);

                // Check if login was successful
                if (responseDto == null || !responseDto.IsSuccess)
                {
                    Log.Warning("Login failed for user {Username}. Result: {Result}", loginRequestDto.UserName, responseDto);
                    ModelState.AddModelError("CustomError", responseDto?.Message ?? "Login failed. Please try again.");
                    TempData["error"] = responseDto?.Message ?? "Login failed. Please try again.";
                    return View(loginRequestDto);
                }

                // Safely deserialize login response
                var loginResponseDto = responseDto.Result != null
                    ? JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result))
                    : null;
                if (loginResponseDto == null)
                {
                    Log.Warning("Login response deserialization failed for user {Username}.", loginRequestDto.UserName);
                    TempData["error"] = "Login failed due to an internal error. Please try again.";
                    return View(loginRequestDto);
                }
                await SignInUser(loginResponseDto);
                // Assuming successful login, set success message and redirect
                _tokenProvider.SetToken(loginResponseDto.Token);
                TempData["success"] = "Login successful!";
                Log.Information("User {Username} logged in successfully.", loginRequestDto.UserName);
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Log any exception that occurred during the login process
                Log.Error(ex, "Error occurred during login for user {Username}.", loginRequestDto.UserName);
                TempData["error"] = "An error occurred while processing your login. Please try again.";
                return View(loginRequestDto);
            }
        }


        [HttpGet]
        public IActionResult Register()
        {
            try
            {
                var roleList = new List<SelectListItem>
                {
                    new SelectListItem{Text=SD.RoleAdmin,Value=SD.RoleAdmin},
                    new SelectListItem{Text=SD.RoleCustomer,Value=SD.RoleCustomer},
                };
                ViewBag.RoleList = roleList;
                return View();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while loading the Register view.");
                TempData["error"] = "An error occurred while loading the registration page.";
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto registrationRequestDto)
        {
            if (!ModelState.IsValid)
            {
                Log.Warning("Invalid registration model state.");
                var roleList = new List<SelectListItem>
                {
                    new SelectListItem{Text=SD.RoleAdmin,Value=SD.RoleAdmin},
                    new SelectListItem{Text=SD.RoleCustomer,Value=SD.RoleCustomer},
                };
                ViewBag.RoleList = roleList;
                return View(registrationRequestDto);
            }

            try
            {
                ResponseDto result = await _authService.RegisterAsync(registrationRequestDto);
                if (result == null || !result.IsSuccess)
                {
                    Log.Warning("Registration failed. Result: {Result}", result);
                    TempData["error"] = result?.Message ?? "Registration failed. Please try again.";
                    return View(registrationRequestDto);
                }

                // Assign role if registration succeeded
                if (string.IsNullOrEmpty(registrationRequestDto.Role))
                {
                    registrationRequestDto.Role = SD.RoleCustomer;
                }

                ResponseDto assignRole = await _authService.AssignRoleAsync(registrationRequestDto);
                if (assignRole == null || !assignRole.IsSuccess)
                {
                    Log.Warning("Role assignment failed. Result: {AssignRoleResult}", assignRole);
                    TempData["error"] = assignRole?.Message ?? "Role assignment failed.";
                    return View(registrationRequestDto);
                }

                TempData["success"] = "Registration successful!";
                Log.Information("User {Username} registered and role assigned successfully.", registrationRequestDto.Email);
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred during registration.");
                TempData["error"] = "An error occurred while processing your registration.";
                return View(registrationRequestDto);
            }
        }
        public async Task<IActionResult> LogoutUser()
        {
            try
            {
                await HttpContext.SignOutAsync();
                _tokenProvider.ClearToken();
                Log.Information("User logged out.");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred during logout.");
                TempData["error"] = "An error occurred while logging out.";
                return RedirectToAction("Error", "Home");
            }
        }

        private async Task SignInUser(LoginResponseDto model)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(model.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));


            identity.AddClaim(new Claim(ClaimTypes.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));

            identity.AddClaim(new Claim(ClaimTypes.Role,
                jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));


            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}
