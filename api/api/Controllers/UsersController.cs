using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
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