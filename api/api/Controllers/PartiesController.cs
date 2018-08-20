using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Controllers.Models;
using api.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Models;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace api.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    public class PartiesController : Controller
    {
        private readonly apiContext _context;
        private readonly IPartyService _partyService;
        private readonly IUserService _userService;

        public PartiesController(apiContext     context,
                                 IPartyService  partyService,
                                 IUserService   userService)
        {
            _context = context;
            _partyService = partyService;
            _userService = userService;
        }

        // Get user's current party
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetParty()
        {
            var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
            var party = await _context.Users.Where(u => u.Id == userId).Select(u => u.CurrentParty).Include(p => p.Members).FirstOrDefaultAsync();
            
            if (party == null)
            {
                return NotFound();
            }

            return Ok(party);
        }

        // Create party with current user as owner
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateParty([FromBody] CreatePartyModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
            var user = await _userService.Find(userId);
            if (user.Owner || user.CurrentParty != null || user.Admin)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Already admin or own a party. Delete or leave that party first");
            }
            
            try
            {
                return Ok(await _partyService.Create(userId, request.Name));
            }
            catch (APIException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e);
            }
        }

        // Delete the party you own
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteParty()
        {
            var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
            var user = await _userService.Find(userId);
            if (user.CurrentParty != null)
            {
                await _partyService.Delete(user.CurrentParty.Id);
            }
            return Ok();
        }


        private bool PartyExists(string id)
        {
            return _context.Parties.Any(e => e.Id == id);
        }
    }
}