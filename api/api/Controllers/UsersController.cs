using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using api.Models;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    public class UsersController : Controller
    {
        private readonly apiContext _context;
        private readonly IUserService _userService;

        public UsersController(apiContext context,
                               IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        // GET: api/Users
        [HttpGet]
        public IEnumerable<User> GetUsers()
        {
            return _context.Users;
        }

        [Route("me")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUser()
        {
            return Ok(await GetAuthorizedUser());
        }

        // Helper method to get the user from the current user's auth token. Assumed user is authorized
        private async Task<User> GetAuthorizedUser()
        {
            var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
            var user = await _userService.Find(userId);
            await _context.Entry(user).Reference(u => u.CurrentParty).LoadAsync();
            await _context.Entry(user).Reference(u => u.PendingParty).LoadAsync();
            return user;
        }

    }
}