using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using test_crud_with_jwt.DAL;
using test_crud_with_jwt.Models;

namespace test_crud_with_jwt.Services
{
    public class AuthService
    {
        private readonly MyAppDbContext _context;
        private readonly JwtService _jwtService;

        public AuthService(MyAppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !VerifyPassword(password, user.PasswordHash))
            {
                return null;
            }

            return _jwtService.GenerateToken(user.Email, user.Id);
        }

        public async Task<bool> RegisterAsync(string email, string password, string name, int age, string? address = null)
        {
            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                return false;
            }

            var user = new User
            {
                Name = name,
                Email = email,
                PasswordHash = HashPassword(password),
                Age = age,
                Address = address,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public int? GetCurrentUserId(string? jwtToken)
        {
            if (string.IsNullOrEmpty(jwtToken))
                return null;

            var principal = _jwtService.ValidateToken(jwtToken);
            if (principal == null)
                return null;

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            return null;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "salt"));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }
}