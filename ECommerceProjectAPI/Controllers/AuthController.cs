using ECommerceProjectAPI.DTOS;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ECommerceProjectAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace ECommerceProjectAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtService _jwtService;

        //public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        //{
        //    _userManager = userManager;
        //    _signInManager = signInManager;
        //    _roleManager = roleManager;
        //}
        //[HttpPost("Register")]
        //public async Task<ActionResult<IdentityUser>> PostRegistration(RegisterDTOS registerDTOS)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        string ErrorMessage = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
        //        return Problem(ErrorMessage);
        //    }
        //    // Create user using IdentityUser with minimal attributes
        //    IdentityUser user = new IdentityUser
        //    {
        //        Email = registerDTOS.Email,
        //        UserName = registerDTOS.Email
        //    };

        //    // Attempt to create user
        //    IdentityResult result = await _userManager.CreateAsync(user, registerDTOS.Password);

        //    if (result.Succeeded)
        //    {
        //        // Sign in the user
        //        await _signInManager.SignInAsync(user, isPersistent: false);
        //        return Ok(user);
        //    }
        //    else
        //    {
        //        // Return error messages
        //        string errorMessage = string.Join("|", result.Errors.Select(e => e.Description));
        //        return Problem(errorMessage);
        //    }
        //}

        //[HttpGet]
        //public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
        //{
        //    IdentityUser user = await _userManager.FindByEmailAsync(email);

        //    if (user == null)
        //    {
        //        return Ok(true); // Email is not registered
        //    }
        //    else
        //    {
        //        return Ok(false); // Email is already registered
        //    }
        //}

        //[HttpPost("LogIn")]
        //public async Task<ActionResult<IdentityUser>> PostLogIn(LogInDTOS logindtos)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        string ErrorMessage = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
        //        return Problem(ErrorMessage);
        //    }
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var result = await _signInManager.PasswordSignInAsync(
        //        logindtos.Email, logindtos.Password, isPersistent: false, lockoutOnFailure: false);

        //    if (result.Succeeded)
        //    {
        //        IdentityUser? user = await _userManager.FindByEmailAsync(logindtos.Email);

        //        if (user == null)
        //        {
        //            return NoContent(); // Login succeeded but user not found
        //        }

        //        return Ok(new
        //        {
        //            Email = user.Email,
        //            UserName = user.UserName
        //        });
        //    }
        //    else
        //    {
        //        return Problem("Invalid email or password");
        //    }
        //}
        //[HttpGet("logout")]
        //public async Task<IActionResult> GetLogout()
        //{
        //    await _signInManager.SignOutAsync();
        //    return NoContent(); // Successfully logged out
        //}

        public AuthController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            JwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTOS registerDTOS)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IdentityUser user = new IdentityUser
            {
                Email = registerDTOS.Email,
                UserName = registerDTOS.Email
            };

            IdentityResult result = await _userManager.CreateAsync(user, registerDTOS.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");

                await _signInManager.SignInAsync(user, isPersistent: false);

                var authenticationResponse = await _jwtService.CreateJwtTokenAsync(user);

                return Ok(authenticationResponse);
            }
            else
            {
                string errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new { Message = errorMessage });
            }
        }

        [HttpGet]
        public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
        {
            IdentityUser user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Ok(true); // Email is not registered
            }
            else
            {
                return Ok(false); // Email is already registered
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LogInDTOS logindtos)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            IdentityUser user = await _userManager.FindByEmailAsync(logindtos.Email);

            if (user == null)
                return Problem("Invalid email or password");

            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, logindtos.Password, lockoutOnFailure: false);

            if (!passwordCheck.Succeeded)
                return Problem("Invalid email or password");

            await _signInManager.SignInAsync(user, isPersistent: false);

            var authenticationResponse = await _jwtService.CreateJwtTokenAsync(user);

            return Ok(authenticationResponse);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> GetLogout()
        {
            await _signInManager.SignOutAsync();
            return NoContent(); // Successfully logged out
        }
    }
}
