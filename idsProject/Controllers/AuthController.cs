using Ids.Data;
using Ids.Models;
using idsProject.Dtos.RegisterDto;
using idsProject.Dtos.User;
using idsProject.Models;
using idsProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace idsProject.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration config,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _context = context;
        }

        // =========================
        // Refresh cookie helpers
        // =========================
        private void SetRefreshTokenCookie(string refreshTokenPlain, DateTime expiresAtUtc)
        {
            // Cross-site cookie (React 5173 -> API https://localhost:7126)
            // SameSite=None requires Secure=true (HTTPS).
            Response.Cookies.Append("refreshToken", refreshTokenPlain, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = expiresAtUtc
            });
        }

        private void ClearRefreshTokenCookie()
        {
            Response.Cookies.Delete("refreshToken");
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
                return BadRequest("FirstName and LastName are required.");

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

            return Ok(new { message = "Registered successfully" });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Email or Password Field are required");

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);

            if (result.IsLockedOut)
                return Unauthorized("Account locked. Try again later.");

            if (!result.Succeeded)
                return Unauthorized("Invalid email or password");

            // ===== ACCESS TOKEN (JWT) =====
            var jwt = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpiresMinutes"]!));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? dto.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            // ===== REFRESH TOKEN (HASHED IN DB, PLAIN IN COOKIE) =====
            var refreshDays = int.Parse(jwt["RefreshTokenDays"] ?? "14");
            var pepper = jwt["RefreshTokenPepper"]!;
            var refreshExpires = DateTime.UtcNow.AddDays(refreshDays);

            // 1) generate plain refresh token
            var refreshTokenPlain = TokenGenerator.GenerateRefreshToken();

            // 2) hash it before saving to DB
            var refreshTokenHash = RefreshTokenHasher.Hash(refreshTokenPlain, pepper);

            // 3) store only the hash (NOT the plain token)
            _context.RefreshTokens.Add(new RefreshToken
            {
                TokenHash = refreshTokenHash,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = refreshExpires,
                CreatedByIp = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _context.SaveChangesAsync();

            // 4) cookie keeps the plain token (HttpOnly so JS can’t read it)
            SetRefreshTokenCookie(refreshTokenPlain, refreshExpires);

            return Ok(new AuthResponseDto
            {
                AccessToken = accessToken,
                ExpiresAt = expires
            });
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponseDto>> Refresh()
        {
            // 1) read plain refresh token from cookie
            var refreshTokenPlain = Request.Cookies["refreshToken"];
            if (string.IsNullOrWhiteSpace(refreshTokenPlain))
                return Unauthorized("Missing refresh token.");

            // 2) hash it to compare with DB
            var jwt = _config.GetSection("Jwt");
            var pepper = jwt["RefreshTokenPepper"]!;
            var refreshTokenHash = RefreshTokenHasher.Hash(refreshTokenPlain, pepper);

            // 3) find token by hash
            var stored = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.TokenHash == refreshTokenHash);

            if (stored == null)
                return Unauthorized("Invalid refresh token.");

            if (stored.IsRevoked)
                return Unauthorized("Refresh token revoked.");

            if (stored.ExpiresAt <= DateTime.UtcNow)
                return Unauthorized("Refresh token expired.");

            // 4) load user
            var user = await _userManager.FindByIdAsync(stored.UserId);
            if (user == null)
                return Unauthorized("User not found.");

            // 5) issue new access token (same as login)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(double.Parse(jwt["ExpiresMinutes"]!));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            // 6) rotate refresh token
            stored.IsRevoked = true;

            var refreshDays = int.Parse(jwt["RefreshTokenDays"] ?? "14");
            var newRefreshExpires = DateTime.UtcNow.AddDays(refreshDays);

            var newRefreshTokenPlain = TokenGenerator.GenerateRefreshToken();
            var newRefreshTokenHash = RefreshTokenHasher.Hash(newRefreshTokenPlain, pepper);

            stored.ReplacedByTokenHash = newRefreshTokenHash;

            _context.RefreshTokens.Add(new RefreshToken
            {
                TokenHash = newRefreshTokenHash,
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = newRefreshExpires,
                CreatedByIp = HttpContext.Connection.RemoteIpAddress?.ToString()
            });

            await _context.SaveChangesAsync();

            SetRefreshTokenCookie(newRefreshTokenPlain, newRefreshExpires);

            return Ok(new AuthResponseDto
            {
                AccessToken = accessToken,
                ExpiresAt = expires
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshTokenPlain = Request.Cookies["refreshToken"];

            // always clear cookie
            ClearRefreshTokenCookie();

            if (string.IsNullOrWhiteSpace(refreshTokenPlain))
                return Ok(new { message = "Logged out." });

            var jwt = _config.GetSection("Jwt");
            var pepper = jwt["RefreshTokenPepper"]!;
            var refreshTokenHash = RefreshTokenHasher.Hash(refreshTokenPlain, pepper);

            var stored = await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.TokenHash == refreshTokenHash);

            if (stored == null)
                return Ok(new { message = "Logged out." });

            var currentUserId = _userManager.GetUserId(User);
            if (stored.UserId != currentUserId)
                return Forbid();

            stored.IsRevoked = true;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Logged out." });
        }
    }
}
