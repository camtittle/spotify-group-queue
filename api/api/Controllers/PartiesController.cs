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

        [Route("all")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllParties()
        {
            return Ok(await _partyService.GetAll());
        }

        // Get user's current party
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetParty()
        {
            var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
            var party = await _partyService.FindByUser(userId);
            
            if (party == null)
            {
                return NotFound();
            }

            await _context.Entry(party).Collection(p => p.Members).LoadAsync();
            await _context.Entry(party).Collection(p => p.PendingMembers).LoadAsync();

            return Ok(party);
        }

        // Create party with current user as owner
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateParty([FromBody] CreatePartyRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await GetAuthorizedUser();
            if (user.Owner || user.CurrentParty != null || user.Admin)
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Already admin or own a party. Delete or leave that party first");
            }
            
            try
            {
                return Ok(await _partyService.Create(user.Id, request.Name));
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
            var user = await GetAuthorizedUser();

            // User is owner of party
            if (user.Owner && user.CurrentParty != null)
            {
                await _partyService.Delete(user.CurrentParty.Id);
            }
            return Ok();
        }

        [Route("leave")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> LeaveParty()
        {
            var user = await GetAuthorizedUser();

            // If user is owner, this is same as deleting the party
            if (user.CurrentParty != null)
            {
                if (user.Owner)
                {
                    await _partyService.Delete(user.CurrentParty.Id);
                }
                else
                {
                    await _partyService.Leave(user.Id);
                }
            }

            return Ok();
        }

        // Request to join a party
        [Route("join")]
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> JoinParty([FromBody] JoinPartyRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await GetAuthorizedUser();
            try
            {
                await _partyService.RequestToJoin(request.PartyId, user);
                return Ok();
            }
            catch (ResourceNotFoundException e)
            {
                return NotFound(e);
            }
        }

        // For the user's current party, get the pending members
        [Route("members/pending")]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPendingMembers()
        {
            var user = await GetAuthorizedUser();

            if (user.CurrentParty != null)
            {
                // TODO: make this not include the pendingparty field of each member (all the same)
                var party = await _partyService.Find(user.CurrentParty.Id);
                return Ok(user.CurrentParty.PendingMembers);
            }

            return NotFound();
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