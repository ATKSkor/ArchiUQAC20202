using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StableAPI.Data;
using StableAPI.Models.Dto;

namespace StableAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly StableContext _context;

        public AuthController(StableContext context)
        {
            _context = context;
        }

        [Route("login")]
        public async Task<IActionResult> OnPostAsync(LoginDto req)
        {
            var user = _context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.UserName == req.Login);

            if (user == null)
            {
                return BadRequest("No such username");
            }

            if (user.PassHash != req.Pass) //TODO: Actually use hash
            {
                return Unauthorized("Wrong password");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                // Refreshing the authentication session should be allowed.

                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.

                IsPersistent = false,
                // Whether the authentication session is persisted across 
                // multiple requests. When used with cookies, controls
                // whether the cookie's lifetime is absolute (matching the
                // lifetime of the authentication ticket) or session-based.

                //IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.

                //RedirectUri = <string>
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Ok();
        }

        [Authorize]
        [HttpPost]
        [Route("logout")]
        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok();
        }

        [HttpGet]
        [Route("denied")]
        public UnauthorizedResult OnUnauthorized()
        {
            return Unauthorized();
        }

        [HttpGet("rights")]
        public ActionResult<RolesDto> GetRights()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;

            var roles = new RolesDto();

            if (claimsIdentity == null || !claimsIdentity.IsAuthenticated)
            {
                return roles;
            } else
            {
                roles.IsConnected = true;
            }

            var userRole = claimsIdentity.FindFirst(ClaimTypes.Role).Value;

            switch (userRole)
            {
                case "admin":
                    roles.IsAdmin = true;
                    break;
                case "groom":
                    roles.IsGroom = true;
                    break;
                case "secretary":
                    roles.IsSecretary = true;
                    break;
            }

            return roles;
        }
    }
}