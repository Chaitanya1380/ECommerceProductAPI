using ECommerceProjectAPI.DTOS;
using Microsoft.AspNetCore.Identity;
using System;


namespace ECommerceProjectAPI.ServiceContracts
{
    public interface IJwtService
    {
        public interface IJwtService
        {
            AuthenticationResponse CreateJwtToken(IdentityUser user);
        }
    }
}
