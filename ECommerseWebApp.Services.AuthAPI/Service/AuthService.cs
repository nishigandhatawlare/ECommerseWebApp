﻿using ECommerseWebApp.Services.AuthAPI.Data;
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
        public AuthService(AppDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            throw new NotImplementedException();
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
