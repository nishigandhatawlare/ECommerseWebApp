﻿namespace ECommerseWebApp.Web.Utility
{
    //static details
    public class SD
    {
        public static string CouponApiBase { get; set; }
        public static string AuthApiBase { get; set; }
        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";
        public const string TokenCookie = "JWTToken";


        public enum ApiType 
        {
            Get,
            Post,
            Put,
            Delete
        }
    }
}
