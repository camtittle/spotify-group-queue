using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Domain.DTOs;
using Api.Domain.Interfaces.Helpers;
using Api.Domain.Interfaces.Repositories;
using Api.Domain.Interfaces.Services;
using Api.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    public class PartiesController : Controller
    {

        private readonly IPartyRepository _partyRepository;
        private readonly IUserRepository _userRepository;

        private readonly IPartyService _partyService;
        private readonly IMembershipService _membershipService;

        private readonly IStatusUpdateHelper _statusUpdateHelper;
        private readonly IJwtHelper _jwtHelper;

        public PartiesController(IPartyRepository partyRepository, IUserRepository userRepository, IPartyService partyService, IMembershipService membershipService, IStatusUpdateHelper statusUpdateHelper, IJwtHelper jwtHelper)
        {
            _partyRepository = partyRepository;
            _userRepository = userRepository;
            _partyService = partyService;
            _membershipService = membershipService;
            _statusUpdateHelper = statusUpdateHelper;
            _jwtHelper = jwtHelper;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllParties()
        {
            // For nwo we just return all the active parties
            var parties = await _partyRepository.GetAll();

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
            var user = await _userRepository.GetById(userId);
            var party = user.GetActiveParty();

            if (party == null)
            {
                return NoContent();
            }

            party = await _partyRepository.GetWithAllProperties(party);

            _statusUpdateHelper.CreatePartyStatusUpdate(party, out var fullMemberStatus, out var pendingMemberStatus);
            var response = user.IsPendingMember ? pendingMemberStatus : fullMemberStatus;

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

            var id = _jwtHelper.GetUserIdFromToken(User);
            var user = await _userRepository.GetById(id);

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
            var id = _jwtHelper.GetUserIdFromToken(User);
            var user = await _userRepository.GetById(id);

            // User is owner of party
            if (user.IsOwner)
            {
                var ownedParty = user.GetActiveParty();
                await _partyService.Delete(ownedParty);
                return Ok();
            }

            return Forbid();
        }

        [HttpPost("leave")]
        [Authorize]
        public async Task<IActionResult> LeaveParty()
        {
            var id = _jwtHelper.GetUserIdFromToken(User);
            var user = await _userRepository.GetById(id);
            var party = user.GetActiveParty();

            // If user is owner, this is same as deleting the party
            if (user.IsOwner)
            {
                await _partyService.Delete(party);
            } else if (party != null)
            {
                await _membershipService.Leave(user);
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

            var id = _jwtHelper.GetUserIdFromToken(User);
            var user = await _userRepository.GetById(id);

            var party = await _partyRepository.Get(request.PartyId);
            if (party == null)
            {
                return BadRequest("Party not found");
            }

            await _membershipService.RequestToJoin(party, user);
            return Ok();
        }
        
    }
}