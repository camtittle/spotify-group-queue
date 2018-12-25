﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Controllers.Models;
using api.Exceptions;
using api.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Models;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllParties()
        {
            // For nwo we just return all the active parties
            var parties = await _partyService.GetAll();

            var response = parties.Select(x => new GetPartiesResponse
            {
                Id = x.Id,
                MemberCount = x.Members?.Count ?? 0,
                Name = x.Name,
                Owner = x.Owner?.Username
            }).ToList();

            return Ok(response);
        }

        // Get user's current party
        [HttpGet("current")]
        [Authorize]
        public async Task<IActionResult> GetParty()
        {
            var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
            var user = await _userService.Find(userId);
            var party = _userService.GetParty(user);

            if (party == null)
            {
                return NoContent();
            }

            await _context.Entry(party).Reference(p => p.Owner).LoadAsync();
            await _context.Entry(party).Collection(p => p.Members).LoadAsync();

            var response = new GetCurrentPartyResponse
            {
                Id = party.Id,
                Name = party.Name,
                Owner = party.Owner.Username,
                Members = party.Members.Select(member => member.Username).ToArray()
            };

            return Ok(response);
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
            if (user.IsOwner)
            {
                return BadRequest("Already member of a party. Leave that party first");
            }

            try
            {
                return Ok(await _partyService.Create(user, request.Name));
            }
            catch (Exception e) when (e is ArgumentNullException || e is ArgumentException)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
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
            if (user.IsOwner)
            {
                await _partyService.Delete(user.OwnedParty);
            }
            return Ok();
        }

        [HttpPost("leave")]
        [Authorize]
        public async Task<IActionResult> LeaveParty()
        {
            var user = await GetAuthorizedUser();
            var party = _userService.GetParty(user);

            // If user is owner, this is same as deleting the party
            if (user.IsOwner)
            {
                await _partyService.Delete(party);
            } else if (party != null)
            {
                await _partyService.Leave(user);
            }

            return Ok();
        }

        // Request to join a party
        [HttpPost("join")]
        [Authorize]
        public async Task<IActionResult> JoinParty([FromBody] JoinPartyRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await GetAuthorizedUser();

            var party = await _partyService.Find(request.PartyId);
            if (party == null)
            {
                return BadRequest("Party not found");
            }

            await _partyService.RequestToJoin(party, user);
            return Ok();
        }

        //// For the user's current party, get the pending members
        //[Route("members/pending")]
        //[HttpGet]
        //[Authorize]
        //public async Task<IActionResult> GetPendingMembers()
        //{
        //    var user = await GetAuthorizedUser();

        //    if (user.IsMember)
        //    {
        //        // TODO: make this not include the pendingparty field of each member (all the same)
        //        var party = await _partyService.Find(user.CurrentParty.Id);
        //        return Ok(user.CurrentParty.PendingMembers);
        //    }

        //    return NotFound();
        //}

        // Helper method to get the user from the current user's auth token. Assumed user is authorized
        private async Task<User> GetAuthorizedUser()
        {
            var userId = User.Claims.Single(c => c.Type == ClaimTypes.PrimarySid).Value;
            var user = await _userService.Find(userId);
            return user;
        }

        
    }
}