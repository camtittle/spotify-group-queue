using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using api.Controllers.Models;
using api.Exceptions;
using api.Models;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace api.Controllers
{
    [Route("api/v1/[controller]")]
    public class AuthController : Controller
    {

        private readonly IConfiguration   _configuration;
        private readonly IUserService     _userService;
        private readonly IPartyService    _partyService;

        public AuthController(IConfiguration   configuration,
                              IUserService     userService,
                              IPartyService    partyService)
        {
            _configuration   = configuration;
            _userService     = userService;
            _partyService    = partyService;
        }

        // Register user with this username and get token
        [Route("register")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            // TODO: add a CAPTCHA to prevent spamming
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Attempt to create the new user
            try
            {
                var user = _userService.Create(request.Username);

                // TODO: verification eg different IP and user agent? (prevent spamming)

                // Create token
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.PrimarySid, user.Id)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "spotifyparty.dev",
                    audience: "spotifyparty.dev",
                    claims: claims,
                    expires: DateTime.Now.AddHours(6),
                    signingCredentials: creds);

                return Ok(new RegisterResponse(user.Username, new JwtSecurityTokenHandler().WriteToken(token)));
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        // Re-generate a valid token for any given username
        // Only for use during development by admins
        [Route("token")]
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Token([FromBody] TokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.DeveloperPassword != _configuration["developerPassword"])
            {
                return BadRequest("Incorrect password");
            }

            // Attempt to find user and generate token
            try
            {
                User user = await _userService.FindByUsername(request.Username);

                if (user == null) return BadRequest("User does not exist");

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.PrimarySid, user.Id)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTSecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "spotifyparty.dev",
                    audience: "spotifyparty.dev",
                    claims: claims,
                    expires: DateTime.Now.AddHours(6),
                    signingCredentials: creds);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            catch (APIException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }

        }

    }
}