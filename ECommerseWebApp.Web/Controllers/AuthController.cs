using ECommerseWebApp.Web.Models;
using ECommerseWebApp.Web.Service.IService;
using ECommerseWebApp.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Serilog;
using System;

namespace ECommerseWebApp.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
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

                // Assuming successful login, set success message and redirect
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

        public IActionResult Logout()
        {
            try
            {
                // Add logic for logging out if needed
                Log.Information("User logged out.");
                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred during logout.");
                TempData["error"] = "An error occurred while logging out.";
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
