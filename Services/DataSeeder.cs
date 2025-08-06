using test_crud_with_jwt.DAL;
using test_crud_with_jwt.Models;

namespace test_crud_with_jwt.Services
{
    public class DataSeeder
    {
        private readonly MyAppDbContext _context;
        private readonly AuthService _authService;

        public DataSeeder(MyAppDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task SeedAsync()
        {
            // Seed sample users if not exists
            if (!_context.Users.Any())
            {
                // Create a few sample users for testing
                var users = new List<User>
                {
                    new User
                    {
                        Name = "Admin User",
                        Email = "admin@test.com",
                        PasswordHash = HashPassword("password123"),
                        Age = 30,
                        Address = "Jakarta, Indonesia",
                        CreatedAt = DateTime.Now.AddDays(-10),
                        UpdatedAt = DateTime.Now.AddDays(-2),
                        Hobbies = new List<Hobby>
                        {
                            new Hobby { Name = "Reading", Description = "Love reading books", CreatedAt = DateTime.Now.AddDays(-10), UpdatedAt = DateTime.Now.AddDays(-10) },
                            new Hobby { Name = "Gaming", Description = "Playing video games", CreatedAt = DateTime.Now.AddDays(-10), UpdatedAt = DateTime.Now.AddDays(-10) }
                        }
                    },
                    new User
                    {
                        Name = "Test User",
                        Email = "test@example.com",
                        PasswordHash = HashPassword("password123"),
                        Age = 25,
                        Address = "Bandung, Indonesia",
                        CreatedAt = DateTime.Now.AddDays(-8),
                        UpdatedAt = DateTime.Now.AddDays(-1),
                        Hobbies = new List<Hobby>
                        {
                            new Hobby { Name = "Cooking", Description = "Cooking delicious food", CreatedAt = DateTime.Now.AddDays(-8), UpdatedAt = DateTime.Now.AddDays(-8) },
                            new Hobby { Name = "Traveling", Description = "Exploring new places", CreatedAt = DateTime.Now.AddDays(-8), UpdatedAt = DateTime.Now.AddDays(-8) }
                        }
                    }
                };

                _context.Users.AddRange(users);
                await _context.SaveChangesAsync();
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password + "salt"));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}