namespace OnlineChat.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using OnlineChat.DataModels;

    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ChatDbContext _context;

        public UsersController(ChatDbContext context)
        {
            _context = context;
        }

        //[Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            else
                return Ok(user);
        }

        [HttpGet(Name = "GetUsers")]
        public async Task<IEnumerable<User>> Get()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostUser), new { id = user.Id }, user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) 
            {
                return Conflict(ex.ToString());
            }

            return NoContent();
        }
    }
}
