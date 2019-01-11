using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Exceptions;
using Microsoft.EntityFrameworkCore;
using api.Models;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

    }
}