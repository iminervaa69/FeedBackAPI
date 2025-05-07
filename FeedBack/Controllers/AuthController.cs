using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using FeedBack.Data.Models;


namespace FeedBack.Controllers
{
    [Route("feedback-api/v1/users")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly FeedBackContext _context;

        public AuthController(IConfiguration configuration, FeedBackContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> login(ReqLogin req)
        {
            var data = await _context.users.FirstOrDefaultAsync(i => i.Email.Equals(req.email));

            if (req.password.Length < 8)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, "Validation error: password must be at least 8 characters.");
            }
            else if (data == null || !data.PasswordHash.Equals(hash(req.password)))
            {
                return Unauthorized(new { message = "Invalid email or password." });
            }

            var refreshToken = generateRefreshToken();
            var accessToken = getToken(data);

            await _context.refreshtokens.AddAsync(new RefreshToken
            {
                UserId = data.Id,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Login successful.",
                data = new
                {
                    userId = data.Id,
                    username = data.Username,
                    role = data.Role,
                    access_token = accessToken,
                    refresh_token = refreshToken
                }
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var tokenRecord = await _context.refreshtokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.RevokedAt == null);

            if (tokenRecord == null || tokenRecord.ExpiresAt < DateTime.UtcNow)
                return Unauthorized(new { message = "Invalid or expired refresh token." });

            var user = await _context.users.FindAsync(tokenRecord.UserId);
            var newAccessToken = getToken(user);
            var newRefreshToken = generateRefreshToken();

            tokenRecord.RevokedAt = DateTime.UtcNow;
            
            await _context.refreshtokens.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });

            await _context.SaveChangesAsync();

            return Ok(new
            {
                access_token = newAccessToken,
                refresh_token = newRefreshToken
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> logout([FromBody] string refreshToken)
        {
            var token = await _context.refreshtokens
                .FirstOrDefaultAsync(t => t.Token == refreshToken && t.RevokedAt == null);

            if (token != null)
            {
                token.RevokedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Logout successful." });
        }

        [HttpPost("register")]
        public async Task<IActionResult> register(ReqRegister req)
        {
            var data = await _context.users.AnyAsync(i => i.Email.Equals(req.email) && i.DeletedAt == null);

            if (data == true)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, new { message = "Validation error: email is invalid." });
            }

            var user = new User
            {
                Username = req.username,
                Name = req.fullname,
                Email = req.email,
                PasswordHash = hash(req.password),
                Role = "marketing"
            };

            _context.users.Add(user);
            _context.SaveChanges();

            return Ok(new
            {
                message = "User registered successfully."
            });
        }

        private string hash(string pass)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(pass);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        private string getToken(User user)
        {
            var jwtSetting = _configuration.GetSection("JWT");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting["Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: jwtSetting["Issuer"],
                audience: jwtSetting["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string generateRefreshToken()
        {
            var randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
                return Convert.ToBase64String(randomBytes);
            }
        }
    }
}
