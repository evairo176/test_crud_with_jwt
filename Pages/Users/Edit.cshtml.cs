using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using test_crud_with_jwt.Models;
using test_crud_with_jwt.Services;

namespace test_crud_with_jwt.Pages.Users
{
    public class EditModel : BasePageModel
    {
        private readonly UserService _userService;

        public EditModel(JwtService jwtService, UserService userService) : base(jwtService)
        {
            _userService = userService;
        }

        [BindProperty]
        public User User { get; set; } = new();

        [BindProperty]
        [MinLength(6, ErrorMessage = "Password minimal 6 karakter")]
        public string? Password { get; set; }

        [BindProperty]
        public List<string> HobbyNames { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var authResult = RequireAuthentication();
            if (authResult != null) return authResult;

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            User = user;
            HobbyNames = user.Hobbies.Select(h => h.Name).ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var authResult = RequireAuthentication();
            if (authResult != null) return authResult;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var success = await _userService.UpdateUserAsync(User, Password, HobbyNames);
            if (success)
            {
                TempData["SuccessMessage"] = "User berhasil diperbarui.";
                return RedirectToPage("Index");
            }

            ModelState.AddModelError(string.Empty, "Gagal memperbarui user. Email mungkin sudah digunakan.");
            return Page();
        }
    }
}