using ECommerseWebApp.Web.Models;
using ECommerseWebApp.Web.Service.IService;
using ECommerseWebApp.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerseWebApp.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new LoginRequestDto();
            return View(loginRequestDto);
        }
        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>
            {
                new SelectListItem{Text=SD.RoleAdmin,Value=SD.RoleAdmin},
                new SelectListItem{Text=SD.RoleCustomer,Value=SD.RoleCustomer},
            };
            //pass roleList to view
            ViewBag.RoleList = roleList;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto registrationRequestDto)
        {
            ResponseDto result = await _authService.RegisterAsync(registrationRequestDto);
            ResponseDto assignRole;
            if (result != null && result.IsSuccess == true) 
            {
                if (string.IsNullOrEmpty(registrationRequestDto.Role)) 
                {
                    registrationRequestDto.Role = SD.RoleCustomer;
                }
                assignRole = await _authService.AssignRoleAsync(registrationRequestDto);
                if (assignRole != null && assignRole.IsSuccess == true) 
                {
                    TempData["success"] = "Registration Successful!";
                    return RedirectToAction(nameof(Login));
                }
            }
            var roleList = new List<SelectListItem>
            {
                new SelectListItem{Text=SD.RoleAdmin,Value=SD.RoleAdmin},
                new SelectListItem{Text=SD.RoleCustomer,Value=SD.RoleCustomer},
            };
            //pass roleList to view
            ViewBag.RoleList = roleList;
            return View(registrationRequestDto);
        }
        public IActionResult Logout()
        {
            return View();
        }
    }
}
