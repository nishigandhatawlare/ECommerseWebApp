﻿using ECommerseWebApp.Services.AuthAPI.Models.Dto;

namespace ECommerseWebApp.Services.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<UserDto> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
    }
}
