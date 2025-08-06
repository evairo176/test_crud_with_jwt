using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using test_crud_with_jwt.DAL;
using test_crud_with_jwt.Models;

namespace test_crud_with_jwt.Services
{
    public class UserService
    {
        private readonly MyAppDbContext _context;

        public UserService(MyAppDbContext context)
        {
            _context = context;
        }

        // Get all users with pagination and search (for Users page)
        public async Task<(List<User> Users, int TotalCount)> GetAllUsersAsync(int page = 1, int pageSize = 10, string searchTerm = "")
        {
            var query = _context.Users.Include(u => u.Hobbies).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.Name.Contains(searchTerm) || u.Email.Contains(searchTerm));
            }

            var totalCount = await query.CountAsync();
            
            var users = await query
                .OrderByDescending(u => u.UpdatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }

        // Get current user data with hobbies
        public async Task<User?> GetCurrentUserAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.Hobbies)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        // Get hobbies for current user with pagination and search
        public async Task<(List<Hobby> Hobbies, int TotalCount)> GetUserHobbiesAsync(int userId, int page = 1, int pageSize = 10, string searchTerm = "")
        {
            var query = _context.Hobbies.Where(h => h.UserId == userId).AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(h => h.Name.Contains(searchTerm) || (h.Description != null && h.Description.Contains(searchTerm)));
            }

            var totalCount = await query.CountAsync();
            
            var hobbies = await query
                .OrderByDescending(h => h.UpdatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (hobbies, totalCount);
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Hobbies)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        // Create new user with hobbies (for Users page)
        public async Task<bool> CreateUserAsync(User user, string password, List<string> hobbyNames)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Check if email already exists
                if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                {
                    return false;
                }

                user.PasswordHash = HashPassword(password);
                user.CreatedAt = DateTime.Now;
                user.UpdatedAt = DateTime.Now;
                
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                foreach (var hobbyName in hobbyNames.Where(h => !string.IsNullOrWhiteSpace(h)))
                {
                    var hobby = new Hobby
                    {
                        Name = hobbyName.Trim(),
                        UserId = user.Id,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.Hobbies.Add(hobby);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        // Update user with hobbies (for Users page)
        public async Task<bool> UpdateUserAsync(User user, string? password, List<string> hobbyNames)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingUser = await _context.Users
                    .Include(u => u.Hobbies)
                    .FirstOrDefaultAsync(u => u.Id == user.Id);

                if (existingUser == null) return false;

                // Check if email is already used by another user
                if (await _context.Users.AnyAsync(u => u.Email == user.Email && u.Id != user.Id))
                {
                    return false;
                }

                existingUser.Name = user.Name;
                existingUser.Email = user.Email;
                existingUser.Age = user.Age;
                existingUser.Address = user.Address;
                existingUser.UpdatedAt = DateTime.Now;

                // Update password if provided
                if (!string.IsNullOrEmpty(password))
                {
                    existingUser.PasswordHash = HashPassword(password);
                }

                // Remove existing hobbies
                _context.Hobbies.RemoveRange(existingUser.Hobbies);

                // Add new hobbies
                foreach (var hobbyName in hobbyNames.Where(h => !string.IsNullOrWhiteSpace(h)))
                {
                    var hobby = new Hobby
                    {
                        Name = hobbyName.Trim(),
                        UserId = user.Id,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.Hobbies.Add(hobby);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        // Delete user (for Users page)
        public async Task<bool> DeleteUserAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users
                    .Include(u => u.Hobbies)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user == null) return false;

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        // Update current user profile
        public async Task<bool> UpdateUserProfileAsync(int userId, string name, string email, int age, string? address)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (existingUser == null) return false;

                // Check if email is already used by another user
                if (await _context.Users.AnyAsync(u => u.Email == email && u.Id != userId))
                {
                    return false;
                }

                existingUser.Name = name;
                existingUser.Email = email;
                existingUser.Age = age;
                existingUser.Address = address;
                existingUser.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        // Add hobby for current user
        public async Task<bool> AddHobbyAsync(int userId, string hobbyName, string? description = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var hobby = new Hobby
                {
                    Name = hobbyName.Trim(),
                    Description = description?.Trim(),
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Hobbies.Add(hobby);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        // Update hobby for current user
        public async Task<bool> UpdateHobbyAsync(int userId, int hobbyId, string hobbyName, string? description = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var hobby = await _context.Hobbies.FirstOrDefaultAsync(h => h.Id == hobbyId && h.UserId == userId);
                if (hobby == null) return false;

                hobby.Name = hobbyName.Trim();
                hobby.Description = description?.Trim();
                hobby.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        // Delete hobby for current user
        public async Task<bool> DeleteHobbyAsync(int userId, int hobbyId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var hobby = await _context.Hobbies.FirstOrDefaultAsync(h => h.Id == hobbyId && h.UserId == userId);
                if (hobby == null) return false;

                _context.Hobbies.Remove(hobby);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        // Update user hobbies (replace all hobbies)
        public async Task<bool> UpdateUserHobbiesAsync(int userId, List<string> hobbyNames)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Remove existing hobbies
                var existingHobbies = await _context.Hobbies.Where(h => h.UserId == userId).ToListAsync();
                _context.Hobbies.RemoveRange(existingHobbies);

                // Add new hobbies
                foreach (var hobbyName in hobbyNames.Where(h => !string.IsNullOrWhiteSpace(h)))
                {
                    var hobby = new Hobby
                    {
                        Name = hobbyName.Trim(),
                        UserId = userId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.Hobbies.Add(hobby);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "salt"));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}