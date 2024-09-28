using ECommerseWebApp.Services.AuthAPI.Data;
using ECommerseWebApp.Services.AuthAPI.Models;
using ECommerseWebApp.Services.AuthAPI.Models.Dto;
using ECommerseWebApp.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;

namespace ECommerseWebApp.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, IJwtTokenGenerator jwtTokenGenerator, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            //if user is not null 
            if (user != null)
            {
                //if role is not present
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult()) 
                { 
                //create role if it does not exists
                _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            // Retrieve the existing user by username
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());

            // Check if the user exists
            if (user == null)
            {
                return new LoginResponseDto()
                {
                    User = null,
                    Token = ""
                };
            }

            // Validate the password if the user exists
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (!isValid)
            {
                return new LoginResponseDto()
                {
                    User = null,
                    Token = ""
                };
            }

            // Generate JWT Token
            var token = _jwtTokenGenerator.GenerateToken(user);

            // Map the user entity to the UserDto
            UserDto userDto = new()
            {
                Email = user.Email,
                ID = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };

            // Return the login response with user and token
            return new LoginResponseDto()
            {
                User = userDto,
                Token = token
            };
        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser applicationUser = new()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber
            };
            try
            {
                //create a user with registration
                var result =await _userManager.CreateAsync(applicationUser, registrationRequestDto.Password);
                //user successfully created, then return such user, whose username is equal to the registrationRequestDto.email
                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.First(u => u.Email == registrationRequestDto.Email);
                    UserDto userDto = new()
                    {
                        Email = userToReturn.Email,
                        ID = userToReturn.Id,
                        Name = userToReturn.Name,
                        PhoneNumber = userToReturn.PhoneNumber
                    };
                    return "";
                }
                else 
                {
                   return result.Errors.FirstOrDefault().Description;
                }

            }
            catch (Exception ex) 
            {
                
            }
            return "Error Encountered!";
          
        }

    }
}
