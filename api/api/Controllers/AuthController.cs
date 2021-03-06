﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Api.Domain.DTOs;
using Api.Domain.Interfaces.Helpers;
using Api.Domain.Interfaces.Repositories;
using Api.Domain.Interfaces.Services;
using Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Api.Controllers
{
    [Route("api/v1/[controller]")]
    public class AuthController : Controller
    {

        private readonly IConfiguration   _configuration;

        private readonly IUserRepository _userRepository;
        private readonly IPartyRepository _partyRepository;

        private readonly IStatusUpdateHelper _statusUpdateHelper;

        private readonly IUserService     _userService;

        public AuthController(IConfiguration configuration, IUserRepository userRepository, IPartyRepository partyRepository, IStatusUpdateHelper statusUpdateHelper, IUserService userService)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _partyRepository = partyRepository;
            _statusUpdateHelper = statusUpdateHelper;
            _userService = userService;
        }

        

        // Register user with this username and get token
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // TODO: add a CAPTCHA to prevent spamming
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Attempt to create the new user
            try
            {
                var user = await _userService.Create(request.Username);

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

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new RegisterResponse(user.Id, user.Username, tokenString, null));
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        // Re-generate a valid token for any given username
        // Only for use during development by admins
        [HttpPost("token")]
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
            var user = await _userRepository.GetByUsername(request.Username);

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

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var party = user.GetActiveParty();
            party = await _partyRepository.GetWithAllProperties(party);
            _statusUpdateHelper.CreatePartyStatusUpdate(party, out var fullMemberStatus, out var pendingMemberStatus);
            var currentPartyModel = user.IsPendingMember ? pendingMemberStatus : fullMemberStatus;

            return Ok(new RegisterResponse(user.Id, user.Username, tokenString, currentPartyModel));
        }

    }
}