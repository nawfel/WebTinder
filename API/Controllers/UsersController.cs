using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    
    public class UsersController : BaseApiController
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            var user = await _context.Users.ToListAsync();
            return Ok(user);
        }
        [Authorize]
        [HttpGet("{userId}")]
        public async Task<ActionResult<AppUser>> GetUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return Ok(user);
        }
    }
}
