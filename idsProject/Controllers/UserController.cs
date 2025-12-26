using Ids.Data;
using Ids.Models;
using idsProject.Dto.AssignRole;
using idsProject.Dtos.RegisterDto;
using idsProject.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace idsProject.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public UserController(AppDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ===============================
        // GET: api/user
        // Get all users
        // ===============================
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll()
        {
            var users = await _context.Users.ToListAsync();
            var result = new List<UserResponseDto>();
            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                result.Add(new UserResponseDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email ?? string.Empty,
                    Roles = roles.ToList(),
                    CreatedAt = u.CreatedAt
                });
            }


            return Ok(result);
        }

        // ===============================
        // GET: api/user/{id}
        // Get user by id
        // ===============================
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetById(string id)
        {
            var u = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (u == null) return NotFound("User not found");

            var roles = await _userManager.GetRolesAsync(u);

            var result = new UserResponseDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email ?? string.Empty,
                Roles = roles.ToList(),
                CreatedAt = u.CreatedAt
            };
            return Ok(result);
        }

        // ===============================
        // POST: api/user
        // Create user
        // ===============================

        [HttpPost]
        public async Task<ActionResult> Create(CreateUserDto dto)
        {
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == dto.Email);

            if (emailExists)
                return BadRequest("Email already exists");

            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                CreatedAt = DateTime.UtcNow
            };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            if (!string.IsNullOrEmpty(dto.Role))
                await _userManager.AddToRoleAsync(user, dto.Role);


            var roles = await _userManager.GetRolesAsync(user);

            var response = new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Roles = roles.ToList(),
                CreatedAt = user.CreatedAt

            };

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, response);

        }

        // ===============================
        // PUT: api/user/{id}
        // Update user
        // ===============================
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponseDto>> Update(string id, UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("User not found");

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;

            var updateUser = await _userManager.UpdateAsync(user);
            if (!updateUser.Succeeded) return BadRequest(updateUser.Errors);

            if (!string.IsNullOrWhiteSpace(dto.Role))
            {

                var currentRoles = await _userManager.GetRolesAsync(user);

                if (currentRoles.Count > 0)
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)

                        return BadRequest(removeResult.Errors);
                }
                var addResult = await _userManager.AddToRoleAsync(user, dto.Role);

                if (!addResult.Succeeded)
                    return BadRequest(addResult.Errors);
            }
            var roles = await _userManager.GetRolesAsync(user);

            var response = new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                Roles = roles.ToList(),
                CreatedAt = user.CreatedAt
            };

            return Ok(Response);
        }

        // ===============================
        // DELETE: api/user/{id}
        // Delete user
        // ===============================
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound("User not found");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPut("{id}/roles")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole(string id, [FromBody] AssignRoleDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Role))
                return BadRequest("Role is required.");

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound("User not found.");

            var roleName = dto.Role.Trim();

            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
                return BadRequest($"Role '{roleName}' does not exist.");

            // optional: if you want ONE role only
            if (dto.ReplaceExistingRoles)
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Count > 0)
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)
                        return BadRequest(removeResult.Errors);
                }
            }

            // avoid duplicate
            if (await _userManager.IsInRoleAsync(user, roleName))
                return Ok(new { message = $"User already has role '{roleName}'." });

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var rolesAfter = await _userManager.GetRolesAsync(user);
            return Ok(new { userId = user.Id, roles = rolesAfter });
        }
    }
}
