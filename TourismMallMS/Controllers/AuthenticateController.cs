using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TourismMallMS.Dtos;
using Microsoft.AspNetCore.Identity;
using TourismMallMS.Models.Entities;
using AutoMapper;
using TourismMallMS.Services;

namespace TourismMallMS.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthenticateController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IShoppingCartRepository _shoppingCartRepository;

        public AuthenticateController(IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IShoppingCartRepository shoppingCartRepository
        )
        {
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _shoppingCartRepository = shoppingCartRepository;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var loginResult = await _signInManager.PasswordSignInAsync(
                loginDto.Email,
                loginDto.Password,
                false,
                false
            );
            if (!loginResult.Succeeded)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByNameAsync(loginDto.Email);

            var signingAlgorithm = SecurityAlgorithms.HmacSha256;
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            };

            var roleNames = await _userManager.GetRolesAsync(user);
            foreach (var roleName in roleNames)
            {
                var roleClaim = new Claim(ClaimTypes.Role, roleName);
                claims.Add(roleClaim);
            }

            var secretByte = Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]);
            var signingkey = new SymmetricSecurityKey(secretByte);
            var signingCredentials = new SigningCredentials(signingkey, signingAlgorithm);

            var token = new JwtSecurityToken(
                issuer: _configuration["Authentication:Issuer"],
                audience: _configuration["Authentication:Audience"],
                claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials
            );

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(tokenStr);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var user = new ApplicationUser()
            {
                UserName = registerDto.Email,
                Email = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest();
            }

            var shoppingCart = new ShoppingCart()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id
            };

            await _shoppingCartRepository.CreateShoppingCartAsync(shoppingCart);
            await _shoppingCartRepository.SaveAsync();

            return Ok();
        }
    }

}
